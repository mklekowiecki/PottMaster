-- Migration: Create photos table
-- Description: Creates a new photos table to support multiple photos per work
-- Date: 2024

-- Enable UUID extension if not already enabled
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create photos table
CREATE TABLE IF NOT EXISTS public.photos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    work_id UUID NOT NULL REFERENCES public.works(id) ON DELETE CASCADE,
    remote_path TEXT NOT NULL,
    "order" INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Create indexes for efficient querying
CREATE INDEX IF NOT EXISTS idx_photos_work_id ON public.photos(work_id);
CREATE INDEX IF NOT EXISTS idx_photos_work_order ON public.photos(work_id, "order");

-- Create function to automatically update updated_at timestamp
CREATE OR REPLACE FUNCTION update_photos_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create trigger to call the function before updates
DROP TRIGGER IF EXISTS trigger_update_photos_updated_at ON public.photos;
CREATE TRIGGER trigger_update_photos_updated_at
    BEFORE UPDATE ON public.photos
    FOR EACH ROW
    EXECUTE FUNCTION update_photos_updated_at();

-- Enable Row Level Security
ALTER TABLE public.photos ENABLE ROW LEVEL SECURITY;

-- Drop existing policies if they exist
DROP POLICY IF EXISTS "Users can view photos of their own works" ON public.photos;
DROP POLICY IF EXISTS "Users can insert photos for their own works" ON public.photos;
DROP POLICY IF EXISTS "Users can update photos of their own works" ON public.photos;
DROP POLICY IF EXISTS "Users can delete photos of their own works" ON public.photos;

-- Create RLS policies
-- Policy: Users can view photos of their own works
CREATE POLICY "Users can view photos of their own works"
    ON public.photos
    FOR SELECT
    USING (
        EXISTS (
            SELECT 1 FROM public.works
            WHERE works.id = photos.work_id
            AND works.user_id = auth.uid()
        )
    );

-- Policy: Users can insert photos for their own works
CREATE POLICY "Users can insert photos for their own works"
    ON public.photos
    FOR INSERT
    WITH CHECK (
        EXISTS (
            SELECT 1 FROM public.works
            WHERE works.id = photos.work_id
            AND works.user_id = auth.uid()
        )
    );

-- Policy: Users can update photos of their own works
CREATE POLICY "Users can update photos of their own works"
    ON public.photos
    FOR UPDATE
    USING (
        EXISTS (
            SELECT 1 FROM public.works
            WHERE works.id = photos.work_id
            AND works.user_id = auth.uid()
        )
    )
    WITH CHECK (
        EXISTS (
            SELECT 1 FROM public.works
            WHERE works.id = photos.work_id
            AND works.user_id = auth.uid()
        )
    );

-- Policy: Users can delete photos of their own works
CREATE POLICY "Users can delete photos of their own works"
    ON public.photos
    FOR DELETE
    USING (
        EXISTS (
            SELECT 1 FROM public.works
            WHERE works.id = photos.work_id
            AND works.user_id = auth.uid()
        )
    );

-- Add comment to the table
COMMENT ON TABLE public.photos IS 'Stores multiple photos for each pottery work. Replaces the single photo_path field in works table.';
COMMENT ON COLUMN public.photos.remote_path IS 'Path to the image file in Supabase Storage (e.g., user_id/work_id/photo.jpg)';
COMMENT ON COLUMN public.photos."order" IS 'Display order of the photo (0-based index for sorting)';
