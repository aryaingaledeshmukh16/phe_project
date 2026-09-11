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

The frontend reads `NEXT_PUBLIC_API_BASE_URL` from `phe-water-ui/.env.local`. Copy `phe-water-ui/.env.example` to that file and set the API origin for the target environment. The fallback remains `http://localhost:5014` for local development.

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
| `GET` | `/api/JEScrutiny/{applicationNo}/history` | Read application history |
| `PUT` | `/api/JEScrutiny/{applicationNo}/action` | Save JE scrutiny action |
| `GET` | `/api/DeScrutiny` | List applications ready for DE scrutiny |
| `GET` | `/api/DeScrutiny/{applicationNo}/history` | Read DE history |
| `PUT` | `/api/DeScrutiny/{applicationNo}/action` | Save DE scrutiny action |
| `GET` | `/api/PHEScrutiny` | List applications ready for PHE scrutiny |
| `GET` | `/api/PHEScrutiny/{applicationNo}/history` | Read PHE history |
| `PUT` | `/api/PHEScrutiny/{applicationNo}/action` | Save PHE scrutiny action |
| `POST` | `/api/Applicants/SiteVisit/{applicationNo}` | Save site visit data and files |

## Separation Notes

This repository contains the form-oriented Next.js UI and the existing ASP.NET Core API. The public form keeps the existing stepper, layout, document upload, and acknowledgement workflow. The JE, DE, and PHE scrutiny routes and their API controllers are retained. The unused public `Header` component was removed; no backend controller, DTO, model, migration, API route, database schema, or workflow transition was changed.

The database entities required by the active workflow are `Applicants` and `ApplicantsLogs`. `JEApplications` and its migration are retained because they are part of the existing database contract, although the active controllers read and write `Applicants` and `ApplicantsLogs`. Required files are stored under `PHE.API/Uploads` and are intentionally ignored.

Authentication is not implemented in this repository. There is no login endpoint, identity store, JWT or cookie setup, or server-side role authorization. The current JE, DE, and PHE pages pass their existing client-side role/user values to the API. The separated application must be connected to the external authentication and authorization system before production use; do not treat those client-side values as security controls. The existing OTP component also calls `/api/Applicant/SendOtp` and `/api/Applicant/VerifyOtp`, but matching backend endpoints are not present in this repository, so OTP remains an external/incomplete dependency and was not invented here.

## Dedicated Repository Status

The current checkout still has its original remote: `https://github.com/aryaingaledeshmukh16/phe_project.git`. It was not repointed or pushed. The requested organization repository `Solapur-Municipal-Corporation-org/PHE-Water-NOC` did not exist when checked, and no GitHub organization credentials or repository-creation capability were available in this workspace. Do not push until an organization administrator creates the dedicated repository and provides an authenticated remote or approved GitHub access.

The original application remote was not repointed, merged with, force-pushed, or otherwise overwritten by this separation work. This checkout contains the uncommitted separation changes shown by `git status`; they have not been committed or pushed. Once the dedicated empty repository exists, the expected clone command is:

```powershell
git clone https://github.com/Solapur-Municipal-Corporation-org/PHE-Water-NOC.git
```

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
#
