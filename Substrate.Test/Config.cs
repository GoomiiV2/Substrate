using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substrate.Test
{
    public class Config : SubstrateConfig
    {
        public override string AppName => "Substrate_Test";
        public string TestString = "Test :>";
    }
}
