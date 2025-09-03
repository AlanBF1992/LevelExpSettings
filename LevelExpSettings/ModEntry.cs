using HarmonyLib;
using LevelExpSettings.Compatibility.LookupAnything;
using LevelExpSettings.Compatibility.SpaceCore;
using LevelExpSettings.Compatibility.UES;
using LevelExpSettings.Compatibility.UIInfoSuite2;
using LevelExpSettings.Compatibility.WoL;
using StardewModdingAPI;

namespace LevelExpSettings
{
    public class ModEntry : Mod
    {
        /// <summary>Monitoring and logging for the mod.</summary>
        public static IMonitor LogMonitor { get; internal set; } = null!;

        /// <summary>Simplified APIs for writing mods.</summary>
        public static IModHelper ModHelper { get; internal set; } = null!;

        /// <summary>Manifest of the mod.</summary>
        new public static IManifest ModManifest { get; internal set; } = null!;

        /// <summary>The mod configuration from the player.</summary>
        public static ModConfig Config { get; internal set; } = null!;

        public static int[] LevelsCalculated { get; internal set; } = new int[20];

        public static bool EnableLevel11To20 { get; internal set; }

        public override void Entry(IModHelper helper)
        {
            LogMonitor = Monitor;
            ModHelper = Helper;
            ModManifest = base.ModManifest;
            Config = helper.ReadConfig<ModConfig>();

            CalculateLevels();
            EnableLevel11To20 = helper.ModRegistry.IsLoaded("DaLion.Professions") || helper.ModRegistry.IsLoaded("Darkmushu.UnifiedExperienceSystem");

            Harmony harmony = new(ModManifest.UniqueID);

            // Vanilla Patches
            VanillaLoader.Loader(helper, harmony);
            LogMonitor.Log("Base Patches Loaded", LogLevel.Info);

            // SpaceCore Compat
            if (helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
            {
                SCLoader.Loader(helper, harmony);
                LogMonitor.Log("SpaceCore Compat Patches Loaded", LogLevel.Info);
            }

            // WoL Compat
            if (helper.ModRegistry.IsLoaded("DaLion.Professions"))
            {
                WoLLoader.Loader(helper, harmony);
                LogMonitor.Log("Walk of Life Compat Patches Loaded", LogLevel.Info);
            }

            // UES Compat
            if (helper.ModRegistry.IsLoaded("Darkmushu.UnifiedExperienceSystem"))
            {
                UESLoader.Loader(helper, harmony);
                LogMonitor.Log("Unified Experience System Compat Patches Loaded", LogLevel.Info);
            }

            // Lookup Anything Compat
            if (helper.ModRegistry.IsLoaded("Pathoschild.LookupAnything"))
            {
                LALoader.Loader(helper, harmony);
                LogMonitor.Log("Lookup Anything Patches Loaded", LogLevel.Info);
            }

            // UI Info Suite 2 Compat
            if (helper.ModRegistry.IsLoaded("Annosz.UiInfoSuite2"))
            {
                UIS2Loader.Loader(helper, harmony);
                LogMonitor.Log("UI Info Suite 2 Patches Loaded", LogLevel.Info);
            }
        }

        internal static void CalculateLevels()
        {
            for (int i = 0; i < 20; i++)
            {
                if (i > 0)
                {
                    LevelsCalculated[i] = LevelsCalculated[i - 1] + Config.Levels[i + 1];
                }
                else
                {
                    LevelsCalculated[i] = Config.Levels[1];
                }

            }
        }
    }
}
