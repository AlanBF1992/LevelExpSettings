using HarmonyLib;
using LevelExpSettings.Compatibility.UES.Patches;
using StardewModdingAPI;

namespace LevelExpSettings.Compatibility.UES
{
    internal static class UESLoader
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static void Loader(IModHelper helper, Harmony harmony)
        {
            UESPatches(harmony);

            helper.Events.GameLoop.SaveLoaded += (_, _) => editUESXpCurve();
        }

        internal static void UESPatches(Harmony harmony)
        {
            // Check new Exp
            harmony.Patch(
                original: AccessTools.Method("UnifiedExperienceSystem.ModEntry:RebuildUesXpCurve"),
                transpiler: new HarmonyMethod(typeof(ModEntryEXPTrackingPatch), nameof(ModEntryEXPTrackingPatch.RebuildUesXpCurveTranspiler))
            );
        }

        internal static void editUESXpCurve()
        {
            var UESInstance = AccessTools.PropertyGetter("UnifiedExperienceSystem.ModEntry:Instance").Invoke(null, null);
            var RebuildUesXpCurve = AccessTools.Method("UnifiedExperienceSystem.ModEntry:RebuildUesXpCurve");
            RebuildUesXpCurve.Invoke(UESInstance, null);
        }
    }
}
