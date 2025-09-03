using HarmonyLib;
using LevelExpSettings.Compatibility.WoL.Patches;
using StardewModdingAPI;

namespace LevelExpSettings.Compatibility.WoL
{
    internal static class WoLLoader
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static void Loader(IModHelper helper, Harmony harmony)
        {
            WoLPatches(harmony);
            // Edit the Experience Curve of WoL Skills when loading the game
            editWoLExperienceCurve();

            helper.Events.GameLoop.SaveLoaded += (_, _) => {
                editWoLExperienceCurve();
                editWolSCExperienceCurve();
            };
        }

        internal static void WoLPatches(Harmony harmony)
        {
            // Check new Exp
            harmony.Patch(
                original: AccessTools.Method("DaLion.Professions.Framework.Patchers.Prestige.FarmerCheckForLevelGainPatcher:FarmerCheckForLevelGainPostfix"),
                transpiler: new HarmonyMethod(typeof(FarmerCheckForLevelGainPatcherPatch), nameof(FarmerCheckForLevelGainPatcherPatch.FarmerCheckForLevelGainPostfixTranspiler))
            );

            // Change UIInfoSuite2 WoL Integration
            harmony.Patch(
                original: AccessTools.Method("DaLion.Professions.Framework.Patchers.Integration.UiInfoSuite.ExperienceBarGetExperienceRequiredToLevelPatcher:ExperienceBarGetExperienceRequiredToLevelPrefix"),
                transpiler: new HarmonyMethod(typeof(ExperienceBarGetExperienceRequiredToLevelPatcher), nameof(ExperienceBarGetExperienceRequiredToLevelPatcher.ExperienceBarGetExperienceRequiredToLevelPrefixTranspiler))
            );
        }

        internal static void editWoLExperienceCurve()
        {
            Type ISkill = AccessTools.TypeByName("DaLion.Professions.Framework.ISkill");
            int[] ExperienceCurve = ModEntry.ModHelper.Reflection.GetProperty<int[]>(ISkill, "ExperienceCurve").GetValue();

            for (int i = 0; i < 20; i++)
            {
                ExperienceCurve[i + 1] = ModEntry.LevelsCalculated[i];
            }
        }

        internal static void editWolSCExperienceCurve()
        {
            var skillList = (string[])AccessTools.Method("SpaceCore.Skills:GetSkillList").Invoke(null, null)!;
            var getSkill = AccessTools.Method("SpaceCore.Skills:GetSkill");

            foreach (string skill in skillList)
            {
                var expCurve = ((dynamic)getSkill.Invoke(null, [skill])!).ExperienceCurve;
                if (expCurve.Length >= 20)
                {
                    for (int i = 10; i < 20; i++)
                    {
                        expCurve[i] = ModEntry.LevelsCalculated[i];
                    }
                }
            }
        }
    }
}
