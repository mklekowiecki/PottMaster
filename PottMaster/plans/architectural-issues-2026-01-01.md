# PottMaster - Architectural Issues Review
**Date**: 2026-01-01  
**Reviewer**: Architect Mode  
**Focus**: Architectural weaknesses and design flaws in current implementation

---

## Executive Summary

The PottMaster solution has a solid foundation but contains **critical architectural issues** that will cause problems in production, particularly around offline-first architecture, data synchronization, and separation of concerns. These issues must be addressed before MVP release.

---

## 🔴 Critical Issues

### 1. **Broken Offline-First Architecture**

**Problem**: The architecture claims to be "offline-first" but has fundamental design flaws:

#### 1.1 Database Schema Mismatch
- **Local SQLite**: Uses `int` auto-increment primary keys ([`LocalWork.cs:8-9`](Models/LocalWork.cs:8))
- **Remote PostgreSQL**: Uses `UUID` primary keys ([`initial_schema.sql:69`](supabase/migrations/20251229171538_initial_schema.sql:69))
- **Impact**: Cannot sync local records to remote database - ID collision guaranteed

```csharp
// LocalWork.cs - WRONG
[PrimaryKey, AutoIncrement]
public int Id { get; set; }  // ❌ int auto-increment

// PostgreSQL schema - CORRECT
id uuid primary key default uuid_generate_v4()  // ✅ UUID
```

**Solution Required**: 
- Change [`LocalWork.Id`](Models/LocalWork.cs:8) to `string` (UUID)
- Generate UUIDs client-side using `Guid.NewGuid()`
- Ensure consistent schema between local and remote

#### 1.2 Missing Sync Infrastructure
- **No sync queue implementation** in client code
- **No conflict resolution logic** (LWW mentioned in docs but not implemented)
- **No retry mechanism** for failed syncs
- **No network detection** logic
- **No background sync service**

**Current State**: [`WorkService.cs`](Services/WorkService.cs) only writes to SQLite, never syncs to Supabase.

**Solution Required**:
- Implement `ISyncService` with queue management
- Add network connectivity monitoring
- Implement background sync worker
- Add conflict resolution using `updated_at` timestamps

#### 1.3 User Profile Storage Inconsistency
[`NewWorkViewModel.cs:150`](ViewModels/NewWorkViewModel.cs:150) tries to fetch user profile from **local SQLite**:
```csharp
var profile = await _dbService.GetByIdAsync<UserProfiles>(user.Id);
```

But [`UserProfiles`](Models/UserProfiles.cs) is **never stored locally** - it only exists in Supabase PostgreSQL.

**Impact**: Work creation will fail when offline (cannot get user initials).

**Solution Required**:
- Create `LocalUserProfile` table in SQLite
- Cache user profile locally after login
- Update profile retrieval logic

---

### 2. **Supabase Client Initialization Issues**

#### 2.1 Synchronous Initialization in Constructor
[`AuthService.cs:15-16`](Services/AuthService.cs:15):
```csharp
_client = new Supabase.Client(Constants.SupabaseBaseUrl, Constants.SupabaseAnonKey);
_client.InitializeAsync();  // ❌ Fire-and-forget async call
```

**Problems**:
- `InitializeAsync()` is called but not awaited
- No guarantee client is ready when methods are called
- Race conditions in authentication flow
- No error handling for initialization failures

**Solution Required**:
```csharp
private readonly Lazy<Task<Client>> _clientTask;

public AuthService()
{
    _clientTask = new Lazy<Task<Client>>(async () =>
    {
        var client = new Client(Constants.SupabaseBaseUrl, Constants.SupabaseAnonKey);
        await client.InitializeAsync();
        return client;
    });
}

private async Task<Client> GetClientAsync() => await _clientTask.Value;
```

#### 2.2 Hardcoded Local Development URLs
[`Constants.cs:5-6`](Constants.cs:5):
```csharp
public const string SupabaseBaseUrl = "http://127.0.0.1:54321";
public const string SupabaseAnonKey = "sb_publishable_ACJWlzQHlZjBrEguHvfOxg_3BJgxAaH";
```

**Problems**:
- Hardcoded localhost URL won't work on mobile devices
- No environment-based configuration
- Anon key exposed in source code
- No production/staging/development separation

**Solution Required**:
- Use platform-specific secure storage for keys
- Implement configuration service with environment detection
- Use `appsettings.json` or environment variables
- Add `.gitignore` for sensitive configuration

---

### 3. **Missing Repository Pattern Implementation**

**Problem**: Architecture document mentions "Repository Pattern" but implementation is incomplete.

#### Current State:
- [`WorkService.cs`](Services/WorkService.cs) mixes business logic with data access
- No clear separation between local and remote data sources
- No abstraction for sync operations

#### What's Missing:
```
IWorkRepository (interface)
├── LocalWorkRepository (SQLite implementation)
├── RemoteWorkRepository (Supabase implementation)
└── SyncWorkRepository (orchestrates both)
```

