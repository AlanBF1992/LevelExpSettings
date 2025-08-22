using HarmonyLib;
using StardewModdingAPI;

namespace LevelExpSettings.Compatibility.SpaceCore
{
    internal static class SCLoader
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static void Loader(IModHelper helper, Harmony _)
        {
            helper.Events.GameLoop.SaveLoaded += (_, _) => editSCExperienceCurve();
        }

        internal static void editSCExperienceCurve()
        {
            var skillList = (string[])AccessTools.Method("SpaceCore.Skills:GetSkillList").Invoke(null, null)!;
            var getSkill = AccessTools.Method("SpaceCore.Skills:GetSkill");

            foreach (string skill in skillList)
            {
                var expCurve = ((dynamic)getSkill.Invoke(null, [skill])!).ExperienceCurve;
                for (int i = 0; i < 10; i++)
                {
                    expCurve[i] = ModEntry.LevelsCalculated[i];
                }
            }
        }
    }
}