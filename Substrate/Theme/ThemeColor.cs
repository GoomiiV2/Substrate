using Hexa.NET.ImGui;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace Substrate
{
    public class ThemeColor
    {
        [JsonIgnore] public Vector4 Primary;
        [JsonIgnore] public Vector4 Darker;
        [JsonIgnore] public Vector4 Lighter;

        [JsonProperty]
        private string PrimaryHex
        {
            get => Utils.Vector4ToHex(Primary);
            set
            {
                Primary = Utils.Vector4FromHex(value);
                ReCalcDarkerAndLighter(DefaultDarkenAmount, DefaultLightenAmount);
            }
        }

        [JsonProperty]
        private string DarkerHex
        {
            get => Utils.Vector4ToHex(Darker);
            set
            {
                Darker = Utils.Vector4FromHex(value);
            }
        }

        [JsonProperty]
        private string LighterHex
        {
            get => Utils.Vector4ToHex(Lighter);
            set
            {
                Lighter = Utils.Vector4FromHex(value);
            }
        }

        public const float DefaultDarkenAmount  = -0.2f;
        public const float DefaultLightenAmount = 0.2f;

        public ThemeColor() { }

        public ThemeColor(Vector4 color)
        {
            Primary = color;
            ReCalcDarkerAndLighter(DefaultDarkenAmount, DefaultLightenAmount);
        }

        public ThemeColor(Vector4 primary, Vector4 darker, Vector4 lighter)
        {
            Primary = primary;
            Darker  = darker;
            Lighter = lighter;
        }

        public void ReCalcDarkerAndLighter(float darkerAmount, float lighterAmount)
        {
            Darker  = Utils.DarkenOrLightenColor(Primary, darkerAmount);
            Lighter = Utils.DarkenOrLightenColor(Primary, lighterAmount);
        }

        public Vector4 Darkened(float amount)
        {
            return Utils.DarkenOrLightenColor(Primary, amount);
        }

        public Vector4 Lightend(float amount)
        {
            return Darkened(amount * -1f);
        }
    }
}