**Impact**:
- Difficult to test (tight coupling to SQLite)
- Cannot swap data sources
- Sync logic will be scattered across services

**Solution Required**:
- Create proper repository interfaces
- Implement local and remote repositories separately
- Add repository coordinator for sync operations

---

### 4. **Data Model Inconsistencies**

#### 4.1 Dual Model Problem
Two separate models for the same entity:
- [`Work.cs`](Models/Work.cs) - Domain model (string Id)
- [`LocalWork.cs`](Models/LocalWork.cs) - SQLite model (int Id)

**Problems**:
- Manual mapping required ([`WorkService.cs:105-143`](Services/WorkService.cs:105))
- Easy to forget mapping new properties
- Increased maintenance burden
- Potential for mapping bugs

**Better Approach**:
- Single domain model with SQLite attributes
- Use `[Ignore]` for computed properties
- Eliminate manual mapping code

#### 4.2 Missing Sync Metadata
[`LocalWork.cs`](Models/LocalWork.cs) has `SyncStatus` but missing:
- `LocalId` (for tracking before sync)
- `RemoteId` (UUID from server)
- `LastSyncAttempt` (for retry logic)
- `SyncError` (for debugging)
- `ConflictResolution` (for handling conflicts)

---

### 5. **Authentication State Management**

#### 5.1 No Persistent Session
[`AuthService.cs`](Services/AuthService.cs) doesn't persist authentication state:
- User must re-login after app restart
- No token refresh mechanism
- No session restoration

**Solution Required**:
- Store session tokens in secure storage
- Implement automatic token refresh
- Add session restoration on app startup

#### 5.2 No Authentication State Propagation
- No global authentication state management
- ViewModels check auth independently
- No centralized "user logged out" event handling

**Solution Required**:
- Implement authentication state service
- Use messaging/events for auth state changes
- Add authentication guard for protected pages

---

### 6. **Dependency Injection Issues**

#### 6.1 Service Provider Anti-Pattern
[`App.xaml.cs`](App.xaml.cs) (assumed based on [`MauiProgram.cs:52`](MauiProgram.cs:52)):
```csharp
App.SetServiceProvider(mauiApp.Services);
```

This suggests static service locator pattern, which is an anti-pattern.

**Problems**:
- Defeats purpose of DI
- Makes testing difficult
- Hides dependencies
- Violates SOLID principles

**Solution Required**:
- Remove static service provider
- Inject dependencies properly through constructors
- Use Shell navigation with DI

#### 6.2 Missing Scoped Services
All services registered as `Singleton` ([`MauiProgram.cs:30-32`](MauiProgram.cs:30)):
```csharp
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IDbService, DbService>();
builder.Services.AddSingleton<IWorkService, WorkService>();
```

**Problems**:
- Shared state across entire app lifetime
- Potential memory leaks
- Cannot isolate user sessions
- Difficult to test

**Solution Required**:
- Use `AddScoped` for user-specific services
- Use `AddTransient` for stateless services
- Keep `AddSingleton` only for truly global services (DbService)

---

### 7. **Error Handling Architecture**

#### 7.1 Inconsistent Error Handling
Multiple error handling patterns:
- Try-catch with string messages ([`NewWorkViewModel.cs:174-178`](ViewModels/NewWorkViewModel.cs:174))
- Custom `AuthResponse<T>` wrapper ([`AuthService.cs:19-34`](Services/AuthService.cs:19))
- No error handling in some methods

**Problems**:
- No centralized error logging
- User sees technical error messages
- No error recovery strategy
- Difficult to debug production issues

**Solution Required**:
- Implement `Result<T>` pattern consistently
- Add centralized error logging service
- Create user-friendly error messages
- Add error recovery mechanisms

#### 7.2 No Offline Error Handling
No strategy for handling operations when offline:
- What happens when user tries to sync without network?
- How are failed operations queued?
- How does user know sync failed?

---

### 8. **Photo Management Issues**

#### 8.1 No Image Compression
[`NewWorkViewModel.cs:114-121`](ViewModels/NewWorkViewModel.cs:114) saves photos without compression:
```csharp
private async Task SavePhotoAsync(FileResult photo)
{
    var localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
    using var stream = await photo.OpenReadAsync();
    using var newStream = File.OpenWrite(localFilePath);
    await stream.CopyToAsync(newStream);  // ❌ No compression
    PhotoPath = localFilePath;
}
```

**Problems**:
- Large photos (5-10MB) stored uncompressed
- Will fill device storage quickly
- Slow sync when online
- Violates architecture requirement: "Client-side compression required"

**Solution Required**:
- Implement image compression service
- Resize to max 1920x1920
- Compress to 80% quality
- Target <500KB per image

#### 8.2 No Photo Sync Strategy
- Photos stored locally but no upload logic
- No tracking of which photos are synced
- No cleanup of synced photos
- No handling of upload failures

---

### 9. **Code Generation Logic Issues**

