-- Migration: Initial PottMaster Database Schema
-- Purpose: Sets up the core tables, relationships, and initial RLS policies for the PottMaster MVP.
-- Affected Tables: user_profiles, work_categories, work_statuses, wiki_material_types, works, glazes, work_glazes, wiki_materials, sync_queue
-- Special Considerations:
--   - UUIDs are used for primary keys to support distributed/offline environments.
--   - TIMESTAMPTZ is used for all timestamps for timezone awareness.
--   - RLS is enabled by default for all tables.
--   - Initial RLS policies are defined for basic access control.

-- Enable uuid-ossp extension for uuid_generate_v4()
create extension if not exists "uuid-ossp" with schema "extensions";

-- 1. Create Tables

-- Table: public.user_profiles
-- Stores user profile information, linked to Supabase auth.users.
create table public.user_profiles (
    id uuid primary key references auth.users(id) on delete cascade,
    email text unique not null,
    initials varchar(10) not null,
    created_at timestamptz default now() not null,
    preferences jsonb default '{}'::jsonb not null
);
comment on table public.user_profiles is 'user profiles for pottmaster application.';
comment on column public.user_profiles.id is 'uuid from auth.users table.';
comment on column public.user_profiles.email is 'user email, unique.';
comment on column public.user_profiles.initials is 'user initials for work code generation.';
comment on column public.user_profiles.preferences is 'jsonb storage for user preferences (language, theme, notifications).';

-- Enable RLS for user_profiles
alter table public.user_profiles enable row level security;

-- Table: public.work_categories
-- Dictionary table for ceramic work categories (e.g., CUP, BOWL).
create table public.work_categories (
    id smallint primary key,
    name varchar(50) unique not null
);
comment on table public.work_categories is 'dictionary table for ceramic work categories.';

-- Enable RLS for work_categories
alter table public.work_categories enable row level security;

-- Table: public.work_statuses
-- Dictionary table for ceramic work statuses (e.g., WET, LEATHER_HARD).
create table public.work_statuses (
    id smallint primary key,
    name varchar(50) unique not null
);
comment on table public.work_statuses is 'dictionary table for ceramic work statuses.';

-- Enable RLS for work_statuses
alter table public.work_statuses enable row level security;

-- Table: public.wiki_material_types
-- Dictionary table for wiki material types (e.g., Clay, Glaze, Tool).
create table public.wiki_material_types (
    id smallint primary key,
    name varchar(50) unique not null
);
comment on table public.wiki_material_types is 'dictionary table for wiki material types.';

-- Enable RLS for wiki_material_types
alter table public.wiki_material_types enable row level security;

-- Table: public.works
-- Stores details about individual ceramic pieces.
create table public.works (
    id uuid primary key default extensions.uuid_generate_v4(),
    user_id uuid not null references public.user_profiles(id) on delete cascade,
    code varchar(20) unique not null,
    category_id smallint not null references public.work_categories(id),
    wall_thickness integer not null,
    photo_path text,
    status_id smallint not null references public.work_statuses(id),
    created_at timestamptz default now() not null,
    drying_started_at timestamptz,
    drying_completed_at timestamptz,
    sync_status varchar(20) default 'pending' not null,
    updated_at timestamptz default now() not null
);
comment on table public.works is 'details about individual ceramic pieces.';
comment on column public.works.code is 'unique identification code (e.g., mk-cup-1224-001).';
comment on column public.works.wall_thickness is 'wall thickness in mm.';
comment on column public.works.photo_path is 'path to the image (local or supabase storage).';
comment on column public.works.sync_status is 'synchronization status (pending, syncing, synced, conflict, error).';

-- Enable RLS for works
alter table public.works enable row level security;

-- Table: public.glazes
-- Stores user's glaze inventory.
create table public.glazes (
    id uuid primary key default extensions.uuid_generate_v4(),
    user_id uuid not null references public.user_profiles(id) on delete cascade,
    name varchar(100) not null,
    manufacturer varchar(100),
    color varchar(50),
    cone_rating varchar(50),
    quantity text,
    notes text,
    sync_status varchar(20) default 'pending' not null,
    updated_at timestamptz default now() not null
);
comment on table public.glazes is 'user''s glaze inventory.';
comment on column public.glazes.quantity is 'free text for quantity (e.g., 500g, half jar).';

-- Enable RLS for glazes
alter table public.glazes enable row level security;

-- Table: public.work_glazes
-- Junction table for many-to-many relationship between works and glazes.
create table public.work_glazes (
    work_id uuid not null references public.works(id) on delete cascade,
    glaze_id uuid not null references public.glazes(id) on delete cascade,
    primary key (work_id, glaze_id)
);
comment on table public.work_glazes is 'junction table for works and glazes.';

