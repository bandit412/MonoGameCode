/**
 * 
 *  File:       Ball.cs
 *  Author:     Allan Anderson
 *  Date:       August 14, 2018
 *  Purpose:    An object that will display as a ball bouncing in 2D
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

namespace BouncingBallDemo
{
    internal class Ball
    {
        #region enums
        protected enum BallState
        {
            Down,
            Up,
            Stopped
        }//end enum
        #endregion

        #region Data Members
        protected Texture2D ballTexture;
        protected static Vector2 ballDimensions = new Vector2(100, 100);
        protected Vector3 Gravity = new Vector3(0, -9.81f, 0);
        BoundingSphere boundingSphere;
        protected Rectangle gameBoundingBox;
        protected Vector3 ballLocation;
        protected Vector3 ballVelocity;
        BallState ballState;
        #endregion

        #region Public Properties
        public Vector2 BallDimensions
        {
            get { return ballDimensions; }
        }//eop

        public Vector3 BallLocation
        {
            get { return ballLocation; }
            set { ballLocation = value; }
        }//eop

        public Vector3 BallVelocity
        {
            get { return ballVelocity; }
            set { ballVelocity = value; }
        }//eop

        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }//eop
        #endregion

        #region Constructor
        public Ball(Vector3 location, Vector3 velocity, Rectangle gameBoundingBox)
        {
            ballLocation = location;
            ballVelocity = velocity;
            this.gameBoundingBox = gameBoundingBox;
            boundingSphere = new BoundingSphere(new Vector3(ballLocation.X + ballDimensions.X / 2, ballLocation.Y + ballDimensions.Y / 2, 0), (ballDimensions.X / 2));
            ballState = BallState.Down;
        }//eom
        #endregion

        #region Internal Methods
        internal void LoadContent(ContentManager content)
        {
            ballTexture = content.Load<Texture2D>("RedBall");
        }//eom

        internal void Update(GameTime gameTime)
        {

            ballLocation -= ballVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds + 0.5f * Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)gameTime.ElapsedGameTime.TotalSeconds;
            boundingSphere.Center = new Vector3(ballLocation.X + ballDimensions.X / 2, ballLocation.Y + ballDimensions.Y / 2, 0);
            ballVelocity += Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
             if (ballLocation.Y >= gameBoundingBox.Bottom-ballDimensions.Y)
            {
                ballVelocity.Y *= -1;
                ballState = BallState.Up;
            }//end if

            if(ballVelocity == Vector3.Zero && ballState == BallState.Down)
            {
                ballState = BallState.Stopped;
            }//end if
            else
            {
                ballState = BallState.Down;
            }//end else
            ballVelocity += Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }//eom

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ballTexture, new Vector2(ballLocation.X, ballLocation.Y), Color.White);
        }//eom
        #endregion

        #region Additional Methods

        #endregion
    }//eoc
}//eon
