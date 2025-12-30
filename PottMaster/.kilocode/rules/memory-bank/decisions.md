# Architectural Decision Log - PottMaster

## Purpose

This document records significant architectural and technical decisions made during the development of PottMaster. Each decision includes context, alternatives considered, and consequences.

## Decision Format

```markdown
### ADR-XXX: [Decision Title]

**Date**: YYYY-MM-DD
**Status**: [Proposed | Accepted | Deprecated | Superseded]
**Deciders**: [Names/Roles]

**Context**:
What is the issue we're trying to solve?

**Decision**:
What did we decide to do?

**Alternatives Considered**:
1. Alternative 1 - Why not chosen
2. Alternative 2 - Why not chosen

**Consequences**:
- Positive: Benefits of this decision
- Negative: Drawbacks or trade-offs
- Neutral: Other impacts

**Related Decisions**: ADR-XXX, ADR-YYY
```

---

## Decisions

### ADR-001: Supabase as Backend-as-a-Service

**Date**: 2024-12-15  
**Status**: Accepted  
**Deciders**: Technical Lead, Product Owner

**Context**:
Need a backend solution that supports offline-first architecture, provides authentication, database, and file storage. Want to minimize backend development time to meet 3-month MVP deadline.

**Decision**:
Use Supabase (PostgreSQL + Auth + Storage) instead of building custom .NET backend.

**Alternatives Considered**:
1. **Custom .NET Backend**
   - ❌ Requires separate backend team
   - ❌ Longer development time
   - ❌ Additional infrastructure management
   - ✅ Maximum customization
   
2. **Firebase**
   - ❌ NoSQL (Firestore) less suitable for relational data
   - ❌ Vendor lock-in concerns
   - ✅ Mature ecosystem
   - ✅ Good offline support
   
3. **AWS Amplify**
   - ❌ More complex setup
   - ❌ Higher learning curve
   - ✅ Comprehensive AWS integration

**Consequences**:
- ✅ Positive: Faster time to market (no backend development)
- ✅ Positive: PostgreSQL provides relational data model
- ✅ Positive: Built-in Row Level Security (RLS)
- ✅ Positive: Real-time capabilities for future features
- ✅ Positive: Open source, can self-host if needed
- ⚠️ Negative: Less control over backend logic
- ⚠️ Negative: Potential scaling costs at high usage
- ⚠️ Neutral: Team needs to learn Supabase APIs

**Related Decisions**: ADR-002 (Offline-First), ADR-004 (PostgreSQL)

---

### ADR-002: Offline-First Architecture with SQLite

**Date**: 2024-12-16  
**Status**: Accepted  
**Deciders**: Technical Lead, UX Designer

**Context**:
Ceramic artists often work in basement studios with poor or no internet connectivity. App must be fully functional offline, with seamless sync when network becomes available.

**Decision**:
Implement offline-first architecture using SQLite as local database, with background synchronization to Supabase when online.

**Alternatives Considered**:
1. **Online-Only with Caching**
   - ❌ Requires internet for core functionality
   - ❌ Poor user experience in offline scenarios
   - ✅ Simpler implementation
   
2. **Realm Database**
   - ❌ Additional dependency
   - ❌ Sync complexity
   - ✅ Built-in sync capabilities
   
3. **Room (Android) + CoreData (iOS)**
   - ❌ Platform-specific implementations
   - ❌ Duplicate sync logic
   - ✅ Native platform integration

**Consequences**:
- ✅ Positive: Full functionality without internet
- ✅ Positive: Fast read/write operations (local)
- ✅ Positive: Better user experience in studios
- ✅ Positive: Reduced server load
- ⚠️ Negative: Sync complexity and conflict resolution
- ⚠️ Negative: Data consistency challenges
- ⚠️ Negative: Increased app complexity

**Related Decisions**: ADR-003 (SQLDelight), ADR-005 (Sync Strategy)

---

### ADR-003: SQLDelight for Type-Safe Database Access

**Date**: 2024-12-16
**Status**: Accepted
**Deciders**: Technical Lead

**Context**:
Need a database solution for Android that provides type safety

**Decision**:
Use SQLDelight for type-safe SQL queries.

