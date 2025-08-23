using HarmonyLib;
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;

namespace LevelExpSettings.Compatibility.WoL.Patches
{
    internal static class ExperienceBarGetExperienceRequiredToLevelPatcher
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static IEnumerable<CodeInstruction> ExperienceBarGetExperienceRequiredToLevelPrefixTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatcher matcher = new(instructions, generator);

                MethodInfo newExperienceLevelsInfo = AccessTools.Method(typeof(FarmerCheckForLevelGainPatcherPatch), nameof(FarmerCheckForLevelGainPatcherPatch.newExperienceLevels));

                //from: __result = ISkill.LEVEL_10_EXP + Config.Masteries.ExpPerPrestigeLevel * (currentLevel - 10 + 1)
                //to:   __result = newExperienceLevels(ISkill.LEVEL_10_EXP, currentLevel - 9)
                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4)
                    )
                    .ThrowIfNotMatch("FarmerCheckForLevelGainPatcherPatch.FarmerCheckForLevelGainPostfixTranspiler: IL code 1 not found")
                    .Advance(2)
                    .RemoveInstructions(9)
                    .Insert(
                        new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(OpCodes.Call, newExperienceLevelsInfo)
                    )
                ;

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4_0)
                    )
                    .SetOpcodeAndAdvance(OpCodes.Ldc_I4_M1)
                ;

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(ExperienceBarGetExperienceRequiredToLevelPrefixTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }
    }
}