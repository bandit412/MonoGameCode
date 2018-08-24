/**
 * 
 *  File:       Ball.cs
 *  Author:     Allan Anderson
 *  Date:       August 7, 2018
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

namespace CollisionDemo
{
    internal class Ball
    {
        #region Data Members
        protected Texture2D redBallTexture;
        SpriteFont courierNew;
        protected static Vector2 ballDimensions = new Vector2(100, 100);
        protected Vector3 ballLocation;
        protected Vector3 ballVelocity;
        protected Rectangle gameBoundingBox;
        BoundingSphere boundingSphere;
        protected Ball[] balls;
        protected int ballID;
        protected int zLayer;
        protected int mass;
        #endregion

        #region Public Properties
        public static Vector2 BallDimensions
        {
            get { return ballDimensions; }
        }//eop

        public Vector3 BallLocation
        {
            get { return ballLocation; }
        }//eop

        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }//eop
        #endregion

        #region Constructor
        public Ball(int ballID, Vector3 ballLocation, Vector3 ballVelocity, Rectangle gameBoundingBox, int mass, Ball[] balls)
        {
            this.ballID = ballID;
            this.mass = mass;
            if (ballLocation.Z == 1)
            {
                this.zLayer = 1;
            }//end if
            else
            {
                this.zLayer = 0;
            }//end else
            this.ballLocation = ballLocation;
            this.ballVelocity = ballVelocity;
            this.gameBoundingBox = gameBoundingBox;
            this.balls = balls;

            // set bounding sphere
            boundingSphere = new BoundingSphere(new Vector3(ballLocation.X + ballDimensions.X / 2, ballLocation.Y + ballDimensions.Y / 2, zLayer), (ballDimensions.X / 2));
        }//eom
        #endregion

        #region Internal Methods
        internal void LoadContent(ContentManager content)
        {
            redBallTexture = content.Load<Texture2D>("RedBall");
            courierNew = content.Load<SpriteFont>("CourierNew");
        }//eom

        internal void Update(GameTime gameTime)
        {
            ballLocation += ballVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            // check if ball location is outside the bounding box
            boundingSphere.Center = new Vector3(ballLocation.X + ballDimensions.X / 2, ballLocation.Y + ballDimensions.Y / 2, zLayer);

            if (ballLocation.Y <= gameBoundingBox.Top || ballLocation.Y + ballDimensions.Y >= gameBoundingBox.Bottom)
            {
                ballVelocity.Y *= -1;
            }//end if
            if (ballLocation.X <= gameBoundingBox.Left || ballLocation.X + ballDimensions.X >= gameBoundingBox.Right)
            {
                ballVelocity.X *= -1;
            }//end if

            ProcessCollisionWithBall();
            ballLocation += ballVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }//eom

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            string ballV = Math.Round(ballVelocity.X, 1) + "\n" + Math.Round(ballVelocity.Y * -1, 1);
            string ballM = "m = " + mass;
            spriteBatch.Draw(redBallTexture, new Vector2(ballLocation.X, ballLocation.Y), Color.White);
            spriteBatch.DrawString(courierNew, ballM, new Vector2(boundingSphere.Center.X - 20, boundingSphere.Center.Y - 30), Color.White);
            spriteBatch.DrawString(courierNew, ballV, new Vector2(boundingSphere.Center.X - 25, boundingSphere.Center.Y - 10), Color.Yellow);
        }//eom
        #endregion

        #region Additional Methods
        internal void ProcessCollisionWithBall()
        {
            foreach (Ball b in balls)
            {
                if (CircelCollision(this, b))
                {
                    Tuple<Vector3, Vector3> finalVelocities = FinalVelocity(this, b);
                    this.ballVelocity = finalVelocities.Item1;
                    b.ballVelocity = finalVelocities.Item2;
                }//end if
            }//end foreach
        }//eom

        internal Tuple<Vector3, Vector3> FinalVelocity(Ball a, Ball b)
        {
            Vector3 n = (a.ballLocation - b.ballLocation);
            n.Normalize();
            float a1 = Vector3.Dot(a.ballVelocity, n);
            float a2 = Vector3.Dot(b.ballVelocity, n);
            float opt = 2 * (a1 - a2) / (a.mass + b.mass);
            Vector3 aVelocity = a.ballVelocity - Vector3.Multiply(n, opt * b.mass);
            Vector3 bVelocity = b.ballVelocity + Vector3.Multiply(n, opt * a.mass);
            return new Tuple<Vector3, Vector3>(aVelocity, bVelocity);
        }//eom

        internal bool CircelCollision(Ball a, Ball b)
        {
            if (a.ballID != b.ballID)
            {
                Vector2 n = new Vector2(a.ballLocation.X - b.ballLocation.X, a.ballLocation.Y - b.ballLocation.Y);
                if (n.Length() <= ballDimensions.X)
                {
                    return true;
                }//end if
                else
                {
                    return false;
                }//end else
            }//end if
            else
            {
                return false;
            }//end else
        }//eom
        #endregion
    }//eoc
}//eon
