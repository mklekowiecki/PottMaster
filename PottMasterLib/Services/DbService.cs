using SQLite;
using PottMasterLib.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace PottMasterLib.Services;

public class DbService : IDbService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;
    private readonly ILogger<DbService> _logger;

    public DbService(ILogger<DbService> logger)
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "pottmaster.db3");
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(_dbPath);

            await _database.CreateTableAsync<LocalWork>();
            await _database.CreateTableAsync<LocalUserProfile>();
            await _database.CreateTableAsync<LocalWorkCategory>();
            await _database.CreateTableAsync<LocalWorkStatus>();
            await _database.CreateTableAsync<LocalPhoto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed");
            throw;
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

    public async Task<LocalWorkCategory?> GetWorkCategoryByIdAsync(int categoryId)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.Table<LocalWorkCategory>()
            .Where(category => category.Id == categoryId)
            .FirstOrDefaultAsync();
    }

    public async Task<LocalWorkStatus?> GetWorkStatusByIdAsync(int statusId)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.Table<LocalWorkStatus>()
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

    public async Task<int> UpsertAllAsync<T>(IEnumerable<T> entities) where T : new()
    {
        if (_database == null)
            await InitializeAsync();

        var rowsAffected = 0;
        foreach (var entity in entities)
        {
            rowsAffected += await _database!.InsertOrReplaceAsync(entity);
        }

        return rowsAffected;
    }

    public async Task<List<LocalPhoto>> GetPhotosByWorkIdAsync(string workId)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.Table<LocalPhoto>()
            .Where(photo => photo.WorkId == workId)
            .OrderBy(photo => photo.Order)
            .ToListAsync();
    }

    public async Task<int> InsertPhotoAsync(LocalPhoto photo)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.InsertAsync(photo);
    }

    public async Task<int> UpdatePhotoAsync(LocalPhoto photo)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.UpdateAsync(photo);
    }

    public async Task<int> DeletePhotoAsync(LocalPhoto photo)
    {
        if (_database == null)
            await InitializeAsync();

        return await _database!.DeleteAsync(photo);
    }

    public async Task<int> DeletePhotosByWorkIdAsync(string workId)
    {
        if (_database == null)
            await InitializeAsync();

        var photos = await GetPhotosByWorkIdAsync(workId);
        var rowsAffected = 0;
        foreach (var photo in photos)
        {
            rowsAffected += await _database!.DeleteAsync(photo);
        }

        return rowsAffected;
    }
}
