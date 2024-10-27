using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substrate.Test
{
    public static class SubApp
    {
        public static Config Config => Substrate.Get<Config>();
    }
}
