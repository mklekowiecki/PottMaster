-- Migration: Add storage bucket and policies for work photos
-- Purpose: Creates the work-photos bucket and secure access policies

-- Create the storage bucket
INSERT INTO storage.buckets (id, name, public)
VALUES ('work-photos', 'work-photos', false)
ON CONFLICT (id) DO NOTHING;

-- Note: RLS is enabled by default on storage.objects in Supabase

-- Policy: Users can upload photos to their own folder
CREATE POLICY "Users can upload their own photos" ON storage.objects
FOR INSERT WITH CHECK (
    bucket_id = 'work-photos' AND
    auth.uid()::text = (string_to_array(name, '/'))[1]
);

-- Policy: Users can view their own photos
CREATE POLICY "Users can view their own photos" ON storage.objects
FOR SELECT USING (
    bucket_id = 'work-photos' AND
    auth.uid()::text = (string_to_array(name, '/'))[1]
);

-- Policy: Users can update their own photos
CREATE POLICY "Users can update their own photos" ON storage.objects
FOR UPDATE USING (
    bucket_id = 'work-photos' AND
    auth.uid()::text = (string_to_array(name, '/'))[1]
);

-- Policy: Users can delete their own photos
CREATE POLICY "Users can delete their own photos" ON storage.objects
FOR DELETE USING (
    bucket_id = 'work-photos' AND
    auth.uid()::text = (string_to_array(name, '/'))[1]
);