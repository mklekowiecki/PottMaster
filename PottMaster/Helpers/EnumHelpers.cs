using PottMasterLib.Models;
using System.Reflection;

namespace PottMaster.Helpers;

public static class EnumHelpers
{
    public static T? CodeToEnum<T>(string code) where T : struct, Enum
    {
        foreach (var val in Enum.GetValues<T>())
        {
            var field = typeof(T).GetField(val.ToString());
            var attr = field?.GetCustomAttribute<CodeAttribute>();
            if (attr?.Code == code)
            {
                return val;
            }
        }
        return null;
    }
}