/**
 * 
 *  File:       TwoDObject.cs
 *  Author:     Allan Anderson
 *  Date:       August 1, 2018
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

namespace TwoDRotation
{
    internal class TwoDObject
    {
        #region Data Members
        protected Vector2 gridCenter;
        protected Vector2 rotationPoint;
        protected List<Point> points;
        protected float[,] rotationMatrix = new float[3, 3];
        protected float rotationAngle;
        protected float direction;
        #endregion

        #region Public Properties
        public float RotationAngle
        {
            get { return rotationAngle; }
            set { rotationAngle = value; }
        }//eop

        public Vector2 RotationPoint
        {
            get { return rotationPoint; }
            set { rotationPoint = value; }
        }//eop

        public List<Point> Points
        {
            get { return points; }
        }//eop

        public float Direction
        {
            get { return direction; }
            set { direction = value; }
        }//eop

        public Vector2 GridCenter
        {
            get { return gridCenter; }
            set { gridCenter = value; }
        }//eop
        #endregion

        #region Constructor
        public TwoDObject(Vector2 gridCenter, Vector2 rotationPoint, float rotationAngle, float direction)
        {
            GridCenter = gridCenter;
            RotationPoint = rotationPoint;
            RotationAngle = rotationAngle;
            Direction = direction;

            // initialize/set the points of the 2D object
            points = new List<Point>();
            points.Add(new Point(new Vector3(gridCenter.X + 60, gridCenter.Y, 1)));
            points.Add(new Point(new Vector3(gridCenter.X + 60, gridCenter.Y - 60, 1)));
            points.Add(new Point(new Vector3(gridCenter.X + 90, gridCenter.Y - 90, 1)));
            points.Add(new Point(new Vector3(gridCenter.X + 120, gridCenter.Y - 60, 1)));
            points.Add(new Point(new Vector3(gridCenter.X + 120, gridCenter.Y, 1)));

            // setup initial rotation matrix
            UpdateRotationMatrix();
        }//eom
        #endregion

        #region "Internal Methods"
        internal void LoadContent(ContentManager content)
        {
            foreach (Point point in points)
            {
                point.LoadContent(content);
            }//end foreach
        }//eom

        internal void Update(GameTime gameTime)
        {
            UpdateRotationMatrix();
            foreach (Point point in points)
            {
                point.PointLocation = RotatePoint(point.PointLocation);
                point.Update(gameTime);
            }//end foreach
        }//eom

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Point point in points)
            {
                point.Draw(gameTime, spriteBatch);
            }//end foreach
        }//eom
        #endregion

        #region Additional Methods
        private void UpdateRotationMatrix()
        {
            float radians = MathHelper.ToRadians(rotationAngle);
            float sine = (float)Math.Sin(radians) * direction;
            float cosine = (float)Math.Cos(radians);
            rotationMatrix[0, 0] = cosine;
            rotationMatrix[0, 1] = sine * -1;
            rotationMatrix[0, 2] = -1 * rotationPoint.X * cosine + rotationPoint.Y * sine + rotationPoint.X;
            rotationMatrix[1, 0] = -1 * rotationMatrix[0, 1];
            rotationMatrix[1, 1] = rotationMatrix[0, 0];
            rotationMatrix[1, 2] = -1 * rotationPoint.X * sine - rotationPoint.Y * cosine + rotationPoint.Y;
            rotationMatrix[2, 0] = 0.0f;
            rotationMatrix[2, 1] = 0.0f;
            rotationMatrix[2, 2] = 1.0f;
        }//eom

        private Vector3 RotatePoint(Vector3 point)
        {
            Vector3 p = Vector3.Zero;
            p.X = rotationMatrix[0, 0] * point.X + rotationMatrix[0, 1] * point.Y + rotationMatrix[0, 2] * point.Z;
            p.Y = rotationMatrix[1, 0] * point.X + rotationMatrix[1, 1] * point.Y + rotationMatrix[1, 2] * point.Z;
            p.Z = rotationMatrix[2, 0] * point.X + rotationMatrix[2, 1] * point.Y + rotationMatrix[2, 2] * point.Z;
            return p;
        }//eom
        #endregion
    }//eoc
}//eon
