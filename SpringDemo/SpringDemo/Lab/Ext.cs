using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.Xna.Framework;
#endregion

namespace SpringDemo.Lab
{
    public static class Ext
    {
        public static bool IsNan(this Vector2 vec)
        {
            return (float.IsNaN(vec.X) || float.IsNaN(vec.Y));
        }//eom

        public static float Clamp(this float source, float min, float max)
        {
            if (source < min)
                return min;
            else if (source > max)
                return max;
            else
                return source;
        }//eom

    }//eoc
}//eon
