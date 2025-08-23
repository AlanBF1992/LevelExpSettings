using StardewModdingAPI;

namespace LevelExpSettings.Compatibility.UIInfoSuite2.Patches
{
    internal static class ExperienceBarPatch
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static bool GetExperienceRequiredToLevelPrefix(int currentLevel, ref int __result)
        {
            if (currentLevel is >= 0 and <= 9)
            {
                __result = ModEntry.LevelsCalculated[currentLevel];
            }
            else
            {
                __result = -1;
            }

            return false;
        }
    }
}
