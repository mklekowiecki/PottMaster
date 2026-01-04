# Glaze Database Structure Update - Summary

## Overview
This document summarizes the proposed updates to the `glazes` table structure in the PottMaster database to accommodate comprehensive glaze tracking based on industry-standard ceramic glaze parameters.

## Current Structure (Basic)
```sql
public.glazes
- id: UUID
- user_id: UUID
- name: VARCHAR(100)
- manufacturer: VARCHAR(100)
- color: VARCHAR(50)
- cone_rating: VARCHAR(50)
- quantity: TEXT
- notes: TEXT
- sync_status: VARCHAR(20)
- updated_at: TIMESTAMPTZ
```

## Proposed Enhanced Structure
```sql
public.glazes
- id: UUID
- user_id: UUID
- name: VARCHAR(100)
- manufacturer: VARCHAR(100)
- batch_date: DATE                    -- NEW
- type_id: SMALLINT                   -- NEW (FK to glaze_types)
- color: VARCHAR(50)
- cone_rating: VARCHAR(50)
- quantity: TEXT
- properties: JSONB                   -- NEW (comprehensive glaze data)
- notes: TEXT
- food_safe: BOOLEAN                  -- NEW (NULL=not tested)
- is_favorite: BOOLEAN                -- NEW
- sync_status: VARCHAR(20)
- created_at: TIMESTAMPTZ             -- NEW
- updated_at: TIMESTAMPTZ
```

## New Dictionary Table: glaze_types

```sql
public.glaze_types
- id: SMALLINT (PK)
- name: VARCHAR(50) UNIQUE
- code: VARCHAR(50) UNIQUE

Initial values:
1 | Low-fire  | LOW_FIRE
2 | Mid-range | MID_RANGE
3 | High-fire | HIGH_FIRE
```

## JSONB Properties Schema

The `properties` JSONB field supports the following structured data based on the glaze-parameters.md checklist:

### 1. Firing Parameters
```json
{
  "firing": {
    "temperature_min": 1200,
    "temperature_max": 1240,
    "temperature_unit": "C",
    "atmosphere": "oxidation|reduction",
    "curve_sensitivity": "low|medium|high"
  }
}
```

### 2. Fired Appearance
```json
{
  "appearance": {
    "transparency": "transparent|semi-transparent|opaque",
    "finish": "gloss|satin|semi-matte|matte",
    "texture": ["smooth", "cratered", "crystalline", "crackle"],
    "special_effects": ["reactive", "speckled", "layered", "metallic", "runny"]
  }
}
```

### 3. Glaze Behavior
```json
{
  "behavior": {
    "melt_fluidity": "low|medium|high",
    "thickness_tolerance": "low|medium|high",
    "color_stability": "stable|variable",
    "repeatability": "low|medium|high"
  }
}
```

### 4. Application Details
```json
{
  "application": {
    "form": "dry_mix|liquid|brushing",
    "methods": ["dipping", "pouring", "spraying", "brushing"],
    "recommended_thickness": "2-3 mm",
    "application_notes": "Apply in 3 thin coats"
  }
}
```

### 5. Clay Compatibility
```json
{
  "clay_compatibility": {
    "best_suited": ["white_clay", "grogged_clay", "porcelain", "dark_clay"],
    "interaction": "neutral|contrasting|highly_reactive"
  }
}
```

### 6. Known Defects & Issues
```json
{
  "defects": {
    "known_issues": ["crazing", "blistering", "pinholing", "crawling", "running"],
    "mitigation_notes": "Reduce application thickness to prevent running"
  }
}
```

### 7. Usage & Safety
```json
{
  "usage": {
    "work_type": "artistic|functional|both",
    "durability": "low|medium|high"
  }
}
```

## New Indexes

```sql
-- Efficient filtering by glaze type
CREATE INDEX idx_glazes_type ON public.glazes(type_id);

-- Quick access to favorite glazes
CREATE INDEX idx_glazes_favorite ON public.glazes(user_id, is_favorite);

-- Enable complex JSONB queries
CREATE INDEX idx_glazes_properties ON public.glazes USING GIN (properties);
```

## Example Queries with New Structure

### Find all glossy, food-safe glazes for cone 6
```sql
SELECT * FROM glazes
WHERE user_id = 'user-uuid'
  AND cone_rating = 'Cone 6'
  AND food_safe = TRUE
  AND properties->'appearance'->>'finish' = 'gloss';
```

### Find all high-fire reduction glazes
```sql
SELECT * FROM glazes
WHERE user_id = 'user-uuid'
  AND type_id = 3  -- HIGH_FIRE
  AND properties->'firing'->>'atmosphere' = 'reduction';
```

