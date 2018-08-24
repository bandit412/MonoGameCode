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

namespace _3DBallDemo
{
    internal class Ball
    {
        Model model;
        #region Data Members
        private BoundingSphere sphere;
        private Vector3 location;
        private Vector3 velocity;
        private Matrix rotation;
        #endregion

        #region Public Properties
        public BoundingSphere Sphere
        {
            get { return sphere; }
            set { sphere = value; }
        }//eop

        public Vector3 Location
        {
            get { return location; }
            set { location = value; }
        }//eop

        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }//eop

        public Matrix Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }//eop
        #endregion

        #region Constructor
        public Ball(Vector3 location, Vector3 velocity)
        {
            Location = location;
            Velocity = velocity;
            Sphere = new BoundingSphere(Location, 75);
            Rotation = Matrix.Identity;
        }//eom
        #endregion

        #region Internal Methods
        internal void LoadContent(ContentManager content)
        {
            model = content.Load<Model>("BeachBall");
        }//eom

        internal void Update(GameTime gameTime)
        {
            location += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            sphere.Center = location;
            // check to see if the ball has contacted a cube face
            if (location.Y + 75 >= 180 || location.Y - 75 <= -180)
                velocity.Y *= -1;
            if (location.X + 75 >= 180 || location.X - 75 <= -180)
                velocity.X *= -1;
            if (location.Z + 75 >= 180 || location.Z - 75 <= -180)
                velocity.Z *= -1;
        }//eom

        internal void Draw(GameTime gameTime, Matrix viewMatrix, Matrix worldMatrix, Matrix projectionMatrix)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.View = viewMatrix;
                    effect.View = Matrix.CreateLookAt(new Vector3(0f, 0f, -360f), Location, Vector3.Up);
                    effect.World = worldMatrix;
                    effect.Projection = projectionMatrix;
                    mesh.Draw();
                }//end foreach

            }//end foreach

        }//eom
        #endregion

        #region Other Methods
        
        #endregion
    }//eoc
}//eon
