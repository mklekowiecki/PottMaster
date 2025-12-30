# Development Guidelines - PottMaster

## Code Organization

### Project Structure

```
app/src/main/java/com/app/pottmaster/
├── data/
│   ├── local/          # SQLite, SQLDelight
│   ├── remote/         # Supabase API clients
│   ├── repository/     # Repository implementations
│   └── model/          # Data models, DTOs
├── domain/
│   ├── model/          # Domain models
│   ├── repository/     # Repository interfaces
│   └── usecase/        # Business logic use cases
├── presentation/
│   ├── screens/        # Screen composables
│   ├── components/     # Reusable UI components
│   ├── viewmodel/      # ViewModels
│   └── navigation/     # Navigation setup
└── di/                 # Dependency injection modules
```

## Naming Conventions

### Kotlin Files

```kotlin
// Use Cases: Verb + Noun + UseCase
CreateWorkUseCase.kt
SyncPendingWorksUseCase.kt
CalculateDryingTimeUseCase.kt

// Repositories: Noun + Repository
WorkRepository.kt
GlazeRepository.kt
UserRepository.kt

// ViewModels: Screen + ViewModel
WorkListViewModel.kt
WorkDetailViewModel.kt
GlazeInventoryViewModel.kt

// Composables: Descriptive noun/verb
WorkListScreen.kt
WorkCard.kt
DryingTimerDisplay.kt
```

### Database Tables

```sql
-- Lowercase with underscores
works
glazes
work_glazes
user_profiles
sync_queue
```

### Variables and Functions

```kotlin
// camelCase for variables and functions
val workList: List<Work>
val dryingTimeInDays: Int
fun calculateDryingTime(wallThickness: Int): Duration

// PascalCase for classes and objects
class WorkRepository
object NetworkConfig
data class Work

// SCREAMING_SNAKE_CASE for constants
const val MAX_WALL_THICKNESS = 50
const val DEFAULT_DRYING_DAYS = 7
```

## Dependency Injection with Koin

```kotlin
// di/AppModule.kt
val appModule = module {
    // Repositories
    single<WorkRepository> { WorkRepositoryImpl(get(), get()) }
    single<GlazeRepository> { GlazeRepositoryImpl(get(), get()) }
    
    // Use Cases
    factory { CreateWorkUseCase(get()) }
    factory { SyncPendingWorksUseCase(get()) }
    
    // ViewModels
    viewModel { WorkListViewModel(get(), get()) }
}

// Platform-specific modules
val androidModule = module {
    single<ImageCompressor> { AndroidImageCompressor() }
}
```

## Offline-First Principles

### 1. Always Write Locally First

```kotlin
// ✅ Correct pattern
suspend fun createWork(work: Work): Result<Work> {
    // 1. Save to local database immediately
    val localWork = work.copy(syncStatus = SyncStatus.PENDING)
    localDataSource.insert(localWork)
    
    // 2. Queue for background sync
    syncManager.queueForSync(localWork.id)
    
    // 3. Return immediately (don't wait for network)
    return Result.success(localWork)
}

// ❌ Wrong: Don't wait for network on write
suspend fun createWork(work: Work): Result<Work> {
    return try {
        remoteDataSource.insert(work) // Blocks on network!
        localDataSource.insert(work)
        Result.success(work)
    } catch (e: Exception) {
        Result.failure(e)
    }
}
```

### 2. Read from Local Database

```kotlin
// ✅ Always read from local
fun getWorks(): Flow<List<Work>> {
    return localDataSource.getWorks()
        .map { works -> works.sortedByDescending { it.createdAt } }
}

// Background sync updates local database
suspend fun syncWorks() {
    if (networkMonitor.isOnline()) {
        val remoteWorks = remoteDataSource.getWorks()
        localDataSource.upsertAll(remoteWorks)
    }
}
```

### 3. Handle Sync States

