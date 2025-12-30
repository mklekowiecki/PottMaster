# REST API Plan

## 1. Resources
- **user_profiles**: Corresponds to `public.user_profiles` table. Manages user details like initials and preferences.
- **works**: Corresponds to `public.works` table. Core resource for ceramic works, including code, status, and timers.
- **glazes**: Corresponds to `public.glazes` table. User's glaze inventory.
- **work_glazes**: Implicit via sub-resource on works/glazes; junction for linking works to glazes (`public.work_glazes`).
- **wiki_materials**: Corresponds to `public.wiki_materials` table. Public knowledge base entries.
- **categories**: Read-only lookup from `public.work_categories`.
- **statuses**: Read-only lookup from `public.work_statuses`.
- **material_types**: Read-only lookup from `public.wiki_material_types`.
- **reports**: Aggregated resource for analytics, derived from works.
- **sync**: Batch endpoint for synchronization, interacts with `public.sync_queue` indirectly.

Note: Dictionary resources (categories, statuses, material_types) are static lookups. sync_queue is internal, not exposed directly.

## 2. Endpoints

### user_profiles
- **HTTP Method**: GET  
  **URL Path**: `/user_profiles/me`  
  **Description**: Retrieve current user's profile.  
  **Query parameters**: None.  
  **JSON request payload structure**: N/A  
  **JSON response payload structure**: `{ "id": "uuid", "email": "string", "initials": "string", "preferences": { "language": "string", "theme": "string" }, "created_at": "timestamp" }`  
  **Success codes and messages**: 200 OK - Profile retrieved successfully.  
  **Error codes and messages**: 401 Unauthorized - Invalid token; 404 Not Found - Profile not found.

- **HTTP Method**: PUT  
  **URL Path**: `/user_profiles/me`  
  **Description**: Update current user's profile (e.g., initials, preferences).  
  **Query parameters**: None.  
  **JSON request payload structure**: `{ "initials": "string (max 10 chars)", "preferences": { "language": "string", "theme": "string", "notificationsEnabled": "boolean" } }`  
  **JSON response payload structure**: Updated profile object (same as GET).  
  **Success codes and messages**: 200 OK - Profile updated.  
  **Error codes and messages**: 400 Bad Request - Invalid initials length or preferences format; 401 Unauthorized.

### works
- **HTTP Method**: GET  
  **URL Path**: `/works`  
  **Description**: List user's works with pagination, filtering, sorting. Supports sync_status filtering for offline sync.  
  **Query parameters**: `?user_id=uuid` (implicit via auth), `status_id=smallint`, `sync_status=string` (PENDING|SYNCED|etc.), `category_id=smallint`, `page=integer (default 1)`, `limit=integer (default 20)`, `sort=string (created_at:desc|code:asc)`.  
  **JSON request payload structure**: N/A  
  **JSON response payload structure**: `{ "data": [ { "id": "uuid", "code": "string", "category": { "id": "smallint", "name": "string" }, "wall_thickness": "integer", "photo_path": "string", "status": { "id": "smallint", "name": "string" }, "created_at": "timestamp", "drying_started_at": "timestamp?", "drying_completed_at": "timestamp?", "sync_status": "string", "updated_at": "timestamp", "remaining_drying_time": "string (computed, e.g., '3 days')" } ], "pagination": { "page": "integer", "limit": "integer", "total": "integer" } }`  
  **Success codes and messages**: 200 OK - Works listed.  
  **Error codes and messages**: 400 Bad Request - Invalid query params; 401 Unauthorized.

- **HTTP Method**: POST  
  **URL Path**: `/works`  
  **Description**: Create a new work, generate/validate code, set initial status (WET), compute drying start, set sync_status=PENDING. Code format: Initials-CatCode-MMYY-Counter (server generates counter).  
  **Query parameters**: None.  
  **JSON request payload structure**: `{ "category_id": "smallint (required)", "wall_thickness": "integer (3-50 mm)", "photo_path": "string (local path for sync)" }` (initials from profile).  
  **JSON response payload structure**: Created work object (as in GET).  
  **Success codes and messages**: 201 Created - Work created with code.  
  **Error codes and messages**: 400 Bad Request - Invalid wall_thickness or category_id; 409 Conflict - Code uniqueness violation; 401 Unauthorized.

- **HTTP Method**: GET  
  **URL Path**: `/works/{id}`  
  **Description**: Retrieve a specific work with computed drying time.  
  **Query parameters**: None.  
  **JSON request payload structure**: N/A  
  **JSON response payload structure**: Single work object (as in list).  
  **Success codes and messages**: 200 OK.  
  **Error codes and messages**: 404 Not Found; 401 Unauthorized.

- **HTTP Method**: PATCH  
  **URL Path**: `/works/{id}`  
  **Description**: Update work (e.g., status, photo_path); validate status progression, update timestamps, set sync_status=PENDING.  
  **Query parameters**: None.  
  **JSON request payload structure**: `{ "status_id": "smallint", "photo_path": "string?", "wall_thickness": "integer?" }`  
  **JSON response payload structure**: Updated work object.  
  **Success codes and messages**: 200 OK.  
  **Error codes and messages**: 400 Bad Request - Invalid status progression (e.g., cannot revert); 404 Not Found.

