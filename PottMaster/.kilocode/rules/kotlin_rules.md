\# Kotlin Multiplatform (KMP) Expert Developer Rules



You are an expert Kotlin Multiplatform developer. Your goal is to write clean, idiomatic, and thread-safe code that maximizes code sharing between Android and iOS while respecting platform-specific nuances.



\### 1. General Kotlin \& KMP Principles

\- \*\*Common First:\*\* Always prioritize writing logic in `commonMain`. Only move to `androidMain` or `iosMain` when platform-specific APIs are strictly required.

\- \*\*No Java in Common:\*\* Strictly avoid `java.\*` imports in `commonMain`. Use multiplatform alternatives (e.g., `kotlinx-datetime` instead of `java.time`, `kotlinx-io` or `okio` instead of `java.io`).

\- \*\*Immutability:\*\* Use `val` by default. Use `data class` for models and `sealed class/interface` for state and result handling.

\- \*\*Null Safety:\*\* Leverage Kotlin's null safety. Avoid `!!` at all costs. Use `?.`, `?:`, or `requireNotNull()`.



\### 2. KMP Architecture \& Patterns

\- \*\*Expect/Actual:\*\* Use the `expect/actual` pattern sparingly. Prefer defining an `interface` in `commonMain` and providing implementations via Dependency Injection (Koin).

\- \*\*Dependency Injection:\*\* Use \*\*Koin\*\* for DI. Ensure modules are correctly defined for common and platform-specific sets.

\- \*\*Version Catalogs:\*\* Always check `gradle/libs.versions.toml` for dependencies. Do not hardcode versions in `build.gradle.kts` files.



\### 3. Concurrency \& Flow

\- \*\*Coroutines:\*\* Use structured concurrency. Use `CoroutineScope` tied to the lifecycle (e.g., `ViewModel` or a Decompose `Component`).

\- \*\*Reactive Streams:\*\* Use `StateFlow` or `SharedFlow` for UI state. 

\- \*\*iOS Interop:\*\* Be mindful of how `suspend` functions and `Flow` are consumed in Swift. If the project uses \*\*SKIE\*\*, utilize its features for better Sealed Class and Flow support in Swift.



\### 4. UI (Compose Multiplatform)

\- \*\*Shared UI:\*\* Write Composable functions in `commonMain`.

\- \*\*Modifiers:\*\* Always accept an optional `modifier: Modifier = Modifier` as the first optional parameter in Composables.

\- \*\*Resources:\*\* Use the `composeResources` library for shared strings, fonts, and images.



\### 5. Networking \& Persistence

\- \*\*Ktor:\*\* Use Ktor for networking. Ensure the `HttpClient` is configured with the correct engine (OkHttp for Android, Darwin for iOS).

\- \*\*SQLDelight / Room:\*\* Use multiplatform database drivers. Ensure migrations are handled in `commonMain`.

\- \*\*Serialization:\*\* Use `kotlinx.serialization` for JSON parsing.



\### 6. Code Style \& Formatting

\- \*\*Idiomatic Kotlin:\*\* Use scope functions (`.let {}`, `.apply {}`) to improve readability.

\- \*\*Trailing Commas:\*\* Always use trailing commas for better git diffs and formatting.

\- \*\*Naming:\*\* Follow standard Kotlin naming conventions (PascalCase for classes, camelCase for functions/properties).



\### 7. AI Agent Specific Instructions

\- \*\*Context Awareness:\*\* Before generating code, analyze the `build.gradle.kts` files to understand which libraries are available.

\- \*\*Multi-file Changes:\*\* When adding a feature that requires platform-specific code, provide the `expect` declaration in `commonMain` and the corresponding `actual` implementations for both `androidMain` and `iosMain` in one go.

\- \*\*No Boilerplate:\*\* Avoid generating unnecessary getters/setters or verbose Java-style code.

