# PostgreSQL Database Plan for PottMaster MVP

This document outlines the PostgreSQL database schema, relationships, indexing strategies, and Row Level Security (RLS) policies for the PottMaster MVP, based on the project requirements and architectural decisions.

## 1. List of Tables with their Columns, Data Types, and Constraints

### `public.user_profiles`
- **id**: UUID (PRIMARY KEY, REFERENCES `auth.users(id)` ON DELETE CASCADE)
- **email**: TEXT (UNIQUE, NOT NULL)
- **initials**: VARCHAR(10) (NOT NULL)
- **created_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)
- **preferences**: JSONB (DEFAULT `{}`, NOT NULL) - Stores user preferences like language, theme, notifications.

### `public.work_categories`
- **id**: SMALLINT (PRIMARY KEY)
- **name**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., `CUP`, `BOWL`, `VASE`

### `public.work_statuses`
- **id**: SMALLINT (PRIMARY KEY)
- **name**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., `WET`, `LEATHER_HARD`, `BONE_DRY`

### `public.wiki_material_types`
- **id**: SMALLINT (PRIMARY KEY)
- **name**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., `Clay`, `Glaze`, `Tool`

### `public.works`
- **id**: UUID (PRIMARY KEY, DEFAULT `uuid_generate_v4()`)
- **user_id**: UUID (NOT NULL, REFERENCES `public.user_profiles(id)` ON DELETE CASCADE)
- **code**: VARCHAR(20) (UNIQUE, NOT NULL) - Unique identification code (e.g., `MK-CUP-1224-001`)
- **category_id**: SMALLINT (NOT NULL, REFERENCES `public.work_categories(id)`)
- **wall_thickness**: INTEGER (NOT NULL) - In mm
- **photo_path**: TEXT - Path to the image (local or Supabase Storage)
- **status_id**: SMALLINT (NOT NULL, REFERENCES `public.work_statuses(id)`)
- **created_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)
- **drying_started_at**: TIMESTAMPTZ
- **drying_completed_at**: TIMESTAMPTZ
- **sync_status**: VARCHAR(20) (DEFAULT `PENDING`, NOT NULL) - `PENDING`, `SYNCING`, `SYNCED`, `CONFLICT`, `ERROR`
- **updated_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)

### `public.glazes`
- **id**: UUID (PRIMARY KEY, DEFAULT `uuid_generate_v4()`)
- **user_id**: UUID (NOT NULL, REFERENCES `public.user_profiles(id)` ON DELETE CASCADE)
- **name**: VARCHAR(100) (NOT NULL)
- **manufacturer**: VARCHAR(100)
- **color**: VARCHAR(50)
- **cone_rating**: VARCHAR(50) - e.g., `Cone 6`
- **quantity**: TEXT - Free text for quantity (e.g., `500g`, `half jar`)
- **notes**: TEXT
- **sync_status**: VARCHAR(20) (DEFAULT `PENDING`, NOT NULL)
- **updated_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)

### `public.work_glazes`
- **work_id**: UUID (NOT NULL, REFERENCES `public.works(id)` ON DELETE CASCADE)
- **glaze_id**: UUID (NOT NULL, REFERENCES `public.glazes(id)` ON DELETE CASCADE)
- **PRIMARY KEY**: (`work_id`, `glaze_id`)

### `public.wiki_materials`
- **id**: UUID (PRIMARY KEY, DEFAULT `uuid_generate_v4()`)
- **name**: VARCHAR(255) (NOT NULL)
- **type_id**: SMALLINT (NOT NULL, REFERENCES `public.wiki_material_types(id)`)
- **manufacturer**: VARCHAR(255)
- **properties**: JSONB - Flexible storage for various material properties
- **user_notes**: TEXT
- **verification_status**: VARCHAR(50) (DEFAULT `UNVERIFIED`, NOT NULL) - `UNVERIFIED`, `COMMUNITY_VERIFIED`, `EXPERT_VERIFIED`
- **submitted_by**: UUID (REFERENCES `public.user_profiles(id)` ON DELETE SET NULL)
- **created_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)
- **updated_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)

### `public.sync_queue`
- **id**: BIGSERIAL (PRIMARY KEY)
- **entity_type**: VARCHAR(50) (NOT NULL)
- **entity_id**: UUID (NOT NULL)
- **operation**: VARCHAR(10) (NOT NULL) - `INSERT`, `UPDATE`, `DELETE`
- **retry_count**: INTEGER (DEFAULT `0`, NOT NULL)
- **last_error**: TEXT
- **created_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)

