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
- **name**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., 'Cup', 'Bowl', 'Vase'
- **code**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., 'CUP', 'BOWL', 'VASE'

### `public.work_statuses`
- **id**: SMALLINT (PRIMARY KEY)
- **name**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., 'Wet', 'Leather Hard', 'Bone Dry'
- **code**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., 'WET', 'LEATHER_HARD', 'BONE_DRY'

### `public.glaze_types`
- **id**: SMALLINT (PRIMARY KEY)
- **name**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., 'Low-fire', 'Mid-range', 'High-fire'
- **code**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., 'LOW_FIRE', 'MID_RANGE', 'HIGH_FIRE'

### `public.wiki_material_types`
- **id**: SMALLINT (PRIMARY KEY)
- **name**: VARCHAR(50) (UNIQUE, NOT NULL) - e.g., 'Clay', 'Glaze', 'Tool'

### `public.works`
- **id**: UUID (PRIMARY KEY, DEFAULT `uuid_generate_v4()`)
- **user_id**: UUID (NOT NULL, REFERENCES `public.user_profiles(id)` ON DELETE CASCADE)
- **code**: VARCHAR(20) (UNIQUE, NOT NULL) - Unique identification code (e.g., `MK-CUP-1224-001`)
- **category_id**: SMALLINT (NOT NULL, REFERENCES `public.work_categories(id)`)
- **wall_thickness**: INTEGER (NOT NULL) - In mm
- **photo_path**: TEXT - Deprecated: Path to the primary image (kept for backward compatibility, use `photos` table instead)
- **status_id**: SMALLINT (NOT NULL, REFERENCES `public.work_statuses(id)`)
- **created_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)
- **drying_started_at**: TIMESTAMPTZ
- **drying_completed_at**: TIMESTAMPTZ
- **sync_status**: VARCHAR(20) (DEFAULT `PENDING`, NOT NULL) - `PENDING`, `SYNCING`, `SYNCED`, `CONFLICT`, `ERROR`
- **updated_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)

### `public.photos`
- **id**: UUID (PRIMARY KEY, DEFAULT `uuid_generate_v4()`)
- **work_id**: UUID (NOT NULL, REFERENCES `public.works(id)` ON DELETE CASCADE)
- **remote_path**: TEXT (NOT NULL) - Path to the image in Supabase Storage
- **order**: INTEGER (NOT NULL, DEFAULT 0) - Display order of the photo
- **created_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)
- **updated_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)

### `public.glazes`
- **id**: UUID (PRIMARY KEY, DEFAULT `uuid_generate_v4()`)
- **user_id**: UUID (NOT NULL, REFERENCES `public.user_profiles(id)` ON DELETE CASCADE)
- **name**: VARCHAR(100) (NOT NULL)
- **manufacturer**: VARCHAR(100)
- **batch_date**: DATE - Date or batch identifier for the glaze
- **type_id**: SMALLINT (REFERENCES `public.glaze_types(id)`) - Low-fire, Mid-range, or High-fire
- **color**: VARCHAR(50)
- **cone_rating**: VARCHAR(50) - e.g., `Cone 6`, `Cone 10`
- **quantity**: TEXT - Free text for quantity (e.g., `500g`, `half jar`)
- **properties**: JSONB (DEFAULT `{}`, NOT NULL) - Structured storage for detailed glaze properties
- **notes**: TEXT - Additional user notes
- **food_safe**: BOOLEAN - NULL = not tested, TRUE = safe, FALSE = not safe
- **is_favorite**: BOOLEAN (DEFAULT FALSE, NOT NULL) - User-defined favorite flag
- **sync_status**: VARCHAR(20) (DEFAULT `PENDING`, NOT NULL)
- **created_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)
- **updated_at**: TIMESTAMPTZ (DEFAULT `now()`, NOT NULL)

