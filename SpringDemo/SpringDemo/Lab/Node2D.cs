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
    public class Node2D
    {
        public float m;     // Mass
        public Vector2 p;   // Position
        public Vector2 a;   // Acceleration
        public Vector2 v;   // Velocity

        public Node2D(Vector2 position, float mass)
        {
            this.m = mass;
            this.p = position;
        }//eom

        public Node2D(float posX, float posY, float mass)
            : this(new Vector2(posX, posY), mass)
        {
        }//eom

        public void Update()
        {
            // Add gravity here?

            v += a;
            p += v;

            v *= 0.97f;
            a *= 0.3f;
        }//eom

        public float Mass
        {
            get { return m; }//end get
            set
            {
                if (value <= 0) return;
                {
                    m = value;
                }//end if
              }//end set
        }//eom
    }//eoc
}//eon
