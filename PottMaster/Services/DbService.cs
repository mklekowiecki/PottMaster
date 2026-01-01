using SQLite;
using PottMaster.Models;

namespace PottMaster.Services;

public class DbService : IDbService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DbService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "pottmaster.db3");
    }

    public async Task InitializeAsync()
    {
        if (_database != null)
            return;

        _database = new SQLiteAsyncConnection(_dbPath);

        await _database.CreateTableAsync<LocalWork>();
        await _database.CreateTableAsync<LocalUserProfile>();
        await _database.CreateTableAsync<WorkCategory>();
        await _database.CreateTableAsync<WorkStatus>();

        await SeedDataAsync();
    }

    private async Task SeedDataAsync()
    {
        var categoriesCount = await _database!.Table<WorkCategory>().CountAsync();
        if (categoriesCount == 0)
        {
            var categories = new[]
            {
                new WorkCategory { Id = 1, Name = "Cup", Code = "CUP" },
                new WorkCategory { Id = 2, Name = "Bowl", Code = "BOWL" },
                new WorkCategory { Id = 3, Name = "Vase", Code = "VASE" },
                new WorkCategory { Id = 4, Name = "Plate", Code = "PLATE" },
                new WorkCategory { Id = 5, Name = "Sculpture", Code = "SCULPTURE" },
                new WorkCategory { Id = 6, Name = "Tile", Code = "TILE" },
                new WorkCategory { Id = 7, Name = "Other", Code = "OTHER" }
            };
            await _database.InsertAllAsync(categories);
        }

        var statusesCount = await _database.Table<WorkStatus>().CountAsync();
        if (statusesCount == 0)
        {
            var statuses = new[]
            {
                new WorkStatus { Id = 1, Code = "WET", Name = "Wet" },
                new WorkStatus { Id = 2, Code = "LEATHER_HARD", Name = "Leather Hard" },
                new WorkStatus { Id = 3, Code = "BONE_DRY", Name = "Bone Dry" },
                new WorkStatus { Id = 4, Code = "BISQUE_FIRED", Name = "Bisque Fired" },
                new WorkStatus { Id = 5, Code = "GLAZED", Name = "Glazed" },
                new WorkStatus { Id = 6, Code = "GLAZE_FIRED", Name = "Glaze Fired" },
                new WorkStatus { Id = 7, Code = "COMPLETED", Name = "Completed" },
                new WorkStatus { Id = 8, Code = "DISCARDED", Name = "Discarded" }
            };
            await _database.InsertAllAsync(statuses);
        }
    }

    public async Task<List<T>> GetAllAsync<T>() where T : new()
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.Table<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync<T>(string id) where T : new()
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.FindAsync<T>(id);
    }

    public async Task<int> InsertAsync<T>(T entity)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.InsertAsync(entity);
    }

    public async Task<int> UpdateAsync<T>(T entity)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.UpdateAsync(entity);
    }

    public async Task<int> DeleteAsync<T>(T entity)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.DeleteAsync(entity);
    }

    public async Task<List<LocalWork>> GetWorksByUserIdAsync(string userId)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.Table<LocalWork>()
            .Where(work => work.UserId == userId)
            .OrderByDescending(work => work.CreatedAt)
            .ToListAsync();
    }

    public async Task<WorkCategory?> GetWorkCategoryByIdAsync(int categoryId)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.Table<WorkCategory>()
            .Where(category => category.Id == categoryId)
            .FirstOrDefaultAsync();
    }

    public async Task<WorkStatus?> GetWorkStatusByIdAsync(int statusId)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.Table<WorkStatus>()
            .Where(status => status.Id == statusId)
            .FirstOrDefaultAsync();
    }

    public async Task<LocalUserProfile?> GetUserProfileByIdAsync(string userId)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.FindAsync<LocalUserProfile>(userId);
    }

    public async Task<int> UpsertUserProfileAsync(LocalUserProfile profile)
    {
        if (_database == null)
            await InitializeAsync();

        var existing = await GetUserProfileByIdAsync(profile.Id);
        if (existing != null)
        {
            return await _database!.UpdateAsync(profile);
        }
        else
        {
            return await _database!.InsertAsync(profile);
        }
    }
}