-- Enable RLS for work_glazes
alter table public.work_glazes enable row level security;

-- Table: public.wiki_materials
-- Public knowledge base for ceramic materials.
create table public.wiki_materials (
    id uuid primary key default extensions.uuid_generate_v4(),
    name varchar(255) not null,
    type_id smallint not null references public.wiki_material_types(id),
    manufacturer varchar(255),
    properties jsonb,
    user_notes text,
    verification_status varchar(50) default 'unverified' not null,
    submitted_by uuid references public.user_profiles(id) on delete set null,
    created_at timestamptz default now() not null,
    updated_at timestamptz default now() not null
);
comment on table public.wiki_materials is 'public knowledge base for ceramic materials.';
comment on column public.wiki_materials.properties is 'jsonb storage for various material properties.';
comment on column public.wiki_materials.verification_status is 'status of wiki entry verification (unverified, community_verified, expert_verified).';

-- Enable RLS for wiki_materials
alter table public.wiki_materials enable row level security;

-- Table: public.sync_queue
-- Queue for managing offline changes to be synchronized with the backend.
create table public.sync_queue (
    id bigserial primary key,
    entity_type varchar(50) not null,
    entity_id uuid not null,
    operation varchar(10) not null, -- insert, update, delete
    retry_count integer default 0 not null,
    last_error text,
    created_at timestamptz default now() not null
);
comment on table public.sync_queue is 'queue for managing offline changes to be synchronized.';

-- Enable RLS for sync_queue
alter table public.sync_queue enable row level security;

-- 2. RLS Policies

-- Policies for public.user_profiles
-- Allow authenticated users to view their own profile
create policy "authenticated users can select their own user_profiles"
on public.user_profiles for select
to authenticated
using (auth.uid() = id);

-- Allow authenticated users to insert their own profile
create policy "authenticated users can insert their own user_profiles"
on public.user_profiles for insert
to authenticated
with check (auth.uid() = id);

-- Allow authenticated users to update their own profile
create policy "authenticated users can update their own user_profiles"
on public.user_profiles for update
to authenticated
using (auth.uid() = id)
with check (auth.uid() = id);

-- Allow authenticated users to delete their own profile
create policy "authenticated users can delete their own user_profiles"
on public.user_profiles for delete
to authenticated
using (auth.uid() = id);

-- Policies for public.work_categories
-- Allow all users to read work categories
create policy "anon can select work_categories"
on public.work_categories for select
to anon
using (true);

create policy "authenticated users can select work_categories"
on public.work_categories for select
to authenticated
using (true);

-- Policies for public.work_statuses
-- Allow all users to read work statuses
create policy "anon can select work_statuses"
on public.work_statuses for select
to anon
using (true);

create policy "authenticated users can select work_statuses"
on public.work_statuses for select
to authenticated
using (true);

-- Policies for public.wiki_material_types
-- Allow all users to read wiki material types
create policy "anon can select wiki_material_types"
on public.wiki_material_types for select
to anon
using (true);

create policy "authenticated users can select wiki_material_types"
on public.wiki_material_types for select
to authenticated
using (true);

-- Policies for public.works
-- Allow authenticated users to select their own works
create policy "authenticated users can select their own works"
on public.works for select
to authenticated
using (auth.uid() = user_id);

-- Allow authenticated users to insert their own works
create policy "authenticated users can insert their own works"
on public.works for insert
to authenticated
with check (auth.uid() = user_id);

-- Allow authenticated users to update their own works
create policy "authenticated users can update their own works"
on public.works for update
to authenticated
using (auth.uid() = user_id)
with check (auth.uid() = user_id);

-- Allow authenticated users to delete their own works
create policy "authenticated users can delete their own works"
on public.works for delete
to authenticated
using (auth.uid() = user_id);

-- Policies for public.glazes
-- Allow authenticated users to select their own glazes
create policy "authenticated users can select their own glazes"
on public.glazes for select
to authenticated
using (auth.uid() = user_id);

-- Allow authenticated users to insert their own glazes
create policy "authenticated users can insert their own glazes"
on public.glazes for insert
to authenticated
with check (auth.uid() = user_id);

-- Allow authenticated users to update their own glazes
create policy "authenticated users can update their own glazes"
on public.glazes for update
to authenticated
using (auth.uid() = user_id)
with check (auth.uid() = user_id);

-- Allow authenticated users to delete their own glazes
create policy "authenticated users can delete their own glazes"
on public.glazes for delete
to authenticated
using (auth.uid() = user_id);

-- Policies for public.work_glazes
-- Allow authenticated users to select their own work_glazes
create policy "authenticated users can select their own work_glazes"
on public.work_glazes for select
to authenticated
using (work_id in (select id from public.works where user_id = auth.uid()));

