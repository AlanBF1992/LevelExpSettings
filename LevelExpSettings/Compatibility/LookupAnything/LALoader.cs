using HarmonyLib;
using LevelExpSettings.Compatibility.LookupAnything.Patches;
using StardewModdingAPI;

namespace LevelExpSettings.Compatibility.LookupAnything
{
    internal static class LALoader
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static void Loader(IModHelper _, Harmony harmony)
        {
            LUAPatches(harmony);
        }

        internal static void LUAPatches(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("Pathoschild.Stardew.LookupAnything.Framework.Lookups.Characters.FarmerSubject:GetData"),
                prefix: new HarmonyMethod(typeof(FarmerSubjectPatch), nameof(FarmerSubjectPatch.GetDataPrefix))
            );
        }
    }
}