### Find glazes suitable for functional ware
```sql
SELECT * FROM glazes
WHERE user_id = 'user-uuid'
  AND food_safe = TRUE
  AND properties->'usage'->>'work_type' IN ('functional', 'both');
```

### Find favorite glazes with good repeatability
```sql
SELECT * FROM glazes
WHERE user_id = 'user-uuid'
  AND is_favorite = TRUE
  AND properties->'behavior'->>'repeatability' = 'high';
```

## Benefits of This Approach

### 1. **Comprehensive Data Capture**
- Captures all parameters from the glaze-parameters.md checklist
- Professional-level documentation for each glaze
- Supports advanced glaze management and troubleshooting

### 2. **Flexible & Future-Proof**
- JSONB allows adding new properties without schema changes
- Properties can be partially populated (not all fields required)
- Easy to extend for specialized glaze types

### 3. **Searchable & Queryable**
- GIN indexes enable fast JSONB queries
- Can filter by any property combination
- Supports complex analytical queries

### 4. **User Experience Improvements**
- `is_favorite` flag for quick access to commonly used glazes
- `food_safe` field crucial for functional pottery
- `batch_date` helps track glaze consistency over time
- Structured data enables smart recommendations (e.g., "glazes similar to this one")

### 5. **Data Integrity**
- `glaze_types` dictionary ensures consistent classification
- Structured JSONB prevents arbitrary data entry
- NULL for `food_safe` clearly indicates "not tested" vs "unsafe"

### 6. **Backward Compatible**
- Existing fields (name, manufacturer, color, cone_rating, quantity, notes) remain unchanged
- Can migrate existing glazes with empty `properties = {}`
- Applications can gradually adopt new features

## Migration Strategy

### Phase 1: Database Schema Update
1. Create `glaze_types` dictionary table
2. Add new columns to `glazes` table
3. Create new indexes
4. Update RLS policies

### Phase 2: Application Updates
1. Create C# model classes for glaze properties
2. Update repository layer to handle JSONB
3. Modify glaze creation/editing UI
4. Add glaze search/filter features

### Phase 3: Data Migration
1. Existing glazes continue to work (properties = {})
2. Users can gradually enhance their glaze entries
3. Import wizard for bulk glaze data entry

## Implementation Notes

### C# Model Example
```csharp
public class LocalGlaze
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime? BatchDate { get; set; }
    public int? TypeId { get; set; }
    public string? Color { get; set; }
    public string? ConeRating { get; set; }
    public string? Quantity { get; set; }
    public GlazeProperties Properties { get; set; } = new();
    public string? Notes { get; set; }
    public bool? FoodSafe { get; set; }
    public bool IsFavorite { get; set; }
    public string SyncStatus { get; set; } = "PENDING";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class GlazeProperties
{
    public FiringProperties? Firing { get; set; }
    public AppearanceProperties? Appearance { get; set; }
    public BehaviorProperties? Behavior { get; set; }
    public ApplicationProperties? Application { get; set; }
    public ClayCompatibilityProperties? ClayCompatibility { get; set; }
    public DefectsProperties? Defects { get; set; }
    public UsageProperties? Usage { get; set; }
}
```

### JSON Serialization
- Use System.Text.Json with proper serialization options
- Store JSONB as string in SQLite (local)
- Supabase handles JSONB natively (remote)

### UI Considerations
- Progressive disclosure: Basic info first, advanced properties optional
- Quick-add mode for simple glazes
- Detailed entry mode for comprehensive documentation
- Import from glaze-parameters.md checklist format

## Next Steps

1. **Review & Approval**: Stakeholder review of proposed structure
2. **Migration Script**: Create Supabase migration for schema changes
3. **Model Classes**: Implement C# classes for new structure
4. **Repository Updates**: Update DbService and SupabaseApi
5. **UI Development**: Design and implement glaze management screens
6. **Testing**: Comprehensive testing of JSONB queries and sync
7. **Documentation**: Update API documentation with new glaze endpoints

## Questions to Consider

1. **Required vs Optional**: Which properties should be required for a glaze entry?
2. **Validation**: Should we validate JSONB structure in the application or database?
3. **Defaults**: What default values make sense for new glazes?
4. **Migration**: How to handle existing glazes during schema update?
5. **Photos**: Should glazes have photo support (glaze test tiles)?

## References

- [glaze-parameters.md](glaze-parameters.md) - Source glaze checklist
- [db-plan.md](db-plan.md) - Updated database plan
- [PostgreSQL JSONB Documentation](https://www.postgresql.org/docs/current/datatype-json.html)
- [GIN Indexes for JSONB](https://www.postgresql.org/docs/current/datatype-json.html#JSON-INDEXING)
