using HarmonyLib;
using LevelExpSettings.Compatibility.UIInfoSuite2.Patches;
using StardewModdingAPI;

namespace LevelExpSettings.Compatibility.UIInfoSuite2
{
    internal static class UIS2Loader
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static void Loader(IModHelper _, Harmony harmony)
        {
            LUAPatches(harmony);
        }

        internal static void LUAPatches(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("UIInfoSuite2.UIElements.ExperienceBar:GetExperienceRequiredToLevel"),
                prefix: new HarmonyMethod(typeof(ExperienceBarPatch), nameof(ExperienceBarPatch.GetExperienceRequiredToLevelPrefix))
            );
        }
    }
}