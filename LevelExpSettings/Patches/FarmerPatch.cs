using StardewModdingAPI;

namespace LevelExpSettings.Patches
{
    internal static class FarmerPatch
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static bool getBaseExperienceForLevelPrefix(int level, ref int __result)
        {
            //Console.WriteLine("It's reading this");
            if (level < 1 || level >  10) return true;

            __result = ModEntry.LevelsCalculated[level - 1];

            return false;
        }
    }
}
