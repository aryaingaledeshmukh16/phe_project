# PHE Water NOC

Web application for submitting and reviewing Public Health Engineering water NOC applications.

The repository contains:

- `phe-water-ui`: Next.js frontend for applicant forms, document upload, acknowledgement receipts, and JE scrutiny.
- `PHE.API`: ASP.NET Core 9 Web API with SQL Server persistence, file uploads, application history, OTP, and scrutiny workflows.
- `swagger.json`: API contract reference.

## Prerequisites

- .NET SDK 9.0 or later
- Node.js 20 or later and npm
- SQL Server with a database for the application data
- Git

## Configuration

The API reads its database connection from `PHE.API/appsettings.json`. This file is intentionally ignored by Git because it can contain credentials or machine-specific settings.

1. Copy `PHE.API/appsettings.example.json` to `PHE.API/appsettings.json`.
2. Set `ConnectionStrings:DefaultConnection` to your local SQL Server connection string.
3. Apply the Entity Framework migrations if the database is not already up to date:

```powershell
cd PHE.API
dotnet ef database update
```

Never commit `appsettings.json`, `.env` files, passwords, API keys, or uploaded documents.

## Run Locally

Start the API in one terminal:

```powershell
cd PHE.API
dotnet run --launch-profile http
```

The API runs at `http://localhost:5014` and Swagger is available at `http://localhost:5014/swagger` in Development.

Start the frontend in a second terminal:

```powershell
cd phe-water-ui
npm install
npm run dev
```

Open `http://localhost:3000` in a browser.

The frontend currently expects the API at `http://localhost:5014`. If the API is hosted elsewhere, update the API URL constants in the frontend components before deployment.

## Main API Routes

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/api/Applicants` | Create an applicant application |
| `GET` | `/api/Applicants` | List applications |
| `GET` | `/api/Applicants/{applicationNo}` | Get application details |
| `PUT` | `/api/Applicants/Layout/{applicationNo}` | Save layout information |
| `POST` | `/api/Applicants/Documents/{applicationNo}` | Upload required documents |
| `GET` | `/api/JEScrutiny` | List applications for JE scrutiny |
| `GET` | `/api/JEScrutiny/{applicationNo}/documents/{documentType}` | View an uploaded document |

## Frontend Scripts

Run these commands from `phe-water-ui`:

```powershell
npm run dev       # Start development server
npm run build     # Create a production build
npm run start     # Serve the production build
npm run lint      # Run ESLint
```

## Project Structure

```text
PHE.API/
	Controllers/    API endpoints
	DTOs/           Request models
	Data/           Entity Framework DbContext
	Migrations/     Database migrations
	Models/         Database entities
	Uploads/        Local uploaded files (ignored by Git)

phe-water-ui/
	app/            Next.js routes and React components
	public/         Static frontend assets
```

## Repository

GitHub: https://github.com/solapur09510-blip/phe_project
