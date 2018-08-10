/**
 * 
 *  File:       Airplane.cs
 *  Author:     Allan Anderson
 *  Date:       August 7, 2018
 *  Purpose:    An object that will be rotated
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

namespace RotationDemo_3D
{
    internal class Airplane
    {
        Model model;

        #region Public Properties
        public Vector3 Location { get; set; }
        public Matrix Rotation { get; set; }
        #endregion

        #region Constructor
        public Airplane(Vector3 location, Matrix rotation)
        {
            Location = location;
            Rotation = rotation;
        }//eom
        #endregion

        #region Internal Methods
        internal void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("piper_pa18");
        }//eom

        internal void Update(GameTime gameTime, Matrix rotationMatrix)
        {
            Rotation = rotationMatrix;
            Location = Vector3.Transform(Location,Rotation);
        }//eom

        internal void Draw(GameTime gameTime, Matrix viewMatrix, Matrix worldMatrix, Matrix projectionMatrix)
        {
            model.Draw(worldMatrix, Rotation * viewMatrix, projectionMatrix);
        }//eom
        #endregion
    }//eoc
}//eon
