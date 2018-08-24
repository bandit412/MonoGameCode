/**
 * 
 *  File:       ForceArrow.cs
 *  Author:     Allan Anderson
 *  Date:       August 4, 2018
 *  Purpose:    To Indicate the magnitude and direction of force vectors
 *  
 **/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AngleForces
{
    internal class ForceArrow
    {
        #region Data Members
        protected float magnitude;
        protected float direction;
        protected Wagon.ForceName name;
        protected Texture2D barTexture;
        protected Vector2 barDimensions = new Vector2(1, 10);
        protected SpriteFont courierNew;
        protected Color color;
        #endregion

        #region Public Properties
        public float Magnitude
        {
            get { return magnitude; }
            set { magnitude = value; }
        }

        public float Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        #endregion

        #region "Constructor"
        public ForceArrow(Wagon.ForceName name, float magnitude, float direction, Color color)
        {
            this.name = name;
            this.magnitude = magnitude;
            this.direction = direction;
            this.color = color;
        }
        #endregion

        #region Internal Methods
        internal void LoadContent(ContentManager content)
        {
            courierNew = content.Load<SpriteFont>("CourierNew");
            barTexture = content.Load<Texture2D>("barWhite");
        }//eom

        internal void Update(GameTime gameTime)
        {

        }//eom

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float radians = Math.Abs((float)MathHelper.ToRadians(direction));
            if (magnitude > 0)
            {
                for (int m = 1; m <= magnitude; m++)
                {
                    switch (name)
                    {
                        case Wagon.ForceName.Applied:
                        case Wagon.ForceName.Gravity:
                        case Wagon.ForceName.Normal:
                        case Wagon.ForceName.Friction:
                            spriteBatch.Draw(
                            barTexture,
                            new Rectangle(
                                (int)(Wagon.CenterOfMass.X + m * (barDimensions.X * Math.Cos(radians))),
                                (int)(Wagon.CenterOfMass.Y - m * Math.Sin(radians)),
                                (int)barDimensions.X,
                                (int)barDimensions.Y),
                            null,
                            this.color,
                            radians * -1,
                            new Vector2(barDimensions.X / 2, barDimensions.Y / 2),
                            SpriteEffects.None,
                            0.0f);
                            break;
                        case Wagon.ForceName.Velocity:
                            spriteBatch.Draw(
                            barTexture,
                            new Rectangle(
                               (int)(Wagon.CenterOfMass.X + Wagon.WagonDimensions.X / 2 + m * (barDimensions.X * Math.Cos(radians))),
                               (int)(Wagon.CenterOfMass.Y - Wagon.WagonDimensions.Y / 4 - m * Math.Sin(radians)),
                               (int)barDimensions.X,
                               (int)barDimensions.Y),
                            null,
                            this.color,
                            radians * -1,
                            new Vector2(barDimensions.X / 2, barDimensions.Y / 2),
                            SpriteEffects.None,
                            0.0f);
                            break;
                        case Wagon.ForceName.Acceleration:
                            spriteBatch.Draw(
                            barTexture,
                            new Rectangle(
                               (int)(Wagon.CenterOfMass.X + Wagon.WagonDimensions.X / 2 + m * (barDimensions.X * Math.Cos(radians))),
                               (int)(Wagon.CenterOfMass.Y + Wagon.WagonDimensions.Y / 3 - m * Math.Sin(radians)),
                               (int)barDimensions.X,
                               (int)barDimensions.Y),
                            null,
                            this.color,
                            radians * -1,
                            new Vector2(barDimensions.X / 2, barDimensions.Y / 2),
                            SpriteEffects.None,
                            0.0f);
                            break;
                        case Wagon.ForceName.NetForce:
                            spriteBatch.Draw(
                            barTexture,
                            new Rectangle(
                               (int)(Wagon.CenterOfMass.X + m * (barDimensions.X * Math.Cos(radians))),
                               (int)(Wagon.CenterOfMass.Y - Wagon.WagonDimensions.Y / 4 - m * Math.Sin(radians)),
                               (int)barDimensions.X,
                               (int)barDimensions.Y),
                            null,
                            this.color,
                            radians * -1,
                            new Vector2(barDimensions.X / 2, barDimensions.Y / 2),
                            SpriteEffects.None,
                            0.0f);
                            break;
                    }//end switch
                }//end for
            }//end if
        }//eom
        #endregion
    }//eoc
}//eon
