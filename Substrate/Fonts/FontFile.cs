using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Substrate
{
    public class FontFile
    {
        public byte[] data;
        private GCHandle handle;

        public string Path { get; }
        public GlyphRange[] Ranges { get; }
        public Vector2 GlyphOffset { get; } = new Vector2(0, 0);
        public bool IsValid { get; private set; } = false;
        public FontFile(string path, GlyphRange[] ranges = null, Vector2? glyphOffset = null)
        {
            Path = path;
            Ranges = ranges;

            if (glyphOffset != null)
            {
                GlyphOffset = (Vector2)glyphOffset;
            }

            ValidateAndLoad();
        }

        public FontFile(string path, Vector2? glyphOffset = null, GlyphRange[] ranges = null)
        {
            Path = path;
            Ranges = ranges;

            if (glyphOffset != null)
            {
                GlyphOffset = (Vector2)glyphOffset;
            }

            ValidateAndLoad();
        }

        public FontFile(string path, GlyphRange range, Vector2? glyphOffset = null)
        {
            Path = path;
            Ranges = new[] { range };

            if (glyphOffset != null)
            {
                GlyphOffset = (Vector2)glyphOffset;
            }

            ValidateAndLoad();
        }

        public FontFile(string path)
        {
            Path = path;

            ValidateAndLoad();
        }

        private object key = new object();
        private void ValidateAndLoad()
        {
            if (Path == "ImGui.Default")
            {
                IsValid = true;
                return;
            }

            lock (key)
            {
                
                data    = Resources.GetEmbeddedAsset(Path) ?? File.ReadAllBytes(Path);
                handle  = GCHandle.Alloc(data, GCHandleType.Pinned);
                IsValid = true;
            }
        }

        internal uint[] GetGlyphRanges()
        {
            if (Ranges == null || Ranges.Length == 0)
            {
                return new uint[0];
            }

            int    i   = 0;
            uint[] ret = new uint[(Ranges.Length * 2) + 1];
            foreach (GlyphRange range in Ranges)
            {
                ret[i] = range.Min;
                ret[i + 1] = range.Max;
                i += 2;
            }
            return ret;
        }

        internal IntPtr GetPinnedData() => handle.AddrOfPinnedObject();
        internal int GetPinnedDataLength() => data?.Length ?? 0;
    }
}
