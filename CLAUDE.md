# SewQ

Offline-first sewing project costing app (.NET MAUI Blazor Hybrid, net10.0).
Implements the Claude Design handoff `Central project workspace design-handoff.zip`
(phone mockup `SewQ.dc.html`) — match that design when touching UI.

## Solution layout

- `src/SewQ.App` — MAUI Blazor Hybrid UI (`net10.0-android`, `net10.0-windows10.0.19041.0`).
  Composition root; pages in `Components/Pages`, UI state in `State/`.
- `src/SewQ.Services` — contracts (`Interfaces/`, `Models/`) + internal implementations + `AddSewQServices()`.
- `src/SewQ.Data` — EF Core: entities, `SewQDbContext`, `Migrations/`, design-time factory.

**Locked architecture rule:** the UI consumes only interfaces and DTOs from
`SewQ.Services`; concrete services stay `internal`; EF entities never cross into the UI.
Entities are deliberately stringly-typed for enums (status/theme/currency) so
`SewQ.Data` doesn't depend on the contracts assembly; `EnumMap` translates.

## Database

- SQLite at `FileSystem.AppDataDirectory/sewq.db3`; all PKs are Guids.
- Schema changes go through real migrations — never `EnsureCreated()`.
  `IDatabaseInitializer.Initialize()` runs `Database.Migrate()` at startup (MauiProgram).
- Add a migration:
  `dotnet ef migrations add <Name> --project src/SewQ.Data --startup-project src/SewQ.Data`
- SQLitePCLRaw is pinned to 3.x in SewQ.Data (2.1.11 pulled by EF has a known CVE).

## Build / run

- Windows: `dotnet build src/SewQ.App -f net10.0-windows10.0.19041.0`
  (unpackaged; exe under `bin/Debug/net10.0-windows10.0.19041.0/win-x64/`)
- Android: `dotnet build src/SewQ.App -f net10.0-android`

## UI conventions

- All styling lives in `wwwroot/app.css`, ported from the design prototype;
  3 themes × dark mode are CSS-variable blocks (`theme-a/b/c` + `.dark`) applied
  by `MainLayout` via `ThemeState`. No Bootstrap.
- Fonts are bundled variable TTFs in `wwwroot/fonts` (offline app — no CDN links).
- Money formatting/labels come from `SewQ.Services.Models.Display` extensions;
  form decimal parsing accepts both `,` and `.` (`UiText.ParseDecimal`).
