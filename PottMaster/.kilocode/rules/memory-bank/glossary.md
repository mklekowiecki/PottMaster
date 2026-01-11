# Glossary & Terminology - PottMaster

## Purpose

This document defines domain-specific terminology, technical abbreviations, and key concepts used throughout the PottMaster project. Use this as a reference to ensure consistent language across documentation and code.

---

## Ceramic & Pottery Terms

### Work Stages

| Term | Definition | Duration |
|------|------------|----------|
| **Wet Clay** | Freshly shaped clay, still plastic and workable | Immediate after creation |
| **Leather Hard** | Partially dried clay, firm but still carvable | 1-3 days after shaping |
| **Bone Dry** | Completely dried clay, ready for bisque firing | 4-14 days (thickness dependent) |
| **Greenware** | Unfired clay objects (wet to bone dry) | - |
| **Bisque / Biscuit** | Clay fired once without glaze (cone 04-06, ~1000°C) | 8-12 hours firing |
| **Glazed** | Bisque ware with glaze applied, ready for final firing | - |
| **Glaze Fired** | Final firing with glaze (cone 5-10, ~1200-1300°C) | 10-16 hours firing |

### Materials

| Term | Definition | Example |
|------|------------|---------|
| **Clay Body** | Mixture of clays and additives for specific properties | Stoneware, Porcelain, Earthenware |
| **Glaze** | Glass-like coating applied to ceramic surface | Clear glaze, Celadon, Tenmoku |
| **Slip** | Liquid clay used for decoration or joining | White slip, colored slip |
| **Grog** | Pre-fired clay added to clay body for texture/strength | Fine grog, coarse grog |
| **Flux** | Material that lowers melting point of glaze | Feldspar, Frit |

### Measurements

| Term | Definition | Common Values |
|------|------------|---------------|
| **Wall Thickness** | Thickness of clay wall in piece | 3-15mm typical |
| **Cone Rating** | Temperature indicator for firing | Cone 04, 6, 10 |
| **Shrinkage** | Clay reduction during drying/firing | 10-15% typical |
| **Absorption Rate** | Porosity of fired clay | 0-15% |

### Processes

| Term | Definition | Notes |
|------|------------|-------|
| **Throwing** | Shaping clay on pottery wheel | Creates round forms |
| **Hand Building** | Shaping clay without wheel | Coiling, slab building, pinching |
| **Trimming** | Refining shape at leather hard stage | Removes excess clay |
| **Bisque Firing** | First firing to harden clay | Makes clay porous for glazing |
| **Glaze Firing** | Final firing to melt glaze | Higher temperature than bisque |
| **Reduction Firing** | Firing with limited oxygen | Creates special glaze effects |

### Defects & Issues

| Term | Definition | Cause |
|------|------------|-------|
| **Cracking** | Splits in clay surface | Too fast drying, uneven thickness |
| **Warping** | Distortion of shape | Uneven drying, poor support |
| **Dunting** | Cracking during cooling | Too fast cooling |
| **Crazing** | Fine cracks in glaze | Glaze/clay expansion mismatch |
| **Pinholing** | Small holes in glaze | Trapped gases, contamination |
| **Bloating** | Swelling/bubbling of clay | Overfiring, organic matter |

---

## Technical Terms

### Architecture & Patterns

| Term | Definition | Usage in PottMaster |
|------|------------|---------------------|
| **BaaS** | Backend as a Service - cloud backend without custom server | Supabase provides auth, database, storage |
| **RLS** | Row Level Security - database-level access control | PostgreSQL policies ensure data isolation |
| **Offline-First** | Architecture prioritizing local data and offline functionality | All operations work without internet |
| **Repository Pattern** | Abstraction layer between data sources and business logic | WorkRepository, GlazeRepository |
| **Use Case** | Single business operation encapsulated in a class | CreateWorkUseCase, SyncWorksUseCase |
| **ViewModel** | UI state management and business logic coordinator | WorkListViewModel, WorkDetailViewModel |

### Data & Synchronization

