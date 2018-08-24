/**
 * 
 *     File:    Point.cs
 *   Author:    Allan Anderson
 *     Date:    August 21, 2018
 *  Purpose:    An object that will display as a point in space
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
    internal class Point
    {
        #region Data Members
        Texture2D pointTexture;
        protected Vector3 pointLocation;
        #endregion

        #region Public Properties
        public Vector3 PointLocation
        {
            get { return pointLocation; }
            set { pointLocation = value; }
        }//eop
        #endregion

        #region Constructor
        public Point(Vector3 pointLocation)
        {
            PointLocation = pointLocation;
        }//eom
        #endregion

        #region Internal Methods
        internal void LoadContent(ContentManager content)
        {
            pointTexture = content.Load<Texture2D>("point");
        }//eom

        internal void Update(GameTime gameTime)
        {

        }//eom

        internal void Draw(GameTime gameTime, Matrix view, Matrix world, Matrix projection)
        {

        }//eom

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }//eom
        #endregion
    }//eoc
}//eon
