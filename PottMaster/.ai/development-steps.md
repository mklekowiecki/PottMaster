# Development Steps for PottMaster MVP

This document outlines the step-by-step development process for building the PottMaster Android application. It follows the MVP scope defined in the PRD and user stories, starting from the current project state. The steps are organized by phases, with references to relevant user stories (US-XXX) and technical guidelines.

**Current Project State** (as of 2025-12-30):
- ✅ Project structure initialized (Android template with Jetpack Compose)
- ✅ PRD and memory bank documentation complete
- ✅ Technology stack defined (Kotlin, SQLDelight, Supabase, Koin)
- ✅ Supabase backend setup (schema, RLS policies, Edge functions: create-work, sync-batch)
- ⏳ Basic UI structure (AuthComponent, MainComponent, Root navigation)
- ⏳ Authentication implementation (email/password login/register with Supabase)
- 🔄 In Progress: Phase 1 - Environment Setup (Koin DI, SQLDelight Local DB, Internationalization Setup)
- ⏳ Upcoming: Phase 2 Authentication, Work Lifecycle, Synchronization, Inventory, Wiki, Analytics

**Estimated Timeline**: < 3 months to v1.0 (target: ~68 developer days, compressible with team parallelization)
**Success Metrics**: Sync success >99.9%, 500 active users in Month 1, +5% Yield Rate improvement
---

## Phase 1: Environment Setup and Foundation (1-2 weeks)
Focus: Ensure reproducible development environment and core infrastructure.

1. **Clone and Initialize Project**
   - Clone the repository: `git clone <repo-url>`
   - Open in Android Studio (or VS Code with Kotlin plugin)
   - Sync Gradle: `./gradlew build` (ensure no errors)
   - Reference: [guidelines.md](.kilocode/rules/memory-bank/guidelines.md) for project structure

2. **Supabase Configuration**
   - Set up local Supabase project: Follow `.ai/supabase-connection-setup.md` (create if missing)
   - Add Supabase URL and anon key to `local.properties` (Android) and `SupabaseConfig.kt`
   - Run migrations: `supabase db push` (applies initial schema and RLS policies)
   - Test Edge Functions: Deploy `create-work` and `sync-batch` via Supabase CLI
   - Verify RLS: US-012 (Data Security)

3. **Dependency Injection Setup (Koin)**
   - Implement Koin modules in `di/AppModule.kt`
   - Initialize in `MainActivity.kt`: `startKoin { modules(appModule) }`
   - Add modules for SupabaseClient, Repositories, UseCases
   - Reference: ADR-007 (Koin for DI), [architecture.md](.kilocode/rules/memory-bank/architecture.md)

4. **Local Database Setup (SQLDelight)**
   - Generate SQLDelight code: `./gradlew generateSqldelightInterfaces`
   - Create database file: `DatabaseDriverFactory` in platform-specific code
   - Implement initial schema in `.sq` files (works, glazes, sync_queue)
   - Test offline queries: Insert/read a test record
   - Reference: ADR-003 (SQLDelight), US-007 (Offline Operation)

5. **Internationalization Setup**
   - Add string resources for supported languages (pl, en, de, es, fr) in `res/values-*/strings.xml`
   - Implement language preference storage in UserPreferences (local DB)
   - Update UI to use `stringResource()` for all text
   - Reference: US-013 (Multi-Language UI), ADR-008 (Compose Resources)

**Milestone**: Run app with basic navigation and auth screen (no crashes, offline mode works).

---

## Phase 2: Authentication and User Management (1 week)
Focus: Secure user onboarding and profile management.

1. **Implement Auth Screens**
   - Create Login/Register Composables using Supabase Auth (email/password, Google/Apple)
   - Handle JWT storage (EncryptedSharedPreferences on Android)
   - Auto-refresh tokens via Supabase SDK
   - Reference: US-001 (User Registration/Login)

2. **User Profile Integration**
   - Fetch/store user profile (initials, preferences) from Supabase after login
   - Sync profile to local DB on first login
   - Update UI theme/language based on preferences
   - Reference: Data model in [architecture.md](.kilocode/rules/memory-bank/architecture.md)

3. **Logout and Session Management**
   - Implement logout: Clear local data, revoke Supabase session
   - Handle token expiration: Redirect to login

**Milestone**: Users can register/login/logout; profile data syncs correctly (test with Supabase dashboard).

---

## Phase 3: Work Lifecycle Management (2-3 weeks)
Focus: Core feature for registering and tracking ceramic works (offline-first).

1. **Work Registration (US-002)**
   - Create WorkRegistrationScreen: Photo capture (CameraX), category selector, wall thickness slider
   - Compress images client-side (Bitmap.compress, <500KB, 1920x1920 max)
   - Save to local SQLite: Generate UUID, set status=WET, syncStatus=PENDING
   - Reference: ADR-006 (Client-Side Image Compression), [glossary.md](.kilocode/rules/memory-bank/glossary.md) for categories/statuses

2. **Unique Code Generation (US-003)**
   - Implement code generator: `{initials}-{CATEGORY}-{MMYY}-{counter}` (local counter, resets monthly)
   - Call Supabase Edge Function `create-work` for cloud uniqueness check (fallback to local if offline)
   - Display code immediately in UI; physically mark on clay
   - Handle duplicates: Append suffix if conflict during sync

