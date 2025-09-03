using HarmonyLib;
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;

namespace LevelExpSettings.Patches
{
    internal static class FarmerPatch
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static bool getBaseExperienceForLevelPrefix(int level, ref int __result)
        {
            if (level < 1 || level > 10) return true;

            __result = ModEntry.LevelsCalculated[level - 1];

            return false;
        }

        internal static IEnumerable<CodeInstruction> getBaseExperienceForLevelTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatcher matcher = new(instructions, generator);

                MethodInfo newExperienceLevelsInfo = AccessTools.Method(typeof(FarmerPatch), nameof(newExperienceLevels));

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Switch)
                    )
                    .ThrowIfNotMatch("FarmerPatch.getBaseExperienceForLevelTranspiler: IL code 1 not found")
                    .Insert(
                        new CodeInstruction(OpCodes.Dup)
                    )
                ;

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4_S)
                    )
                    .ThrowIfNotMatch("FarmerPatch.getBaseExperienceForLevelTranspiler: IL code 2 not found")
                    .Set(OpCodes.Call, newExperienceLevelsInfo)
                ;

                for (int i = 0; i < 9 ; i++)
                {
                    matcher
                        .MatchStartForward(
                            new CodeMatch(OpCodes.Ldc_I4)
                        )
                        .ThrowIfNotMatch($"FarmerPatch.getBaseExperienceForLevelTranspiler: IL code 3{i} not found")
                        .Set(OpCodes.Call, newExperienceLevelsInfo)
                    ;
                }

                matcher
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldc_I4_M1)
                    )
                    .ThrowIfNotMatch("FarmerPatch.getBaseExperienceForLevelTranspiler: IL code 4 not found")
                    .SetAndAdvance(OpCodes.Pop, null)
                    .Insert(
                        new CodeInstruction(OpCodes.Ldc_I4_M1)
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

        internal static int newExperienceLevels(int i)
        {
            return ModEntry.LevelsCalculated[i];
        }
    }
}
