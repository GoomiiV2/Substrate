using Hexa.NET.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vortice.Mathematics;

namespace Substrate
{
    public static class Utils
    {
        public static Vector4 Vector4FromHex(string hexColor)
        {
            var vec4 = new Vector4();
            if (hexColor.StartsWith('#'))
            {
                var color      = hexColor.AsSpan(1);
                var colorBytes = Convert.FromHexString(color);

                if (colorBytes.Length >= 1)
                    vec4.X = ByteToFloatColor(colorBytes[0]);

                if (colorBytes.Length >= 2)
                    vec4.Y = ByteToFloatColor(colorBytes[1]);

                if (colorBytes.Length >= 3)
                    vec4.Z = ByteToFloatColor(colorBytes[2]);

                if (colorBytes.Length >= 4)
                    vec4.W = ByteToFloatColor(colorBytes[3]);
            }

            return vec4;
        }

        public static string Vector4ToHex(Vector4 vec4)
        {
            var hexStr = $"#{FloatToByteColor(vec4.X):X2}{FloatToByteColor(vec4.Y):X2}{FloatToByteColor(vec4.Z):X2}{FloatToByteColor(vec4.W):X2}";
            return hexStr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ByteToFloatColor(byte color)
        {
            return (1f / 255f) * color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte FloatToByteColor(float color)
        {
            return (byte)(color * 255);
        }

        public static Vector4 DarkenOrLightenColor(Vector4 color, float amount)
        {
            float h = 0, s = 0, v = 0;
            float r = 0, g = 0, b = 0;
            ImGui.ColorConvertRGBtoHSV(color.X, color.Y, color.Z, ref h, ref s, ref v);
            v += amount;
            ImGui.ColorConvertHSVtoRGB(h, s, v, ref r, ref g, ref b);
            var newColor = new Vector4(r, g, b, 1.0f);

            return newColor;
        }
    }
}
