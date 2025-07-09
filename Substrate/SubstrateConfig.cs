using Hexa.NET.ImGui;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Substrate
{
    public class SubstrateConfig
    {
        [JsonIgnore] public virtual string AppName         => "Substrate";
        [JsonIgnore] public virtual string AppDataPath     => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
        [JsonIgnore] public virtual string LogsDir         => Path.Combine(AppDataPath, "Logs");
        [JsonIgnore] public virtual bool EnableFileLogging => true;
        [JsonIgnore] public virtual string ConfigPath      => Path.Combine(AppDataPath, "config.json");
        [JsonIgnore] public virtual string ThemesDir       => Path.Combine(AppDataPath, "Themes");

        [JsonIgnore] public int ConfigFrameworkVersion = 1; // if the substrate framework updates defaults in the config incrment this
        [JsonIgnore] public int ConfigAppVersion       = 1; // if an app updates config defaults incrment this
        public Version ConfigVersion                   => new Version(ConfigFrameworkVersion, ConfigAppVersion);
        public string Theme                            = "Imgui Dark";
        public Rendering Rendering                     = new Rendering();
    }

    public class Rendering
    {
        public bool VSync                = true;
        public bool DebugRenderingDevice = true;
        [JsonConverter(typeof(StringEnumConverter))]
        public GraphicsBackend GfxAPI    = GraphicsBackend.Direct3D11;
    }
}