```kotlin
// Show sync status in UI
@Composable
fun WorkCard(work: Work) {
    Card {
        // ... work content
        
        when (work.syncStatus) {
            SyncStatus.PENDING -> {
                Icon(Icons.Default.CloudOff, "Not synced")
            }
            SyncStatus.SYNCING -> {
                CircularProgressIndicator()
            }
            SyncStatus.SYNCED -> {
                Icon(Icons.Default.CloudDone, "Synced")
            }
            SyncStatus.ERROR -> {
                Icon(Icons.Default.Error, "Sync failed")
            }
        }
    }
}
```

## Code Style

### Composable Functions

```kotlin
// ✅ Good: Clear, single responsibility
@Composable
fun WorkListScreen(
    viewModel: WorkListViewModel = koinViewModel()
) {
    val uiState by viewModel.uiState.collectAsState()
    
    Scaffold(
        topBar = { WorkListTopBar() },
        floatingActionButton = { AddWorkFab() }
    ) { padding ->
        WorkListContent(
            works = uiState.works,
            onWorkClick = viewModel::onWorkClick,
            modifier = Modifier.padding(padding)
        )
    }
}

@Composable
private fun WorkListContent(
    works: List<Work>,
    onWorkClick: (Work) -> Unit,
    modifier: Modifier = Modifier
) {
    LazyColumn(modifier = modifier) {
        items(works) { work ->
            WorkCard(
                work = work,
                onClick = { onWorkClick(work) }
            )
        }
    }
}
```

### ViewModel Pattern

```kotlin
class WorkListViewModel(
    private val getWorksUseCase: GetWorksUseCase,
    private val syncWorksUseCase: SyncWorksUseCase
) : ViewModel() {
    
    private val _uiState = MutableStateFlow(WorkListUiState())
    val uiState: StateFlow<WorkListUiState> = _uiState.asStateFlow()
    
    init {
        loadWorks()
    }
    
    private fun loadWorks() {
        viewModelScope.launch {
            getWorksUseCase()
                .catch { error ->
                    _uiState.update { it.copy(error = error.message) }
                }
                .collect { works ->
                    _uiState.update { it.copy(works = works, isLoading = false) }
                }
        }
    }
    
    fun onRefresh() {
        viewModelScope.launch {
            syncWorksUseCase()
        }
    }
}

data class WorkListUiState(
    val works: List<Work> = emptyList(),
    val isLoading: Boolean = true,
    val error: String? = null
)
```

### Error Handling

```kotlin
// Use Result type for operations that can fail
sealed class Result<out T> {
    data class Success<T>(val data: T) : Result<T>()
    data class Error(val exception: Exception) : Result<Nothing>()
}

// Use in repositories
suspend fun createWork(work: Work): Result<Work> {
    return try {
        localDataSource.insert(work)
        Result.Success(work)
    } catch (e: Exception) {
        Result.Error(e)
    }
}

// Handle in ViewModels
fun createWork(work: Work) {
    viewModelScope.launch {
        when (val result = repository.createWork(work)) {
            is Result.Success -> {
                _uiState.update { it.copy(success = true) }
            }
            is Result.Error -> {
                _uiState.update { it.copy(error = result.exception.message) }
            }
        }
    }
}
```

## Testing Strategy

### Unit Tests

```kotlin
// Test use cases
class CalculateDryingTimeUseCaseTest {
    private lateinit var useCase: CalculateDryingTimeUseCase
    
    @BeforeTest
    fun setup() {
        useCase = CalculateDryingTimeUseCase()
    }
    
    @Test
    fun `thin wall returns 4 days`() {
        val result = useCase(wallThickness = 5)
        assertEquals(Duration.days(4), result)
    }
    
    @Test
    fun `thick wall returns 14 days`() {
        val result = useCase(wallThickness = 20)
        assertEquals(Duration.days(14), result)
    }
}
```

### Repository Tests

