# ECommerceAPI

A simple, opinionated ASP.NET Core Web API for an e-commerce sample application targeting .NET 10.

## Features
- Product and category management
- Orders and shopping cart API surface
- Layered solution (models, services, API)

## Requirements
- .NET 10 SDK (https://dotnet.microsoft.com)
- Optional: Visual Studio 2022/2026 or VS Code

## Getting started
1. Clone the repository (if you haven't already):

   git clone https://github.com/<your-username>/<repo-name>.git
   cd ECommerceAPI

2. Restore and build:

   dotnet restore
   dotnet build

3. Run the API:

   dotnet run --project src/ECommerce.API

   The API will start on the configured port (check console output). You can also run from Visual Studio by opening the solution file `ECommerceAPI.slnx` and pressing F5.

## Tests
If there are test projects in the solution, run them with:

   dotnet test

## Docker (optional)
Add a Dockerfile to the API project root to containerize the application. Example build/run:

   docker build -t ecommerceapi:latest .
   docker run -p 5000:80 ecommerceapi:latest

## Configuration
- Use appsettings.json or environment variables for connection strings and secrets.
- Do not commit secrets or user-specific files; .gitignore excludes common binaries and user files.

## Contributing
- Fork the repo, open a feature branch, and create a pull request. Keep changes focused and add tests when possible.

## License
Specify your license here (e.g., MIT). If you don't want to choose a license yet, add one later in a LICENSE file.

---
Edit this README to reflect project-specific details (project paths, sample requests, DB setup, third-party services).
