# UI Architecture for PottMaster

## 1. UI Structure Overview

The UI architecture for PottMaster is designed as an offline-first, mobile-centric application using Compose Multiplatform for shared UI components across Android and iOS. It follows a clean, intuitive structure emphasizing ease of use for ceramic artists, with a focus on quick access to work management, timers, and inventory. The structure prioritizes a bottom navigation bar for primary sections (Works, Glazes, Wiki, Reports) and a modal or full-screen overlay for secondary actions like adding items or authentication. All views support localization, offline functionality with sync indicators, and accessibility features such as semantic labels and high-contrast modes. Security is integrated through secure authentication flows and data isolation indicators. The design addresses key user pain points like drying control, work identification, and documentation by providing prominent timers, unique codes, and searchable inventories.

## 2. View List

### AuthScreen
- **View path**: /auth
- **Main purpose**: Handle user registration, login, and profile setup to enable secure access and data synchronization.
- **Key information to display**: Login form (email/password), social sign-in buttons (Google, Apple), error messages, progress indicators for auth processes.
- **Key view components**: Form inputs, buttons, branded logo, privacy policy link.
- **UX, accessibility, and security considerations**: Simple, one-tap options for quick entry; screen reader support for form labels; secure token handling without exposing credentials; biometric auth fallback if available.

### WorkListScreen
- **View path**: /works
- **Main purpose**: Display and manage a list of all user works, showing status, timers, and sync states.
- **Key information to display**: List of works with codes, categories, photos, current status, remaining drying time, sync status icons.
- **Key view components**: Scrollable list (LazyColumn), search/filter bar, floating action button (FAB) for adding works, pull-to-refresh for manual sync.
- **UX, accessibility, and security considerations**: Visual status badges for quick scanning; large touch targets and voice-over for list items; data filtered by user ID via RLS, no cross-user visibility.

### AddWorkScreen
- **View path**: /works/add
- **Main purpose**: Allow users to register a new work with photo, category, and thickness details, generating a unique code immediately.
- **Key information to display**: Camera/photo picker, category dropdown, wall thickness slider, generated code preview, form validation feedback.
- **Key view components**: Image capture/preview, form fields, slider, submit button, offline save confirmation.
- **UX, accessibility, and security considerations**: Step-by-step wizard flow to reduce cognitive load; haptic feedback on slider; image compression handled client-side; local save with pending sync flag.

### WorkDetailScreen
- **View path**: /works/{id}
- **Main purpose**: Provide detailed view of a single work, including timer management and status updates.
- **Key information to display**: Full photo, work code, status timeline, countdown timer, applied glazes list, notes section.
- **Key view components**: Image viewer, progress indicator for drying, status update buttons (e.g., "Mark as Ready"), edit FAB.
- **UX, accessibility, and security considerations**: Real-time timer updates without full refresh; ARIA live regions for timer announcements; user-owned data only, with offline editing queued for sync.

### GlazeListScreen
- **View path**: /glazes
- **Main purpose**: Manage personal glaze inventory, view stock, and link to works.
- **Key information to display**: List of glazes with names, manufacturers, quantities, sync status.
- **Key view components**: Scrollable list, search bar, FAB for adding glazes, link button to assign to works.
- **UX, accessibility, and security considerations**: Quantity low-stock warnings; sortable list with keyboard navigation; private user data protected by RLS.

### AddGlazeScreen
- **View path**: /glazes/add
- **Main purpose**: Add a new glaze to inventory with details like name, color, and notes.
- **Key information to display**: Form for glaze properties, quantity input, optional photo.
- **Key view components**: Text fields, dropdowns for cone ratings, submit button.
- **UX, accessibility, and security considerations**: Auto-save drafts offline; form validation with clear error messages; secure local storage.

### AssignGlazeScreen
- **View path**: /works/{id}/assign-glaze
- **Main purpose**: Link existing glazes to a specific work for documentation.
- **Key information to display**: List of available glazes, search, multi-select checkboxes.
- **Key view components**: Searchable list, selection checkboxes, confirm button.
- **UX, accessibility, and security considerations**: Bulk selection for efficiency; focus management for screen readers; updates queued for sync.

