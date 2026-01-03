using Microsoft.Extensions.Logging;
using PottMasterLib.Models;
using Supabase.Postgrest;

namespace PottMasterLib.Services;

public class SupabaseApi : IApiEndpoint
{
    private readonly Supabase.Client _supabaseClient;
    private readonly ILogger<SupabaseApi> _logger;

    public SupabaseApi(Supabase.Client supabaseClient, ILogger<SupabaseApi> logger)
    {
        _supabaseClient = supabaseClient;
        _logger = logger;
    }

    public async Task<Result<List<IWork>>> GetWorksByUserIdAsync(string userId)
    {
        try
        {
            var response = await _supabaseClient
                .From<RemoteWork>()
                .Filter("user_id", Constants.Operator.Equals, userId)
                .Get();

            var works = response.Models.Cast<IWork>().ToList();
            return Result<List<IWork>>.Success(works);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get works for user {UserId}", userId);
            return Result<List<IWork>>.Failure("Failed to load works from server", ex);
        }
    }

    public async Task<Result<IWork>> GetWorkByIdAsync(string workId)
    {
        try
        {
            var response = await _supabaseClient
                .From<RemoteWork>()
                .Filter("id", Constants.Operator.Equals, workId)
                .Single();

            if (response == null)
            {
                return Result<IWork>.Failure("Work not found");
            }

            return Result<IWork>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work {WorkId}", workId);
            return Result<IWork>.Failure("Failed to load work from server", ex);
        }
    }

    public async Task<Result<IWork>> UpsertWorkAsync(IWork work)
    {
        try
        {
            var remoteWork = new RemoteWork
            {
                Id = work.Id,
                UserId = work.UserId,
                Code = work.Code,
                CategoryId = work.CategoryId,
                WallThickness = work.WallThickness,
                PhotoPath = work.PhotoPath,
                StatusId = work.StatusId,
                CreatedAt = work.CreatedAt,
                DryingStartedAt = work.DryingStartedAt,
                DryingCompletedAt = work.DryingCompletedAt,
                SyncStatus = work.SyncStatus,
                UpdatedAt = work.UpdatedAt
            };

            var response = await _supabaseClient
                .From<RemoteWork>()
                .Upsert(remoteWork);

            var upserted = response.Models.FirstOrDefault();
            if (upserted == null)
            {
                return Result<IWork>.Failure("Failed to upsert work");
            }

            return Result<IWork>.Success(upserted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert work {WorkId}", work.Id);
            return Result<IWork>.Failure("Failed to save work to server", ex);
        }
    }

    public async Task<Result> DeleteWorkAsync(string workId)
    {
        try
        {
            await _supabaseClient
                .From<RemoteWork>()
                .Filter("id", Constants.Operator.Equals, workId)
                .Delete();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete work {WorkId}", workId);
            return Result.Failure("Failed to delete work from server", ex);
        }
    }

    public async Task<Result<List<IWorkCategory>>> GetWorkCategoriesAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<WorkCategory>()
                .Get();

            var categories = response.Models.Cast<IWorkCategory>().ToList();
            return Result<List<IWorkCategory>>.Success(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work categories");
            return Result<List<IWorkCategory>>.Failure("Failed to load categories from server", ex);
        }
    }

    public async Task<Result<List<IWorkStatus>>> GetWorkStatusesAsync()
    {
        try
        {
            var response = await _supabaseClient
                .From<WorkStatus>()
                .Get();

            var statuses = response.Models.Cast<IWorkStatus>().ToList();
            return Result<List<IWorkStatus>>.Success(statuses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work statuses");
            return Result<List<IWorkStatus>>.Failure("Failed to load statuses from server", ex);
        }
    }

    public async Task<Result<IUserProfile>> GetUserProfileByIdAsync(string userId)
    {
        try
        {
            var response = await _supabaseClient
                .From<UserProfiles>()
                .Filter("id", Constants.Operator.Equals, userId)
                .Single();

            if (response == null)
            {
                return Result<IUserProfile>.Failure("User profile not found");
            }

            return Result<IUserProfile>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user profile {UserId}", userId);
            return Result<IUserProfile>.Failure("Failed to load user profile from server", ex);
        }
    }

    public async Task<Result<IUserProfile>> UpsertUserProfileAsync(IUserProfile profile)
    {
        try
        {
            var remoteProfile = new UserProfiles
            {
                Id = profile.Id,
                Initials = profile.Initials,
                Email = profile.Email,
                CreatedAt = profile.CreatedAt
            };

            var response = await _supabaseClient
                .From<UserProfiles>()
                .Upsert(remoteProfile);

            var upserted = response.Models.FirstOrDefault();
            if (upserted == null)
            {
                return Result<IUserProfile>.Failure("Failed to upsert user profile");
            }

            return Result<IUserProfile>.Success(upserted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert user profile {UserId}", profile.Id);
            return Result<IUserProfile>.Failure("Failed to save user profile to server", ex);
        }
    }
}
