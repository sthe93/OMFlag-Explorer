# Flag Explorer Application

## Overview

Flag Explorer is a full-stack web application that allows users to browse and explore country flags, view country details, and interact with an intuitive UI. It leverages:

* **Backend API**: ASP.NET Core Web API for fetching country data from an external service.
* **Frontend**: ASP.NET Core MVC with Razor views for a dynamic, server-rendered UI.
* **Testing**: Comprehensive integration and unit tests using xUnit, Moq, and the ASP.NET Core TestHost.
* **CI/CD**: GitHub Actions pipeline for automated build, test, and deployment.

## Features

* 🌐 Browse a list of all countries and their flags.
* 🔍 Search and filter countries by name.
* 📊 View detailed country information: capital, population, region, and more.
* 🚀 Fast, responsive UI built with Bootstrap.
* 🧪 Automated tests ensure reliability and stability.
* 📦 Docker support for containerized deployments.

## Architecture

```plaintext
User → MVC Frontend → Backend API → External Country Service (REST)
```

* **MVC Frontend**: Razor Pages, Bootstrap CSS, FontAwesome icons.
* **API Client**: Typed HTTP client (`CountryApi`) registered in `Startup`/`Program.cs`.
* **Testing**: In-memory TestServer for integration tests; HttpMessageHandler mocking for controller tests.

## Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/) (optional)

### Clone Repository

```bash
git clone https://github.com/sthe93/OMFlag-Explorer.git
cd OMFlag-Explorer
```

### Configure API Access

By default, the application uses `https://restcountries.com/v3.1` as the external service. To change the base URL, update the `CountryApi` client in `appsettings.json`:

```json
"HttpClients": {
  "CountryApi": {
    "BaseAddress": "https://restcountries.com/v3.1/"
  }
}
```

### Build & Run

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run API + MVC
dotnet run --project src/FlagExplorer.Web
```

Browse to [https://localhost:5031] to view the app.

## Testing

Run all tests with coverage:

```bash
# From solution root
dotnet test --collect:"XPlat Code Coverage"
```

To generate an HTML report from the TRX:

```bash
dotnet test src/FlagExplorer.Web.Tests \
  --logger "trx;LogFileName=TestResults.trx" \
  --results-directory src/FlagExplorer.Web.Tests/TestResults

reportunit \
  src/FlagExplorer.Web.Tests/TestResults/TestResults.trx \
  src/FlagExplorer.Web.Tests/TestResults/TestResults.html
```



## CI/CD Pipeline

A sample GitHub Actions workflow is defined in `.github/workflows/ci.yml`:

* **Build** on push to `main`
* **Run tests** and publish code coverage
* **Publish** 

Customize by editing the workflow file.

## Contributing

1. Fork the repo
2. Create a feature branch (`git checkout -b feature/XYZ`)
3. Commit your changes (`git commit -m "Add XYZ feature"`)
4. Push to your branch (`git push origin feature/XYZ`)
5. Open a Pull Request

## License

This project is licensed under the [MIT License](LICENSE).
