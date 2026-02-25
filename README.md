# TeamPulse – Frontend & Backend

This repository contains a Nuxt-based frontend and a .NET backend for the TeamPulse application.

## Project Structure

- `frontend` – Nuxt 4 / Vue 3 SPA
- `backend` – .NET solution (`TeamPulse.sln`) containing:
  - `TeamPulse.Domain`
  - `TeamPulse.Application`
  - `TeamPulse.Infrastructure`
  - `TeamPulse.Api` (Web API)
  - `TeamPulse.Tests` (test project)

---

## Prerequisites

- **Node.js** (LTS recommended) and **npm**
- **.NET SDK** (version matching your environment; .NET 8 recommended)

---

## Frontend (`frontend`)

The frontend is a Nuxt 4 + Vue 3 application.

### Install dependencies

```bash
cd frontend
npm install
```

### Run in development mode

```bash
npm run dev
```

This starts the Nuxt dev server (by default on `http://localhost:3000` unless configured otherwise).

### Build for production

```bash
npm run build
```

### Generate static site (if used)

```bash
npm run generate
```

### Preview production build

```bash
npm run preview
```

---

## Backend (`backend`)

The backend is a .NET solution defined in `TeamPulse.sln`.

### Open in Visual Studio / Rider

1. Open `backend/TeamPulse.sln` in your IDE.
2. Set `TeamPulse.Api` as the startup project.
3. Run the solution (Debug/Run from your IDE).

### Run from the command line

From the repository root:

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/TeamPulse.Api/TeamPulse.Api.csproj
```

By default, the API will start on the port configured in `TeamPulse.Api` (commonly `http://localhost:5000` or similar, depending on your launch settings).

---

## Running Frontend & Backend Together

1. **Start the backend API** (via IDE or `dotnet run` as above).
2. **Start the frontend dev server**:

   ```bash
   cd frontend
   npm run dev
   ```

3. Access the app in your browser at the frontend URL (typically `http://localhost:3000`), which will make requests to the running backend API.

