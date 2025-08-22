using HarmonyLib;
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

                MethodInfo newExperienceLevelsInfo = AccessTools.Method(typeof(FarmerCheckForLevelGainPatcherPatch), nameof(newExperienceLevels));

                //from: requiredExpForThisLevel = ISkill.LEVEL_10_EXP + ProfessionsMod.Config.Masteries.ExpPerPrestigeLevel * i
                //to:   requiredExpForThisLevel = newExperienceLevels(ISkill.LEVEL_10_EXP, i)
                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Conv_I8)
                    )
                    .ThrowIfNotMatch("FarmerCheckForLevelGainPatcherPatch.FarmerCheckForLevelGainPostfixTranspiler: IL code 1 not found")
                    .RemoveInstructions(5)
                    .Advance(1)
                    .Insert(
                        new CodeInstruction(OpCodes.Call, newExperienceLevelsInfo)
                    )
                    .Advance(2)
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

        internal static int newExperienceLevels(int _, int i)
        {
            return ModEntry.LevelsCalculated[9 + i];
        }
    }
}
