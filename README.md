# 🎬 dapper-demo

A modern .NET 9 solution demonstrating **Dapper** for high-performance data access, featuring:

- 🖥️ **Console Application** with a rich, interactive CLI using [Spectre.Console](https://spectreconsole.net/)
- 🌐 **Minimal API** with OpenAPI/Swagger UI for easy RESTful access
- 🗄️ Clean separation of data access, business logic, and presentation

---

## 🚀 Features

### Console App (`QueryingData`)
- **Interactive CLI**: Uses Spectre.Console for beautiful, colored menus, tables, and prompts.
- **Dapper Data Access**: All database operations (CRUD, bulk, relational queries) are performed using Dapper for speed and simplicity.
- **Bulk Operations**: Efficient inserts/updates/deletes with Z.Dapper.Plus.
- **Rich Output**: Results are displayed in styled tables and panels for a great terminal experience.

### Minimal API (`VideoRental.API`)
- **REST Endpoints**: Exposes film and actor data via minimal API endpoints.
- **Swagger UI**: Auto-generated, interactive API docs at `/swagger`.
- **FluentValidation**: Ensures robust input validation for all endpoints.
- **FluentResults**: Consistent result and error handling.
- **Caching**: Uses in-memory caching for fast category lookups.

---

## 🛠️ How It Works

### Data Access Layer
- **Dapper** is used for all SQL queries and commands, providing lightweight, high-performance mapping between C# objects and PostgreSQL tables.
- **Repository Pattern**: Data access is encapsulated in repository classes, making the codebase clean and testable.
- **Bulk Operations**: Z.Dapper.Plus is used for high-volume data manipulation.

### Console UI
- **Spectre.Console** powers the CLI, offering:
  - 🎨 Colorful menus and prompts
  - 📊 Tabular data display
  - 📝 User-friendly input and feedback

### Minimal API
- **Endpoints** for films and actors, supporting:
  - Get all, get by ID, search, pagination, and category filtering
  - Input validation and error reporting
- **Swagger/OpenAPI**: Instantly explore and test the API in your browser.

---

## 🏁 Getting Started

1. **Clone the repo**
2. **Set up PostgreSQL** (see `appsettings.Development.json` for connection string)
3. **Run the Console App**