| Term | Definition | Implementation |
|------|------------|----------------|
| **Sync Status** | State of data synchronization with cloud | PENDING, SYNCING, SYNCED, ERROR |
| **LWW** | Last-Write-Wins - conflict resolution strategy | Timestamp-based conflict resolution |
| **DTO** | Data Transfer Object - data structure for API communication | Maps between domain models and API |
| **Entity** | Database table representation | SQLite-net-pcl attribute-based models |
| **Migration** | Database schema version update | Manual schema updates via SQLite commands |

### Mobile Development

| Term | Definition | Technology |
|------|------------|-----------|
| **ContentPage** | Basic page container in .NET MAUI | XAML pages |
| **ObservableCollection** | Reactive collection for data binding | MVVM data binding |
| **async/await** | Asynchronous programming model | Task-based async operations |
| **DI** | Dependency Injection - providing dependencies to classes | CommunityToolkit.Mvvm |
| **i18n** | Internationalization - supporting multiple languages | .NET MAUI Localization |
| **l10n** | Localization - adapting app for specific locale | Resource files (.resx) |
| **CultureInfo** | Regional settings for language, date, number formats | en-US, pl-PL, de-DE |
---

## Business Metrics

| Metric | Definition | Calculation | Target |
|--------|------------|-------------|--------|
| **Yield Rate** | Percentage of works successfully completed | (Completed / Total Created) × 100 | >85% |
| **Time-to-Market** | Development time from start to v1.0 release | Calendar days | <90 days |
| **Sync Success Rate** | Percentage of successful synchronizations | (Successful Syncs / Total Sync Attempts) × 100 | >99.9% |
| **Active Users** | Users who logged in within last 30 days | Count of unique users | 500 (Month 1) |
| **Average Drying Time** | Mean time from creation to bone dry | Sum(drying times) / Count(works) | Varies by thickness |

---

## Code Identifiers

### Work Categories

| Code | Category | Description |
|------|----------|-------------|
| `CUP` | Cup | Drinking vessels |
| `BOWL` | Bowl | Bowls of all sizes |
| `VASE` | Vase | Flower vases |
| `PLATE` | Plate | Plates and platters |
| `SCULPTURE` | Sculpture | Decorative sculptures |
| `TILE` | Tile | Ceramic tiles |
| `OTHER` | Other | Miscellaneous items |

### Work Statuses

| Status | Description | Next Status |
|--------|-------------|-------------|
| `WET` | Freshly created | LEATHER_HARD |
| `LEATHER_HARD` | Partially dried | BONE_DRY |
| `BONE_DRY` | Fully dried | BISQUE_FIRED |
| `BISQUE_FIRED` | First firing complete | GLAZED |
| `GLAZED` | Glaze applied | GLAZE_FIRED |
| `GLAZE_FIRED` | Final firing complete | COMPLETED |
| `COMPLETED` | Finished piece | - |
| `DISCARDED` | Failed/broken | - |

### Sync Statuses

| Status | Description | User Action |
|--------|-------------|-------------|
| `PENDING` | Created locally, not synced | None (automatic) |
| `SYNCING` | Currently uploading to cloud | None (in progress) |
| `SYNCED` | Successfully synchronized | None (complete) |
| `CONFLICT` | Sync conflict detected | May need resolution |
| `ERROR` | Sync failed, will retry | Check network |

---

## Abbreviations

### Project-Specific

| Abbreviation | Full Term | Context |
|--------------|-----------|---------|
| **PM** | PottMaster | Project name |
| **MVP** | Minimum Viable Product | Initial release scope |
| **PRD** | Product Requirements Document | Planning document |
| **ADR** | Architectural Decision Record | Decision documentation |

### Technical

| Abbreviation | Full Term | Context |
|--------------|-----------|---------|
| **API** | Application Programming Interface | Supabase API, REST API |
| **JWT** | JSON Web Token | Authentication token |
| **SQL** | Structured Query Language | Database queries |
| **UI** | User Interface | Visual components |
| **UX** | User Experience | User interaction design |
| **CRUD** | Create, Read, Update, Delete | Basic data operations |
| **ORM** | Object-Relational Mapping | Database abstraction |
| **CDN** | Content Delivery Network | Image distribution (future) |
| **i18n** | Internationalization | Multi-language support |
| **l10n** | Localization | Regional adaptation |
| **RTL** | Right-to-Left | Text direction for Arabic, Hebrew |

