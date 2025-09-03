using HarmonyLib;
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;

namespace LevelExpSettings.Compatibility.UES.Patches
{
    internal static class ModEntryEXPTrackingPatch
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static IEnumerable<CodeInstruction> RebuildUesXpCurveTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatcher matcher = new(instructions, generator);

                MethodInfo checkExpInfo = AccessTools.Method(typeof(ModEntryEXPTrackingPatch), nameof(checkExp));
                MethodInfo resetExpInfo = AccessTools.Method(typeof(ModEntryEXPTrackingPatch), nameof(resetStep));

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_R8)
                    )
                    .ThrowIfNotMatch("ModEntryEXPTrackingPatch.RebuildUesXpCurveTranspiler: IL code 1 not found")
                ;

                //var resetStep = matcher.InstructionsWithOffsets(1, 6);
                //resetStep.Insert(0, new CodeInstruction(OpCodes.Ldc_R8, 1));


                //from: int add = Math.Max(1, (int)Math.Round(step));
                //to:   int add = checkExp(L, Math.Max(1, (int)Math.Round(step)));
                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4_1),
                        new CodeMatch(OpCodes.Ldloc_1),
                        new CodeMatch(OpCodes.Call)
                    )
                    .ThrowIfNotMatch("ModEntryEXPTrackingPatch.RebuildUesXpCurveTranspiler: IL code 1 not found")
                    .Insert(
                        new CodeInstruction(OpCodes.Ldloc_S, 7)
                    )
                    .Advance(6)
                    .Insert(
                        new CodeInstruction(OpCodes.Call, checkExpInfo)
                    )
                ;

                //if lvl = 20
                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldloc_1),
                        new CodeMatch(OpCodes.Ldc_R8)
                    )
                    .ThrowIfNotMatch("ModEntryEXPTrackingPatch.RebuildUesXpCurveTranspiler: IL code 2 not found")
                    .Insert(
                        new CodeInstruction(OpCodes.Ldloc_S, 7)
                    )
                    .Advance(5)
                    .SetInstruction(
                        new CodeInstruction(OpCodes.Call, resetExpInfo)
                    )
                ;



                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(RebuildUesXpCurveTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }

        internal static int checkExp(int L, int exp)
        {
            if (L > 20)
            {
                return exp;
            }
            return ModEntry.Config.Levels[L];
        }

        internal static double resetStep(int L, double step, double G)
        {
            // G = 1 + g
            if (L <= 20) return step;
            return step * G;
        }
    }
}
