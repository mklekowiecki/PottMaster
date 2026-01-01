-- Migration: Insert initial dictionary data for work_categories, work_statuses, and wiki_material_types

-- Insert data into work_categories
do $$
begin
    if not exists (select 1 from public.work_categories where id = 1) then
        insert into public.work_categories (id, name, code) values
        (1, 'Cup', 'CUP'),
        (2, 'Bowl', 'BOWL'),
        (3, 'Vase', 'VASE'),
        (4, 'Plate', 'PLATE'),
        (5, 'Sculpture', 'SCULPTURE'),
        (6, 'Tile', 'TILE'),
        (7, 'Other', 'OTHER');
    end if;
end $$;

-- Insert data into work_statuses
do $$
begin
    if not exists (select 1 from public.work_statuses where id = 1) then
        insert into public.work_statuses (id, name, code) values
        (1, 'Wet', 'WET'),
        (2, 'Leather Hard', 'LEATHER_HARD'),
        (3, 'Bone Dry', 'BONE_DRY'),
        (4, 'Bisque Fired', 'BISQUE_FIRED'),
        (5, 'Glazed', 'GLAZED'),
        (6, 'Glaze Fired', 'GLAZE_FIRED'),
        (7, 'Completed', 'COMPLETED'),
        (8, 'Discarded', 'DISCARDED');
    end if;
end $$;

-- Insert data into wiki_material_types
do $$
begin
    if not exists (select 1 from public.wiki_material_types where id = 1) then
        insert into public.wiki_material_types (id, name) values
        (1, 'Clay'),
        (2, 'Glaze'),
        (3, 'Tool');
    end if;
end $$;