### Development

| Abbreviation | Full Term | Context |
|--------------|-----------|---------|
| **PR** | Pull Request | Code review process |
| **CI/CD** | Continuous Integration/Deployment | Automated build/deploy |
| **QA** | Quality Assurance | Testing process |
| **TDD** | Test-Driven Development | Development methodology |
| **IDE** | Integrated Development Environment | Android Studio, Xcode |

---

## User Roles

| Role | Description | Permissions |
|------|-------------|-------------|
| **User** | Regular ceramic artist | Create/manage own works, read public wiki |
| **Expert** | Verified ceramics expert | User permissions + verify wiki entries |
| **Admin** | System administrator | All permissions + user management |

---

## File Naming Conventions

### Code Files

```
WorkRepository.cs           // Repository interface/implementation
CreateWorkUseCase.cs        // Use case
WorkListViewModel.cs        // ViewModel
WorkListPage.xaml           // XAML page
WorkCard.xaml               // UI component
Work.cs                     // Domain model
WorkDto.cs                  // Data transfer object
WorkEntity.cs               // Database entity
```
### Database Files

No specific query files; uses SQLite-net-pcl attributes in C# models for ORM.
### Resource Files

```
Colors.xaml                 // Color definitions
Styles.xaml                 // Theme definitions
AppResources.resx           // Localized strings (default English)
AppResources.pl.resx        // Polish translations
AppResources.de.resx        // German translations
AppResources.es.resx        // Spanish translations
AppResources.fr.resx        // French translations
```
### Supported Languages

| Code | Language | Status |
|------|----------|--------|
| `pl` | Polish | Default |
| `en` | English | MVP |
| `de` | German | MVP |
| `es` | Spanish | MVP |
| `fr` | French | MVP |

---

## Common Patterns

### Naming Patterns

| Pattern | Example | Usage |
|---------|---------|-------|
| `{Entity}Repository` | WorkRepository | Data access layer |
| `{Action}{Entity}UseCase` | CreateWorkUseCase | Business logic |
| `{Screen}ViewModel` | WorkListViewModel | UI state management |
| `{Screen}Screen` | WorkListScreen | Screen composable |
| `{Entity}Card` | WorkCard | List item component |
| `{Entity}Dto` | WorkDto | API data structure |

### Code Identifiers

| Pattern | Example | Description |
|---------|---------|-------------|
| Work Code | `MK-CUP-1224-001` | Initials-Category-MMYY-Counter |
| User ID | `uuid-v4` | Supabase auth.uid() |
| Work ID | `uuid-v4` | Unique work identifier |
| Glaze ID | `uuid-v4` | Unique glaze identifier |

---

## Domain Concepts

### Work Lifecycle

```
Creation → Drying → Bisque Firing → Glazing → Glaze Firing → Completion
   ↓          ↓           ↓             ↓            ↓            ↓
  WET → LEATHER_HARD → BONE_DRY → BISQUE_FIRED → GLAZED → GLAZE_FIRED → COMPLETED
                                                                              ↓
                                                                         DISCARDED
```

### Data Flow

```
User Action → ViewModel → UseCase → Repository → Local DB (SQLite)
                                                      ↓
                                                 Sync Queue
                                                      ↓
                                              (when online)
                                                      ↓
                                            Remote DB (PostgreSQL)
```

### Sync Flow

```
Local Change → Mark PENDING → Queue for Sync → Network Available → Upload to Cloud
                                                                          ↓
                                                                    Mark SYNCED
```

---

## References

### External Resources

- [Ceramic Arts Network](https://ceramicartsnetwork.org/) - Ceramic terminology
- [Supabase Docs](https://supabase.com/docs) - Backend platform
- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/) - Mobile framework

### Internal Documents

- [PRD](./brief.md) - Product requirements
- [Architecture](./architecture.md) - Technical architecture
- [Guidelines](./guidelines.md) - Development standards
- [User Stories](./user-stories.md) - Feature requirements
- [Decisions](./decisions.md) - Architectural decisions

---

**Last Updated**: 2026-01-11
**Document Owner**: Development Team
**Review Cycle**: Monthly or when new terms introduced