-- Allow authenticated users to insert their own work_glazes
create policy "authenticated users can insert their own work_glazes"
on public.work_glazes for insert
to authenticated
with check (work_id in (select id from public.works where user_id = auth.uid()));

-- Allow authenticated users to update their own work_glazes
create policy "authenticated users can update their own work_glazes"
on public.work_glazes for update
to authenticated
using (work_id in (select id from public.works where user_id = auth.uid()))
with check (work_id in (select id from public.works where user_id = auth.uid()));

-- Allow authenticated users to delete their own work_glazes
create policy "authenticated users can delete their own work_glazes"
on public.work_glazes for delete
to authenticated
using (work_id in (select id from public.works where user_id = auth.uid()));

-- Policies for public.wiki_materials
-- Allow anonymous users to select verified wiki entries
create policy "anon can select verified wiki_materials"
on public.wiki_materials for select
to anon
using (verification_status in ('expert_verified', 'community_verified'));

-- Allow authenticated users to select verified wiki entries
create policy "authenticated users can select verified wiki_materials"
on public.wiki_materials for select
to authenticated
using (verification_status in ('expert_verified', 'community_verified'));

-- Allow authenticated users to insert new wiki entries
create policy "authenticated users can insert wiki_materials"
on public.wiki_materials for insert
to authenticated
with check (auth.uid() = submitted_by);

-- Policies for public.sync_queue
-- Allow authenticated users to select their own sync queue entries
create policy "authenticated users can select their own sync_queue"
on public.sync_queue for select
to authenticated
using (entity_id in (select id from public.works where user_id = auth.uid()) or entity_id in (select id from public.glazes where user_id = auth.uid()));

-- Allow authenticated users to insert their own sync queue entries
create policy "authenticated users can insert their own sync_queue"
on public.sync_queue for insert
to authenticated
with check (entity_id in (select id from public.works where user_id = auth.uid()) or entity_id in (select id from public.glazes where user_id = auth.uid()));

-- Allow authenticated users to update their own sync queue entries
create policy "authenticated users can update their own sync_queue"
on public.sync_queue for update
to authenticated
using (entity_id in (select id from public.works where user_id = auth.uid()) or entity_id in (select id from public.glazes where user_id = auth.uid()))
with check (entity_id in (select id from public.works where user_id = auth.uid()) or entity_id in (select id from public.glazes where user_id = auth.uid()));

-- Allow authenticated users to delete their own sync queue entries
create policy "authenticated users can delete their own sync_queue"
on public.sync_queue for delete
to authenticated
using (entity_id in (select id from public.works where user_id = auth.uid()) or entity_id in (select id from public.glazes where user_id = auth.uid()));


-- 3. Create Indexes

-- Index for public.works(user_id, status_id)
create index idx_works_user_status on public.works(user_id, status_id);
comment on index idx_works_user_status is 'for efficient filtering of works by user and status.';

-- Index for public.works(sync_status)
create index idx_works_sync_status on public.works(sync_status);
comment on index idx_works_sync_status is 'for quick retrieval of works pending synchronization.';

-- Index for public.works(code)
create unique index idx_works_code on public.works(code);
comment on index idx_works_code is 'unique index to enforce uniqueness and speed up lookups by work code.';

-- Index for public.glazes(user_id)
create index idx_glazes_user on public.glazes(user_id);
comment on index idx_glazes_user is 'for efficient filtering of glazes by user.';

-- Index for public.glazes(name)
create index idx_glazes_name on public.glazes(name);
comment on index idx_glazes_name is 'for efficient searching in the glaze inventory by name.';

-- Index for public.wiki_materials for full-text search
create index idx_wiki_materials_search on public.wiki_materials using gin (to_tsvector('english', name || ' ' || coalesce(manufacturer, '')));
comment on index idx_wiki_materials_search is 'for full-text search on wiki material names and manufacturers.';

-- 4. Add Triggers for updated_at columns

-- Function to update updated_at column
create or replace function update_updated_at_column()
returns trigger as $$
begin
    new.updated_at = now();
    return new;
end;
$$
language plpgsql;

-- Trigger for public.user_profiles
create trigger user_profiles_updated_at_trigger
before update on public.user_profiles
for each row
execute function update_updated_at_column();

-- Trigger for public.works
create trigger works_updated_at_trigger
before update on public.works
for each row
execute function update_updated_at_column();

-- Trigger for public.glazes
create trigger glazes_updated_at_trigger
before update on public.glazes
for each row
execute function update_updated_at_column();

-- Trigger for public.wiki_materials
create trigger wiki_materials_updated_at_trigger
before update on public.wiki_materials
for each row
execute function update_updated_at_column();
