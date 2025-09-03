using HarmonyLib;
using LevelExpSettings.Compatibility.GMCM;
using LevelExpSettings.Patches;
using StardewModdingAPI;
using StardewValley;

namespace LevelExpSettings
{
    internal static class VanillaLoader
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;

        internal static void Loader(IModHelper helper, Harmony harmony)
        {
            VanillaPatches(harmony);

            helper.Events.GameLoop.GameLaunched += (_, _) => GMCMConfig();
            helper.Events.GameLoop.SaveLoaded += (_, _) => ModEntry.CalculateLevels();
        }

        private static void VanillaPatches(Harmony harmony)
        {
            // Base
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getBaseExperienceForLevel)),
                prefix: new HarmonyMethod(typeof(FarmerPatch), nameof(FarmerPatch.getBaseExperienceForLevelPrefix))
                //transpiler: new HarmonyMethod(typeof(FarmerPatch), nameof(FarmerPatch.getBaseExperienceForLevelTranspiler))
            );
        }

        private static void GMCMConfig()
        {
            var configMenu = ModEntry.ModHelper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // Register mod
            configMenu.Register(
                mod: ModEntry.ModManifest,
                reset: () => ModEntry.Config = new ModConfig(),
                save: () => ModEntry.ModHelper.WriteConfig(ModEntry.Config),
                titleScreenOnly: true
            );

            configMenu.AddPageLink(
                mod: ModEntry.ModManifest,
                pageId: "Vanilla Exp",
                text: () => "1 - 10 Exp"
            );

            // Enable Level 11 to 20
            if (ModEntry.EnableLevel11To20)
            {
                configMenu.AddPageLink(
                    mod: ModEntry.ModManifest,
                    pageId: "11 - 20 Exp",
                    text: () => "11 - 20 Exp"
                );
            }

            #region 1to10
            /****************
             * Level 1 - 10 *
             ****************/
            configMenu.AddPage(
                mod: ModEntry.ModManifest,
                pageId: "Vanilla Exp"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[1].ToString(),
                setValue: (value) => ModEntry.Config.Levels[1] = int.TryParse(value, out int result) ? result : 100,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 1 }),
                tooltip: () => "Default: 100"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[2].ToString(),
                setValue: (value) => ModEntry.Config.Levels[2] = int.TryParse(value, out int result) ? result : 280,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 2 }),
                tooltip: () => "Default: 280"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[3].ToString(),
                setValue: (value) => ModEntry.Config.Levels[3] = int.TryParse(value, out int result) ? result : 390,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 3 }),
                tooltip: () => "Default: 390"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[4].ToString(),
                setValue: (value) => ModEntry.Config.Levels[4] = int.TryParse(value, out int result) ? result : 530,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 4 }),
                tooltip: () => "Default: 530"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[5].ToString(),
                setValue: (value) => ModEntry.Config.Levels[5] = int.TryParse(value, out int result) ? result : 850,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 5 }),
                tooltip: () => "Default: 850"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[6].ToString(),
                setValue: (value) => ModEntry.Config.Levels[6] = int.TryParse(value, out int result) ? result : 1150,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 6 }),
                tooltip: () => "Default: 1150"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[7].ToString(),
                setValue: (value) => ModEntry.Config.Levels[7] = int.TryParse(value, out int result) ? result : 1500,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 7 }),
                tooltip: () => "Default: 1500"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[8].ToString(),
                setValue: (value) => ModEntry.Config.Levels[8] = int.TryParse(value, out int result) ? result : 2100,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 8 }),
                tooltip: () => "Default: 2100"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[9].ToString(),
                setValue: (value) => ModEntry.Config.Levels[9] = int.TryParse(value, out int result) ? result : 3100,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 9 }),
                tooltip: () => "Default: 3100"
            );

            configMenu.AddTextOption(
                mod: ModEntry.ModManifest,
                getValue: () => ModEntry.Config.Levels[10].ToString(),
                setValue: (value) => {
                    ModEntry.Config.Levels[10] = int.TryParse(value, out int result) ? result : 5000;
                    LogMonitor.Log(ModEntry.ModHelper.Translation.Get("experience-warning"), LogLevel.Warn);
                },
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 10 }),
                tooltip: () => "Default: 5000"
            );
            #endregion

            #region 11to20
            /*****************
             * Level 11 - 20 *
             *****************/
            if (!ModEntry.EnableLevel11To20) return;

            configMenu.AddPage(
                mod: ModEntry.ModManifest,
                pageId: "11 - 20 Exp"
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
                setValue: (value) => ModEntry.Config.Levels[20] = int.TryParse(value, out int result) ? result : 5000,
                name: () => ModEntry.ModHelper.Translation.Get("experience-for-level", new { lvl = 20 }),
                tooltip: () => "Default: 5000"
            );
            #endregion
        }
    }
}