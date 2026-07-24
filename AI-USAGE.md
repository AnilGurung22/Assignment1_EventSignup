# AI Usage Disclosure

**Course:** CST8359 – ASP.NET Core MVC
**Assignment:** Assignment 2 – Event Manager 2.0
**Student:** Anil Gurung (041120678)
**Branch:** `assignment2`

Per the assignment instructions ("AI usage is allowed, but you need to reference where you use it"), this document records where AI tools were used in this project.

---

## Summary

The application was written by me. AI tools were used afterwards for code review, debugging, and guidance on Azure portal configuration — not to generate the initial implementation.

---

## Tools used

- **Claude (Anthropic)** — via the web chat interface
- **GitHub Copilot** — inline suggestions in Visual Studio 2022

---

## Work done by me

- Designed and wrote the `Event` and `Attendee` models
- Wrote `Data/EventManagerContext.cs`, including the `OnModelCreating` relationship configuration
- Wrote `Data/DbInitializer.cs` and chose all seed data
- Wrote `Services/IBlobStorageService.cs` and `Services/BlobStorageService.cs`
- Wrote `Controllers/EventsController.cs` and `Controllers/AttendeesController.cs`, including the attribute routing structure
- Wrote all Razor views under `Views/Events/` and `Views/Attendees/`
- Created and configured every Azure resource in the portal (SQL Database, Storage Account, container, firewall rules, budget)
- Ran all EF Core migrations and database commands
- Manually tested every CRUD path and the file upload flow
- Verified blob public accessibility via a direct URL in a private browser window
- Captured all submission screenshots

---

## Where AI was used

### 1. Debugging runtime errors

AI was used to interpret error output and identify fixes to code I had already written:

- **SQL error 40613** (`Database ... is not currently available`) — identified as the serverless auto-pause cold start. Fix applied: added `EnableRetryOnFailure` to the `UseSqlServer` call in `Program.cs`.
- **`FormatException: Settings must be of the form "name=value"`** on blob upload — traced to a configuration key mismatch. `BlobStorageService` reads `BlobStorage:ConnectionString` / `BlobStorage:ContainerName`, but my User Secrets had been written with `AzureBlob:` keys, and the placeholder value in `appsettings.json` was non-null so the null check did not catch it. Fix applied: aligned the key names.
- **`Update-Database` reporting "already up to date"** — identified as a missing `Migrations/` folder. Fix applied: ran `Add-Migration InitialCreate` first.
- **`/events` not resolving** — worked through the routing configuration, `Program.cs` pipeline, and startup logs to confirm the app was healthy and the issue was a stale browser state.

### 2. Code review suggestions

- Suggested changing `DbInitializer` from `context.Database.EnsureCreated()` to `context.Database.Migrate()`, so the schema is created through migrations rather than bypassing them — relevant both to the EF Core requirement and to first-run behaviour on a deployed instance.
- Identified an obsolete `Controllers/EventController.cs` left over from Assignment 1 that was no longer referenced, and it was removed from the branch.

### 3. Azure portal configuration guidance

AI was used as a reference while I set up the Azure resources:

- Locating the Azure SQL free-tier offer and confirming the correct settings (Serverless, overage billing disabled)
- Which firewall settings are required (client IP rule, plus the "Allow Azure services and resources to access this server" exception)
- Confirming that "Allow enabling anonymous access on individual containers" must be enabled at the storage account level before a container can be set to Blob-level anonymous read access
- Setting up a cost budget and confirming the subscription spending limit

All resources were created by me through the portal.

### 4. Git and deployment troubleshooting

- Resolving a branch name casing issue on Windows (`Assignment2` vs `assignment2`) that had produced duplicate branches on GitHub
- Interpreting the App Service publish failure (`SubscriptionIsOverQuotaForSku`, Current Limit: 0) and evaluating the available options

### 5. GitHub Copilot

Copilot was enabled in Visual Studio during development and provided inline autocomplete suggestions. Suggestions were accepted only where they matched what I was already writing.

---

## Note

All code in this repository was written and understood by me. Where AI identified a defect, I applied and tested the fix myself.