**Alternatives Considered**:
1. **Room**
   - ✅ Excellent Android integration
   - ❌ More boilerplate for simple queries
   
2. **Raw SQLite**
   - ❌ No type safety
   - ❌ Error-prone string queries
   - ✅ Maximum flexibility

**Consequences**:
- ✅ Positive: Type-safe queries at compile time
- ✅ Positive: SQL knowledge directly applicable
- ✅ Positive: Excellent IDE support
- ⚠️ Negative: Learning curve for SQLDelight-specific patterns
- ⚠️ Neutral: Need to write SQL manually

**Related Decisions**: ADR-002 (Offline-First)

---

### ADR-004: PostgreSQL for Cloud Database

**Date**: 2024-12-16  
**Status**: Accepted  
**Deciders**: Technical Lead

**Context**:
Need a cloud database that supports relational data model, provides strong consistency, and integrates with Supabase.

**Decision**:
Use PostgreSQL (via Supabase) for cloud data storage.

**Alternatives Considered**:
1. **MongoDB/NoSQL**
   - ❌ Less suitable for relational ceramic data
   - ❌ Weaker consistency guarantees
   - ✅ Flexible schema
   
2. **MySQL**
   - ❌ Not default Supabase option
   - ✅ Widely known
   
3. **SQLite (cloud-hosted)**
   - ❌ Not designed for concurrent access
   - ❌ Limited scalability

**Consequences**:
- ✅ Positive: Relational model fits domain well
- ✅ Positive: ACID compliance
- ✅ Positive: Row Level Security (RLS) support
- ✅ Positive: JSON support for flexible fields
- ✅ Positive: Full-text search capabilities
- ⚠️ Neutral: Schema migrations need management

**Related Decisions**: ADR-001 (Supabase)

---

### ADR-005: Last-Write-Wins Conflict Resolution

**Date**: 2024-12-17  
**Status**: Accepted  
**Deciders**: Technical Lead, Product Owner

**Context**:
When syncing offline changes, conflicts can occur if the same work is modified on multiple devices. Need a simple, predictable conflict resolution strategy for MVP.

**Decision**:
Use Last-Write-Wins (LWW) strategy based on `updated_at` timestamp for conflict resolution.

**Alternatives Considered**:
1. **Manual Conflict Resolution**
   - ❌ Complex UI for conflict resolution
   - ❌ Poor user experience
   - ✅ User has full control
   
2. **Operational Transformation (OT)**
   - ❌ Very complex to implement
   - ❌ Overkill for MVP
   - ✅ Sophisticated merging
   
3. **First-Write-Wins**
   - ❌ Later changes lost
   - ❌ Confusing for users

**Consequences**:
- ✅ Positive: Simple to implement and understand
- ✅ Positive: Predictable behavior
- ✅ Positive: No user intervention needed
- ⚠️ Negative: Potential data loss in rare multi-device scenarios
- ⚠️ Negative: May need revision for collaborative features
- ⚠️ Neutral: Acceptable for single-user MVP

**Related Decisions**: ADR-002 (Offline-First)

---

### ADR-006: Client-Side Image Compression

**Date**: 2024-12-18  
**Status**: Accepted  
**Deciders**: Technical Lead

**Context**:
Users will take photos of ceramic works. Need to balance image quality with storage costs and upload bandwidth, especially in poor network conditions.

**Decision**:
Compress images on client-side before upload: max 1920x1920px, 80% JPEG quality, target <500KB per image.

**Alternatives Considered**:
1. **Server-Side Compression**
   - ❌ Wastes bandwidth uploading large files
   - ❌ Slower user experience
   - ✅ Centralized processing
   
2. **No Compression**
   - ❌ High storage costs
   - ❌ Slow uploads in poor network
   - ✅ Maximum quality
   
3. **Aggressive Compression (<200KB)**
   - ❌ Noticeable quality loss
   - ✅ Minimal storage/bandwidth

**Consequences**:
- ✅ Positive: Faster uploads
- ✅ Positive: Lower storage costs
- ✅ Positive: Better offline experience
- ✅ Positive: Acceptable quality for documentation
- ⚠️ Negative: Some quality loss
- ⚠️ Neutral: Platform-specific compression code needed