3. **Timer Management (US-004)**
   - Calculate drying time: Based on wall thickness (4-14 days algorithm)
   - Use kotlinx-datetime for countdown: Update every minute via WorkManager (background)
   - Display progress: Days remaining, progress bar in WorkListScreen
   - Start drying timer on creation (dryingStartedAt = now)

4. **Status Updates (US-005)**
   - Create WorkDetailScreen: Show photo, code, timer, status
   - Allow manual advancement: WET → LEATHER_HARD → BONE_DRY → etc. (no reverse)
   - Confirmation dialog; update local DB, mark PENDING for sync
   - Handle discard: Set status=DISCARDED

5. **Work List UI**
   - Implement WorkListScreen: LazyColumn of WorkCards (photo, code, status, sync icon)
   - Sort by createdAt descending; filter by status/category
   - Pull-to-refresh triggers manual sync

**Milestone**: Create/track works offline; timers update; statuses advance (test drying calculations).

---

## Phase 4: Offline Synchronization (1-2 weeks)
Focus: Seamless data sync between local and cloud.

1. **Background Sync Implementation (US-008)**
   - Create SyncManager: Use WorkManager for periodic sync (every 15min when online)
   - Triggers: App foreground, network change, manual refresh
   - Process sync_queue: Batch uploads via Edge Function `sync-batch`
   - Handle photos: Upload to Supabase Storage, update paths

2. **Conflict Resolution**
   - Implement LWW: Compare updated_at timestamps (local vs remote)
   - On conflict: Push local if newer; pull remote otherwise
   - Update syncStatus: PENDING → SYNCING → SYNCED/ERROR
   - Retry logic: Exponential backoff for failures

3. **Network Monitoring**
   - Use ConnectivityManager (Android) to detect online/offline
   - Show offline indicator in UI; queue operations when offline

**Milestone**: Create works offline, sync when online; verify data in Supabase (no conflicts in single-device test).

---

## Phase 5: Material Inventory and Wiki (1-2 weeks)
Focus: Glaze management and public knowledge base.

1. **Glaze Inventory (US-006)**
   - Create GlazeInventoryScreen: Add/edit glazes (name, manufacturer, cone, quantity, notes)
   - Many-to-many link: Assign glazes to works (in WorkDetailScreen)
   - Store in local DB; sync to Supabase (RLS-protected)

2. **Wiki Browsing (US-009)**
   - Implement WikiScreen: Full-text search on public wiki_materials table
   - Fetch via Supabase SDK (read-only for verified entries)
   - Display: Material details, trust tags (verified/community)
   - Offline caching: Store recent searches in local DB

3. **Wiki Submission (Basic, Post-MVP Extension)**
   - Allow authenticated inserts to wiki_materials (pending verification)
   - Reference: US-010 (Expert Verification) for future admin panel

**Milestone**: Add/link glazes to works; browse/search wiki (online); data syncs correctly.

---

## Phase 6: Analytics and Polish (1 week)
Focus: Reporting and final touches.

1. **Pottery Wrapped Report (US-011)**
   - Create ReportsScreen: Monthly stats from local DB (yield rate, completed vs total)
   - Visualize: Pie chart (Compose Charts lib), export as image
   - Calculate: (Completed / Total) * 100; most used category

2. **UI/UX Polish**
   - Add loading states, error handling (snackbars)
   - Theme support: Light/dark/system (US-013)
   - Accessibility: Content descriptions for images, semantic labels

3. **Testing**
   - Unit tests: UseCases, Repositories (JUnit, KotlinTest)
   - UI tests: Compose UI testing for screens
   - Integration: Offline/online sync scenarios
   - Reference: [guidelines.md](.kilocode/rules/memory-bank/guidelines.md) Testing Strategy

4. **Documentation Updates**
   - Update memory bank: Add new ADRs if decisions made
   - Write user guide in app (onboarding screens)

**Milestone**: Generate reports; app passes tests; ready for beta testing.

---

## Phase 7: Deployment and Launch (1 week)
Focus: Release preparation.

1. **Build and Signing**
   - Generate release APK/AAB: `./gradlew assembleRelease`
   - Set up Google Play Console (if applicable)
   - Obfuscation: Enable ProGuard/R8

2. **Monitoring and Analytics**
   - Integrate crash reporting (Firebase Crashlytics or Sentry)
   - Track metrics: Sync success, user growth

3. **Launch Checklist**
   - Verify all US-001 to US-013 acceptance criteria
   - Beta test: 10-20 ceramic artists
   - Monitor initial sync rates and yield improvements

**Post-Launch**: Gather feedback, plan Phase 2 (real-time, advanced analytics).

---

## General Development Guidelines
- **Offline-First**: Always write to local DB first; sync in background ([architecture.md](.kilocode/rules/memory-bank/architecture.md))
- **Clean Architecture**: Presentation → Domain → Data layers
- **Code Review**: Follow Git workflow in [guidelines.md](.kilocode/rules/memory-bank/guidelines.md)
- **Tools**: Android Studio, Supabase CLI, SQLDelight plugin
- **Branching**: `feature/us-XXX-description` for each story
- **Commit Messages**: `feat: Implement US-002 work registration`

For questions, reference the memory bank or consult the Technical Lead.

**Last Updated**: 2025-12-30  
**Document Owner**: Development Team  
**Review Cycle**: Bi-weekly during MVP