### WikiScreen
- **View path**: /wiki
- **Main purpose**: Browse and search public knowledge base for materials.
- **Key information to display**: Search results with material details, verification badges, full entry views.
- **Key view components**: Search bar, results list, detail modals, filter by type (clay, glaze).
- **UX, accessibility, and security considerations**: Offline caching of popular entries; semantic search results; public read-only access with no user data exposure.

### ReportsScreen
- **View path**: /reports
- **Main purpose**: View monthly analytics like yield rate and productivity summaries.
- **Key information to display**: Charts (pie for yield, bar for categories), metrics (completed works, average time), export options.
- **Key view components**: Charts, date picker for periods, share button.
- **UX, accessibility, and security considerations**: Data visualizations with alt text; color-blind friendly palettes; aggregated user-only data.

### SettingsScreen
- **View path**: /settings
- **Main purpose**: Manage app preferences, language, and account settings.
- **Key information to display**: Language selector, theme toggle, sync settings, logout button.
- **Key view components**: List of toggles/switches, profile info, about section.
- **UX, accessibility, and security considerations**: Immediate language switch with confirmation; secure logout clears tokens; accessibility settings integration.

## 3. User Journey Map

1. **Onboarding/Auth**: User opens app → lands on AuthScreen → registers/logs in (US-001) → sets initials/profile → navigates to WorkListScreen.
2. **Daily Work Management**: From WorkListScreen, user views works with timers (US-004) → taps a work → WorkDetailScreen shows details and allows status update (US-005) → if needed, navigates to AssignGlazeScreen to link materials (US-006).
3. **Adding New Work**: From WorkListScreen, taps FAB → AddWorkScreen → captures photo, selects category/thickness (US-002) → code generates (US-003) → saves locally → returns to updated WorkListScreen with pending sync indicator (US-007, US-008).
4. **Inventory Management**: Bottom nav to GlazeListScreen → taps FAB → AddGlazeScreen → adds details → saves and syncs.
5. **Research**: Bottom nav to WikiScreen → searches materials (US-009) → views details.
6. **Review Progress**: Bottom nav to ReportsScreen → views monthly summary (US-011).
7. **Settings and Offline Handling**: Access SettingsScreen for preferences (US-013); throughout, offline mode shows local data with sync queue progress; on reconnect, background sync updates UI without disruption.
8. **Edge Cases**: Network error → shows offline banner; auth failure → retry or social options; empty states → guided prompts to add first work.

## 4. Layout and Navigation Structure

- **Primary Navigation**: Bottom navigation bar with icons/labels for Works (home), Glazes, Wiki, Reports. Settings accessible via top-right avatar menu or profile icon.
- **Secondary Navigation**: Stack-based for details (e.g., WorkListScreen → WorkDetailScreen); modals for quick actions like adding glazes; back button/gesture support.
- **Global Elements**: Top app bar with title, search icon where applicable, and sync status indicator; FAB on list screens for creation.
- **Offline Integration**: Persistent bottom banner for sync status; all navigation works offline, with graceful degradation (e.g., no real-time wiki updates).
- **Deep Linking**: Support for direct access to work details via code scan/share.

## 5. Key Components

- **WorkCard**: Reusable card for work lists showing photo, code, status badge, timer snippet; supports tap for details and long-press for actions.
- **TimerDisplay**: Countdown component with days/hours, color-coded by urgency; accessible via live announcements.
- **SyncIndicator**: Icon/badge showing pending/syncing/synced states across lists.
- **CategorySelector**: Dropdown or chips for work/glaze categories; searchable for wiki.
- **ImageViewer**: Zoomable image with compression preview; supports camera integration.
- **StatusStepper**: Horizontal stepper showing work lifecycle stages; tappable for updates.
- **SearchBar**: Unified search with filters; supports voice input for accessibility.
- **ErrorBanner**: Dismissible banner for offline/errors; with retry buttons.
- **LanguageSwitcher**: Dropdown in settings; triggers app recomposition for i18n.