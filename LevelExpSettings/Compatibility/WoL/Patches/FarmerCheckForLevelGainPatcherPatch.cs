using HarmonyLib;
using LevelExpSettings.Patches;
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;

namespace LevelExpSettings.Compatibility.WoL.Patches
{
    internal static class FarmerCheckForLevelGainPatcherPatch
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static IEnumerable<CodeInstruction> FarmerCheckForLevelGainPostfixTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatcher matcher = new(instructions, generator);

                MethodInfo newExpCalculated = AccessTools.PropertyGetter(typeof(ModEntry), nameof(ModEntry.LevelsCalculated));

                //from: requiredExpForThisLevel = ISkill.LEVEL_10_EXP + ProfessionsMod.Config.Masteries.ExpPerPrestigeLevel * i
                //to:   requiredExpForThisLevel = ModEntry.LevelsCalculated[i + 9]
                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4)
                    )
                    .ThrowIfNotMatch("FarmerCheckForLevelGainPatcherPatch.FarmerCheckForLevelGainPostfixTranspiler: IL code 1 not found")
                    .SetAndAdvance(OpCodes.Call, newExpCalculated)
                    .RemoveInstructions(5)
                    .Advance(1)
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Ldelem_I4)
                    )
                    .Advance(1)
                    .RemoveInstructions(2)
                ;

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(FarmerCheckForLevelGainPostfixTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }
    }
}
