using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis
{
    [Flags]
    public enum PhysicsCategory : ulong
    {
        Map = 0x1,
        Player = 0x2,
        Dynamite = 0x4,
        WaterArea = 0x8,
        All = ulong.MaxValue,
    }

    [Flags]
    public enum PhysicsMask : ulong
    {
        Map = ulong.MaxValue,
        Player = PhysicsCategory.Map | PhysicsCategory.Player | PhysicsCategory.WaterArea,

        // Doesn not collide with player
        Dynamite = PhysicsCategory.Map | PhysicsCategory.Dynamite,

        // Sensor events with everything
        // A query filter should exclude PhysicsFilter.WaterArea
        WaterArea = ulong.MaxValue,

        All = ulong.MaxValue,
    }

    // Concept utility for bitwise operations
    public static class BitUtil
    {
        //public static ulong Exclude(ulong value, ulong exclude)
        //{
        //    return ;
        //}
    }
}
