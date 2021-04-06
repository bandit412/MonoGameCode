/**
 *  Name:       ProjectileDemo.cs   
 *  Author:     Allan Anderson
 *  Date:       August 1, 2018
 *  Modified:   April 6, 2021
 *  Purpose:    To demonstrate projectile motion in 2D
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace ProjectileDemo
{
    public class ProjectileDemo : Game
    {
        #region Constants
        private const int WindowWidth = 1024;
        private const int WindowHeight = 768;
        private const int WindowMargin = 30;
        private const int InstructionSpot = 550;
        // string messages
        private const string GameOver = "Game Over";
        private const string ResetQuit = "Press R to redo or Q to quit";
        private int GameOverLength = GameOver.Length;
        private int ResetQuitLength = ResetQuit.Length;
        #endregion

        #region Enums
        protected enum DrawingState
        {
            Initialize,
            Drawing,
            Paused,
            Reset,
            Done
        }
        #endregion

        #region Data Members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont courierNew;
        Texture2D gridPoint;
        DrawingState drawingState;
        Rectangle gameBoundingBox;
        Vector3 pointPosition;
        // used for line drawing
        BasicEffect basicEffect;
        List<VertexPositionColor> vertices;
        // Choose values that work for you
        Vector3 initialPosition = Vector3.Zero;
        float firingAngle;
        float firingMagnitude;
        Vector3 velocity;
        Vector3 acceleration = new Vector3(0, -9.81f, 0);
        float time = 0;
        float maxHeight = 0;
        #endregion

        #region Constructor
        public ProjectileDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//eom
        #endregion

        #region Game Methods
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();
            gameBoundingBox = new Rectangle(0, 0, WindowWidth, WindowHeight);
            drawingState = DrawingState.Initialize;
            pointPosition = Vector3.Zero;
            // firing vector
            firingAngle = 0.0f;
            firingMagnitude = 0.0f;
            velocity = Vector3.Zero;
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                graphics.GraphicsDevice.Viewport.Height, 0,     // bottom, top
                0, 1);                                          // near, far plane 

            vertices = new List<VertexPositionColor>();
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            gridPoint = Content.Load<Texture2D>("point");
        }//eom

        protected override void Update(GameTime gameTime)
        {
            InputKeyManager.Triggers currentKeys = InputKeyManager.Read();

            switch (drawingState)
            {
                case DrawingState.Initialize:
                    // initialize starting values
                    vertices.Add(new VertexPositionColor(new Vector3(0, WindowHeight, 0), Color.Red));
                    vertices.Add(new VertexPositionColor(new Vector3(0, WindowHeight, 0), Color.Red));

                    if ((currentKeys & InputKeyManager.Triggers.UpArrow) != 0)
                    {
                        firingAngle += 1.0f;
                        if (firingAngle > 89)
                        {
                            firingAngle = 89;
                        }//end if
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.DownArrow) != 0)
                    {
                        firingAngle -= 1.0f;
                        if (firingAngle < 0.0f)
                        {
                            firingAngle = 0.0f;
                        }//end if
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.LeftArrow) != 0)
                    {
                        firingMagnitude -= 5.0f;
                        if (firingMagnitude < 0.0f)
                        {
                            firingMagnitude = 0.0f;
                        }//end if
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.RightArrow) != 0)
                    {
                        firingMagnitude += 5.0f;
                        if (firingMagnitude > 250)
                        {
                            firingMagnitude = 250;
                        }//end if
                    }//end if
                    // create component form of velocity
                    velocity = new Vector3((float)(firingMagnitude * Math.Cos(MathHelper.ToRadians(firingAngle))), (float)(firingMagnitude * Math.Sin(MathHelper.ToRadians(firingAngle))), 0);

                    // move the starting posiion up and down
                    if ((currentKeys & InputKeyManager.Triggers.Up) != 0)
                    {
                        initialPosition.Y += 1;
                        if (initialPosition.Y > WindowHeight)
                        {
                            initialPosition.Y = WindowHeight;
                        }//end if
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.Down) != 0)
                    {
                        initialPosition.Y -= 1;
                        if (initialPosition.Y < 0)
                        {
                            initialPosition.Y = 0;
                        }//end if
                    }//end if
                    // now set the starting pointPosition
                    pointPosition.X = initialPosition.X;
                    pointPosition.Y = WindowHeight - initialPosition.Y;

                    // press spacebar to start the game
                    if ((currentKeys & InputKeyManager.Triggers.Fire) != 0)
                    {
                        drawingState = DrawingState.Drawing;
                    }//end if
                    break;
                case DrawingState.Drawing:
                    // draw objects on the screen
                    time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    pointPosition = initialPosition + velocity * time + 0.5f * acceleration * time * time;
                    pointPosition.Y = WindowHeight - pointPosition.Y;
                    vertices.Add(new VertexPositionColor(pointPosition, Color.Red));

                    // check if point is off the screen
                    if (pointPosition.Y - WindowHeight >= 0 || pointPosition.X >= WindowWidth)
                    {
                        drawingState = DrawingState.Reset; // may need to add another state
                    }//end if
                    // reset
                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        drawingState = DrawingState.Reset;
                    }//end if
                    GetMaximumHeight();
                    break;
                case DrawingState.Paused:
                    // pause drawing on the scren
                    break;
                case DrawingState.Reset:
                    // reset/clear the drawing
                    if ((currentKeys & InputKeyManager.Triggers.Reset) != 0)
                    {
                        time = 0.0f;
                        firingAngle = 0.0f;
                        firingMagnitude = 0.0f;
                        maxHeight = 0.0f;
                        velocity = Vector3.Zero;
                        pointPosition = Vector3.Zero;
                        initialPosition = Vector3.Zero;
                        vertices.Clear();
                        drawingState = DrawingState.Initialize;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.Quit) != 0)
                    {
                        drawingState = DrawingState.Done;
                    }//end if
                    break;
                case DrawingState.Done:
                    // finished - exit
                    this.Exit();
                    break;
            }//end switch
            base.Update(gameTime);
        }//eom

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AliceBlue);
            spriteBatch.Begin();
            switch (drawingState)
            {
                case DrawingState.Initialize:
                    // initial game startup
                    spriteBatch.DrawString(courierNew, "Projectile Demo", Vector2.Zero, Color.Black);
                    spriteBatch.DrawString(courierNew, "Press Up/Down arrows to increase/decrease firing angle", new Vector2(WindowMargin, InstructionSpot), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press Left/Right arrows to decrease/increase firing velocity", new Vector2(WindowMargin, InstructionSpot + WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press U to move starting position up", new Vector2(WindowMargin, InstructionSpot + WindowMargin * 2), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press D to move starting position down", new Vector2(WindowMargin, InstructionSpot + WindowMargin * 3), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Velocity Vector: " + firingMagnitude + "@" + firingAngle + " = [" + velocity.X + "  " + velocity.Y + "]",
                        new Vector2(WindowMargin, WindowHeight - WindowMargin * 2), Color.Red);
                    spriteBatch.DrawString(courierNew, "Starting position: (" + initialPosition.X + "," + (initialPosition.Y) + ")",
                        new Vector2(WindowMargin, WindowHeight - WindowMargin), Color.Red);
                    spriteBatch.Draw(gridPoint, new Vector2(pointPosition.X, pointPosition.Y), Color.White);
                    break;
                case DrawingState.Drawing:
                    // drawing
                    spriteBatch.DrawString(courierNew, "Projectile Location = (" + pointPosition.X + "," + (WindowHeight - pointPosition.Y) + ")", Vector2.Zero, Color.Black);
                    spriteBatch.DrawString(courierNew, "         Max Height = " + maxHeight, new Vector2(0, 30), Color.Black);
                    spriteBatch.DrawString(courierNew, "               Time = " + time + " seconds", new Vector2(0, 60), Color.Black);
                    spriteBatch.Draw(gridPoint, new Vector2(pointPosition.X, pointPosition.Y), Color.White);
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices.ToArray(), 0, vertices.Count - 1);
                    break;
                case DrawingState.Paused:
                    // game is paused
                    spriteBatch.DrawString(courierNew, "Game is Paused", new Vector2(WindowWidth / 2 - 100, WindowHeight / 2 - 10), Color.Blue);
                    break;
                case DrawingState.Reset:
                    spriteBatch.DrawString(courierNew, "Projectile Location = (" + pointPosition.X + "," + (WindowHeight - pointPosition.Y) + ")", Vector2.Zero, Color.Black);
                    spriteBatch.DrawString(courierNew, "         Max Height = " + maxHeight, new Vector2(0, 30), Color.Black);
                    spriteBatch.DrawString(courierNew, "               Time = " + time + " seconds", new Vector2(0, 60), Color.Black);
                    // setup for reset
                    spriteBatch.DrawString(courierNew,
                        ResetQuit,
                        new Vector2(WindowWidth / 2 - ((ResetQuitLength / 2) * 14), WindowHeight / 2),
                        Color.Black);
                    break;
                case DrawingState.Done:
                    // finished
                    break;
            }//end switch
            spriteBatch.End();
            base.Draw(gameTime);
        }//eom
        #endregion

        #region Other Methods
        private void GetMaximumHeight()
        {
            if (WindowHeight - pointPosition.Y > maxHeight)
            {
                maxHeight = WindowHeight - pointPosition.Y;
            }//end if
        }//eom
        #endregion
    }//eoc
}//eon