## 2. Relationships Between Tables

- `public.user_profiles` 1:N `public.works`: Each user can have multiple works.
- `public.user_profiles` 1:N `public.glazes`: Each user can have multiple glazes.
- `public.works` N:M `public.glazes`: A work can have multiple glazes, and a glaze can be used on multiple works. This is resolved by the `public.work_glazes` junction table.
- `public.user_profiles` 1:N `public.wiki_materials`: Users can submit wiki entries.
- `public.work_categories` 1:N `public.works`: Each work belongs to a category.
- `public.work_statuses` 1:N `public.works`: Each work has a status.
- `public.wiki_material_types` 1:N `public.wiki_materials`: Each wiki material has a type.

## 3. Indexes

- `idx_works_user_status` on `public.works(user_id, status_id)`: For efficient filtering of works by user and status.
- `idx_works_sync_status` on `public.works(sync_status)`: For quick retrieval of works pending synchronization.
- `idx_works_code` on `public.works(code)`: Unique index to enforce uniqueness and speed up lookups by work code.
- `idx_glazes_user` on `public.glazes(user_id)`: For efficient filtering of glazes by user.
- `idx_glazes_name` on `public.glazes(name)`: For efficient searching in the glaze inventory by name.
- `idx_wiki_materials_search` on `public.wiki_materials` using `GIN (to_tsvector('english', name || ' ' || COALESCE(manufacturer, '')))`: For full-text search on wiki material names and manufacturers.

## 4. PostgreSQL Policies

### `public.user_profiles`
- **"Users can view and update their own profile"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` their own `user_profiles` record based on `auth.uid() = id`.

### `public.works`
- **"Users can only access their own works"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` their own `works` records based on `auth.uid() = user_id`.

### `public.glazes`
- **"Users can only access their own glazes"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` their own `glazes` records based on `auth.uid() = user_id`.

### `public.work_glazes`
- **"Users can only access their own work_glazes"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` `work_glazes` records that are linked to their own `works`.

### `public.wiki_materials`
- **"Anyone can read verified wiki entries"**: Allows all users to `SELECT` wiki entries that have a `verification_status` of `EXPERT_VERIFIED` or `COMMUNITY_VERIFIED`.
- **"Authenticated users can submit wiki entries"**: Allows authenticated users to `INSERT` new wiki entries, with a `CHECK` that `submitted_by` matches `auth.uid()`.
- **"Experts/Admins can update wiki entries"**: (Placeholder) Allows users with specific roles (e.g., `ADMIN`, `EXPERT` based on `initials` in `user_profiles`) to `UPDATE` wiki entries.

## 5. Additional Notes or Explanations about Design Decisions

- **UUIDs for Primary Keys**: Used for distributed and offline-first environments, reducing conflicts during synchronization.
- **`TIMESTAMPTZ` for Timestamps**: Ensures timezone awareness and consistency across different regions.
- **`updated_at` Triggers**: Automatic `updated_at` column updates using a PostgreSQL trigger for Last-Write-Wins (LWW) conflict resolution.
- **`JSONB` for `user_profiles.preferences`**: Provides flexibility for evolving user preferences without requiring schema migrations for minor changes.
- **Dictionary Tables for Categories, Statuses, and Types**: Replaced `VARCHAR` fields with foreign keys to dedicated dictionary tables ([`work_categories`](#publicwork_categories), [`work_statuses`](#publicwork_statuses), [`wiki_material_types`](#publicwiki_material_types)). This provides:
    - **Data Integrity**: Ensures only predefined values are used.
    - **Consistency**: Centralized management of categories/statuses/types.
    - **Performance**: Joins on `SMALLINT` are faster than string comparisons.
    - **Flexibility**: Easier to add, modify, or deprecate categories/statuses/types without altering main tables.
- **`TEXT` for `photo_path` and `glaze.quantity`**: Provides flexibility for storing varying lengths of data. Application logic will handle interpretation of `photo_path` (local vs. cloud) and `glaze.quantity` (free text).
- **`sync_queue` Table**: Dedicated table for managing offline changes and ensuring reliable synchronization with the Supabase backend.
- **Supabase `auth.users` Integration**: `user_profiles.id` is directly linked to `auth.users.id` to leverage Supabase's authentication system and ensure data integrity.
- **RLS Implementation**: Policies are designed to enforce data isolation, ensuring users can only access and modify their own data, and controlling access to public wiki content.
