# User Stories Reference - PottMaster

## Overview

This document provides a quick reference for all user stories in the PottMaster MVP. Stories are organized by feature area for easy navigation.

## Feature Areas

1. [Authentication & User Management](#authentication--user-management)
2. [Work Lifecycle Management](#work-lifecycle-management)
3. [Material & Inventory Management](#material--inventory-management)
4. [Offline & Synchronization](#offline--synchronization)
5. [Knowledge Base (Wiki)](#knowledge-base-wiki)
6. [Analytics & Reporting](#analytics--reporting)
7. [Security & Privacy](#security--privacy)
8. [Internationalization](#internationalization)

---

## Authentication & User Management

### US-001: User Registration and Login

**Priority**: 🔴 Critical (MVP Blocker)

**Description**: As a user, I want to create an account so that my projects are securely synchronized with the Supabase cloud.

**Acceptance Criteria**:
1. User can register via email
2. One-Tap Sign-in (Google/Apple) option is available
3. After logging in, profile data (initials) are retrieved from PostgreSQL

**Technical Notes**:
- Use Supabase Auth SDK
- Store JWT token in secure storage (Keychain/Keystore)
- Implement token refresh mechanism

**Dependencies**: None

**Estimated Effort**: 5 days

---

## Work Lifecycle Management

### US-002: New Work Registration

**Priority**: 🔴 Critical (MVP Blocker)

**Description**: As a ceramicist, I want to add a new work so that the system can start counting down the drying time.

**Acceptance Criteria**:
1. ✅ User takes a photo, which is compressed and saved in SQLite
2. ✅ User selects a category and sets the wall thickness slider
3. ✅ System saves a record with a pending synchronization status

**Technical Notes**:
- Compress images to max 1920x1920, 80% quality, <500KB
- Save to local SQLite immediately
- Mark with `sync_status = PENDING`

**Dependencies**: US-001 (Authentication)

**Estimated Effort**: 8 days

---

### US-003: Unique Identification Code

**Priority**: 🔴 Critical (MVP Blocker)

**Description**: As a user, I want to receive a short code to physically apply it to wet clay.

**Acceptance Criteria**:
1. ✅ Code is generated locally (offline) according to the mask: `Initials-CatCode-MMYY-Counter`
2. ✅ Code is visible immediately after saving the record in SQLite

**Code Format Examples**:
- `MK-CUP-1224-001` (Maria Klein, Cup, December 2024, 1st piece)
- `JD-BOWL-0125-042` (John Doe, Bowl, January 2025, 42nd piece)

**Technical Notes**:
- Counter resets monthly
- Category codes: CUP, BOWL, VASE, PLATE, SCULPTURE, OTHER
- Generate offline without network dependency

**Dependencies**: US-002 (Work Registration)

**Estimated Effort**: 3 days

---

### US-004: Process Timer Management

**Priority**: 🟡 High

**Description**: As a user, I want to see how much time is left until safe object processing.

**Acceptance Criteria**:
1. ✅ Application displays a real-time counter (Countdown)
2. ✅ Algorithm adjusts the time based on the selected wall thickness

**Drying Time Algorithm**:
| Wall Thickness | Drying Time |
|----------------|-------------|
| ≤ 5mm          | 4 days      |
| 6-10mm         | 7 days      |
| 11-15mm        | 10 days     |
| > 15mm         | 14 days     |

**Technical Notes**:
- Use `kotlinx-datetime` for time calculations
- Display countdown in days, hours, minutes
- Update UI every minute when app is active

**Dependencies**: US-002 (Work Registration)

**Estimated Effort**: 5 days

---

### US-005: Manual Status Correction

**Priority**: 🟡 High

**Description**: As a user, I want to manually mark a stage as ready to move to the next firing phase.

**Acceptance Criteria**:
1. ✅ User can click the "Ready for Firing" button ahead of time
2. ✅ System stops the timer and updates the status in the local database

**Work Statuses**:
- `WET` → Initial state
- `LEATHER_HARD` → Partially dried
- `BONE_DRY` → Fully dried
- `BISQUE_FIRED` → First firing complete
- `GLAZED` → Glaze applied
- `GLAZE_FIRED` → Final firing complete
- `COMPLETED` → Finished piece
- `DISCARDED` → Failed/broken

**Technical Notes**:
- Allow status progression only (no backwards)
- Confirmation dialog for status change
- Update `sync_status = PENDING` on change

**Dependencies**: US-004 (Timer Management)

**Estimated Effort**: 3 days

---

## Material & Inventory Management

### US-006: Glaze Inventory

**Priority**: 🟢 Medium

**Description**: As a user, I want to maintain a list of my glazes to know what I have in the workshop.

**Acceptance Criteria**:
1. ✅ Ability to add name, manufacturer, and quantity description
2. ✅ Ability to link glaze to a specific registered work

**Glaze Fields**:
- Name (required)
- Manufacturer (optional)
- Color (optional)
- Cone Rating (e.g., "Cone 6", "Cone 10")
- Quantity (free text, e.g., "500g", "half jar")
- Notes (optional)

**Technical Notes**:
- Many-to-many relationship with works
- Offline-first storage in SQLite
- Sync to Supabase when online

**Dependencies**: US-002 (Work Registration)

**Estimated Effort**: 5 days

---

## Offline & Synchronization

### US-007: Offline Operation

**Priority**: 🔴 Critical (MVP Blocker)

**Description**: As a user working in the basement, I want to have access to all application functions without reception.

**Acceptance Criteria**:
1. ✅ All queries are directed to SQLite via SQLDelight
2. ✅ Application does not block the UI due to lack of network

**Technical Notes**:
- All write operations go to local SQLite first
- Read operations always from local database
- Network operations are background-only
- Show sync status indicators in UI

**Dependencies**: None (foundational requirement)

**Estimated Effort**: Integrated into all features

---

### US-008: Automatic Synchronization

**Priority**: 🔴 Critical (MVP Blocker)

**Description**: As a user, I want my data to go to the cloud without my intervention after leaving the studio.

**Acceptance Criteria**:
1. ✅ Application sends new records from SQLite to PostgreSQL when a network is detected
2. ✅ Photos are uploaded to Supabase Storage
3. ✅ The "synchronized" flag is set to true after a successful operation

**Sync Triggers**:
- App comes to foreground
- Network becomes available
- Manual refresh by user
- Periodic background sync (every 15 minutes)

**Technical Notes**:
- Implement sync queue with retry logic
- Handle conflicts with Last-Write-Wins strategy
- Show sync progress in UI
- Batch operations for efficiency

**Dependencies**: US-001 (Authentication), US-007 (Offline)

**Estimated Effort**: 10 days

---

## Knowledge Base (Wiki)

### US-009: Wiki Browsing

**Priority**: 🟢 Medium

**Description**: As a user, I want to check clay information in the public database.

**Acceptance Criteria**:
1. ✅ Full-text search in the public Supabase table
2. ✅ Displaying trust tags for each entry

**Wiki Entry Fields**:
- Material name
- Type (Clay, Glaze, Tool)
- Manufacturer
- Properties (firing temp, color, etc.)
- User notes
- Verification status (Verified, Pending, Community)
- Submitted by (user)

**Technical Notes**:
- Public table accessible to all users
- Read-only for regular users
- Search by name, manufacturer, properties

**Dependencies**: US-001 (Authentication)

**Estimated Effort**: 6 days

---

### US-010: Expert Verification

**Priority**: 🔵 Low (Post-MVP)

**Description**: As an administrator/expert, I want to approve user entries so that the Wiki database is reliable.

**Acceptance Criteria**:
1. ✅ Expert sees a panel with unverified entries
2. ✅ Ability to change the entry status to verified with one click

**Verification Levels**:
- 🔴 Unverified (new submissions)
- 🟡 Community Verified (multiple user confirmations)
- 🟢 Expert Verified (approved by expert)

**Technical Notes**:
- Separate admin role in Supabase
- Admin-only RLS policies
- Notification system for new submissions

**Dependencies**: US-009 (Wiki Browsing)

**Estimated Effort**: 4 days

---

## Analytics & Reporting

### US-011: Pottery Wrapped

**Priority**: 🔵 Low (Post-MVP)

**Description**: As a user, I want to see my successes visually at the end of the month.

**Acceptance Criteria**:
1. ✅ Displaying the number of successfully completed works
2. ✅ Visualization of the Yield Rate on a pie chart

**Metrics to Display**:
- Total works created
- Works completed successfully
- Works discarded/failed
- Yield Rate percentage
- Most used category
- Average drying time
- Monthly comparison

**Technical Notes**:
- Calculate from local SQLite data
- Use Compose charts library
- Generate monthly reports
- Share functionality (image export)

**Dependencies**: US-002, US-005 (Work lifecycle)

**Estimated Effort**: 6 days

---

## Security & Privacy

### US-012: Data Security (RLS)

**Priority**: 🔴 Critical (MVP Blocker)

**Description**: As a user, I want to be sure that no one else sees my private projects in the cloud.

**Acceptance Criteria**:
1. ✅ Queries to PostgreSQL are filtered by Row Level Security based on `auth.uid()`
2. ✅ It is not possible to read another user's record even if the work ID is known

**RLS Policies Required**:

```sql
-- Works table
CREATE POLICY "Users can only access their own works"
ON works FOR ALL
USING (auth.uid() = user_id);

-- Glazes table
CREATE POLICY "Users can only access their own glazes"
ON glazes FOR ALL
USING (auth.uid() = user_id);

-- Wiki (public read, authenticated write)
CREATE POLICY "Anyone can read verified wiki entries"
ON wiki_materials FOR SELECT
USING (verified = true);

CREATE POLICY "Authenticated users can submit wiki entries"
ON wiki_materials FOR INSERT
WITH CHECK (auth.uid() = submitted_by);
```

**Technical Notes**:
- Enable RLS on all user data tables
- Test policies thoroughly
- Audit logs for security events

**Dependencies**: US-001 (Authentication)

**Estimated Effort**: 3 days

---

## Internationalization

### US-013: Multi-Language UI Support

**Priority**: 🟡 High

**Description**: As a user, I want to use the application in my preferred language so that I can understand all UI elements and messages.

**Acceptance Criteria**:
1. ✅ User can select their preferred language from settings
2. ✅ All UI text, labels, and messages are displayed in the selected language
3. ✅ Language preference is persisted across app sessions
4. ✅ App respects device language settings by default

**Supported Languages (MVP)**:
- Polish (pl) - Default
- English (en)

**Technical Notes**:
- Use Android resources for string localization
- Store language preference in local database
- Implement fallback to English for missing translations
- Support RTL languages in future phases

**UI Elements to Localize**:
- Screen titles and navigation labels
- Button text and action labels
- Form field labels and placeholders
- Error messages and validation text
- Timer and status labels
- Category names
- Help text and tooltips

**Dependencies**: US-001 (Authentication for preference storage)

**Estimated Effort**: 5 days

---

## Story Mapping

### MVP Phase 1 (Critical Path)
```
US-001 → US-002 → US-003 → US-004 → US-005
   ↓        ↓
US-012   US-007 → US-008
   ↓
US-013 (Multi-language)
```

### MVP Phase 2 (Enhanced Features)
```
US-006 (Glaze Inventory)
US-009 (Wiki Browsing)
```

### Post-MVP
```
US-010 (Expert Verification)
US-011 (Pottery Wrapped)
```

## Estimation Summary

| Priority | Stories | Total Effort |
|----------|---------|--------------|
| 🔴 Critical | 6 | ~34 days |
| 🟡 High | 3 | ~13 days |
| 🟢 Medium | 2 | ~11 days |
| 🔵 Low | 2 | ~10 days |
| **Total** | **13** | **~68 days** |

**Note**: Estimates are for single developer. With parallel work and team collaboration, timeline can be compressed to meet 3-month MVP goal.

## Quick Reference Tags

- **Authentication**: US-001, US-012
- **Work Management**: US-002, US-003, US-004, US-005
- **Inventory**: US-006
- **Offline/Sync**: US-007, US-008
- **Wiki**: US-009, US-010
- **Analytics**: US-011
- **Internationalization**: US-013

---

**Last Updated**: 2025-12-30
**Document Owner**: Product Team
**Review Cycle**: Sprint planning (bi-weekly)