#### 9.1 Race Condition in Counter
[`WorkService.cs:58-64`](Services/WorkService.cs:58):
```csharp
var existingWorks = await _dbService.GetWorksByUserIdAsync(work.UserId);
var filteredWorks = existingWorks
    .Where(w => w.Code.StartsWith($"{userInitials}-{categoryCode}-{monthYear}-"))
    .ToList();
var counter = filteredWorks.Count + 1;  // ❌ Race condition
```

**Problem**: If two works are created simultaneously (unlikely but possible), they could get the same counter.

**Solution Required**:
- Use database transaction with lock
- Or use atomic increment operation
- Or generate UUID-based codes

#### 9.2 Code Generation in Wrong Layer
Code generation is in [`WorkService`](Services/WorkService.cs) but architecture says it should be in Edge Function `create-work`.

**Inconsistency**: 
- Local: Client generates code
- Remote: Server generates code (per architecture doc)
- Result: Potential code conflicts

---

### 10. **Missing Cross-Cutting Concerns**

#### 10.1 No Logging Infrastructure
- No structured logging
- Only `Debug.WriteLine` statements
- Cannot diagnose production issues
- No log aggregation

**Solution Required**:
- Implement `ILogger` throughout
- Add log levels (Debug, Info, Warning, Error)
- Consider remote logging service

#### 10.2 No Analytics/Telemetry
- No crash reporting
- No performance monitoring
- No user behavior tracking
- Cannot measure success metrics

#### 10.3 No Validation Layer
- Validation scattered in ViewModels
- No centralized validation rules
- No server-side validation
- Easy to bypass validation

---

## 🟡 Design Concerns

### 11. **Localization Implementation**

#### 11.1 Hardcoded Polish Strings
[`NewWorkViewModel.cs:128-134`](ViewModels/NewWorkViewModel.cs:128):
```csharp
await Shell.Current.DisplayAlert("Walidacja", "Prosz? wybra? kategori?", AppResources.Ok);
```

**Problems**:
- Mixed Polish and resource strings
- Inconsistent localization approach
- Some strings not in resource files

**Solution Required**:
- Move ALL strings to `AppResources.resx`
- Remove hardcoded Polish strings
- Ensure consistent localization

#### 11.2 Culture Set Globally
[`MauiProgram.cs:47-49`](MauiProgram.cs:47):
```csharp
CultureInfo culture = new CultureInfo("pl"); 
Thread.CurrentThread.CurrentCulture = culture;
Thread.CurrentThread.CurrentUICulture = culture;
```

**Problems**:
- Forces Polish for all users
- Ignores device language settings
- No user preference support

**Solution Required**:
- Respect device culture by default
- Allow user to override in settings
- Store preference in user profile

---

### 12. **Testing Concerns**

#### 12.1 No Test Infrastructure
- No unit tests
- No integration tests
- No UI tests
- Cannot verify offline-first behavior

#### 12.2 Difficult to Test
- Static dependencies
- Tight coupling
- No interfaces for external dependencies
- No mocking strategy

---

## 📋 Architecture Violations Summary

| Issue | Severity | Documented | Implemented | Impact |
|-------|----------|------------|-------------|--------|
| Offline-First | 🔴 Critical | ✅ Yes | ❌ No | App won't work offline |
| UUID Primary Keys | 🔴 Critical | ✅ Yes | ❌ No | Sync will fail |
| Repository Pattern | 🔴 Critical | ✅ Yes | ⚠️ Partial | Poor separation |
| Image Compression | 🔴 Critical | ✅ Yes | ❌ No | Storage issues |
| Sync Service | 🔴 Critical | ✅ Yes | ❌ No | No cloud sync |
| Auth Persistence | 🟡 High | ⚠️ Implied | ❌ No | Poor UX |
| Error Handling | 🟡 High | ❌ No | ⚠️ Partial | Hard to debug |
| Logging | 🟡 High | ❌ No | ❌ No | No diagnostics |

---

## 🎯 Recommended Action Plan

### Phase 1: Fix Critical Blockers (Before MVP)
1. **Fix database schema mismatch** (UUID primary keys)
2. **Implement sync service** with queue and retry
3. **Add image compression** service
4. **Fix Supabase client initialization**
5. **Implement user profile caching**

### Phase 2: Improve Architecture (MVP+)
6. **Refactor to proper repository pattern**
7. **Add authentication state management**
8. **Implement centralized error handling**
9. **Add logging infrastructure**
10. **Fix DI anti-patterns**

### Phase 3: Production Readiness (Post-MVP)
11. **Add comprehensive testing**
12. **Implement analytics/telemetry**
13. **Add validation layer**
14. **Performance optimization**
15. **Security audit**

---

## 📚 References

- [Architecture Document](../.kilocode/rules/memory-bank/architecture.md)
- [PRD](../.kilocode/rules/memory-bank/brief.md)
- [User Stories](../.kilocode/rules/memory-bank/user-stories.md)
- [MAUI Guidelines](../.kilocode/rules/maui-guidelines.md)

---

**Next Steps**: Prioritize Phase 1 issues and create detailed implementation tasks for each fix.
