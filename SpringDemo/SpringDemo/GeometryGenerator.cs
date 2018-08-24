using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.Xna.Framework;
using SpringDemo.Lab;
#endregion

namespace SpringDemo
{
    static class GeometryGenerator
    {
        public static Tuple<List<Node2D>, List<Spring2D>> CreatePendulum(float x1, float y1, float m1, float x2, float y2, float m2, float restD = -1, float k = -3)
        {
            List<Node2D> nodes = new List<Node2D>();
            List<Spring2D> springs = new List<Spring2D>();

            Node2D node1 = new Node2D(new Vector2(x1, y1), m1);
            Node2D node2 = new Node2D(new Vector2(x2, y2), m2);
            nodes.Add(node1);
            nodes.Add(node2);

            Spring2D spring = new Spring2D(node1, node2, restD, k);
            springs.Add(spring);

            return new Tuple<List<Node2D>, List<Spring2D>>(nodes, springs);
        }//eom

        public static Tuple<List<Node2D>, List<Spring2D>> CreateMesh(float px, float py, int w, int h, float dx, float dy, float m, float restD1 = -1, float restD2 = -1, float k = -3)
        {
            List<Node2D> meshNodes = new List<Node2D>();
            List<Spring2D> springs = new List<Spring2D>();

            // Creating nodes
            for (int y = 1; y <= h; y++)
            {
                for (int x = 1; x <= w; x++)
                {
                    Node2D node = new Node2D(new Vector2(px + x * dx, py + y * dy), m);
                    meshNodes.Add(node);
                }//end for
            }//end for

            for (int i = 0; i < w - 1; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Spring2D spr = new Spring2D(meshNodes[i + w * j], meshNodes[i + 1 + w * j], restD1, k);
                    springs.Add(spr);
                }//end for
            }//end for

            for (int i = 0; i < h - 1; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    Spring2D spr = new Spring2D(meshNodes[i * w + j], meshNodes[(i + 1) * w + j], restD1, k);
                    springs.Add(spr);
                }//end for
            }//end for

            // Diagonally
            //      \
            for (int y = 0; y < h - 1; y++)
            {
                for (int x = 0; x < w - 1; x++)
                {
                    Spring2D spr = new Spring2D(meshNodes[x + y * w], meshNodes[x + (y + 1) * w + 1], restD2, k);
                    springs.Add(spr);
                }//end for
            }//end for

            //      /
            for (int y = 0; y < h - 1; y++)
            {
                for (int x = 1; x < w; x++)
                {
                    Spring2D spr = new Spring2D(meshNodes[x + y * w], meshNodes[x + (y + 1) * w - 1], restD2, k);
                    springs.Add(spr);
                }//end for
            }//end for

            return new Tuple<List<Node2D>, List<Spring2D>>(meshNodes, springs);
        }//eom

        public static Tuple<List<Node2D>, List<Spring2D>> CreateCircle(float cx, float cy, float r, int count, float cm, float m, float restD1 = -1, float restD2 = -1, float k = -3)
        {
            List<Node2D> circleNodes = new List<Node2D>();
            List<Spring2D> springs = new List<Spring2D>();

            Vector2 center = new Vector2(cx, cy);
            Node2D centerNode = new Node2D(center, cm);

            float step = (float)(Math.PI * 2f) / count;
            for (var i = 0; i < count; i++)
            {
                var t = i * step;
                var temp = new Node2D(cx + r * (float)Math.Sin(t), cy + r * (float)Math.Cos(t), m);
                circleNodes.Add(temp);
            }//end for

            for (int i = 0; i < circleNodes.Count - 1; i++)
            {
                Spring2D spr = new Spring2D(circleNodes[i], circleNodes[i + 1], restD2, k);
                springs.Add(spr);
            }//end for

            foreach (Node2D b in circleNodes)
            {
                Spring2D spr2 = new Spring2D(b, centerNode, restD1, k);
                springs.Add(spr2);
            }//end for

            Spring2D sprr = new Spring2D(circleNodes.First(), circleNodes.Last(), restD2, k);
            springs.Add(sprr);

            circleNodes.Add(centerNode);

            return new Tuple<List<Node2D>, List<Spring2D>>(circleNodes, springs);
        }//eom
    }//eoc
}//eon
