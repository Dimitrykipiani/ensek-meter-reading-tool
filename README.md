# ENSEK Meter Reading Tool

A full-stack solution for uploading and validating customer meter readings from CSV files.  
Built using ASP.NET Core Web API, with optional React frontend and xUnit-based test coverage.

## 🧩 Project Structure

/api → ASP.NET Core Web API
/tests → xUnit test project
/client → (optional) React frontend


## ⚙️ Technologies

- ASP.NET Core 8 Web API
- Entity Framework Core (SQL Server LocalDB)
- CsvHelper (CSV parsing)
- xUnit + Moq (unit testing)
- React (optional client app)

## 📦 Features

- `POST /meter-reading-uploads` endpoint to upload CSV files
- Validates each record:
  - ✅ Must belong to a known Account ID
  - ✅ Meter read value must match `NNNNN` format
  - ❌ Duplicates are rejected
  - ❌ Invalid/unknown accounts are rejected
- Seeds account data from CSV at startup
- Cleanly structured with service-layer separation
- In-memory and SQL Server modes supported

## 🚀 Getting Started

### 📥 Clone the repo

```bash
git clone https://github.com/YOUR_USERNAME/ensek-meter-reading-tool.git
cd ensek-meter-reading-tool
