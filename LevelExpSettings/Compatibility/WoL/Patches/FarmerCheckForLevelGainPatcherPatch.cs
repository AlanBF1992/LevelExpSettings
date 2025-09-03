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

                MethodInfo newExperienceLevelsInfo = AccessTools.Method(typeof(FarmerPatch), nameof(FarmerPatch.newExperienceLevels));

                //from: requiredExpForThisLevel = ISkill.LEVEL_10_EXP + ProfessionsMod.Config.Masteries.ExpPerPrestigeLevel * i
                //to:   requiredExpForThisLevel = newExperienceLevels(i)
                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4)
                    )
                    .ThrowIfNotMatch("FarmerCheckForLevelGainPatcherPatch.FarmerCheckForLevelGainPostfixTranspiler: IL code 1 not found")
                    .SetAndAdvance(OpCodes.Ldloc_0, null)
                    .RemoveInstructions(6)
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Call, newExperienceLevelsInfo)
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
