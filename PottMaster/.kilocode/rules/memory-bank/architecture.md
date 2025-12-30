# Technical Architecture - PottMaster

## Architecture Overview

PottMaster follows an **offline-first, mobile-centric architecture** using Kotlin for Android.

```
┌─────────────────────────────────────────────────────────────┐
│                     Mobile Clients                          │
│  ┌──────────────────────┐                                  │
│  │   Android App        │                                  │
│  │  (Jetpack Compose)   │                                  │
│  └──────────┬───────────┘                                  │
│             │                                              │
│             │                                              │
│             │                                              │
│         ┌──────────────▼──────────────────┐                 │
│         │   Android Business Logic        │                 │
│         │  - ViewModels                   │                 │
│         │  - Use Cases                    │                 │
│         │  - Repositories                 │                 │
│         └──────────────┬──────────────────┘                 │
│                        │                                     │
│         ┌──────────────▼──────────────────┐                 │
│         │   Local Data Layer (SQLite)     │                 │
│         │   - SQLDelight                  │                 │
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
| **Logic** | Kotlin | Business logic, data models, repositories |
| **UI Framework** | Jetpack Compose | Native Android UI |
| **Navigation** | Compose Navigation | Screen routing |
| **Dependency Injection** | Koin | DI framework for Android |
| **Localization** | Android Resources | Multi-language support |

### Data Layer

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Local Database** | SQLite | Offline data storage |
| **Database Framework** | SQLDelight | Type-safe SQL queries |
| **Serialization** | Kotlinx Serialization | JSON parsing |
| **Image Handling** | Coil | Image loading & caching |

### Backend Layer

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **BaaS Platform** | Supabase | Backend as a Service |
| **Database** | PostgreSQL | Cloud data storage |
| **Authentication** | Supabase Auth | User management |
| **File Storage** | Supabase Storage | Image/file hosting |
| **Real-time** | Supabase Realtime | Future: live updates |

## API Layer

The REST API is implemented using Supabase Kotlin SDK (`supabase-kt` v2.4.0) for direct CRUD operations and Edge Functions for complex business logic. The client is initialized in Koin DI module using platform-specific configuration.

### Supabase Client Setup
- **Configuration**: Uses `SupabaseConfig` object to provide project URL and anon key.
  - Android: Injected via `BuildConfig` from `local.properties`.
- **Initialization**:
  ```kotlin
  single<SupabaseClient> {
      createSupabaseClient(
          supabaseUrl = SupabaseConfig.url,
          supabaseKey = SupabaseConfig.anonKey
      ) {
          install(GoTrue)
          // Future: install(Postgrest, Storage, Realtime)
      }
  }
  ```
- **Direct SDK Endpoints**: Basic CRUD for user_profiles, works (GET, PATCH, DELETE), glazes, wiki_materials (GET, POST for submissions).
- **Edge Functions**:
  - `create-work`: POST /works - Generates unique identification code (Initials-CatCode-MMYY-Counter), inserts work with initial WET status.
  - `sync-batch`: POST /sync - Batch sync for pending works and glazes, LWW conflict resolution using updated_at, photo path updates.
  - `pottery-wrapped`: GET /reports/pottery-wrapped - Aggregates monthly yield rate and stats.
- **Integration**: Repositories inject `SupabaseClient` and call Edge Functions via HTTP or SDK for DB ops. Client handles photo compression and upload to Storage before API calls.
- **Validation**: Server-side checks for code uniqueness, status progression, user ownership via RLS.

For detailed setup instructions, see `.ai/supabase-connection-setup.md`.


## Architecture Patterns

### 1. Clean Architecture (Layered)

```
┌─────────────────────────────────────┐
│     Presentation Layer              │
│  - Composables                      │
│  - ViewModels                       │
│  - UI State                         │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│     Domain Layer                    │
│  - Use Cases                        │
│  - Business Logic                   │
│  - Domain Models                    │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│     Data Layer                      │
│  - Repositories (Interface)         │
│  - Data Sources (Local/Remote)      │
│  - DTOs & Mappers                   │
└─────────────────────────────────────┘
```

### 2. Repository Pattern

Each data entity has a repository that:
- Abstracts data source (local SQLite vs remote Supabase)
- Implements offline-first logic
- Manages synchronization state
- Handles conflict resolution

```kotlin
interface WorkRepository {
    suspend fun getWorks(): Flow<List<Work>>
    suspend fun createWork(work: Work): Result<Work>
    suspend fun syncPendingWorks(): Result<Unit>
}

