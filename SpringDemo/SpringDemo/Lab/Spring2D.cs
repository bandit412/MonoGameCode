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
    public class Spring2D
    {
        public float k = -1;    // Stiffness
        public float d = 40;    // Rest distance
        public float b = 0.03f; // Coefficient of damping

        public Node2D node1, node2;

        bool isString = false;  // Strings will not generate any force when compressed

        public Spring2D(Node2D node1, Node2D node2, float restDistance = -1, float stiffness = -1)
        {
            this.node1 = node1;
            this.node2 = node2;

            // If rest distance isn't defined then use the real distance between two the nodes
            if (restDistance == -1)
                d = Vector2.Distance(node1.p, node2.p);
            else
                d = restDistance;

            this.k = stiffness;
        }//eom

        public void Update()
        {
            //    F = -k(|x|-d)(x/|x|) - bv            
            float xAbs = Vector2.Distance(node1.p, node2.p);
            if (isString && xAbs < d) return;

            Vector2 F1 = -k * (xAbs - d) * (Vector2.Normalize(node2.p - node1.p) / xAbs) - b * (node1.v - node2.v);
            Vector2 F2 = -k * (xAbs - d) * (Vector2.Normalize(node1.p - node2.p) / xAbs) - b * (node2.v - node1.v);

            // Nans propagate through the nodes network, we don't want that
            if (F1.IsNan() || F2.IsNan()) return;

            // Add acceleration. Updating velocity\positions should happen after all springs are updated
            node1.a += F1 / node1.m;
            node2.a += F2 / node2.m;
        }//eom

        public float Rest
        {
            get
            {
                float xAbs = Vector2.Distance(node1.p, node2.p);
                return d / xAbs;
            }//end get
        }//eom

        public void ToggleString()
        {
            isString = !isString;
        }//eom

        public bool StringMode
        {
            get { return isString; }
            set { isString = value; }
        }//eom

        public float Stiffness
        {
            get { return k; }
            set { k = value; }
        }//eom
    }//eoc
}//eon