```kotlin
class WorkRepositoryTest {
    private lateinit var repository: WorkRepository
    private lateinit var localDataSource: FakeWorkLocalDataSource
    private lateinit var remoteDataSource: FakeWorkRemoteDataSource
    
    @BeforeTest
    fun setup() {
        localDataSource = FakeWorkLocalDataSource()
        remoteDataSource = FakeWorkRemoteDataSource()
        repository = WorkRepositoryImpl(localDataSource, remoteDataSource)
    }
    
    @Test
    fun `createWork saves to local database`() = runTest {
        val work = createTestWork()
        
        repository.createWork(work)
        
        val saved = localDataSource.getById(work.id)
        assertEquals(work.id, saved?.id)
        assertEquals(SyncStatus.PENDING, saved?.syncStatus)
    }
}
```

### UI Tests (Compose)

```kotlin
class WorkListScreenTest {
    @Test
    fun `displays list of works`() = runComposeUiTest {
        val testWorks = listOf(
            createTestWork(code = "MK-CUP-1224-001"),
            createTestWork(code = "MK-BOWL-1224-002")
        )
        
        setContent {
            WorkListScreen(
                viewModel = FakeWorkListViewModel(works = testWorks)
            )
        }
        
        onNodeWithText("MK-CUP-1224-001").assertIsDisplayed()
        onNodeWithText("MK-BOWL-1224-002").assertIsDisplayed()
    }
}
```

## Performance Guidelines

### 1. Image Handling

```kotlin
// Compress before saving
fun saveWorkPhoto(bitmap: Bitmap): String {
    val compressed = compressImage(
        bitmap = bitmap,
        maxWidth = 1920,
        maxHeight = 1920,
        quality = 80
    )
    return saveToLocalStorage(compressed)
}

// Use Coil for efficient loading
@Composable
fun WorkImage(photoPath: String) {
    AsyncImage(
        model = photoPath,
        contentDescription = "Work photo",
        modifier = Modifier.size(200.dp),
        contentScale = ContentScale.Crop
    )
}
```

### 2. Database Queries

```kotlin
// ✅ Use indexes for frequent queries
CREATE INDEX idx_works_user_status ON works(user_id, status);

// ✅ Limit results when appropriate
SELECT * FROM works 
WHERE user_id = ? 
ORDER BY created_at DESC 
LIMIT 50;

// ✅ Use Flow for reactive queries
fun getWorks(): Flow<List<Work>> = 
    database.workQueries
        .selectAll()
        .asFlow()
        .mapToList()
```

### 3. Lazy Loading

```kotlin
// Use LazyColumn for lists
@Composable
fun WorkList(works: List<Work>) {
    LazyColumn {
        items(
            items = works,
            key = { it.id }  // Important for performance
        ) { work ->
            WorkCard(work)
        }
    }
}
```

## Security Best Practices

### 1. Never Store Sensitive Data in Plain Text

```kotlin
// ✅ Use platform-specific secure storage
// Android: Use EncryptedSharedPreferences
```

### 2. Validate User Input

```kotlin
fun validateWorkCode(code: String): Boolean {
    val pattern = Regex("^[A-Z]{2}-[A-Z]+-\\d{4}-\\d{3}$")
    return code.matches(pattern)
}

fun sanitizeUserInput(input: String): String {
    return input.trim().take(MAX_INPUT_LENGTH)
}
```

### 3. Use RLS Policies

```sql
-- Ensure users can only access their own data
CREATE POLICY "Users access own works"
ON works FOR ALL
USING (auth.uid() = user_id);
```

## Git Workflow

### Branch Naming

```
feature/work-creation
feature/glaze-inventory
bugfix/sync-crash
hotfix/auth-token-refresh
```

### Commit Messages

```
feat: Add work creation screen
fix: Resolve sync conflict on network reconnect
refactor: Extract drying time calculation to use case
docs: Update architecture documentation
test: Add unit tests for WorkRepository
```

### Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] New feature
- [ ] Bug fix
- [ ] Refactoring
- [ ] Documentation

