# Media Inventory Manager

ASP.NET Core Web API + Razor Pages application for managing media products.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A modern browser

## Setup

1. Open the solution in Visual Studio 2022 or use the command line.
2. Restore NuGet packages (automatically done on build).
3. Apply database migrations:
4. Run the project:

5. Open the displayed URL (e.g., https://localhost:7210).

## Features

- **REST API**: `GET /api/products` (with pagination & search), `POST /api/products`
- **Image Upload**: `POST /api/upload` with file type validation, unique naming, auto‑resize to 800px width and WebP conversion
- **Server‑Side Rendering**: Product list on the homepage via ViewComponent
- **Client‑Side Pagination**: Smooth page changes with URL updates
- **Search**: Real‑time case‑insensitive search
- **In‑Memory Caching**: Product list cache cleared on new data

## Technologies

- ASP.NET Core 8.0
- Entity Framework Core + SQLite
- SixLabors.ImageSharp for image processing
- FluentValidation for request validation
- Tailwind CSS for UI styling