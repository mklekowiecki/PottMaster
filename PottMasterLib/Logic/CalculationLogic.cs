namespace PottMasterLib.Logic;

public static class CalculationLogic
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
}