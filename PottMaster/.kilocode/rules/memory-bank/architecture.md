# Technical Architecture - PottMaster

## Architecture Overview

PottMaster follows an **offline-first, cross-platform architecture** using .NET MAUI.

```
┌─────────────────────────────────────────────────────────────┐
│                     Mobile Clients                          │
│  ┌──────────────────────┐                                  │
│  │   .NET MAUI App      │                                  │
│  │     (XAML)           │                                  │
│  └──────────┬───────────┘                                  │
│             │                                              │
│             │                                              │
│             │                                              │
│         ┌──────────────▼──────────────────┐                 │
│         │   Business Logic                │                 │
│         │  - ViewModels                   │                 │
│         │  - Services                     │                 │
│         │  - Repositories                 │                 │
│         └──────────────┬──────────────────┘                 │
│                        │                                     │
│         ┌──────────────▼──────────────────┐                 │
│         │   Local Data Layer (SQLite)     │                 │
│         │   - SQLite-net-pcl              │                 │
│         │   - Offline-first storage       │                 │
│         │   - Sync queue management       │                 │
│         └──────────────┬──────────────────┘                 │
└────────────────────────┼──────────────────────────────────┘
                         │
                         │ Network Available
                         │
         ┌───────────────▼──────────────────┐
         │      Supabase Backend            │
         │  ┌────────────────────────────┐  │
         │  │  Supabase Auth             │  │
         │  │  - Email/Password          │  │
         │  │  - Google Sign-In          │  │
         │  │  - Apple Sign-In           │  │
         │  └────────────────────────────┘  │
         │  ┌────────────────────────────┐  │
         │  │  PostgreSQL Database       │  │
         │  │  - Row Level Security      │  │
         │  │  - User data isolation     │  │
         │  │  - Public Wiki tables      │  │
         │  └────────────────────────────┘  │
         │  ┌────────────────────────────┐  │
         │  │  Supabase Storage          │  │
         │  │  - Image uploads           │  │
         │  │  - Client-side compression │  │
         │  └────────────────────────────┘  │
         └─────────────────────────────────┘
```

## Technology Stack

### Mobile Layer

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Logic** | C# | Business logic, data models, repositories |
| **UI Framework** | .NET MAUI (XAML) | Cross-platform native UI |
| **Navigation** | Shell Navigation | Screen routing |
| **Dependency Injection** | CommunityToolkit.Mvvm | MVVM with DI |
| **Localization** | .NET MAUI Localization | Multi-language support |

### Data Layer

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Local Database** | SQLite | Offline data storage |
| **Database Framework** | SQLite-net-pcl | SQLite ORM |
| **Serialization** | System.Text.Json | JSON parsing |
| **Image Handling** | .NET MAUI Image | Image loading & caching |

### Backend Layer

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **BaaS Platform** | Supabase | Backend as a Service |
| **Database** | PostgreSQL | Cloud data storage |
| **Authentication** | Supabase Auth | User management |
| **File Storage** | Supabase Storage | Image/file hosting |
| **Real-time** | Supabase Realtime | Future: live updates |

## API Layer

The REST API is implemented using Supabase .NET Client (Supabase-CSharp) wrapped in SupabaseApi service for direct CRUD operations and Edge Functions for complex business logic. The client is initialized using dependency injection with platform-specific configuration.

### Supabase Client Setup
- **Configuration**: Uses configuration object to provide project URL and anon key.
  - Loaded from appsettings or secure storage.
- **Initialization**:
  ```csharp
  // In MauiProgram.cs
  builder.Services.AddSingleton(new Supabase.Client(supabaseUrl, supabaseKey));
  // Configure auth, database, storage
  ```
- **Direct SDK Endpoints**: Basic CRUD for user_profiles, works (GET, PATCH, DELETE), glazes, wiki_materials (GET, POST for submissions).
- **Edge Functions**:
  - `create-work`: POST /works - Generates unique identification code (Initials-CatCode-MMYY-Counter), inserts work with initial WET status.
  - `sync-batch`: POST /sync - Batch sync for pending works and glazes, LWW conflict resolution using updated_at, photo path updates.
  - `pottery-wrapped`: GET /reports/pottery-wrapped - Aggregates monthly yield rate and stats.
- **Integration**: Repositories inject `ISupabaseClient` and call Edge Functions via HTTP or SDK for DB ops. Client handles photo compression and upload to Storage before API calls.
- **Validation**: Server-side checks for code uniqueness, status progression, user ownership via RLS.

For detailed setup instructions, see `.ai/supabase-connection-setup.md`.

## Future Scalability

### Phase 2 Considerations
- Real-time collaboration (Supabase Realtime)
- Advanced analytics (separate analytics DB)
- CDN for image delivery
- Edge functions for complex operations
- GraphQL layer for flexible queries
- RTL language support (Arabic, Hebrew)

## References

- [Supabase Documentation](https://supabase.com/docs)
- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)

---

**Last Updated**: 2026-01-11
**Document Owner**: Technical Lead
**Review Cycle**: After major architectural changes