**JSONB properties structure**:
```json
{
  "firing": {
    "temperature_min": 1200,
    "temperature_max": 1240,
    "temperature_unit": "C",
    "atmosphere": "oxidation|reduction",
    "curve_sensitivity": "low|medium|high"
  },
  "appearance": {
    "transparency": "transparent|semi-transparent|opaque",
    "finish": "gloss|satin|semi-matte|matte",
    "texture": ["smooth", "cratered", "crystalline", "crackle"],
    "special_effects": ["reactive", "speckled", "layered", "metallic", "runny"]
  },
  "behavior": {
    "melt_fluidity": "low|medium|high",
    "thickness_tolerance": "low|medium|high",
    "color_stability": "stable|variable",
    "repeatability": "low|medium|high"
  },
  "application": {
    "form": "dry_mix|liquid|brushing",
    "methods": ["dipping", "pouring", "spraying", "brushing"],
    "recommended_thickness": "2-3 mm",
    "application_notes": "text"
  },
  "clay_compatibility": {
    "best_suited": ["white_clay", "grogged_clay", "porcelain", "dark_clay"],
    "interaction": "neutral|contrasting|highly_reactive"
  },
  "defects": {
    "known_issues": ["crazing", "blistering", "pinholing", "crawling", "running"],
    "mitigation_notes": "text"
  },
  "usage": {
    "work_type": "artistic|functional|both",
    "durability": "low|medium|high"
  }
}
```

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

## Dictionary Values

### Work Categories

| ID | Name    | Code      |
|----|---------|-----------|
| 1  | Cup     | CUP       |
| 2  | Bowl    | BOWL      |
| 3  | Vase    | VASE      |
| 4  | Plate   | PLATE     |
| 5  | Sculpture | SCULPTURE |
| 6  | Tile    | TILE      |
| 7  | Other   | OTHER     |

### Work Statuses

| ID | Name          | Code          |
|----|---------------|---------------|
| 1  | Wet           | WET           |
| 2  | Leather Hard  | LEATHER_HARD  |
| 3  | Bone Dry      | BONE_DRY      |
| 4  | Bisque Fired  | BISQUE_FIRED  |
| 5  | Glazed        | GLAZED        |
| 6  | Glaze Fired   | GLAZE_FIRED   |
| 7  | Completed     | COMPLETED     |
| 8  | Discarded     | DISCARDED     |

### Glaze Types

| ID | Name      | Code       |
|----|-----------|------------|
| 1  | Low-fire  | LOW_FIRE   |
| 2  | Mid-range | MID_RANGE  |
| 3  | High-fire | HIGH_FIRE  |

### Wiki Material Types

| ID | Name  |
|----|-------|
| 1  | Clay  |
| 2  | Glaze |
| 3  | Tool  |

## 3. Relationships Between Tables

- `public.user_profiles` 1:N `public.works`: Each user can have multiple works.
- `public.user_profiles` 1:N `public.glazes`: Each user can have multiple glazes.
- `public.works` 1:N `public.photos`: Each work can have multiple photos.
- `public.works` N:M `public.glazes`: A work can have multiple glazes, and a glaze can be used on multiple works. This is resolved by the `public.work_glazes` junction table.
- `public.user_profiles` 1:N `public.wiki_materials`: Users can submit wiki entries.
- `public.work_categories` 1:N `public.works`: Each work belongs to a category.
- `public.work_statuses` 1:N `public.works`: Each work has a status.
- `public.glaze_types` 1:N `public.glazes`: Each glaze has a firing type.
- `public.wiki_material_types` 1:N `public.wiki_materials`: Each wiki material has a type.

## 4. Indexes

- `idx_works_user_status` on `public.works(user_id, status_id)`: For efficient filtering of works by user and status.
- `idx_works_sync_status` on `public.works(sync_status)`: For quick retrieval of works pending synchronization.
- `idx_works_code` on `public.works(code)`: Unique index to enforce uniqueness and speed up lookups by work code.
- `idx_photos_work_id` on `public.photos(work_id)`: For efficient retrieval of all photos for a specific work.
- `idx_photos_work_order` on `public.photos(work_id, order)`: For efficient retrieval of photos in display order.
- `idx_glazes_user` on `public.glazes(user_id)`: For efficient filtering of glazes by user.
- `idx_glazes_name` on `public.glazes(name)`: For efficient searching in the glaze inventory by name.
- `idx_glazes_type` on `public.glazes(type_id)`: For efficient filtering by glaze type.
- `idx_glazes_favorite` on `public.glazes(user_id, is_favorite)`: For efficient retrieval of user's favorite glazes.
- `idx_glazes_properties` on `public.glazes` using `GIN (properties)`: For efficient querying of JSONB properties.
- `idx_wiki_materials_search` on `public.wiki_materials` using `GIN (to_tsvector('english', name || ' ' || COALESCE(manufacturer, '')))`: For full-text search on wiki material names and manufacturers.

