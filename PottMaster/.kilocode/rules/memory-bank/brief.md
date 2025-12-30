I have updated the PRD document, simplifying the technology stack. Removing the .NET OpenAPI layer in favor of native integration with Supabase will allow for faster MVP delivery while maintaining full scalability and security.



# Product Requirements Document (PRD) - PottMaster



## 1. Product Overview



PottMaster is a mobile application supporting ceramic artists in managing technological processes. The system uses Kotlin for Android, enabling precise tracking of drying stages and cataloging works based on unique identification codes.



## 2. Technical Architecture



The technology stack has been optimized for Time-to-Market and reliability in offline conditions:



* Mobile: Kotlin for Android.

* Local database: SQLite (SQLDelight) – offline-first support.

* Backend as a Service (BaaS): Supabase.

* Cloud database: PostgreSQL with Row Level Security (RLS) mechanism.

* Authentication: Supabase Auth (Email, Google, Apple ID).

* File storage: Supabase Storage with client-side image compression policy.



## 3. User Problem



The main challenges addressed by the application:



* Lack of control over drying time (risk of cracking).

* Mixing up works in shared studios (visual identification is unreliable).

* Difficulty in maintaining reliable documentation of materials used (glazes, clays).

* Lack of data on studio efficiency (Yield Rate).



## 4. Functional Requirements



### 4.1. Identity Management



* Secure login and data synchronization between devices.

* Storage of user preferences and unique initials.

* Multi-language UI support for international users.



### 4.2. Work Lifecycle



* Object registration: photo, category, manual determination of wall thickness.

* Automatic generation of identification codes (initials + date + counter).

* Process timers: automatic calculation of drying and firing stages.



### 4.3. Knowledge Base and Inventory



* Personal glaze inventory with stock control.

* Public catalog (Wiki) with an expert verification system.



### 4.4. Data Analysis



* Monthly performance reports (Pottery Wrapped).



## 5. Product Boundaries



### 5.1. Within MVP Scope



* Full offline support (SQLite).

* Integration with Supabase (Auth, Database, Storage).

* Coding system and timers.

* Basic Wiki version.



### 5.2. Outside MVP Scope



* Proprietary .NET server infrastructure.

* Advanced AI algorithms for image analysis.

* Social modules and marketplace.



## 6. User Stories



### US-001: Supabase Authentication



* ID: US-001

* Title: User registration and login

* Description: As a user, I want to create an account so that my projects are securely synchronized with the Supabase cloud.

* Acceptance Criteria:



1. The user can register via email.

2. One-Tap Sign-in (Google/Apple) option is available.

3. After logging in, profile data (initials) are retrieved from PostgreSQL.



### US-002: New Work Registration



* ID: US-002

* Title: Adding a project to the local database

* Description: As a ceramicist, I want to add a new work so that the system can start counting down the drying time.

* Acceptance Criteria:



1. The user takes a photo, which is compressed and saved in SQLite.

2. The user selects a category and sets the wall thickness slider.

3. The system saves a record with a pending synchronization status.



### US-003: Unique Identification Code



* ID: US-003

* Title: Generating work designation

* Description: As a user, I want to receive a short code to physically apply it to wet clay.

* Acceptance Criteria:



1. The code is generated locally (offline) according to the mask: Initials-CatCode-MMYY-Counter.

2. The code is visible immediately after saving the record in SQLite.



### US-004: Process Timer Management



* ID: US-004

* Title: Tracking drying progress

* Description: As a user, I want to see how much time is left until safe object processing.

* Acceptance Criteria:



1. The application displays a real-time counter (Countdown).

2. The algorithm adjusts the time based on the selected wall thickness (e.g., 5mm = 4 days, 10mm = 7 days).



### US-005: Manual Status Correction



* ID: US-005

* Title: Accelerating the work stage

* Description: As a user, I want to manually mark a stage as ready to move to the next firing phase.

* Acceptance Criteria:



1. The user can click the "Ready for Firing" button ahead of time.

2. The system stops the timer and updates the status in the local database.



### US-006: Glaze Inventory



* ID: US-006

* Title: Cataloging owned materials

* Description: As a user, I want to maintain a list of my glazes to know what I have in the workshop.

* Acceptance Criteria:



1. Ability to add name, manufacturer, and quantity description.

2. Ability to link glaze to a specific registered work.



### US-007: Offline Operation



* ID: US-007

* Title: Data handling without Internet

* Description: As a user working in the basement, I want to have access to all application functions without reception.

* Acceptance Criteria:



1. All queries are directed to SQLite via SQLDelight.

2. The application does not block the UI due to lack of network.



### US-008: Automatic Synchronization



* ID: US-008

* Title: Sending data to Supabase

* Description: As a user, I want my data to go to the cloud without my intervention after leaving the studio.

* Acceptance Criteria:



1. The application sends new records from SQLite to PostgreSQL when a network is detected.

2. Photos are uploaded to Supabase Storage.

3. The "synchronized" flag is set to true after a successful operation.



### US-009: Wiki Browsing



* ID: US-009

* Title: Searching for clay information

* Description: As a user, I want to check clay parameters in the public database.

* Acceptance Criteria:



1. Full-text search in the public Supabase table.

2. Displaying trust tags for each entry.



### US-010: Expert Verification



* ID: US-010

* Title: Assigning credibility status to entries

* Description: As an administrator/expert, I want to approve user entries so that the Wiki database is reliable.

* Acceptance Criteria:



1. The expert sees a panel with unverified entries.

2. Ability to change the entry status to verified with one click.



### US-011: Pottery Wrapped



* ID: US-011

* Title: Generating productivity report

* Description: As a user, I want to see my successes visually at the end of the month.

* Acceptance Criteria:



1. Displaying the number of successfully completed works.

2. Visualization of the Yield Rate on a pie chart.



### US-012: Data Security (RLS)



* ID: US-012

* Title: User data isolation

* Description: As a user, I want to be sure that no one else sees my private projects in the cloud.

* Acceptance Criteria:



1. Queries to PostgreSQL are filtered by Row Level Security based on auth.uid().

2. It is not possible to read another user's record even if the work ID is known.



## 7. Success Metrics



* Time-to-Market: Time from start of work to release of version 1.0 (goal: under 3 months).

* Synchronization: Percentage of data successfully transferred from offline mode (goal: 99.9%).

* User Growth: Number of active Supabase accounts in the first month (goal: 500).

* Yield Rate: Average studio efficiency of users measured in the application (goal: increase by 5% after 3 months of system use).