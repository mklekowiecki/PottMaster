# .NET MAUI Development Guidelines - PottMaster

## Project Structure

```
PottMaster/
├── Models/          # Domain models
├── ViewModels/      # MVVM ViewModels
├── Views/           # XAML pages and layouts
├── Services/        # Business services and repositories
├── Platforms/       # Platform-specific code
├── Resources/       # Images, fonts, styles
├── Converters/      # Value converters for XAML
└── Helpers/         # Utility classes
```

## Naming Conventions

### C# Files
- Classes: PascalCase (e.g., WorkViewModel.cs)
- Interfaces: IPascalCase (e.g., IWorkRepository.cs)
- Methods: PascalCase (e.g., CreateWorkAsync())
- Properties: PascalCase (e.g., WorksList)
- Variables: camelCase (e.g., workList)
- Constants: UPPER_SNAKE_CASE (e.g., MAX_WALL_THICKNESS)

### XAML Files
- Pages: PascalCase.xaml (e.g., WorkListPage.xaml)
- Controls: PascalCase.xaml (e.g., WorkCardControl.xaml)

## MVVM Pattern

Use CommunityToolkit.Mvvm for ObservableObject, RelayCommand, etc.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class WorkListViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Work> works = new();

    [RelayCommand]
    private async Task LoadWorksAsync()
    {
        var result = await _workService.GetWorksAsync();
        Works = new ObservableCollection<Work>(result);
    }

    [RelayCommand]
    private async Task AddWorkAsync()
    {
        // Implementation
    }
}
```

### XAML Binding
```xml
<!-- WorkListPage.xaml -->
<ContentPage x:Class="PottMaster.Views.WorkListPage">
    <CollectionView ItemsSource="{Binding Works}">
        <CollectionView.ItemTemplate>
            <DataTemplate>
                <views:WorkCardControl Work="{Binding .}" />
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>
</ContentPage>
```

## Dependency Injection

Use Microsoft.Extensions.DependencyInjection in MauiProgram.cs.

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Services
        builder.Services.AddSingleton<IWorkRepository, WorkRepository>();
        builder.Services.AddSingleton<IWorkService, WorkService>();
        builder.Services.AddTransient<WorkListViewModel>();

        return builder.Build();
    }
}
```

## Offline-First with SQLite

Use SQLite-net-pcl for database operations.

```csharp
using SQLite;

public interface IWorkRepository
{
    Task<List<Work>> GetWorksAsync();
    Task<Work> CreateWorkAsync(Work work);
    Task SyncPendingWorksAsync();
}

public class WorkRepository : IWorkRepository
{
    private readonly SQLiteAsyncConnection _database;
    private readonly ISupabaseClient _supabase;

    public WorkRepository(IServiceProvider serviceProvider)
    {
        _database = serviceProvider.GetRequiredService<SQLiteAsyncConnection>();
        _supabase = serviceProvider.GetRequiredService<ISupabaseClient>();
    }

    public async Task<List<Work>> GetWorksAsync()
    {
        return await _database.Table<Work>().ToListAsync();
    }

    public async Task<Work> CreateWorkAsync(Work work)
    {
        work.SyncStatus = SyncStatus.Pending;
        await _database.InsertAsync(work);
        return work;
    }
}
```

## Supabase Integration

Use Supabase-CSharp client.

```csharp
// Initialize in DI
builder.Services.AddSingleton(new Supabase.Client(SupabaseUrl, SupabaseAnonKey));

// Usage in service
public class WorkService
{
    private readonly Client _supabase;

    public async Task SyncWorkAsync(Work work)
    {
        var response = await _supabase
            .From("works")
            .Insert(work)
            .Execute();

        if (response.IsSuccess)
        {
            work.SyncStatus = SyncStatus.Synced;
        }
    }
}
```

## Code Style

- Follow C# coding conventions (see [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions))
- Use async/await for all I/O operations
- Use #region for organizing large classes
- Avoid blocking calls on UI thread
- Use INotifyPropertyChanged via ObservableObject

## Testing

### Unit Tests with xUnit
```csharp
public class WorkServiceTests
{
    [Fact]
    public async Task CreateWork_SavesToLocalDatabase()
    {
        // Arrange
        var mockRepo = new Mock<IWorkRepository>();
        var service = new WorkService(mockRepo.Object);

        // Act
        await service.CreateWorkAsync(new Work());

        // Assert
        mockRepo.Verify(r => r.CreateWorkAsync(It.IsAny<Work>()), Times.Once);
    }
}
```

### UI Tests
Use .NET MAUI UI Tests for platform-specific testing.

## Performance Best Practices

- Use compiled bindings in XAML where possible
- Lazy load images with ImageSource.FromFile
- Use MessagingCenter or StrongReferenceMessenger for loose coupling
- Implement virtualization for long lists (CollectionView)
- Compress images before storage/upload

## Security

- Store API keys in secure storage (Preferences or SecureStorage)
- Use HTTPS for all network calls
- Validate user input
- Implement proper error handling without exposing sensitive info

## References

- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- [CommunityToolkit.Mvvm](https://docs.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [Supabase-CSharp](https://github.com/supabase-community/supabase-csharp)
- [SQLite-net-pcl](https://github.com/praeclarum/sqlite-net)

---
**Last Updated**: 2025-12-30
**Document Owner**: Development Team