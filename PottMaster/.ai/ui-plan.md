# UI Implementation Plan – PottMaster (.NET MAUI)

## 1. Overview

This plan outlines the step-by-step approach for implementing the PottMaster MVP UI in .NET MAUI, based on the user stories and design guidelines. The focus is on modularity, localization, offline-first UX, and clear status feedback.

---

## 2. General Principles

- **MVVM Pattern**: Use ViewModels for all pages.
- **Localization**: All UI text via RESX files.
- **Accessibility**: Ensure color contrast, font scaling, and screen reader support.
- **Responsive Layout**: Use MAUI layouts for adaptive design (mobile/tablet/desktop).
- **Offline-First**: UI must not block on network; show sync status.

---

## 3. Page-by-Page Implementation (Status)

### 3.1 Authentication & User Management

- **LoginPage** *(Done)*
  - Email/password fields, "Sign in with Google/Apple" buttons.
  - Error and loading states.
  - Link to SignupPage.
- **SignupPage** *(Done)*
  - Registration form (email, password, confirm).
  - Validation and error feedback.
- **Profile/Settings** *(Not Started)*
  - Display initials, allow logout, language selection.

### 3.2 Work Lifecycle Management

- **MainPage (Work List)** 
  - List of works with status, photo, countdown timer.
  - "Add New Work" FAB/button.
- **NewWorkPage** *(Not Started)*
  - Photo capture/upload, category picker, wall thickness slider.
  - Save button, validation.
- **WorkDetailPage** *(Not Started)*
  - Show unique code, drying timer, status progression.
  - "Ready for Firing" button with confirmation dialog.

### 3.3 Material & Inventory Management

- **GlazeInventoryPage** *(Not Started)*
  - List of glazes, add/edit glaze dialog.
  - Link glazes to works.

### 3.4 Knowledge Base (Wiki)

- **WikiPage** *(Not Started)*
  - Search bar, list of materials/tools.
  - Detail view with trust tags.

### 3.5 Analytics & Reporting

- **AnalyticsPage** *(Not Started)*
  - Monthly summary, pie chart (MAUI Graphics), yield rate.
  - Share/export report.

### 3.6 Offline & Synchronization

- **SyncStatusIndicator** *(Not Started)*
  - Global component in AppShell.
  - Shows sync state (pending, syncing, error, up-to-date).

---

## 4. Cross-Cutting Features (Status)

- **Localization** *(In Progress)*
  - All UI strings in `AppResources.resx` and `AppResources.en.resx`.
  - Language selector in settings. *(Not Started)*
- **Theming** *(In Progress)*
  - Use `Styles.xaml` for colors, fonts, and spacing.
- **Dialogs & Toasts** *(Not Started)*
  - Use MAUI Community Toolkit for dialogs, confirmations, and error toasts.

---

## 5. Implementation Sequence (with Status)

1. **Authentication UI**: LoginPage *(Done)*, SignupPage *(Done)*, Profile/Settings *(Not Started)*.
2. **Work Management**: MainPage *(Done)*, NewWorkPage *(Not Started)*, WorkDetailPage *(Not Started)*.
3. **Offline/Sync Indicator**: SyncStatusIndicator *(Not Started)*.
4. **Glaze Inventory**: GlazeInventoryPage *(Not Started)*.
5. **Wiki**: WikiPage *(Not Started)*.
6. **Analytics**: AnalyticsPage *(Not Started)*.
7. **Localization**: Integrate RESX *(In Progress)*, language selector *(Not Started)*.
8. **Polish & Accessibility**: Theming *(In Progress)*, accessibility, responsive tweaks *(Not Started)*.

---

## 6. UI Component Reuse

- **Custom Controls**: Status badge, countdown timer, photo thumbnail, sync indicator.
- **Data Templates**: For lists (works, glazes, wiki entries).

---

## 7. Testing & Review

- **Manual UI walkthroughs** for each user story.
- **Localization checks** for all supported languages.
- **Offline/online scenario testing**.
- **Accessibility review**.

---

## 8. References

- [User Stories Reference](../.kilocode/rules/memory-bank/user-stories.md)
- [MAUI UI Guidelines](../.kilocode/rules/maui-guidelines.md)
- [AppResources.resx](../Resources/AppResources.resx)

---

**Last updated:** 2026-01-01

---

**Status Legend:**
- *(Done)*: Implemented and available in codebase
- *(In Progress)*: Partially implemented or under development
- *(Not Started)*: No implementation yet