/**
 * 
 *  File:       TwoDObject.cs
 *  Author:     Allan Anderson
 *  Date:       June 22, 2011
 *  Modified:   August 7, 2018
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

namespace SpinningProjectile
{
    internal class TwoDObject
    {
        #region Data Members
        protected Vector2 launchPoint;
        protected Vector2 centerOfMass;
        protected List<Point> points;
        protected float[,] rotationMatrix = new float[3, 3];
        protected float rotationAngle;
        protected float deltaX, deltaY;   // used for the change in centeOfMasss
        #endregion

        #region Properties
        public float RotationAngle
        {
            get { return rotationAngle; }
            set { rotationAngle = value; }
        }//eop

        public Vector2 CenterOfMass
        {
            get { return centerOfMass; }
            set
            {
                deltaX = value.X - centerOfMass.X;
                deltaY = value.Y - centerOfMass.Y;
                centerOfMass = value;
            }//end set
        }//eop

        public List<Point> Points
        {
            get { return points; }
        }//eop
        #endregion

        #region Constructor
        public TwoDObject(Vector2 launchPoint, float rotationAngle)
        {
            this.launchPoint = launchPoint;
            this.centerOfMass = this.launchPoint;
            this.rotationAngle = rotationAngle;

            // initialize/set the points of the 2D object
            points = new List<Point>();
            points.Add(new Point(new Vector3(launchPoint.X - 30, launchPoint.Y + 30, 1)));
            points.Add(new Point(new Vector3(launchPoint.X - 30, launchPoint.Y - 30, 1)));
            points.Add(new Point(new Vector3(launchPoint.X, launchPoint.Y - 60, 1)));
            points.Add(new Point(new Vector3(launchPoint.X + 30, launchPoint.Y - 30, 1)));
            points.Add(new Point(new Vector3(launchPoint.X + 30, launchPoint.Y + 30, 1)));

            // setup initial rotation matrix
            UpdateRotationMatrix();
        }//eom
        #endregion

        #region Internal Methods
        internal void LoadContent(ContentManager content)
        {
            foreach (Point point in points)
            {
                point.LoadContent(content);
            }//end foreach
        }//eom

        internal void Update(GameTime gameTime)
        {
            // move the center of mass along a projectile motion

            UpdateRotationMatrix();
            foreach (Point point in points)
            {
                UpdatePoint(point);
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
        private void UpdatePoint(Point p)
        {
            // this method updates the locxation of each of the point in the List<Point>
            // based on a change of the points location
            p.PointLocation = new Vector3(p.PointLocation.X + deltaX, p.PointLocation.Y + deltaY, p.PointLocation.Z);
        }//eom

        private void UpdateRotationMatrix()
        {
            float radians = MathHelper.ToRadians(rotationAngle);
            float sine = (float)Math.Sin(radians);
            float cosine = (float)Math.Cos(radians);
            rotationMatrix[0, 0] = cosine;
            rotationMatrix[0, 1] = sine * -1;
            rotationMatrix[0, 2] = -1 * centerOfMass.X * cosine + centerOfMass.Y * sine + centerOfMass.X;
            rotationMatrix[1, 0] = -1 * rotationMatrix[0, 1];
            rotationMatrix[1, 1] = rotationMatrix[0, 0];
            rotationMatrix[1, 2] = -1 * centerOfMass.X * sine - centerOfMass.Y * cosine + centerOfMass.Y;
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

        private void SetCenterOfMass()
        {
            float xValues = 0, yValues = 0;
            foreach (Point p in points)
            {
                xValues += p.PointLocation.X;
                yValues += p.PointLocation.Y;
            }//end foreach
            centerOfMass = new Vector2(xValues / points.Count, yValues / points.Count);
        }//eom
        #endregion
    }//eoc
}//eon
