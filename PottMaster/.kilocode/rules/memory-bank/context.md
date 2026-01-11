# Project Context - PottMaster

## Project Overview

**PottMaster** is a mobile application designed to support ceramic artists in managing technological processes. The app enables precise tracking of drying stages and cataloging of ceramic works using unique identification codes.

### Key Information
- **Project Type**: Cross-platform Mobile Application (.NET MAUI)
- **Target Users**: Ceramic artists, pottery studios
- **Development Phase**: MVP Development
- **Timeline Goal**: Under 3 months to v1.0
- **Primary Language**: C#

## Problem Statement

Ceramic artists face several critical challenges:

1. **Drying Control**: Lack of precise control over drying time leads to cracking risk
2. **Work Identification**: Visual identification is unreliable in shared studios, causing mix-ups
3. **Material Documentation**: Difficulty maintaining reliable records of materials used (glazes, clays)
4. **Efficiency Tracking**: No data on studio efficiency (Yield Rate)

## Solution Approach

PottMaster addresses these challenges through:

- **Offline-First Architecture**: Full functionality without internet connection
- **Automated Timers**: Intelligent drying time calculation based on wall thickness
- **Unique Coding System**: Automatic generation of identification codes (Initials-CatCode-MMYY-Counter)
- **Material Cataloging**: Personal glaze inventory with stock control
- **Knowledge Base**: Public Wiki with expert verification system
- **Analytics**: Monthly performance reports (\"Pottery Wrapped\")

## Current State

### Completed
- Project structure initialized (.NET MAUI template)
- PRD documentation complete
- Technology stack defined (.NET MAUI)
- Supabase backend setup (initial schema, RLS policies, Edge functions for create-work and sync-batch)
- Basic UI structure (MainPage, AppShell, LoginPage, SignupPage, NewWorkPage, WorkDetailPage)
- Authentication implementation (email/password login/register with Supabase .NET client)
- Basic navigation with Shell (Login, Signup, Main, NewWork, WorkDetail pages)
- Domain models created (Work.cs, LocalWork.cs, UserProfiles.cs, LocalUserProfile.cs, WorkCategory.cs, WorkStatus.cs, RemoteWork.cs, Result.cs)
- Services implemented:
  - AuthService.cs, DbService.cs, WorkService.cs, SyncService.cs
  - ImageService.cs (photo capture, picking, compression)
  - AlertService.cs (UI alerts and confirmations)
  - ErrorHandlingService.cs (centralized error handling)
  - AuthStateService.cs (authentication state management)
  - Interfaces: IAuthService, IDbService, IWorkService, ISyncService, IImageService, IAlertService, IErrorHandlingService, IAuthStateService
- Repositories implemented:
  - LocalWorkRepository.cs (SQLite-based local storage)
  - IWorkRepository.cs interface
- ViewModels created (LoginViewModel.cs, SignupViewModel.cs, MainViewModel.cs, NewWorkViewModel.cs, WorkDetailViewModel.cs)
- Converters implemented:
  - InvertedBoolConverter.cs
  - IsNotNullConverter.cs
  - IsNullConverter.cs
  - WallThicknessToDaysConverter.cs (drying time calculation)
- Localization resources:
  - AppResources.resx (Polish - default)
  - AppResources.en.resx (English)
  - Full localization for NewWorkPage and WorkDetailPage
- Memory bank initialized and updated for .NET MAUI
- CI/CD pipeline configured with MAUI workload installation for GitHub Actions, updated with path filters to trigger only on code changes and branch changed to 'main'
- **Full SQLite/SQLite-net-pcl integration and offline support via DbService** ✅
- **Complete work registration implementation (US-002) with photo compression and local storage** ✅
- **Unique identification code generation (US-003)** ✅
- **Manual status correction (US-005)** ✅
- **Supabase data sync implementation (US-008)** ✅
- **Process timer management (US-004)** ✅ - Automatic status advancement and notifications when drying completes
- **SupabaseApi service implemented** - Direct Supabase client wrapper for API operations
- **Multiple photos per work implemented** - New Photo entity, database table, UI for adding/viewing multiple photos, sync support
- **Glaze Catalogue Migration**: Created migration script `20260106120000_populate_glaze_catalogue.sql` that inserts 12 professional glaze entries with complete technical specifications
- **System User**: Added system user profile for seeded data management
- **Glaze Properties**: Implemented detailed JSONB properties for each glaze including firing requirements, appearance, behavior, and application methods
- **Glaze Inventory Management**: Implementation with search, filtering, favorites, CRUD operations, and detailed glaze properties editing via tabbed interface (work-glaze linking pending)
  - GlazeInventoryPage with search bar, favorites filter, swipe-to-delete, and FAB for adding new glazes
  - GlazeDetailPage with comprehensive tabbed interface (Basic, Firing, Appearance, Behavior, Application, Advanced)
  - GlazeInventoryViewModel with full CRUD operations, search/filtering, and favorites management
  - GlazeDetailViewModel (NewGlazeViewModel.cs) with complete property management and validation
  - GlazeService and IGlazeService for data operations
  - GlazeRepository for local data access
  - Full integration with Supabase for cloud synchronization
  - Database schema supports work-glaze relationships (work_glazes junction table) but UI functionality not yet implemented

### Upcoming
- Image handling and upload to Supabase Storage
- Glaze inventory features (US-006)
- Wiki browsing and submission (US-009)

## Current Focus
- Glaze inventory management system fully implemented and tested
- Wiki browsing and submission features in development
- Analytics and reporting features planned for post-MVP

## Next Steps
- Implement work-glaze linking functionality (US-006 completion)
- Implement Wiki browsing functionality (US-009)
- Add expert verification system for Wiki entries (US-010)
- Develop Pottery Wrapped analytics feature (US-011)

## Project Boundaries

### Within MVP Scope ✅
- Full offline support (SQLite)
- Integration with Supabase (Auth, Database, Storage)
- Coding system and timers
- Work lifecycle management
- Glaze inventory management (completed)
- Automatic synchronization
- Basic Wiki browsing (in progress)

### Outside MVP Scope ❌
- Advanced AI algorithms for image analysis
- Social modules and marketplace
- Advanced analytics beyond basic Yield Rate
- Expert verification system for Wiki entries
- Pottery Wrapped analytics feature
- Creating social elements- creating groups, adding friends, sharing works within groups
- Adding logs to separate database table, sending logs during sync
- Adding photos to already existing works

## Success Metrics

| Metric | Target | Purpose |
|--------|--------|---------|
| Time-to-Market | < 3 months | Validate market fit quickly |
| Synchronization Success | 99.9% | Ensure data reliability |
| User Growth (Month 1) | 500 active accounts | Market validation |
| Yield Rate Improvement | +5% after 3 months | User value demonstration |

## Key Stakeholders

- **Primary Users**: Individual ceramic artists
- **Secondary Users**: Pottery studio managers
- **Expert Reviewers**: Ceramics experts for Wiki verification
- **Development Team**: .NET MAUI developers, Backend engineers

## Technical Constraints

1. **Offline-First Requirement**: Must work without internet in basement studios
2. **Platform**: Cross-platform (.NET MAUI)
3. **Data Security**: Row Level Security (RLS) for user data isolation
4. **Image Storage**: Client-side compression required for bandwidth efficiency
5. **Synchronization**: Background sync when network available

## Domain-Specific Considerations

### Ceramic Process Stages
1. **Wet Clay**: Initial shaping
2. **Leather Hard**: Partially dried, can be trimmed
3. **Bone Dry**: Fully dried, ready for bisque firing
4. **Bisque Fired**: First firing (cone 04-06)
5. **Glazed**: Glaze applied
6. **Glaze Fired**: Final firing (cone 5-10)

### Critical Timing
- Drying time varies by wall thickness (5mm = 4 days, 10mm = 7 days)
- Moving to next stage too early = cracking risk
- Moving too late = inefficiency

## References

- Full PRD: [brief.md](./brief.md)
- Technical Architecture: [architecture.md](./architecture.md)
- Development Guidelines: [guidelines.md](./guidelines.md)
- User Stories: [user-stories.md](./user-stories.md)

---

**Last Updated**: 2026-01-11
**Document Owner**: Development Team
**Review Cycle**: Weekly during MVP phase