## Testing
- [ ] Unit tests added/updated
- [ ] Manual testing completed
- [ ] Tested on Android

## Screenshots (if applicable)
```

## Documentation Standards

### Code Comments

```kotlin
/**
 * Calculates the recommended drying time based on wall thickness.
 * 
 * The calculation follows ceramic best practices:
 * - Thin walls (≤5mm): 4 days
 * - Medium walls (6-10mm): 7 days
 * - Thick walls (11-15mm): 10 days
 * - Very thick walls (>15mm): 14 days
 * 
 * @param wallThickness Wall thickness in millimeters
 * @return Recommended drying duration
 */
fun calculateDryingTime(wallThickness: Int): Duration {
    // Implementation
}
```

### README Files

Each major module should have a README:

```markdown
# Work Management Module

## Purpose
Handles creation, tracking, and lifecycle management of ceramic works.

## Key Components
- `WorkRepository`: Data access layer
- `CreateWorkUseCase`: Business logic for work creation
- `WorkListViewModel`: UI state management

## Usage Example
[Code example]
```

## Internationalization Guidelines

### String Resources

```xml
<!-- composeApp/src/main/res/values/strings.xml -->
<resources>
    <string name="app_name">PottMaster</string>
    <string name="work_list_title">Moje prace</string>
    <string name="add_work_button">Dodaj pracę</string>
    <string name="drying_time_format">Pozostało %d dni</string>
</resources>

<!-- composeApp/src/main/res/values-en/strings.xml -->
<resources>
    <string name="app_name">PottMaster</string>
    <string name="work_list_title">My Works</string>
    <string name="add_work_button">Add Work</string>
    <string name="drying_time_format">%d days remaining</string>
</resources>
```

### Using Localized Strings

```kotlin
// ✅ Good: Use string resources
@Composable
fun WorkListScreen() {
    val title = stringResource(Res.string.work_list_title)
    
    Scaffold(
        topBar = {
            TopAppBar(title = { Text(title) })
        }
    ) { /* content */ }
}

// ❌ Bad: Hardcoded strings
@Composable
fun WorkListScreen() {
    Scaffold(
        topBar = {
            TopAppBar(title = { Text("My Works") })  // Don't do this!
        }
    ) { /* content */ }
}
```

### String Formatting

```kotlin
// With placeholders
val daysRemaining = 5
val message = stringResource(
    Res.string.drying_time_format,
    daysRemaining
)

// Plurals
val worksCount = 3
val message = pluralStringResource(
    Res.plurals.works_count,
    worksCount,
    worksCount
)
```

### Language Selection

```kotlin
// Store user preference
class UserPreferencesRepository {
    suspend fun setLanguage(languageCode: String) {
        dataStore.edit { preferences ->
            preferences[LANGUAGE_KEY] = languageCode
        }
    }
    
    fun getLanguage(): Flow<String> {
        return dataStore.data.map { it[LANGUAGE_KEY] ?: "pl" }
    }
}

// Apply language in UI
@Composable
fun App() {
    val language by userPreferences.getLanguage().collectAsState("pl")
    
    CompositionLocalProvider(
        LocalLanguage provides language
    ) {
        // App content
    }
}
```

### Localization Checklist

- [ ] All user-facing text uses string resources
- [ ] String resources exist for all supported languages
- [ ] Placeholders used for dynamic content
- [ ] Plurals handled correctly
- [ ] Date/time formatted according to locale
- [ ] Numbers formatted according to locale
- [ ] No hardcoded strings in code
- [ ] Fallback to default language works

## References

- [Kotlin Coding Conventions](https://kotlinlang.org/docs/coding-conventions.html)
- [Compose Best Practices](https://developer.android.com/jetpack/compose/performance)
- [Compose Resources Documentation](https://www.jetbrains.com/help/kotlin-multiplatform-dev/compose-images-resources.html)

---

**Last Updated**: 2025-12-30
**Document Owner**: Development Team
**Review Cycle**: Monthly or when standards change
