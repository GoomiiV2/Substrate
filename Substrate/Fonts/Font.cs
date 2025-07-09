using Hexa.NET.ImGui;
using System.Runtime.InteropServices;
using System.Text;
using Hexa.NET.ImGui.Utilities;

namespace Substrate
{
    public class Font
    {
        private Dictionary<byte, ImFontPtr> pointers;
        private List<byte> sizes;
        public string Name { get; private set; }
        public IReadOnlyList<FontFile> Files { get; private set; }
        public IReadOnlyList<byte> Sizes { get => sizes; }


        public Font(string name, byte size, FontFile file) => Init(name, new List<byte> { size }, new List<FontFile> { file });

        public Font(string name, byte size, List<FontFile> files) => Init(name, new List<byte> { size }, files);

        public Font(string name, List<byte> sizes, FontFile file) => Init(name, sizes, new List<FontFile> { file });
        public Font(string name, List<byte> sizes, List<FontFile> files) => Init(name, sizes, files);

        private void Init(string name, List<byte> sizes, List<FontFile> files)
        {
            pointers = new();
            this.sizes = ValidateSizes(sizes);
            Files = ValidateFiles(files);
            Name = name;
        }

        public ImFontPtr GetPointer(byte fontSize = 0)
        {
            if (fontSize == 0)
            {
                fontSize = Sizes[0];
            }

            if (pointers.ContainsKey(fontSize))
            {
                return pointers[fontSize];
            }

            return ImGui.GetIO().FontDefault;
        }

        public void AddSize(byte size)
        {
            if (!sizes.Contains(size))
            {
                sizes.Add(size);
            }
        }

        public void Clear()
        {
            pointers.Clear();
        }

        private IReadOnlyList<FontFile> ValidateFiles(List<FontFile> files)
        {
            if (files == null)
            {
                return new List<FontFile>();
            }

            if (files.Count == 0)
            {
                return files;
            }

            List<FontFile> ret = new List<FontFile>();

            foreach (FontFile file in files)
            {
                if (file.IsValid)
                {
                    ret.Add(file);
                }
                else
                {
                    Console.WriteLine($"Font '{Name}', file {file.Path} is invalid and will not be loaded.");
                }
            }

            return ret;
        }
        private List<byte> ValidateSizes(List<byte> sizes)
        {
            if (sizes == null || sizes.Count == 0)
            {
                return new List<byte> { 13 };
            }

            List<byte> ret = new List<byte>();
            foreach (byte size in sizes)
            {
                if (!ret.Contains(size))
                {
                    ret.Add(size);
                }
            }

            return ret;
        }

        private int DigitCount(int n)
        {
            return n == 0 ? 1 : (n > 0 ? 1 : 2) + (int)Math.Log10(Math.Abs((double)n));
        }

        internal void Build()
        {
            Clear();

            foreach (byte size in Sizes)
            {
                pointers.Add(size, Create(size));
            }
        }

        private unsafe ImFontPtr Create(byte fontSize)
        {
            ImGuiFontBuilder builder = new();
            if (Files[0].Ranges != null)
                builder.AddFontFromMemoryTTF(Files[0].data, fontSize, [Files[0].Ranges[0].Min, Files[0].Ranges[0].Max]);
            else
                builder.AddFontFromMemoryTTF(Files[0].data, fontSize, new GlyphRanges());
                
            return builder.Build();
        }
        private unsafe ImFontConfigPtr CreateImFontConfigPtr(byte fontSize, bool mergeMode)
        {
            ImFontConfigPtr configPtr = ImGui.ImFontConfig().Handle;
            configPtr.OversampleH = 1;
            configPtr.OversampleV = 1;
            configPtr.PixelSnapH = true;
            configPtr.RasterizerMultiply = 1f;
            configPtr.MergeMode = mergeMode;
            configPtr.SizePixels = fontSize;
            return configPtr;
        }

    }

    public struct GlyphRange
    {
        public ushort Min;
        public ushort Max;

        public GlyphRange(ushort min, ushort max)
        {
            Min = min;
            Max = max;
        }
    }
}
