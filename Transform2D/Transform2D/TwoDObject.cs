/**
 * 
 *  File:       TwoDObject.cs
 *  Author:     Allan Anderson
 *  Date:       August 13, 2018
 *  Purpose:    An object that the game will move
 *  
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Transform2D
{
    internal class TwoDObject
    {
        #region Data Members
        protected Vector2 gridCenter;
        protected List<Point> points;
        protected List<Point> drawPoints;
        protected float[,] transformMatrix = new float[3, 3];
        protected Vector2 scale;
        protected Vector2 shift;
        #endregion

        #region Public Properties
        public List<Point> Points
        {
            get { return points; }
        }//eop

        public Vector2 GridCenter
        {
            get { return gridCenter; }
            set { gridCenter = value; }
        }//eop

        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }//eop

        public Vector2 Shift
        {
            get { return shift; }
            set { shift = value; }
        }//eop
        #endregion

        #region Constructor
        public TwoDObject(Vector2 gridCenter, Vector2 shift, Vector2 scale)
        {
            GridCenter = gridCenter;
            Scale = scale;
            Shift = shift;

            // initialize/set the points of the 2D object
            points = new List<Point>
            {
                //points.Add(new Point(new Vector3(gridCenter.X + 60, gridCenter.Y, 1)));
                //points.Add(new Point(new Vector3(gridCenter.X + 60, gridCenter.Y - 60, 1)));
                //points.Add(new Point(new Vector3(gridCenter.X + 90, gridCenter.Y - 90, 1)));
                //points.Add(new Point(new Vector3(gridCenter.X + 120, gridCenter.Y - 60, 1)));
                //points.Add(new Point(new Vector3(gridCenter.X + 120, gridCenter.Y, 1)));

                // unscaled points
                new Point(new Vector3(6, 0, 1)),
                new Point(new Vector3(6, 6, 1)),
                new Point(new Vector3(9, 9, 1)),
                new Point(new Vector3(12, 6, 1)),
                new Point(new Vector3(12, 0, 1))
            };

            drawPoints = new List<Point>();
            for(int i = 0; i < points.Count; i++)
            {
                drawPoints.Add(new Point(new Vector3(
                    gridCenter.X + points[i].PointLocation.X, 
                    gridCenter.Y - points[i].PointLocation.Y, 
                    points[i].PointLocation.Z)));
            }//end for

            // setup initial transform matrix
            UpdateTransformMatrix();
        }//eom
        #endregion

        #region "Internal Methods"
        internal void LoadContent(ContentManager content)
        {
            foreach (Point point in drawPoints)
            {
                point.LoadContent(content);
            }//end foreach

        }//eom

        internal void Update(GameTime gameTime)
        {
            UpdateTransformMatrix();
            foreach (Point point in points)
            {
                point.PointLocation = TransformPoint(point.PointLocation);
                point.Update(gameTime);
            }//end foreach
            for (int i = 0; i < points.Count; i ++)
            {
                drawPoints[i].PointLocation = new Vector3(
                    gridCenter.X + points[i].PointLocation.X,
                    gridCenter.Y - points[i].PointLocation.Y,
                    points[i].PointLocation.Z);
                drawPoints[i].Update(gameTime);
            }//end for

        }//eom

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Point point in drawPoints)
            {
                point.Draw(gameTime, spriteBatch);
            }//end foreach
        }//eom
        #endregion

        #region Additional Methods
        private void UpdateTransformMatrix()
        {
            transformMatrix[0, 0] = scale.X;
            transformMatrix[0, 1] = 0;
            transformMatrix[0, 2] = shift.X;
            transformMatrix[1, 0] = 0;
            transformMatrix[1, 1] = scale.Y;
            transformMatrix[1, 2] = shift.Y;
            transformMatrix[2, 0] = 0;
            transformMatrix[2, 1] = 0;
            transformMatrix[2, 2] = 1;
        }//eom

        private Vector3 TransformPoint(Vector3 point)
        {
            Vector3 p = Vector3.Zero;
            p.X = transformMatrix[0, 0] * point.X + transformMatrix[0, 1] * point.Y + transformMatrix[0, 2] * point.Z;
            p.Y = transformMatrix[1, 0] * point.X + transformMatrix[1, 1] * point.Y + transformMatrix[1, 2] * point.Z;
            p.Z = transformMatrix[2, 0] * point.X + transformMatrix[2, 1] * point.Y + transformMatrix[2, 2] * point.Z;
            return p;
        }//eom

        #endregion
    }//eoc
}//eon