## 5. PostgreSQL Policies

### `public.user_profiles`
- **"Users can view and update their own profile"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` their own `user_profiles` record based on `auth.uid() = id`.

### `public.works`
- **"Users can only access their own works"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` their own `works` records based on `auth.uid() = user_id`.

### `public.photos`
- **"Users can only access photos of their own works"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` `photos` records that are linked to their own `works` (via join with `works.user_id`).

### `public.glazes`
- **"Users can only access their own glazes"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` their own `glazes` records based on `auth.uid() = user_id`.

### `public.glaze_types`
- **"Anyone can read glaze types"**: Allows all users to `SELECT` glaze types for use in glaze creation/filtering.

### `public.work_glazes`
- **"Users can only access their own work_glazes"**: Allows users to `SELECT`, `INSERT`, `UPDATE`, `DELETE` `work_glazes` records that are linked to their own `works`.

### `public.wiki_materials`
- **"Anyone can read verified wiki entries"**: Allows all users to `SELECT` wiki entries that have a `verification_status` of `EXPERT_VERIFIED` or `COMMUNITY_VERIFIED`.
- **"Authenticated users can submit wiki entries"**: Allows authenticated users to `INSERT` new wiki entries, with a `CHECK` that `submitted_by` matches `auth.uid()`.
- **"Experts/Admins can update wiki entries"**: (Placeholder) Allows users with specific roles (e.g., `ADMIN`, `EXPERT` based on `initials` in `user_profiles`) to `UPDATE` wiki entries.

## 6. Additional Notes or Explanations about Design Decisions

- **UUIDs for Primary Keys**: Used for distributed and offline-first environments, reducing conflicts during synchronization.
- **`TIMESTAMPTZ` for Timestamps**: Ensures timezone awareness and consistency across different regions.
- **`updated_at` Triggers**: Automatic `updated_at` column updates using a PostgreSQL trigger for Last-Write-Wins (LWW) conflict resolution.
- **`JSONB` for `user_profiles.preferences`**: Provides flexibility for evolving user preferences without requiring schema migrations for minor changes.
- **Dictionary Tables for Categories, Statuses, and Types**: Replaced `VARCHAR` fields with foreign keys to dedicated dictionary tables ([`work_categories`](#publicwork_categories), [`work_statuses`](#publicwork_statuses), [`glaze_types`](#publicglaze_types), [`wiki_material_types`](#publicwiki_material_types)). This provides:
    - **Data Integrity**: Ensures only predefined values are used.
    - **Consistency**: Centralized management of categories/statuses/types.
    - **Performance**: Joins on `SMALLINT` are faster than string comparisons.
    - **Flexibility**: Easier to add, modify, or deprecate categories/statuses/types without altering main tables.
- **`TEXT` for `photo_path` and `glaze.quantity`**: Provides flexibility for storing varying lengths of data. Application logic will handle interpretation of `photo_path` (local vs. cloud) and `glaze.quantity` (free text).
- **`JSONB` for `glazes.properties`**: Provides comprehensive, flexible storage for detailed glaze characteristics following the industry-standard glaze checklist parameters. Supports indexing via GIN for efficient querying of specific properties (e.g., finding all glossy glazes, or glazes suitable for functional ware). The structured format ensures data consistency while allowing for future extensibility. Applications can query specific paths like `properties->'appearance'->>'finish' = 'gloss'` or use containment operators for complex searches.
- **`food_safe` as BOOLEAN with NULL**: Three-state field (NULL = not tested, TRUE = certified safe, FALSE = known unsafe) is critical for functional pottery and liability considerations.
- **`is_favorite` Flag**: Allows users to quickly access their most-used glazes, improving UX for glaze selection in the work creation flow.
- **`batch_date` Field**: Helps track glaze batches, important for consistency and troubleshooting when glaze behavior varies.
- **`glaze_types` Dictionary**: Standardizes the primary classification (Low-fire/Mid-range/High-fire) which is the most fundamental glaze characteristic affecting kiln selection and firing schedules.
- **`photos` Table**: Separate table for managing multiple photos per work, replacing the single `photo_path` field approach. The `photo_path` field in `works` table is kept for backward compatibility but should be considered deprecated.