class WorkRepositoryImpl(
    private val localDataSource: WorkLocalDataSource,
    private val remoteDataSource: WorkRemoteDataSource,
    private val syncManager: SyncManager
) : WorkRepository {
    // Implementation
}
```

### 3. Offline-First Strategy

**Data Flow:**
1. **Write Operations**: Always write to local SQLite first
2. **Mark for Sync**: Flag record with `sync_status = PENDING`
3. **Background Sync**: When network available, sync to Supabase
4. **Update Status**: Mark as `sync_status = SYNCED` on success
5. **Read Operations**: Always read from local SQLite

**Sync States:**
```kotlin
enum class SyncStatus {
    PENDING,    // Created locally, not yet synced
    SYNCING,    // Currently being synced
    SYNCED,     // Successfully synced to cloud
    CONFLICT,   // Sync conflict detected
    ERROR       // Sync failed, will retry
}
```

## Data Models

### Core Entities

#### Work (Ceramic Piece)
```kotlin
data class Work(
    val id: String,                    // UUID
    val userId: String,                // Owner ID
    val code: String,                  // e.g., "MK-CUP-1224-001"
    val category: WorkCategory,        // CUP, BOWL, VASE, etc.
    val wallThickness: Int,            // in mm
    val photoPath: String?,            // Local or cloud path
    val status: WorkStatus,            // WET, LEATHER_HARD, etc.
    val createdAt: Instant,
    val dryingStartedAt: Instant?,
    val dryingCompletedAt: Instant?,
    val syncStatus: SyncStatus,
    val glazeIds: List<String>         // Applied glazes
)
```

#### Glaze
```kotlin
data class Glaze(
    val id: String,
    val userId: String,
    val name: String,
    val manufacturer: String?,
    val color: String?,
    val coneRating: String?,           // e.g., "Cone 6"
    val quantity: String?,             // Free text
    val notes: String?,
    val syncStatus: SyncStatus
)
```

#### User Profile
```kotlin
data class UserProfile(
    val id: String,                    // Supabase auth.uid()
    val email: String,
    val initials: String,              // For code generation
    val createdAt: Instant,
    val preferences: UserPreferences
)

data class UserPreferences(
    val language: String,              // ISO 639-1 code (e.g., "en", "pl", "de")
    val theme: String,                 // "light", "dark", "system"
    val notificationsEnabled: Boolean
)
```

## Database Schema

### SQLite (Local)

```sql
-- Works table
CREATE TABLE works (
    id TEXT PRIMARY KEY,
    user_id TEXT NOT NULL,
    code TEXT NOT NULL UNIQUE,
    category TEXT NOT NULL,
    wall_thickness INTEGER NOT NULL,
    photo_path TEXT,
    status TEXT NOT NULL,
    created_at INTEGER NOT NULL,
    drying_started_at INTEGER,
    drying_completed_at INTEGER,
    sync_status TEXT NOT NULL DEFAULT 'PENDING',
    updated_at INTEGER NOT NULL
);

-- Glazes table
CREATE TABLE glazes (
    id TEXT PRIMARY KEY,
    user_id TEXT NOT NULL,
    name TEXT NOT NULL,
    manufacturer TEXT,
    color TEXT,
    cone_rating TEXT,
    quantity TEXT,
    notes TEXT,
    sync_status TEXT NOT NULL DEFAULT 'PENDING',
    updated_at INTEGER NOT NULL
);

-- Work-Glaze junction table
CREATE TABLE work_glazes (
    work_id TEXT NOT NULL,
    glaze_id TEXT NOT NULL,
    PRIMARY KEY (work_id, glaze_id),
    FOREIGN KEY (work_id) REFERENCES works(id) ON DELETE CASCADE,
    FOREIGN KEY (glaze_id) REFERENCES glazes(id) ON DELETE CASCADE
);

