/**
 * 
 *  File:       Wagons.cs
 *  Author:     Allan Anderson
 *  Date:       August 4, 2018
 *  Purpose:    An object that the game will move
 *  
 **/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AngleForces
{
    internal class Wagon
    {
        #region Constants
        protected Vector2 GRAVITY = new Vector2(0.0f, -9.8f);
        protected Vector2 CENTER_OF_WAGON = new Vector2(97, 36);
        protected const int HANDLE_OFFSET = 25;
        protected const int X_OFFSET = 5;
        protected const int Y_OFFSET = 12;
        #endregion

        #region Enums
        public enum ForceName
        {
            Applied,
            Gravity,
            Normal,
            Friction,
            Velocity,
            Acceleration,
            NetForce
        }
        #endregion

        #region Data Members
        protected Texture2D wagonTexture;
        protected Texture2D handleTexture;
        protected static Vector2 wagonDimensions = new Vector2(218, 114);
        protected static Vector2 handleDimensions = new Vector2(112, 25);
        protected Vector2 wagonLocation;
        protected Vector2 handleLocation;
        protected static Vector2 centerOfMass;
        protected float handleAngle;
        protected float forceMagnitude;
        protected float mass;
        protected float frictionCoefficient;
        protected Rectangle gameBoundingBox;

        // collection of ForceArrow
        protected List<ForceArrow> arrows;

        // forces in 2D
        protected Vector2 weight;
        protected Vector2 forceNormal;
        protected Vector2 forceApplied;
        protected Vector2 forceFriction;
        protected Vector2 forceFrictionMax;
        protected Vector2 forceNet;
        protected Vector2 velocity = Vector2.Zero;
        protected Vector2 acceleration = Vector2.Zero;
        #endregion

        #region Public Properties
        public static Vector2 WagonDimensions
        {
            get { return wagonDimensions; }
        }//eop

        public static Vector2 HandleDimensions
        {
            get { return handleDimensions; }
        }//eop

        public Vector2 Location
        {
            get { return wagonLocation; }
            set
            {
                wagonLocation = value;
                handleLocation = new Vector2(wagonLocation.X + wagonDimensions.X - HANDLE_OFFSET, wagonLocation.Y + 53 - handleDimensions.Y / 2);
                centerOfMass = wagonLocation + CENTER_OF_WAGON;
            }//end set
        }//eop

        public static Vector2 CenterOfMass
        {
            get { return centerOfMass; }
        }//eop

        public Vector2 Weight
        {
            get { return weight; }
        }//eop

        public Vector2 ForceNormal
        {
            get { return forceNormal; }
        }//eop

        public Vector2 ForceApplied
        {
            get { return forceApplied; }
        }//eop

        public Vector2 ForceFriction
        {
            get { return forceFriction; }
        }//eop

        public Vector2 ForceFrictionMax
        {
            get { return forceFrictionMax; }
        }//eop

        public Vector2 ForceNet
        {
            get { return forceNet; }
        }//eop

        public float Mass
        {
            get { return mass; }
            set
            {
                if (value < 10.0f)
                {
                    mass = 10.0f;
                }//end if
                else if (value > 1000.0f)
                {
                    mass = 1000.0f;
                }//end esle if
                else
                {
                    mass = value;
                }//end else
            }//end set
        }//eop

        public float FrictionCoefficient
        {
            get { return frictionCoefficient; }
            set
            {
                if (value < 0.001f)
                {
                    frictionCoefficient = 0.001f;
                }//end if
                else if (value > 0.5f)
                {
                    frictionCoefficient = 0.5f;
                }//end else if
                else
                {
                    frictionCoefficient = value;
                }//end else
            }//end set
        }//eop

        public float HandleAngle
        {
            get { return handleAngle; }
            set
            {
                if (value < -85)
                {
                    handleAngle = -85;
                }//end if
                else if (value > 0)
                {
                    handleAngle = 0;
                }//end else if
                else
                {
                    handleAngle = value;
                }//end else
            }//end set
        }//eop

        public float ForceMagnitude
        {
            get { return forceMagnitude; }
            set
            {
                if (value < 0)
                {
                    forceMagnitude = 0;
                }//end if
                else if (value > 10000)
                {
                    forceMagnitude = 10000;
                }//end else if
                else
                {
                    forceMagnitude = value;
                }//end else
            }//end set
        }//eop

        public Vector2 Velocity
        {
            get { return velocity; }
        }//eop

        public Vector2 Acceleration
        {
            get { return acceleration; }
        }//eop
        #endregion

        #region Constructor
        public Wagon(Vector2 location, float handleAngle, float forceMagnitude, float mass, float frictionCoefficient, Rectangle gameBoundingBox)
        {
            wagonLocation = location;
            centerOfMass = wagonLocation + CENTER_OF_WAGON;
            this.Mass = mass;
            this.FrictionCoefficient = frictionCoefficient;
            this.HandleAngle = handleAngle;
            this.ForceMagnitude = forceMagnitude;
            this.mass = 100.0f; // represents 100kg
            this.handleLocation = new Vector2(wagonLocation.X + wagonDimensions.X - HANDLE_OFFSET, wagonLocation.Y + 53 - handleDimensions.Y / 2);
            this.gameBoundingBox = gameBoundingBox;
            // set the forces acting on the Wagon
            CalculateForces();
            // instantiate/create arrows
            arrows = new List<ForceArrow>();
            arrows.Add(new ForceArrow(ForceName.Applied, forceMagnitude, handleAngle, Color.Orange));
            arrows.Add(new ForceArrow(ForceName.Gravity, weight.Y, 270, Color.Green));
            arrows.Add(new ForceArrow(ForceName.Normal, forceNormal.Y, 90, Color.Navy));
            arrows.Add(new ForceArrow(ForceName.Friction, forceFriction.X, 180, Color.Crimson));
            arrows.Add(new ForceArrow(ForceName.Velocity, velocity.X, 0, Color.Purple));
            arrows.Add(new ForceArrow(ForceName.Acceleration, acceleration.X, 0, Color.Gold));
            arrows.Add(new ForceArrow(ForceName.NetForce, forceNet.X, 0, Color.Blue));
        }//eom
        #endregion

        #region Internal Methods
        internal void LoadContent(ContentManager content)
        {
            wagonTexture = content.Load<Texture2D>("wagon");
            handleTexture = content.Load<Texture2D>("handle");
            foreach (ForceArrow arrow in arrows)
            {
                arrow.LoadContent(content);
            }//end foreach
        }//eom

        internal void Update(GameTime gameTime)
        {
            // TODO:
            //  1. Update handleAngle
            //  2. Update forces
            CalculateForces();
            CalculateMotion(gameTime);
            //  3. Update Force Arrows
            UpdateArrowValues();
            foreach (ForceArrow arrow in arrows)
            {
                arrow.Update(gameTime);
            }//end foreach
            //  4. Update position of wagon and handle
            wagonLocation += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            handleLocation = new Vector2(wagonLocation.X + wagonDimensions.X - HANDLE_OFFSET, wagonLocation.Y + 53 - handleDimensions.Y / 2);
            centerOfMass = wagonLocation + CENTER_OF_WAGON;
        }//eom

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(wagonTexture, wagonLocation, Color.White);
            spriteBatch.Draw(
                handleTexture,
                new Rectangle((int)handleLocation.X + X_OFFSET, (int)handleLocation.Y + Y_OFFSET, (int)handleDimensions.X, (int)handleDimensions.Y),
                null,
                Color.White,
                MathHelper.ToRadians(handleAngle),
                new Vector2(X_OFFSET, Y_OFFSET),
                SpriteEffects.None,
                0.0f);
            // draw the force arrows
            foreach (ForceArrow arrow in arrows)
            {
                arrow.Draw(gameTime, spriteBatch);
            }//end foreach
        }//eom
        #endregion

        #region Additional Methods
        private void CalculateForces()
        {
            weight = mass * GRAVITY;
            forceApplied = new Vector2((float)Math.Abs(Math.Cos(MathHelper.ToRadians(handleAngle))), (float)Math.Abs(Math.Sin(MathHelper.ToRadians(handleAngle)))) * forceMagnitude;
            forceNormal = weight * -1 - new Vector2(0, forceApplied.Y);
            forceFrictionMax = new Vector2(forceNormal.Y, 0) * frictionCoefficient * -1;
            if (Math.Abs(forceApplied.X) > Math.Abs(forceFrictionMax.X))
            {
                forceFriction = forceFrictionMax;
            }//end if
            else
            {
                forceFriction = new Vector2(forceApplied.X, 0) * -1;
            }//end else
            this.forceNet = new Vector2(forceApplied.X + forceFriction.X, 0);
        }//eom

        private void UpdateArrowValues()
        {
            // update the magnitude of all entries in List<ForceArrow>
            arrows[0].Magnitude = forceMagnitude / 10;
            arrows[0].Direction = handleAngle;
            arrows[1].Magnitude = Math.Abs(weight.Y) / 10;
            arrows[1].Direction = 270;
            arrows[2].Magnitude = forceNormal.Y / 10;
            arrows[2].Direction = 90;
            arrows[3].Magnitude = Math.Abs(forceFriction.X) / 10;
            arrows[3].Direction = 180;
            arrows[6].Magnitude = forceNet.X / 10;
            arrows[6].Direction = 0;
        }//eom

        private void CalculateMotion(GameTime gameTime)
        {
            acceleration = forceNet / mass;
            velocity = acceleration * (float)gameTime.TotalGameTime.Seconds;
            arrows[4].Magnitude = velocity.X;
            arrows[4].Direction = 0;
            arrows[5].Magnitude = acceleration.X;
            arrows[5].Direction = 0;
        }//eom
        #endregion
    }//eoc
}//eon
