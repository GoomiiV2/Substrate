using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Substrate
{
    public class Resources
    {

        // Get an embedded asset, checks for it in the current app, then the assemably thats claling this and then the base lib (Substrate)
        // This lets you override a file in Core from your app if needed
        public static byte[] GetEmbeddedAsset(string path)
        {
            var coreAss = Assembly.GetAssembly(typeof(Resources));
            var callAss = Assembly.GetCallingAssembly();
            var appAss  = Assembly.GetEntryAssembly();

            var data = GetEmbeddedAssetFromAssembly(appAss, path) ??
                GetEmbeddedAssetFromAssembly(callAss, path) ??
                GetEmbeddedAssetFromAssembly(coreAss, path);

            return data;
        }

        private static byte[] GetEmbeddedAssetFromAssembly(Assembly ass, string path)
        {
            if (ass == null)
                return null;

            var fullPath = $"{Path.GetFileNameWithoutExtension(ass.ManifestModule.Name)}.{path}";

            var stream = ass.GetManifestResourceStream(fullPath);
            if (stream != null)
            {
                using (Stream s = stream)
                {
                    byte[] ret = new byte[s.Length];
                    s.Read(ret, 0, (int)s.Length);
                    return ret;
                }
            }

            return null;
        }
    }
}
