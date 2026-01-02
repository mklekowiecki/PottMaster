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
- CI/CD pipeline configured with MAUI workload installation for GitHub Actions
- **Full SQLite/SQLite-net-pcl integration and offline support via DbService** ✅
- **Complete work registration implementation (US-002) with photo compression and local storage** ✅
- **Unique identification code generation (US-003)** ✅
- **Manual status correction (US-005)** ✅
- **Supabase data sync implementation (US-008)** ✅
- **Process timer management (US-004)** ✅ - Automatic status advancement and notifications when drying completes

### Upcoming
- Image handling and upload to Supabase Storage
- Glaze inventory features (US-006)
- Wiki browsing and submission (US-009)

## Project Boundaries

### Within MVP Scope ✅
- Full offline support (SQLite)
- Integration with Supabase (Auth, Database, Storage)
- Coding system and timers
- Basic Wiki version
- Work lifecycle management
- Glaze inventory
- Automatic synchronization

### Outside MVP Scope ❌
- Proprietary .NET server infrastructure
- Advanced AI algorithms for image analysis
- Social modules and marketplace
- Advanced analytics beyond basic Yield Rate

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

**Last Updated**: 2026-01-01
**Document Owner**: Development Team
**Review Cycle**: Weekly during MVP phase