-- Sync queue
CREATE TABLE sync_queue (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    entity_type TEXT NOT NULL,
    entity_id TEXT NOT NULL,
    operation TEXT NOT NULL,  -- INSERT, UPDATE, DELETE
    retry_count INTEGER DEFAULT 0,
    last_error TEXT,
    created_at INTEGER NOT NULL
);
```

### PostgreSQL (Supabase)

Similar schema with additional:
- Row Level Security (RLS) policies
- Indexes for performance
- Triggers for updated_at timestamps
- Public wiki tables (separate from user data)

## Security Architecture

### Row Level Security (RLS)

```sql
-- Example RLS policy for works table
CREATE POLICY "Users can only access their own works"
ON works
FOR ALL
USING (auth.uid() = user_id);

-- Public Wiki access
CREATE POLICY "Anyone can read verified wiki entries"
ON wiki_materials
FOR SELECT
USING (verified = true);

CREATE POLICY "Users can insert wiki entries"
ON wiki_materials
FOR INSERT
WITH CHECK (auth.uid() = submitted_by);
```

### Authentication Flow

1. User signs in via Supabase Auth
2. Receive JWT token
3. Store token securely (Keychain/Keystore)
4. Include token in all Supabase requests
5. Token auto-refresh handled by Supabase SDK

## Synchronization Strategy

### Conflict Resolution

**Strategy**: Last-Write-Wins (LWW) with `updated_at` timestamp

```kotlin
suspend fun resolveConflict(local: Work, remote: Work): Work {
    return if (local.updatedAt > remote.updatedAt) {
        // Local is newer, push to remote
        remoteDataSource.update(local)
        local
    } else {
        // Remote is newer, update local
        localDataSource.update(remote)
        remote
    }
}
```

### Sync Triggers

1. **App Foreground**: Check for pending syncs
2. **Network Available**: Background sync worker
3. **Manual Refresh**: User-initiated sync
4. **Periodic**: Every 15 minutes when app active

## Performance Considerations

### Image Optimization

```kotlin
// Client-side compression before upload
fun compressImage(bitmap: Bitmap): ByteArray {
    val outputStream = ByteArrayOutputStream()
    bitmap.compress(
        Bitmap.CompressFormat.JPEG,
        quality = 80,  // 80% quality
        outputStream
    )
    return outputStream.toByteArray()
}

// Max dimensions: 1920x1920
// Target size: < 500KB per image
```

### Database Indexing

```sql
-- Frequently queried fields
CREATE INDEX idx_works_user_status ON works(user_id, status);
CREATE INDEX idx_works_sync_status ON works(sync_status);
CREATE INDEX idx_glazes_user ON glazes(user_id);
```

## Internationalization Architecture

### String Resources

```
composeApp/src/main/res/
├── values/
│   └── strings.xml              # Default (Polish)
├── values-en/
│   └── strings.xml              # English
├── values-de/
│   └── strings.xml              # German
├── values-es/
│   └── strings.xml              # Spanish
└── values-fr/
    └── strings.xml              # French
```

### Language Selection Flow

```kotlin
// Store language preference
data class UserPreferences(
    val language: String = "pl"  // Default to Polish
)

// Access localized strings
@Composable
fun WorkListScreen() {
    val strings = stringResource(Res.string.work_list_title)
    // Use strings in UI
}

// Change language at runtime
fun setLanguage(languageCode: String) {
    // Update user preferences
    // Restart activity/recompose UI
}
```

### Localization Best Practices

1. **String Formatting**: Use placeholders for dynamic content
   ```xml
   <string name="drying_time_remaining">%d days remaining</string>
   ```

2. **Plurals**: Handle plural forms correctly
   ```xml
   <plurals name="works_count">
       <item quantity="one">%d work</item>
       <item quantity="other">%d works</item>
   </plurals>
   ```

3. **Date/Time Formatting**: Use locale-aware formatters
   ```kotlin
   val formatter = DateTimeFormatter.ofLocalizedDate(FormatStyle.MEDIUM)
       .withLocale(Locale.forLanguageTag(userPreferences.language))
   ```

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
- [SQLDelight Documentation](https://cashapp.github.io/sqldelight/)

---

**Last Updated**: 2025-12-30
**Document Owner**: Technical Lead
**Review Cycle**: After major architectural changes
