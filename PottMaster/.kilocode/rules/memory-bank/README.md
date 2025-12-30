# PottMaster Memory Bank

## Overview

This memory bank contains comprehensive documentation for the PottMaster project - an Android mobile application for ceramic artists. All documents are designed to provide quick context and reference for development work.

## Document Structure

### 📋 [brief.md](./brief.md) - Product Requirements Document (PRD)
**Purpose**: Complete product specification and requirements  
**Use When**: Understanding product vision, features, and user stories  
**Key Sections**:
- Product overview and technical architecture
- User problems and solutions
- Functional requirements
- Complete user stories (US-001 to US-012)
- Success metrics

---

### 🎯 [context.md](./context.md) - Project Context
**Purpose**: Quick project overview and current state  
**Use When**: Onboarding, understanding project scope and boundaries  
**Key Sections**:
- Project overview and problem statement
- Current development state
- Project boundaries (in/out of scope)
- Success metrics
- Domain-specific considerations

---

### 🏗️ [architecture.md](./architecture.md) - Technical Architecture
**Purpose**: System design and technical decisions  
**Use When**: Implementing features, understanding data flow, making technical decisions  
**Key Sections**:
- Architecture diagrams
- Technology stack details
- Data models and database schemas
- Offline-first strategy
- Security architecture (RLS)
- Synchronization patterns

---

### 📐 [guidelines.md](./guidelines.md) - Development Guidelines
**Purpose**: Code standards and best practices  
**Use When**: Writing code, reviewing PRs, setting up development environment  
**Key Sections**:
- Project structure and organization
- Naming conventions
- Offline-first principles
- Code style and patterns
- Testing strategy
- Performance guidelines

---

### 📖 [user-stories.md](./user-stories.md) - User Stories Reference
**Purpose**: Feature requirements organized by area  
**Use When**: Planning sprints, implementing features, understanding acceptance criteria  
**Key Sections**:
- Stories organized by feature area
- Priority indicators
- Acceptance criteria
- Technical notes
- Effort estimates
- Story mapping and dependencies

---

### 📝 [decisions.md](./decisions.md) - Architectural Decision Log
**Purpose**: Record of significant technical decisions  
**Use When**: Understanding why certain technologies/patterns were chosen  
**Key Sections**:
- Decision records (ADR-001 to ADR-010)
- Context and alternatives considered
- Consequences and trade-offs
- Future decisions to consider

**Current Decisions**:
- ADR-001: Supabase as BaaS
- ADR-002: Offline-First Architecture
- ADR-003: SQLDelight
- ADR-004: PostgreSQL
- ADR-005: Last-Write-Wins Conflict Resolution
- ADR-006: Client-Side Image Compression
- ADR-007: Koin for DI
- ADR-008: Compose Resources for Internationalization

---

### 📚 [glossary.md](./glossary.md) - Terminology & Glossary
**Purpose**: Domain terms, technical abbreviations, and concepts  
**Use When**: Understanding unfamiliar terms, ensuring consistent language  
**Key Sections**:
- Ceramic and pottery terms
- Technical terms and patterns
- Business metrics
- Code identifiers and conventions
- Common abbreviations

---

## Quick Reference

### Technology Stack
- **Mobile**: Android
- **UI**: Jetpack Compose
- **Local DB**: SQLite + SQLDelight
- **Backend**: Supabase (PostgreSQL + Auth + Storage)
- **DI**: Koin
- **Platforms**: Android

### Key Principles
1. **Offline-First**: All operations work without internet
2. **Type Safety**: Kotlin across entire stack
3. **Security**: Row Level Security (RLS) for data isolation
4. **User Experience**: Fast, reliable, works in basement studios

### Project Goals
- **Time-to-Market**: < 3 months to MVP
- **Sync Success**: > 99.9%
- **User Growth**: 500 active users in month 1
- **Yield Rate**: +5% improvement after 3 months

## Navigation Guide

### For New Developers
1. Start with [`context.md`](./context.md) - Understand the project
2. Read [`architecture.md`](./architecture.md) - Learn the technical design
3. Review [`guidelines.md`](./guidelines.md) - Follow coding standards
4. Reference [`glossary.md`](./glossary.md) - Learn domain terminology

### For Feature Implementation
1. Check [`user-stories.md`](./user-stories.md) - Find acceptance criteria
2. Review [`architecture.md`](./architecture.md) - Understand data flow
3. Follow [`guidelines.md`](./guidelines.md) - Apply best practices
4. Update [`decisions.md`](./decisions.md) - Document significant choices

### For Planning & Design
1. Review [`brief.md`](./brief.md) - Understand full requirements
2. Check [`context.md`](./context.md) - Verify scope boundaries
3. Consult [`decisions.md`](./decisions.md) - Learn from past decisions
4. Reference [`user-stories.md`](./user-stories.md) - Prioritize features

## Document Maintenance

### Update Frequency
- **context.md**: Weekly during active development
- **architecture.md**: When architectural changes occur
- **guidelines.md**: Monthly or when standards change
- **user-stories.md**: Sprint planning (bi-weekly)
- **decisions.md**: When significant decisions made
- **glossary.md**: When new terms introduced
- **brief.md**: Major product pivots only

### Ownership
- **Product Owner**: brief.md, context.md, user-stories.md
- **Technical Lead**: architecture.md, decisions.md, guidelines.md
- **Development Team**: glossary.md, all documents (contributions)

### Review Process
1. Changes proposed via PR
2. Reviewed by document owner
3. Team discussion for major changes
4. Update "Last Updated" date
5. Merge and communicate changes

## Memory Bank Usage

### For AI Assistants
This memory bank provides comprehensive context for:
- Understanding project requirements
- Making informed technical decisions
- Following established patterns
- Maintaining consistency
- Avoiding repeated questions

### For Human Developers
Use this memory bank to:
- Onboard quickly to the project
- Find answers to common questions
- Understand historical context
- Make aligned decisions
- Contribute effectively

## File Structure

```
.kilo/rules/memory-bank/
├── README.md              # This file - navigation guide
├── brief.md               # Product Requirements Document
├── context.md             # Project context and overview
├── architecture.md        # Technical architecture
├── guidelines.md          # Development guidelines
├── user-stories.md        # User stories reference
├── decisions.md           # Architectural decision log
└── glossary.md            # Terminology and glossary
```

## Contributing

When updating memory bank documents:

1. **Keep it Current**: Update documents as project evolves
2. **Be Concise**: Clear, actionable information
3. **Cross-Reference**: Link related documents
4. **Use Examples**: Code snippets, diagrams, tables
5. **Date Updates**: Always update "Last Updated" field
6. **Maintain Format**: Follow existing structure

## Support

For questions about:
- **Product Features**: See brief.md or user-stories.md
- **Technical Design**: See architecture.md or decisions.md
- **Code Standards**: See guidelines.md
- **Terminology**: See glossary.md
- **Project Status**: See context.md

---

**Memory Bank Initialized**: 2025-12-29
**Last Updated**: 2025-12-30
**Version**: 1.0.0
**Status**: ✅ Complete and Active
