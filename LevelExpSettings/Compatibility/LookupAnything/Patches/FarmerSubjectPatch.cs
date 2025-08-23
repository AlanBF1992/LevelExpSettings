using StardewModdingAPI;
using System.Reflection;

namespace LevelExpSettings.Compatibility.LookupAnything.Patches
{
    internal static class FarmerSubjectPatch
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static void GetDataPrefix(object __instance)
        {
            PropertyInfo constantsInfo = __instance.GetType().GetProperty("Constants", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var constants = constantsInfo.GetValue(__instance)!;

            PropertyInfo expPerLevelInfo = constants.GetType().GetProperty("PlayerSkillPointsPerLevel")!;
            var expPerLevel = (int[])expPerLevelInfo.GetValue(constants)!;

            for (int i = 0; i < 10; i++)
            {
                expPerLevel[i] = ModEntry.LevelsCalculated[i];
            }

            PropertyInfo playerMaxInfo = constants.GetType().GetProperty("PlayerMaxSkillPoints")!;
            playerMaxInfo.SetValue(constants, ModEntry.LevelsCalculated[9]);
        }
    }
}