**Related Decisions**: ADR-001 (Supabase Storage)

---

### ADR-007: Koin for Dependency Injection

**Date**: 2024-12-19
**Status**: Accepted
**Deciders**: Technical Lead

**Context**:
Need a dependency injection framework that is easy to set up for an Android project.

**Decision**:
Use Koin for dependency injection.

**Alternatives Considered**:
1. **Manual DI**
   - ❌ Boilerplate code
   - ❌ Error-prone
   - ✅ No dependencies
   
2. **Kodein**
   - ⚠️ Similar to Koin
   - ⚠️ Smaller community
   
3. **Dagger/Hilt**
   - ❌ Complex setup
   - ✅ Compile-time safety

**Consequences**:
- ✅ Positive: Simple, lightweight
- ✅ Positive: Easy to learn
- ✅ Positive: Good documentation
- ⚠️ Negative: Runtime DI (no compile-time checks)
- ⚠️ Neutral: Adequate for MVP scope

**Related Decisions**: None

---

### ADR-008: Compose Resources for Internationalization

**Date**: 2025-12-29
**Status**: Accepted
**Deciders**: Technical Lead, Product Owner

**Context**:
PottMaster needs to support multiple languages to serve an international user base of ceramic artists. The app should support at least Polish (primary market), English, German, Spanish, and French in the MVP phase.

**Decision**:
Use Compose Multiplatform Resources for string localization with Polish as the default language.

**Alternatives Considered**:
1. **Platform-Specific Resources (Android strings.xml + iOS .strings)**
   - ❌ Duplicate translation files
   - ❌ Inconsistent translations between platforms
   - ❌ More maintenance overhead
   - ✅ Native platform integration
   
2. **Third-Party Libraries (Moko Resources)**
   - ⚠️ Additional dependency
   - ⚠️ Similar functionality to Compose Resources
   - ✅ Mature solution
   
3. **Manual String Management**
   - ❌ No compile-time safety
   - ❌ Error-prone
   - ❌ Difficult to maintain
   - ✅ Maximum flexibility

**Consequences**:
- ✅ Positive: Single source of truth for all translations
- ✅ Positive: Type-safe string access in shared code
- ✅ Positive: Automatic fallback to default language
- ✅ Positive: Standard Android resource format (strings.xml)
- ✅ Positive: Easy to add new languages
- ⚠️ Negative: Requires recomposition for language changes
- ⚠️ Neutral: Translation files need to be kept in sync

**Implementation Details**:
- Default language: Polish (pl)
- Supported languages: Polish, English, German, Spanish, French
- Store user language preference in local database
- Respect device language settings by default
- Fallback to English for missing translations

**Related Decisions**: None

---

## Decision Status Summary

| Status | Count | Decisions |
|--------|-------|-----------|
| Accepted | 8 | ADR-001 through ADR-008 |
| Proposed | 0 | - |
| Deprecated | 0 | - |
| Superseded | 0 | - |

## Future Decisions to Consider

### Pending Evaluation

1. **Real-time Collaboration**
   - When: Post-MVP
   - Context: Multiple users working on shared studio inventory
   - Options: Supabase Realtime, WebSockets, Polling

2. **Advanced Analytics**
   - When: Post-MVP
   - Context: Complex analytics beyond basic Yield Rate
   - Options: Separate analytics DB, BigQuery, local aggregation

3. **Image Recognition**
   - When: Future phase
   - Context: Auto-categorize works from photos
   - Options: TensorFlow Lite, Cloud Vision API, On-device ML

4. **Marketplace Integration**
   - When: Future phase
   - Context: Selling finished works
   - Options: Stripe, PayPal, In-app purchases

## Review Process

- **Frequency**: Review decisions quarterly or when major changes proposed
- **Participants**: Technical Lead, Development Team, Product Owner
- **Criteria**: Still valid? Need updates? Should be deprecated?

---

**Last Updated**: 2025-12-30
**Document Owner**: Technical Lead
**Review Cycle**: Quarterly or on major architectural changes

**Last Updated**: 2025-12-30
