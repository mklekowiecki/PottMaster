-- Migration: Add code column to work_categories
-- Purpose: Adds a code field to the work_categories table for unique category codes.

alter table public.work_categories add column code varchar(50) unique not null;