using HarmonyLib;
using LevelExpSettings.Patches;
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

                MethodInfo newExpCalculated = AccessTools.PropertyGetter(typeof(ModEntry), nameof(ModEntry.LevelsCalculated));

                //from: __result = ISkill.LEVEL_10_EXP + Config.Masteries.ExpPerPrestigeLevel * (currentLevel - 10 + 1)
                //to:   __result = ModEntry.LevelsCalculated[currentLevel]
                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4)
                    )
                    .ThrowIfNotMatch("FarmerCheckForLevelGainPatcherPatch.FarmerCheckForLevelGainPostfixTranspiler: IL code 1 not found")
                    .SetInstruction(
                        new CodeInstruction(OpCodes.Call, newExpCalculated)
                    )
                    .Advance(2)
                    .RemoveInstructions(9)
                    .Insert(
                        new CodeInstruction(OpCodes.Ldelem_I4)
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