using HarmonyLib;
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;

namespace LevelExpSettings.Patches
{
    internal static class FarmerPatch
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static IEnumerable<CodeInstruction> getBaseExperienceForLevelTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatcher matcher = new(instructions, generator);

                MethodInfo newExpCalculated = AccessTools.PropertyGetter(typeof(ModEntry), nameof(ModEntry.LevelsCalculated));

                matcher
                    .Start()
                    .Insert(
                        new CodeInstruction(OpCodes.Call, newExpCalculated)
                    )
                ;

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Switch)
                    )
                    .ThrowIfNotMatch("FarmerPatch.getBaseExperienceForLevelTranspiler: IL code 1 not found")
                    .RemoveInstructions(23)
                    .Insert(
                        new CodeInstruction(OpCodes.Ldelem_I4)
                    )
                ;

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(getBaseExperienceForLevelTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }
    }
}
