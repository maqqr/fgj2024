using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTSInMars
{
    [Flags]
    public enum PhysicsLayers
    {
        Selection = 1 << 0,
        Grid = 1 << 1,
    }
}
