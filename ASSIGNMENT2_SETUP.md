# Assignment 2 – Event Manager 2.0 — Setup Guide

Code changes are complete. What's left is Azure setup and deployment, which require your own Azure login, so you'll need to do these steps yourself.

## 1. Create the `assignment2` branch

```bash
git checkout -b assignment2
git add .
git commit -m "Assignment 2: EF Core, Azure SQL, Blob Storage, attribute routing, full CRUD"
git push -u origin assignment2
```

## 2. What was built

- **Models**: `Event` (added `Description`, `BannerUrl`) and `Attendee` (added `Id` (string), `EventId`, `Event` nav property).
- **Data/EventManagerContext.cs**: EF Core `DbContext` with `Events` and `Attendees` DbSets, cascade delete configured.
- **Data/DbInitializer.cs**: calls `EnsureCreated()` and seeds 3 events with 2 attendees each.
- **Services/BlobStorageService.cs**: uploads a banner image to Azure Blob Storage, creates the container as public-blob if needed, returns the blob URL.
- **Controllers/EventsController.cs**: full CRUD for events, attribute-routed at `/events`, `/events/{id}`, `/events/create`, `/events/{id}/edit`, `/events/{id}/delete`. Handles banner image upload on Create/Edit.
- **Controllers/AttendeesController.cs**: full CRUD for attendees nested under an event, attribute-routed at `/events/{eventId}/attendees`, `/events/{eventId}/attendees/create`, `/events/{eventId}/attendees/{id}/edit`, `/events/{eventId}/attendees/{id}/delete`.
- **Program.cs**: registers `EventManagerContext` with SQL Server, registers `BlobStorageService`, runs `DbInitializer` on startup.
- **appsettings.json**: placeholders added for `ConnectionStrings:DefaultConnection` and `BlobStorage:ConnectionString` / `BlobStorage:ContainerName`.
- **csproj**: added `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`, `Azure.Storage.Blobs`.

**Cleanup needed on your machine** (I couldn't delete files from this environment — please remove manually):
```bash
git rm Assignment1_EventSignup/Controllers/EventController.cs
git rm -r Assignment1_EventSignup/Views/Event
```
(`EventController.cs` currently just contains a comment saying it's obsolete — safe to delete, it's fully replaced by `EventsController.cs`.)

## 3. Create Azure SQL Database

Follow your course's Content → Extra Material → Azure Database guide, or roughly:

1. Azure Portal → Create a resource → SQL Database.
2. Create a new server (or reuse one), set admin login/password, choose a region.
3. Server firewall: enable "Allow Azure services and resources to access this server", and add your local IP for local testing.
4. Once created, go to the database → Connection strings → ADO.NET, copy it.
5. Paste it into `appsettings.json` under `ConnectionStrings:DefaultConnection` (replace the placeholder), or better, set it as a **User Secret** / App Service setting so you don't commit credentials:
   ```bash
   cd Assignment1_EventSignup
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
   ```

## 4. Create Azure Blob Storage

1. Azure Portal → Create a resource → Storage account.
2. Once created, go to Access keys → copy the connection string.
3. Set it as a user secret (recommended) or in `appsettings.json`:
   ```bash
   dotnet user-secrets set "BlobStorage:ConnectionString" "<your-storage-connection-string>"
   ```
4. The app creates the `event-banners` container automatically on first upload with public blob access — no manual container setup needed.

## 5. Run migrations / verify database

The app uses `EnsureCreated()` in `DbInitializer`, so no `dotnet ef migrations` step is strictly required — the schema is created automatically on first run and seeded with 3 events / 2 attendees each. If your instructor wants actual EF Core migrations instead:

```bash
dotnet tool install --global dotnet-ef   # if not already installed
dotnet ef migrations add InitialCreate
dotnet ef database update
```
(If you add migrations, remove `EnsureCreated()` from `DbInitializer.cs` and instead call `context.Database.Migrate()`.)

## 6. Run locally

```bash
cd Assignment1_EventSignup
dotnet restore
dotnet run
```
Browse to the URL shown (e.g. `https://localhost:5001/events`).

## 7. Deploy to Azure

1. Azure Portal → Create a resource → App Service (or reuse the publish profile already in `Properties/PublishProfiles`).
2. In Visual Studio: right-click project → Publish → select your App Service target → Publish.
3. In the App Service → Configuration → Application settings, add:
   - `ConnectionStrings__DefaultConnection` = your Azure SQL connection string
   - `BlobStorage__ConnectionString` = your storage connection string
   - `BlobStorage__ContainerName` = `event-banners`
   (Double underscore `__` is how App Service maps to nested config keys.)
4. Restart the App Service, browse to the site URL, confirm `/events` loads.

## 8. Screenshots to capture for submission

- Event list (`/events`)
- One event's Details page showing the banner image
- Attendee list for an event (`/events/{id}/attendees`)
- Azure SQL Database table data (Query editor in Azure Portal, or SSMS)

## 9. AI usage note

Since AI usage must be referenced in your submission, add a line such as:
> "Claude (Anthropic) was used to scaffold the EF Core DbContext, DbInitializer, Azure Blob Storage upload service, EventsController/AttendeesController with attribute routing, and associated Razor views for Assignment 2."
