using HarmonyLib;
using LevelExpSettings.Compatibility.GMCM;
using LevelExpSettings.Compatibility.WoL.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LevelExpSettings.Compatibility.WoL
{
    internal static class WoLLoader
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static void Loader(IModHelper helper, Harmony harmony)
        {
            WoLPatches(harmony);
            // Edit the Experience Curve of WoL Skills
            // Editarlo antes de que 
            editWoLExperienceCurve();

            helper.Events.GameLoop.GameLaunched += (_, _) => GMCMConfigWoL();
        }

        internal static void WoLPatches(Harmony harmony)
        {
            // Check new Exp
            harmony.Patch(
                original: AccessTools.Method("DaLion.Professions.Framework.Patchers.Prestige.FarmerCheckForLevelGainPatcher:FarmerCheckForLevelGainPostfix"),
                transpiler: new HarmonyMethod(typeof(FarmerCheckForLevelGainPatcherPatch), nameof(FarmerCheckForLevelGainPatcherPatch.FarmerCheckForLevelGainPostfixTranspiler))
            );
        }
        private static void GMCMConfigWoL()
        {
            var configMenu = ModEntry.ModHelper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.AddPage(
                mod: ModEntry.ModManifest,
                pageId: "WoL Exp"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[11].ToString(),
                setValue: (value) => ModEntry.Config.Levels[11] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 11 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[12].ToString(),
                setValue: (value) => ModEntry.Config.Levels[12] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 12 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[13].ToString(),
                setValue: (value) => ModEntry.Config.Levels[13] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 13 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[14].ToString(),
                setValue: (value) => ModEntry.Config.Levels[14] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 14 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[15].ToString(),
                setValue: (value) => ModEntry.Config.Levels[15] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 15 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[16].ToString(),
                setValue: (value) => ModEntry.Config.Levels[16] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 16 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[17].ToString(),
                setValue: (value) => ModEntry.Config.Levels[17] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 17 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[18].ToString(),
                setValue: (value) => ModEntry.Config.Levels[18] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 18 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[19].ToString(),
                setValue: (value) => ModEntry.Config.Levels[19] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 19 }),
                tooltip: () => "Default: 5000"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[20].ToString(),
                setValue: (value) => {
                    ModEntry.Config.Levels[20] = int.TryParse(value, out int result) ? result : 5000;
                    ModEntry.CalculateLevels();
                    editWoLExperienceCurve();
                    editWolSCExperienceCurve();
                },
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 20 }),
                tooltip: () => "Default: 5000"
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