- **HTTP Method**: DELETE  
  **URL Path**: `/works/{id}`  
  **Description**: Delete a work and linked glazes.  
  **Query parameters**: None.  
  **JSON request payload structure**: N/A  
  **JSON response payload structure**: N/A  
  **Success codes and messages**: 204 No Content.  
  **Error codes and messages**: 404 Not Found; 401 Unauthorized.

- **HTTP Method**: POST  
  **URL Path**: `/works/{id}/glazes`  
  **Description**: Link glazes to work (add to junction).  
  **Query parameters**: None.  
  **JSON request payload structure**: `{ "glaze_ids": ["uuid[]"] }`  
  **JSON response payload structure**: `{ "linked_glazes": [glaze objects] }`  
  **Success codes and messages**: 201 Created.  
  **Error codes and messages**: 400 Bad Request - Invalid glaze_ids (must be user's).

- **HTTP Method**: GET  
  **URL Path**: `/works/{id}/glazes`  
  **Description**: List glazes linked to work.  
  **Query parameters**: None.  
  **JSON request payload structure**: N/A  
  **JSON response payload structure**: Array of glaze objects.  
  **Success codes and messages**: 200 OK.

### glazes
- **HTTP Method**: GET  
  **URL Path**: `/glazes`  
  **Description**: List user's glazes, with pagination/filtering.  
  **Query parameters**: `?name=string`, `page=integer`, `limit=integer`.  
  **JSON request payload structure**: N/A  
  **JSON response payload structure**: `{ "data": [ { "id": "uuid", "name": "string", "manufacturer": "string?", "color": "string?", "cone_rating": "string?", "quantity": "string?", "notes": "string?", "sync_status": "string", "updated_at": "timestamp" } ], "pagination": {...} }`  
  **Success codes and messages**: 200 OK.  
  **Error codes and messages**: 401 Unauthorized.

- **HTTP Method**: POST  
  **URL Path**: `/glazes`  
  **Description**: Create a new glaze, set sync_status=PENDING.  
  **Query parameters**: None.  
  **JSON request payload structure**: `{ "name": "string (required, max 100)", "manufacturer": "string?", "color": "string (max 50)?", "cone_rating": "string?", "quantity": "string?", "notes": "string?" }`  
  **JSON response payload structure**: Created glaze object.  
  **Success codes and messages**: 201 Created.  
  **Error codes and messages**: 400 Bad Request - Missing name.

- **HTTP Method**: GET  
  **URL Path**: `/glazes/{id}`  
  **Description**: Retrieve a specific glaze.  
  **Success codes and messages**: 200 OK.  
  **Error codes and messages**: 404 Not Found.

- **HTTP Method**: PATCH  
  **URL Path**: `/glazes/{id}`  
  **Description**: Update glaze, set sync_status=PENDING.  
  **JSON request payload structure**: Partial glaze fields.  
  **Success codes and messages**: 200 OK.  
  **Error codes and messages**: 400 Bad Request - Invalid fields.

- **HTTP Method**: DELETE  
  **URL Path**: `/glazes/{id}`  
  **Description**: Delete glaze (cascades links).  
  **Success codes and messages**: 204 No Content.

### wiki_materials
- **HTTP Method**: GET  
  **URL Path**: `/wiki_materials`  
  **Description**: Search and list verified wiki materials.  
  **Query parameters**: `?search=string` (full-text), `type_id=smallint`, `verified=boolean (default true)`, `page=integer`, `limit=integer`.  
  **JSON request payload structure**: N/A  
  **JSON response payload structure**: `{ "data": [ { "id": "uuid", "name": "string", "type": { "id": "smallint", "name": "string" }, "manufacturer": "string?", "properties": {}, "user_notes": "string?", "verification_status": "string", "submitted_by": "uuid?", "created_at": "timestamp" } ], "pagination": {...} }`  
  **Success codes and messages**: 200 OK.  
  **Error codes and messages**: 400 Bad Request - Invalid search.

- **HTTP Method**: POST  
  **URL Path**: `/wiki_materials`  
  **Description**: Submit new wiki material (unverified).  
  **Query parameters**: None.  
  **JSON request payload structure**: `{ "name": "string (required, max 255)", "type_id": "smallint (required)", "manufacturer": "string?", "properties": {}, "user_notes": "string?" }`  
  **JSON response payload structure**: Created object.  
  **Success codes and messages**: 201 Created.  
  **Error codes and messages**: 400 Bad Request - Missing required fields; 401 Unauthorized.

- **HTTP Method**: PATCH  
  **URL Path**: `/wiki_materials/{id}`  
  **Description**: Update wiki material (experts only) or verify status.  
  **JSON request payload structure**: `{ "verification_status": "string (UNVERIFIED|COMMUNITY_VERIFIED|EXPERT_VERIFIED)?", "properties": {}? }`  
  **Success codes and messages**: 200 OK.  
  **Error codes and messages**: 403 Forbidden - Non-expert update; 404 Not Found.

### reports
- **HTTP Method**: GET  
  **URL Path**: `/reports/pottery-wrapped`  
  **Description**: Generate monthly yield rate report.  
  **Query parameters**: `?period=string (e.g., 2024-12)`.  
  **JSON request payload structure**: N/A  
  **JSON response payload structure**: `{ "total_works": "integer", "completed_works": "integer", "yield_rate": "number (percentage)", "most_used_category": "string", "avg_drying_time": "string" }`  
  **Success codes and messages**: 200 OK.  
  **Error codes and messages**: 400 Bad Request - Invalid period.

### sync
- **HTTP Method**: POST  
  **URL Path**: `/sync`  
  **Description**: Batch sync pending changes (works, glazes); update sync_status, resolve conflicts via LWW (updated_at).  
  **Query parameters**: None.  
  **JSON request payload structure**: `{ "works": [work objects with sync_status=PENDING], "glazes": [glaze objects] }` (includes photo_paths for upload).  
  **JSON response payload structure**: `{ "synced": [ids], "conflicts": [ { "id": "uuid", "resolution": "local|remote" } ], "errors": [] }`  
  **Success codes and messages**: 200 OK - Sync completed.  
  **Error codes and messages**: 400 Bad Request - Invalid batch; 429 Too Many Requests - Rate limit.

### Lookup Endpoints (Read-only)
- **HTTP Method**: GET  
  **URL Path**: `/categories`, `/statuses`, `/material_types`  
  **Description**: Retrieve dictionary lists.  
  **Query parameters**: None.  
  **JSON response payload structure**: Array of { "id": "smallint", "name": "string" }.  
  **Success codes and messages**: 200 OK.

## 3. Authentication and Authorization
Supabase Auth is used for JWT-based authentication (email/password, Google, Apple Sign-In). Clients include JWT in Authorization: Bearer header for all endpoints. RLS policies enforce authorization at DB level: e.g., USING (auth.uid() = user_id) for user-owned resources; public SELECT for verified wiki_materials. Expert verification requires role check (e.g., via user_profiles.initials or custom role claim in JWT). No additional API middleware needed; Supabase handles token validation. Rate limiting via Supabase (default 100 req/min per IP/user). Assumptions: Client handles token refresh; API assumes valid JWT or rejects with 401.

## 4. Validation and Business Logic
- **Validation Conditions**:
  - **user_profiles**: initials max 10 chars (VARCHAR(10)), email unique (DB constraint, API 409 on conflict), preferences JSONB (validate structure: language enum ['en','pl',etc.], theme ['light','dark','system']).
  - **works**: code unique (VARCHAR(20), regex ^[A-Z]{2}-[A-Z]+-\d{4}-\d{3}$, API validates on create), wall_thickness 3-50 (INTEGER NOT NULL, API 400 if out of range), category_id/status_id valid FK (DB rejects, API pre-check), sync_status enum (PENDING/SYNCING/SYNCED/CONFLICT/ERROR, default PENDING).
  - **glazes**: name required max 100 chars (VARCHAR(100) NOT NULL), color max 50 (VARCHAR(50)), sync_status enum default PENDING.
  - **work_glazes**: glaze_ids must belong to user (API check via RLS), no duplicates (composite PK).
  - **wiki_materials**: name required max 255 (VARCHAR(255) NOT NULL), type_id valid FK, verification_status enum default UNVERIFIED.
  - General: UUIDs for IDs, timestamps TIMESTAMPTZ (API accepts ISO strings), photo_path non-empty string. API uses JSON schema validation (e.g., via Supabase edge functions if needed); DB constraints catch FK/integrity errors (API maps to 400/409).

- **Business Logic Implementation**:
  - **Code Generation**: On POST /works, fetch user initials, category name, current month/year, query max counter for user/month/code prefix, increment, format code, check uniqueness.
  - **Drying Timer**: On GET /works/{id}, compute remaining_time: based on wall_thickness (≤5mm:4d, 6-10:7d, 11-15:10d, >15:14d), from drying_started_at or created_at if null; if status=BONE_DRY, 0. Update drying_completed_at on status change if timer elapsed.
  - **Status Progression**: On PATCH /works/{id}, validate new status > current (e.g., enum order WET=1, LEATHER_HARD=2,...); if advancing, set drying_started_at if needed, compute completion.
  - **Linking Glazes**: POST /works/{id}/glazes inserts to junction, validates ownership.
  - **Synchronization**: POST /sync processes batch: for each entity, upsert if updated_at > remote, else conflict; upload photos to Storage, update sync_status=SYNCED/ERROR, increment retry_count on failure.
  - **Wiki Search**: GET /wiki_materials uses GIN index for ?search (to_tsvector on name/manufacturer).
  - **Reports**: GET /reports aggregates user's works by period: count total/completed (status=COMPLETED), yield_rate = (completed/total)*100; uses idx_works_user_status.
  - **LWW Conflicts**: During sync, compare updated_at; keep newer, log conflicts.
  - Assumptions: Timer logic server-side for consistency, but client can compute locally for offline; photo uploads via Supabase Storage API (integrate path in response). All updates trigger updated_at via DB trigger. Offline sync assumes client sends only PENDING items.
