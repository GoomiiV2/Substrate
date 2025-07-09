
using Hexa.NET.ImGui;
using Newtonsoft.Json;
using Substrate.Logging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hexa.NET.ImGuizmo;

namespace Substrate
{
    public static partial class Substrate
    {
        public static SubstrateApp App { get; private set; }
        public static SubstrateConfig Config { get; private set; }
        public static Resources Resources { get; private set; }
        public static Logger Log { get; private set; }
        public static Theme Theme { get; private set; }
        public static Dictionary<string, Theme> Themes { get; private set; } = new Dictionary<string, Theme>();
        public static FontManager Fonts { get; private set; }
        public static Modals Modals { get; private set; }

        public static T Get<T>(SubstrateConfig? config = null) where T : SubstrateConfig => (T)Config;
        public static T Get<T>(SubstrateApp? app       = null) where T : SubstrateApp    => (T)App;

        public static void Init<AppType, ConfigType>(string[] args = null)
            where AppType : SubstrateApp, new()
            where ConfigType : SubstrateConfig, new()
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            ImGuizmo.SetImGuiContext(context);

            Fonts  = new FontManager();
            Config = new ConfigType(); // default first
            Log    = new Logger();
            LoadConfig<ConfigType>();
            CreateFolders();
            App    = new AppType();
            Modals = new Modals();

            ReloadThemes();

            App.Init(args);

            SetTheme(Config?.Theme ?? Theme.ImguiDark.Name);

            App.StartRenderLoop();
        }

        private static void CreateFolders()
        {
            if (!Directory.Exists(Config.AppDataPath))
                Directory.CreateDirectory(Config.AppDataPath);

            if (!Directory.Exists(Config.ThemesDir))
                Directory.CreateDirectory(Config.ThemesDir);
        }

        public static void LoadConfig<ConfigType>() where ConfigType : SubstrateConfig, new()
        {
            if (File.Exists(Config.ConfigPath))
            {
                try
                {
                    var configTxt = File.ReadAllText(Config.ConfigPath);
                    Config = JsonConvert.DeserializeObject<ConfigType>(configTxt);
                }
                catch (Exception ex)
                {
                    Log.Error(LogCat.Substrate, "Couldn't load config from {configPath}, due to: {error}", Config.ConfigPath, ex);
                }
            }
            else
            {
                SaveConfig();
            }

            Log.Info(LogCat.Substrate, "Loaded config");
        }

        public static void SaveConfig()
        {
            var configTxt = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(Config.ConfigPath, configTxt);

            Log.Trace(LogCat.Substrate, "Saved config");
        }

        // Set a theme from code, this won't get saved to the config
        public static unsafe void SetTheme(Theme theme)
        {
            Theme = theme;

            ImGuiStyle* stylePtr = ImGui.GetStyle().Handle;
            Marshal.StructureToPtr(Theme.Imgui, (IntPtr)stylePtr, true);
        }

        public static bool SetTheme(string themeName)
        {
            if (Themes.TryGetValue(themeName, out var theme))
            {
                SetTheme(theme);
                Config.Theme = themeName;
                SaveConfig();
                return true;
            }

            // Default to dark
            SetTheme(Theme.ImguiDark.Name);
            return false;
        }

        public static void ReloadThemes()
        {
            Themes.TryAdd(Theme.ImguiLight.Name, Theme.ImguiLight);
            Themes.TryAdd(Theme.ImguiDark.Name, Theme.ImguiDark);

            foreach(var theme in Themes.Where(x => !x.Value.IsBuiltIn))
            {
                Themes.Remove(theme.Key);
            }

            var themeFiles = Directory.GetFiles(Config.ThemesDir, "*.theme.json");
            foreach (var themeFile in themeFiles)
            {
                var theme = Theme.Load(themeFile);
                if (theme != null)
                    Themes.TryAdd(theme.Name, theme);
            }

            if (Theme != null)
                SetTheme(Theme.Name);
        }
    }
}
