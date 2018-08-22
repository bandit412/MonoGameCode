/**
 * 
 *     File:    ThreeDObject.cs
 *   Author:    Allan Anderson
 *     Date:    August 21, 2018
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

namespace Transform3D
{
    internal class ThreeDObject
    {
        #region Data Members
        protected Vector3 camTarget;
        protected List<Point> points;
        protected List<Point> drawPoints;
        protected Matrix transformMatrix;
        protected Vector3 scale;
        protected Vector3 shift;
        #endregion

        #region Public Properties
        public List<Point> Points
        {
            get { return points; }
        }//eop

        public Vector3 CamTarget
        {
            get { return camTarget; }
            set { camTarget = value; }
        }//eop

        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }//eop

        public Vector3 Shift
        {
            get { return shift; }
            set { shift = value; }
        }//eop
        #endregion

        #region Constructor
        public ThreeDObject(Vector3 camTarget, Vector3 shift, Vector3 scale)
        {
            CamTarget = camTarget;
            Scale = scale;
            Shift = shift;

            // initialize/set the points of the 3D object
            points = new List<Point>
            {
                // unscaled points
                new Point(new Vector3(6, 0, -5)),   //0
                new Point(new Vector3(6, 6, -5)),   //1
                new Point(new Vector3(9, 9, -5)),   //2
                new Point(new Vector3(12, 6, -5)),  //3
                new Point(new Vector3(12, 0, -5)),  //4
                new Point(new Vector3(6, 0, 5)),    //5
                new Point(new Vector3(6, 6, 5)),    //6
                new Point(new Vector3(9, 9, 5)),    //7
                new Point(new Vector3(12, 6, 5)),   //8
                new Point(new Vector3(12, 0, 5))    //9
            };

            // drawPoints
            drawPoints = new List<Point>();
            for(int i = 0; i < points.Count; i++)
            {
                drawPoints.Add(new Point(new Vector3(
                    camTarget.X + points[i].PointLocation.X,
                    camTarget.Y + points[i].PointLocation.Y,
                    camTarget.Z + points[i].PointLocation.Z)));
            }//end for

            // setup initial transform matrix
            transformMatrix = new Matrix();
            UpdateTransformMatrix();
        }//eom
        #endregion

        #region Internal Methods
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
            for (int i = 0; i < points.Count; i++)
            {
                drawPoints[i].PointLocation = new Vector3(
                    camTarget.X + points[i].PointLocation.X,
                    camTarget.Y - points[i].PointLocation.Y,
                    camTarget.Z - points[i].PointLocation.Z);
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

        #region Other Methods
        private void UpdateTransformMatrix()
        {
            transformMatrix.M11 = Scale.X;
            transformMatrix.M22 = Scale.Y;
            transformMatrix.M33 = Scale.Z;
            transformMatrix.M14 = Shift.X;
            transformMatrix.M24 = Shift.Y;
            transformMatrix.M34 = Shift.Z;
            transformMatrix.M44 = 1;
        }//eom

        private Vector3 TransformPoint(Vector3 point)
        {
            Vector3 p = Vector3.Zero;
            p.X = transformMatrix.M11 * point.X + transformMatrix.M12 * point.Y + transformMatrix.M13 * point.Z + transformMatrix.M14;
            p.Y = transformMatrix.M21 * point.X + transformMatrix.M22 * point.Y + transformMatrix.M23 * point.Z + transformMatrix.M24;
            p.Z = transformMatrix.M31 * point.X + transformMatrix.M32 * point.Y + transformMatrix.M33 * point.Z + transformMatrix.M34;
            return p;
        }//eom
        #endregion
    }//eoc
}//eom
