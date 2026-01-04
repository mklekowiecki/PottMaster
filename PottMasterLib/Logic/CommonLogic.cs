using PottMasterLib.Models;

namespace PottMasterLib.Logic;

public static class CommonLogic
{
    /// <summary>
    /// Calculates the drying days based on wall thickness.
    /// Business rule: We approximate drying duration based on wall thickness so that works reach a safe "bone dry"
    /// state before firing. These thresholds are derived from studio practice and include a safety
    /// buffer to reduce the risk of cracking or explosions in the kiln:
    /// - ≤ 5 mm walls: 4 days (very thin pieces dry quickly).
    /// - ≤ 10 mm walls: 7 days (standard thickness, needs about a week).
    /// - ≤ 15 mm walls: 10 days (thicker pieces require extra time).
    /// - > 15 mm walls: 14 days (very thick / heavy pieces get the maximum drying time).
    /// If the studio's guidelines change, update this mapping accordingly.
    /// </summary>
    /// <param name="wallThickness">The wall thickness in mm</param>
    /// <returns>The number of drying days</returns>
    public static int CalculateDryingDays(int wallThickness)
    {
        return wallThickness switch
        {
            <= 5 => 4,
            <= 10 => 7,
            <= 15 => 10,
            _ => 14
        };
    }

    /// <summary>
    /// Determines whether the status with the specified identifier can be changed.
    /// </summary>
    /// <remarks>A status is considered changeable if its identifier is less than the value representing the
    /// 'Completed' status in the WorkStatusCode enumeration.</remarks>
    /// <param name="statusId">The identifier of the status to evaluate.</param>
    /// <returns>true if the status can be changed; otherwise, false.</returns>
    public static bool CanStatusBeChanged(int statusId)
    {
        return statusId < (int)WorkStatusCode.Completed; // 
    }

    /// <summary>
    /// Generates a unique work code based on user initials, category code, and existing works.
    /// Format: Initials-CatCode-MMYY-Counter (e.g., MK-CUP-1224-001)
    /// Counter resets monthly and is incremented based on existing works for the user in the same month/category.
    /// The counter is limited to 20 to prevent excessive works per month/category.
    /// </summary>
    /// <param name="userInitials">User's initials (e.g., "MK")</param>
    /// <param name="categoryCode">Category code (e.g., "CUP")</param>
    /// <param name="existingWorks">List of user's existing works to determine the counter</param>
    /// <returns>The generated work code</returns>
    /// <exception cref="InvalidOperationException">Thrown when the counter exceeds 20</exception>
    public static string GenerateWorkCode(string userInitials, string categoryCode, IEnumerable<LocalWork> existingWorks)
    {
        var monthYear = DateTime.UtcNow.ToString("MMyy");

        var filteredWorks = existingWorks
            .Where(w => w.Code.StartsWith($"{userInitials}-{categoryCode.Substring(0,3)}-{monthYear}-"))
            .ToList();

        var counter = filteredWorks.Count + 1;
        
        var result = $"{userInitials}-{categoryCode.Substring(0,3)}-{monthYear}-{counter:D3}";

        //Take last 20 characters
        if (result.Length > 20)
            result = result.Substring(result.Length - 20, 20);
          
        return result;
    }
}