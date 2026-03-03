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

                MethodInfo newExpCalculatedInfo = AccessTools.Method(typeof(FarmerPatch), nameof(newExpCalculated));

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Switch)
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Call, newExpCalculatedInfo)
                    )
                ;

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Switch)
                    )
                    .ThrowIfNotMatch("FarmerPatch.getBaseExperienceForLevelTranspiler: IL code 1 not found")
                    .RemoveInstructions(23)
                ;

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                LogMonitor.Log($"Failed in {nameof(getBaseExperienceForLevelTranspiler)}:\n{ex}", LogLevel.Error);
                return instructions;
            }
        }

        private static int newExpCalculated(int i)
        {
            return i switch
            {
                >= 0 and <= 19 => ModEntry.LevelsCalculated[i],
                _ => -1,
            };
        }
    }
}
