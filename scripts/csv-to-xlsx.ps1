<#
Convert a CSV to XLSX using Excel COM automation (Windows with Excel installed).
Usage (from repo root):
  powershell -ExecutionPolicy Bypass -File scripts\csv-to-xlsx.ps1 -CsvPath samples\products_example.csv -OutPath samples\products_example.xlsx

This script requires Microsoft Excel on the machine.
#>

param(
	[string]$CsvPath = "samples\products_example.csv",
	[string]$OutPath = "samples\products_example.xlsx"
)

$csvFull = Resolve-Path $CsvPath -ErrorAction Stop
$outFull = Resolve-Path (Split-Path $OutPath -Parent) -ErrorAction SilentlyContinue
if (-not $outFull) {
	New-Item -ItemType Directory -Path (Split-Path $OutPath -Parent) -Force | Out-Null
}

$excel = New-Object -ComObject Excel.Application
$excel.DisplayAlerts = $false
try {
	$workbook = $excel.Workbooks.Open($csvFull.Path)
	# 51 = xlOpenXMLWorkbook (xlsx)
	$workbook.SaveAs((Resolve-Path $OutPath).Path, 51)
	$workbook.Close($false)
} finally {
	$excel.Quit()
	[System.Runtime.Interopservices.Marshal]::ReleaseComObject($workbook) | Out-Null
	[System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null
	[GC]::Collect()
	[GC]::WaitForPendingFinalizers()
}

Write-Host "Saved XLSX to: $OutPath"
