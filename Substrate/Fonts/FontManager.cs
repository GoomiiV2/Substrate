﻿using Hexa.NET.ImGui;
using System.Numerics;
using System.Reflection;
using Hexa.NET.ImGui.Utilities;

namespace Substrate
{
    public class FontManager
    {
        public Dictionary<string, Font> Fonts;
        public string DefaultFont = "";
        public List<Assembly> ResourceAssemblies;

        public const string Regular     = "Regular";
        public const string Bold        = "Bold";
        public const string Italic      = "Italic";
        public const string BoldItalic  = "BoldItalic";
        public const string ProggyClean = "ProggyClean";
        public const string FAS         = "FAS";

        public FontManager()
        {
            Fonts = new Dictionary<string, Font>();
            ResourceAssemblies = new List<Assembly>();
            
            AddFont(new Font("Regular", 18, new FontFile("Assets.Fonts.SourceSansPro-Regular.ttf", new Vector2(0, -1))));
            AddFont(new Font("Bold", 18, new FontFile("Assets.Fonts.SourceSansPro-Bold.ttf", new Vector2(0, -1))));
            AddFont(new Font("Italic", 18, new FontFile("Assets.Fonts.SourceSansPro-Italic.ttf", new Vector2(0, -1))));
            AddFont(new Font("BoldItalic", 18, new FontFile("Assets.Fonts.SourceSansPro-BoldItalic.ttf", new Vector2(0, -1))));
            //AddFont(new Font("ProggyClean", 13, new FontFile("ImGui.Default")));
            AddFont(new Font("FAS", [13, 18], new FontFile("Assets.Fonts.FAS.ttf", new GlyphRange(0x0021, 0xF8FF)))); //new GlyphRange(0xE000, 0xF8FF))));
            AddFont(new Font("UbuntuMono-Regular", 13, new FontFile("Assets.Fonts.UbuntuMono-Regular.ttf")));

            if (OperatingSystem.IsWindows())
            {
                AddFont(new Font("SegoeUI", 18, new FontFile(@"C:\Windows\Fonts\segoeui.ttf")));
                AddFont(new Font("SegoeUI-SemiBold", 18, new FontFile(@"C:\Windows\Fonts\seguisb.ttf")));
                AddFont(new Font("SegoeUI-Bold", 18, new FontFile(@"C:\Windows\Fonts\segoeuib.ttf")));
            }
        }

        public void AddFont(Font font)
        {
            if (!Fonts.ContainsKey(font.Name))
            {
                Fonts.Add(font.Name, font);
            }
        }

        public ImFontPtr GetImFontPointer(string font, byte fontSize = 0)
        {
            if (Fonts.ContainsKey(font))
            {
                return Fonts[font].GetPointer(fontSize);
            }

            return ImGui.GetIO().FontDefault;
        }

        public void PushFont(string font, byte fontSize = 0) => ImGui.PushFont(GetImFontPointer(font, fontSize));

        public void PopFont() => ImGui.PopFont();

        public void Clear()
        {
            if (Fonts == null)
            {
                return;
            }

            foreach (Font font in Fonts.Values)
            {
                font.Clear();
            }
        }

        public void RegisterResourceAssembly(Assembly assembly)
        {
            if (!ResourceAssemblies.Contains(assembly))
            {
                ResourceAssemblies.Add(assembly);
            }
        }

        private GlyphRange[] BuildExtendedRange()
        {
            return new[]
            {
                new GlyphRange(0x0020, 0x052F), // 0020 — 007F  	Basic Latin
                                                // 00A0 — 00FF  	Latin-1 Supplement
                                                // 0100 — 017F  	Latin Extended-A
                                                // 0180 — 024F  	Latin Extended-B
                                                // 0250 — 02AF  	IPA Extensions
                                                // 02B0 — 02FF  	Spacing Modifier Letters
                                                // 0300 — 036F  	Combining Diacritical Marks
                                                // 0370 — 03FF  	Greek and Coptic
                                                // 0400 — 04FF  	Cyrillic
                                                // 0500 — 052F  	Cyrillic Supplementary
                                                

                new GlyphRange(0x2000, 0x2BFF), // 2000 — 206F  	General Punctuation
                                                // 2070 — 209F  	Superscripts and Subscripts
                                                // 20A0 — 20CF  	Currency Symbols
                                                // 20D0 — 20FF  	Combining Diacritical Marks for Symbols	
                                                // 2100 — 214F  	Letterlike Symbols
                                                // 2150 — 218F  	Number Forms
                                                // 2190 — 21FF  	Arrows
                                                // 2200 — 22FF  	Mathematical Operators
                                                // 2300 — 23FF  	Miscellaneous Technical
                                                // 2400 — 243F  	Control Pictures
                                                // 2440 — 245F  	Optical Character Recognition
                                                // 2460 — 24FF  	Enclosed Alphanumerics
                                                // 2500 — 257F  	Box Drawing
                                                // 2580 — 259F  	Block Elements
                                                // 25A0 — 25FF  	Geometric Shapes
                                                // 2600 — 26FF  	Miscellaneous Symbols
                                                // 2700 — 27BF  	Dingbats
                                                // 27C0 — 27EF  	Miscellaneous Mathematical Symbols-A
                                                // 27F0 — 27FF  	Supplemental Arrows-A
                                                // 2800 — 28FF  	Braille Patterns
                                                // 2900 — 297F  	Supplemental Arrows-B
                                                // 2980 — 29FF  	Miscellaneous Mathematical Symbols-B
                                                // 2A00 — 2AFF  	Supplemental Mathematical Operators
                                                // 2B00 — 2BFF  	Miscellaneous Symbols and Arrows

            };
        }
    }
}
