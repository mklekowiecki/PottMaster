-- Migration: Enhanced Glaze Management System
-- Purpose: Add comprehensive glaze tracking with JSONB properties
-- Date: 2025-01-04

-- 1. Create glaze_types dictionary table
CREATE TABLE IF NOT EXISTS public.glaze_types (
    id SMALLINT PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    code VARCHAR(50) UNIQUE NOT NULL
);

COMMENT ON TABLE public.glaze_types IS 'Dictionary table for glaze types (low-fire, mid-range, high-fire)';

-- Enable RLS for glaze_types
ALTER TABLE public.glaze_types ENABLE ROW LEVEL SECURITY;

-- Allow all users to read glaze types
CREATE POLICY "anon can select glaze_types"
ON public.glaze_types FOR SELECT
TO anon
USING (true);

CREATE POLICY "authenticated users can select glaze_types"
ON public.glaze_types FOR SELECT
TO authenticated
USING (true);

-- Insert default glaze types
INSERT INTO public.glaze_types (id, name, code) VALUES
(1, 'Low-fire', 'LOW_FIRE'),
(2, 'Mid-range', 'MID_RANGE'),
(3, 'High-fire', 'HIGH_FIRE')
ON CONFLICT (id) DO NOTHING;

-- 2. Add new columns to glazes table
ALTER TABLE public.glazes
ADD COLUMN IF NOT EXISTS batch_date DATE,
ADD COLUMN IF NOT EXISTS type_id SMALLINT REFERENCES public.glaze_types(id),
ADD COLUMN IF NOT EXISTS properties JSONB DEFAULT '{}'::jsonb NOT NULL,
ADD COLUMN IF NOT EXISTS food_safe BOOLEAN,
ADD COLUMN IF NOT EXISTS is_favorite BOOLEAN DEFAULT false NOT NULL,
ADD COLUMN IF NOT EXISTS created_at TIMESTAMPTZ DEFAULT now() NOT NULL;

-- 3. Create indexes for efficient queries
CREATE INDEX IF NOT EXISTS idx_glazes_type ON public.glazes(type_id);
CREATE INDEX IF NOT EXISTS idx_glazes_favorite ON public.glazes(user_id, is_favorite);
CREATE INDEX IF NOT EXISTS idx_glazes_properties ON public.glazes USING GIN (properties);

-- 4. Add comments for documentation
COMMENT ON COLUMN public.glazes.batch_date IS 'Date when the glaze batch was purchased/mixed';
COMMENT ON COLUMN public.glazes.type_id IS 'Foreign key to glaze_types (low-fire, mid-range, high-fire)';
COMMENT ON COLUMN public.glazes.properties IS 'JSONB storage for comprehensive glaze properties (firing, appearance, behavior, etc.)';
COMMENT ON COLUMN public.glazes.food_safe IS 'NULL=not tested, true=safe, false=not safe';
COMMENT ON COLUMN public.glazes.is_favorite IS 'Flag for quick access to frequently used glazes';

-- 5. Update trigger to set updated_at on glaze changes (if not already exists)
-- The trigger is already created in the initial schema migration
