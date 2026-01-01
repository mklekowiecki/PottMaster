-- Migration: Add code column to work_statuses
-- Purpose: Adds a code field to the work_statuses table for unique status codes.

alter table public.work_statuses add column code varchar(50) unique not null;