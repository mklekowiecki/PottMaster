# Plan for Implementing Multiple Pictures per Work

## Overview

This plan outlines the changes required to allow users to add multiple pictures to a single work in PottMaster. Currently, each work supports only one photo. The implementation will involve database schema changes, backend updates, UI modifications, and synchronization logic updates.

## Current State Analysis

- **Work Model**: Contains a single `PhotoPath` property
- **Database**: SQLite table `works` has `photo_path` column
- **UI**: NewWorkPage allows selecting one photo, WorkDetailPage displays one image
- **Sync**: Single photo uploaded to Supabase Storage

## Proposed Architecture

### Data Model Changes
- Create new `Photo` entity with:
  - `Id` (primary key)
  - `WorkId` (foreign key to works)
  - `Path` (local file path)
  - `RemotePath` (Supabase Storage URL)
  - `Order` (display order)
  - `SyncStatus` (pending, syncing, synced, error)
  - `CreatedAt`, `UpdatedAt`

- Modify `Work` model to have `Photos` collection instead of single `PhotoPath`

### Database Schema
- Add `photos` table with above fields
- Maintain backward compatibility for existing works with single photo
- Add migration script for existing data

## Implementation Steps

### 1. Database & Model Updates
- [ ] Create `Photo.cs` model in `PottMasterLib/Models/`
- [ ] Update `Work.cs` to include `List<Photo> Photos` property
- [ ] Create database migration for `photos` table
- [ ] Update `DbService` to handle photo CRUD operations
- [ ] Modify existing work queries to include photos

### 2. Backend & Sync Updates
- [ ] Update Supabase schema to support multiple photos per work
- [ ] Modify `SyncService` to upload multiple photos
- [ ] Update `SupabaseApi` to handle photo operations
- [ ] Implement photo conflict resolution (LWW strategy)

### 3. UI Updates
- [ ] Modify `NewWorkPage.xaml` to allow multiple photo selection
  - Add "Add Photo" button
  - Display selected photos in a horizontal scroll view
  - Allow removing individual photos
- [ ] Update `WorkDetailPage.xaml` to display multiple photos
  - Implement photo carousel or grid layout
  - Add swipe gestures for navigation
- [ ] Update photo display components for better UX

### 4. Business Logic Updates
- [ ] Modify `ImageService` to handle multiple image capture/selection
- [ ] Update `NewWorkViewModel` for photo management
- [ ] Update `WorkDetailViewModel` for photo display
- [ ] Add photo ordering and deletion logic

### 5. Localization & Polish
- [ ] Update `AppResources.resx` for multiple photo strings
- [ ] Add loading states for photo uploads
- [ ] Implement photo compression for multiple images
- [ ] Add error handling for failed photo operations

### 6. Testing & Validation
- [ ] Test offline photo addition and sync
- [ ] Validate backward compatibility with existing works
- [ ] Test photo ordering and deletion
- [ ] Performance testing with multiple large images

## Success Criteria

- Users can add unlimited photos to a work
- Photos sync correctly between devices
- UI remains responsive with multiple images
- Backward compatibility maintained
- No breaking changes to existing functionality

## Risk Mitigation

- Implement gradual rollout with feature flag
- Add comprehensive error handling
- Monitor storage usage and performance
- Provide migration path for existing data

## Timeline Estimate

- Database & Models: 2-3 days
- Backend & Sync: 3-4 days
- UI Updates: 4-5 days
- Testing & Polish: 2-3 days
- **Total: 11-15 days**

## Dependencies

- Requires Supabase schema updates
- May need additional storage quota monitoring
- UI components may need custom controls for photo management