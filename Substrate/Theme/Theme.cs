using Hexa.NET.ImGui;
using Newtonsoft.Json;
using Serilog.Core;
using Substrate.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct3D11.Debug;

namespace Substrate
{
    public class Theme
    {
        public static readonly Theme ImguiDark    = new ImguiDarkTheme();
        public static readonly Theme ImguiLight   = new ImguiLightTheme();
        public static readonly Theme PurpliiTheme = new PurpliiTheme();

        public string Name = "";
        [JsonIgnore]
        public bool IsBuiltIn         = false;
        public ImGuiStyle Imgui       = new ImGuiStyle();
        public string Font = FontManager.Regular;
        public Dictionary<ThemeColors, ThemeColor> Colors = new Dictionary<ThemeColors, ThemeColor>();

        [JsonIgnore] public ThemeColor Info    => Colors[ThemeColors.Info] ?? new ThemeColor();
        [JsonIgnore] public ThemeColor Success => Colors[ThemeColors.Success] ?? new ThemeColor();
        [JsonIgnore] public ThemeColor Warning => Colors[ThemeColors.Warning] ?? new ThemeColor();
        [JsonIgnore] public ThemeColor Error   => Colors[ThemeColors.Error] ?? new ThemeColor();

        public static Theme Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var themeTxt = File.ReadAllText(path);
                    var theme    = JsonConvert.DeserializeObject<Theme>(themeTxt, new JsonSerializerSettings()
                    {
                        Converters        = [new ImGuiStyleConverter()],
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    return theme;
                }
                catch (Exception ex)
                {
                    Substrate.Log.Error(LogCat.Substrate, "Error loading theme at {path} {ex}", path, ex);
                }
            }

            return null;
        }

        public void Save(string path)
        {
            try
            {
                var themeTxt = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
                {
                    Converters        = [new ImGuiStyleConverter()],
                    NullValueHandling = NullValueHandling.Ignore
                });
                File.WriteAllText(path, themeTxt);
            }
            catch (Exception ex)
            {
                Substrate.Log.Error(LogCat.Substrate, "Error saving theme at {path} {ex}", path, ex);
            }
        }
    }

    public class ImguiDarkTheme : Theme 
    {
        public unsafe ImguiDarkTheme()
        {
            Name = "Imgui Dark";
            IsBuiltIn = true;
            ImGuiStyle* stylePtr = ImGui.GetStyle().Handle;
            ImGui.StyleColorsDark(stylePtr);
            Imgui = (ImGuiStyle)Marshal.PtrToStructure((IntPtr)stylePtr, typeof(ImGuiStyle));

            Colors[ThemeColors.Info]    = new ThemeColor(Utils.Vector4FromHex("#2196F3FF"));
            Colors[ThemeColors.Success] = new ThemeColor(Utils.Vector4FromHex("#00C853FF"));
            Colors[ThemeColors.Warning] = new ThemeColor(Utils.Vector4FromHex("#FFA800FF"));
            Colors[ThemeColors.Error]   = new ThemeColor(Utils.Vector4FromHex("#F64E62FF"));
        }
    }

    public class ImguiLightTheme : Theme
    {
        public unsafe ImguiLightTheme()
        {
            Name = "Imgui Light";
            IsBuiltIn = true;
            ImGuiStyle* stylePtr = ImGui.GetStyle().Handle;
            ImGui.StyleColorsLight(stylePtr);
            Imgui = (ImGuiStyle)Marshal.PtrToStructure((IntPtr)stylePtr, typeof(ImGuiStyle));

            Colors[ThemeColors.Info]    = new ThemeColor(Utils.Vector4FromHex("#2196F3FF"));
            Colors[ThemeColors.Success] = new ThemeColor(Utils.Vector4FromHex("#00C853FF"));
            Colors[ThemeColors.Warning] = new ThemeColor(Utils.Vector4FromHex("#FFA800FF"));
            Colors[ThemeColors.Error]   = new ThemeColor(Utils.Vector4FromHex("#F64E62FF"));
        }
    }

    public class PurpliiTheme : ImguiDarkTheme
    {
        public unsafe PurpliiTheme() : base()
        {
            Name      = "Purplii";
            IsBuiltIn = true;

            Imgui = new ImGuiStyle()
            {
                Colors_0  = Utils.Vector4FromHex("#FFFFFFFF"),
                Colors_1  = Utils.Vector4FromHex("#666666FF"),
                Colors_2  = Utils.Vector4FromHex("#2C2933FF"),
                Colors_3  = Utils.Vector4FromHex("#2C2933FF"),
                Colors_4  = Utils.Vector4FromHex("#2C2933FF"),
                Colors_5  = Utils.Vector4FromHex("#1E1E1EB5"),
                Colors_6  = Utils.Vector4FromHex("#FFFFFF0F"),
                Colors_7  = Utils.Vector4FromHex("#6B6B6B89"),
                Colors_8  = Utils.Vector4FromHex("#6B6B6B66"),
                Colors_9  = Utils.Vector4FromHex("#8E8E8EAA"),
                Colors_10 = Utils.Vector4FromHex("#5F185AFF"),
                Colors_11 = Utils.Vector4FromHex("#7E1B77FF"),
                Colors_12 = Utils.Vector4FromHex("#5F185AFF"),
                Colors_13 = Utils.Vector4FromHex("#555555FF"),
                Colors_14 = Utils.Vector4FromHex("#3D3D3D87"),
                Colors_15 = Utils.Vector4FromHex("#686868FF"),
                Colors_16 = Utils.Vector4FromHex("#848484FF"),
                Colors_17 = Utils.Vector4FromHex("#C1C1C1FF"),
                Colors_18 = Utils.Vector4FromHex("#A5A5A5FF"),
                Colors_19 = Utils.Vector4FromHex("#848484FF"),
                Colors_20 = Utils.Vector4FromHex("#A3A3A3FF"),
                Colors_21 = Utils.Vector4FromHex("#89898959"),
                Colors_22 = Utils.Vector4FromHex("#84848496"),
            };
        }
    }
}
