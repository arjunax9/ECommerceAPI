using ECommerce.Business.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;
using ECommerce.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IInventoryService _inventoryService;

    public ProductsController(IProductService service, IInventoryService inventoryService)
    {
        _service = service;
        _inventoryService = inventoryService;
    }

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Product product)
    {
        // Ensure we don't try to insert an explicit Id for an identity column
        product.Id = 0;

        var created = await _service.AddAsync(product);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Import products in bulk from an Excel (.xlsx) file. Expected header row: Name, Description, Price, Category, Quantity
    /// </summary>
    [HttpPost("import")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var imported = new List<Product>();

        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();

        if (ext == ".csv" || file.ContentType == "text/csv")
        {
            // Simple CSV parsing (header row required)
            using var sr = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            string? headerLine = await sr.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
                return BadRequest("CSV file has no header row.");

            var headers = headerLine.Split(',').Select(h => h.Trim().Trim('"')).ToList();

            string? line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = line.Split(',').Select(c => c.Trim().Trim('"')).ToList();

                string name = GetColumnValue(headers, cols, "Name") ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name)) continue;

                string description = GetColumnValue(headers, cols, "Description") ?? string.Empty;
                decimal.TryParse(GetColumnValue(headers, cols, "Price"), out var price);
                string category = GetColumnValue(headers, cols, "Category") ?? string.Empty;
                int.TryParse(GetColumnValue(headers, cols, "Quantity"), out var quantity);

                var product = new Product { Id = 0, Name = name, Description = description, Price = price, Category = category };
                var created = await _service.AddAsync(product);
                if (quantity > 0) await _inventoryService.UpdateQuantityAsync(created.Id, quantity);
                imported.Add(created);
            }
        }
        else
        {
            // Try Excel
            try
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();

                // Map headers to column numbers
                var headerRow = worksheet.Row(1);
                var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int c = 1; c <= headerRow.CellCount(); c++)
                {
                    var h = headerRow.Cell(c).GetString();
                    if (!string.IsNullOrWhiteSpace(h) && !map.ContainsKey(h)) map[h.Trim()] = c;
                }

                int lastRow = worksheet.LastRowUsed().RowNumber();
                for (int r = 2; r <= lastRow; r++)
                {
                    var row = worksheet.Row(r);
                    string name = map.ContainsKey("Name") ? row.Cell(map["Name"]).GetString() : row.Cell(1).GetString();
                    if (string.IsNullOrWhiteSpace(name)) continue;
                    string description = map.ContainsKey("Description") ? row.Cell(map["Description"]).GetString() : string.Empty;
                    decimal.TryParse(map.ContainsKey("Price") ? row.Cell(map["Price"]).GetString() : string.Empty, out var price);
                    string category = map.ContainsKey("Category") ? row.Cell(map["Category"]).GetString() : string.Empty;
                    int.TryParse(map.ContainsKey("Quantity") ? row.Cell(map["Quantity"]).GetString() : string.Empty, out var quantity);

                    var product = new Product { Id = 0, Name = name, Description = description ?? string.Empty, Price = price, Category = category ?? string.Empty };
                    var created = await _service.AddAsync(product);
                    if (quantity > 0) await _inventoryService.UpdateQuantityAsync(created.Id, quantity);
                    imported.Add(created);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to parse uploaded file.", details = ex.Message });
            }
        }

        return Ok(new { imported = imported.Count, products = imported });
    }

    private static string? GetColumnValue(List<string> headers, List<string> cols, string name)
    {
        var idx = headers.FindIndex(h => string.Equals(h, name, StringComparison.OrdinalIgnoreCase));
        if (idx < 0) return null;
        return idx < cols.Count ? cols[idx] : null;
    }

    [HttpPut("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        product.Id = id;

        await _service.UpdateAsync(product);

        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}
