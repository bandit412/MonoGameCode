/**
 *  File:       Launcher.cs
 *  Author:     Allan Anderson
 *  Date:       August 7, 2018
 *  Purpose:    To demonstrate a spinning projectile in 2D space
 **/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpinningProjectile
{
    public class Launcher : Game
    {
        #region Constants
        private const int WINDOW_WIDTH = 1024;
        private const int WINDOW_HEIGHT = 768;
        private const int WINDOW_MARGIN = 30;
        private const int INSTRUCTION_SPOT = 550;
        // string messages
        private const string GAME_OVER = "Game Over";
        private const string RESET_QUIT = "Press R to redo or Q to quit";
        private int GAME_OVER_LENGTH = GAME_OVER.Length;
        private int RESET_QUIT_LENGTH = RESET_QUIT.Length;
        #endregion

        #region Enums
        protected enum DrawingState
        {
            Initialize,
            Drawing,
            Paused,
            Reset,
            Done
        }//end enum
        #endregion

        #region Data Members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont courierNew;
        Texture2D gridPoint;
        DrawingState drawingState;
        Rectangle gameBoundingBox;
        Vector3 launchPoint = new Vector3(WINDOW_MARGIN, WINDOW_MARGIN, 0);
        float firingAngle;
        float firingMagnitude;
        Vector3 pointPosition;
        Vector3 velocity;
        Vector3 acceleration = new Vector3(0, -9.81f, 0);
        float time = 0.0f;

        // used for line drawing
        BasicEffect basicEffect;
        VertexPositionColor[] vertices;
        List<VertexPositionColor> pathVertices;

        //external references
        internal TwoDObject twoDObject;
        #endregion

        #region Constructor
        public Launcher()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//eom
        #endregion

        #region Game Methods
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.ApplyChanges();
            gameBoundingBox = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
            drawingState = DrawingState.Initialize;

            // for parabolic motion
            velocity = Vector3.Zero;
            pointPosition = new Vector3(launchPoint.X, WINDOW_HEIGHT - launchPoint.Y, launchPoint.Z);
            firingAngle = 45.0f;
            firingMagnitude = 100.0f;

            twoDObject = new TwoDObject(new Vector2(pointPosition.X, pointPosition.Y), 1.0f);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                graphics.GraphicsDevice.Viewport.Height, 0,     // bottom, top
                0, 1);                                          // near, far plane 
            SetVertices();
            pathVertices = new List<VertexPositionColor>();
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            gridPoint = Content.Load<Texture2D>("point");
            twoDObject.LoadContent(Content);
        }//eom

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }//eom

        protected override void Update(GameTime gameTime)
        {
            InputKeyManager.Triggers currentKeys = InputKeyManager.Read();
            switch (drawingState)
            {
                case DrawingState.Initialize:
                    // initialize and set drawing state
                    pathVertices.Add(new VertexPositionColor(new Vector3(pointPosition.X, pointPosition.Y, 0), Color.AliceBlue));
                    //pathVertices.Add(new VertexPositionColor(new Vector3(pointPosition.X, pointPosition.Y, 0), Color.AliceBlue));

                    // change firing angle and magnitude based on keyboard input
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
                        launchPoint.Y += 1;
                        if (launchPoint.Y > WINDOW_HEIGHT)
                        {
                            launchPoint.Y = WINDOW_HEIGHT;
                        }//end if
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.Down) != 0)
                    {
                        launchPoint.Y -= 1;
                        if (launchPoint.Y < 0)
                        {
                            launchPoint.Y = 0;
                        }//end if
                    }//end if

                    // now set the starting pointPosition
                    pointPosition.X = launchPoint.X;
                    pointPosition.Y = WINDOW_HEIGHT - launchPoint.Y;

                    // press spacebar to start the game
                    if ((currentKeys & InputKeyManager.Triggers.Fire) != 0)
                    {
                        drawingState = DrawingState.Drawing;
                    }//end if
                    break;
                case DrawingState.Drawing:
                    // draw objects
                    time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    pointPosition = launchPoint + velocity * time + 0.5f * acceleration * time * time;
                    pointPosition.Y = WINDOW_HEIGHT - pointPosition.Y;
                    pathVertices.Add(new VertexPositionColor(pointPosition, Color.Red));
                    twoDObject.CenterOfMass = new Vector2(pointPosition.X, pointPosition.Y);
                    twoDObject.Update(gameTime);
                    SetVertices();

                    // check if point is off the screen
                    if (pointPosition.Y - WINDOW_HEIGHT >= 0 || pointPosition.X >= WINDOW_WIDTH)
                    {
                        drawingState = DrawingState.Reset; // may need to add another state
                    }//end if

                    // pause

                    // reset
                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        drawingState = DrawingState.Reset;
                    }//end if
                    break;
                case DrawingState.Paused:
                    // drawing is paused
                    break;
                case DrawingState.Reset:
                    // reset/clear the drawing

                    if ((currentKeys & InputKeyManager.Triggers.Reset) != 0)
                    {
                        time = 0.0f;
                        firingAngle = 45.0f;
                        firingMagnitude = 100.0f;
                        velocity = Vector3.Zero;
                        launchPoint = new Vector3(WINDOW_MARGIN, WINDOW_MARGIN, 0);
                        pointPosition = new Vector3(launchPoint.X, WINDOW_HEIGHT - launchPoint.Y, launchPoint.Z);
                        pathVertices.Clear();
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
                    // initial drawing state
                    spriteBatch.DrawString(courierNew, "Spinning Projectile in 2D", Vector2.Zero, Color.Black);
                    // initial messages
                    spriteBatch.DrawString(courierNew, "Press Up/Down arrows to increase/decrease firing angle", new Vector2(WINDOW_MARGIN, INSTRUCTION_SPOT), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press Left/Right arrows to decrease/increase firing velocity", new Vector2(WINDOW_MARGIN, INSTRUCTION_SPOT + WINDOW_MARGIN), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press U to move starting position up", new Vector2(WINDOW_MARGIN, INSTRUCTION_SPOT + WINDOW_MARGIN * 2), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press D to move starting position down", new Vector2(WINDOW_MARGIN, INSTRUCTION_SPOT + WINDOW_MARGIN * 3), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Velocity Vector: " + firingMagnitude + "@" + firingAngle + " = [" + velocity.X + "  " + velocity.Y + "]", new Vector2(WINDOW_MARGIN, WINDOW_HEIGHT - WINDOW_MARGIN * 2), Color.Red);
                    spriteBatch.DrawString(courierNew, "Starting position: (" + launchPoint.X + "," + (launchPoint.Y) + ")", new Vector2(WINDOW_MARGIN, WINDOW_HEIGHT - WINDOW_MARGIN), Color.Red);
                    spriteBatch.Draw(gridPoint, new Vector2(pointPosition.X, pointPosition.Y), Color.White);
                    break;
                case DrawingState.Drawing:
                    // draw the objects on the screen
                    spriteBatch.DrawString(courierNew, "Projectile Location = (" + pointPosition.X + "," + (WINDOW_HEIGHT - pointPosition.Y) + ")", Vector2.Zero, Color.Black);
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(gridPoint, new Vector2(pointPosition.X, pointPosition.Y), Color.White);
                    graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, pathVertices.ToArray(), 0, pathVertices.Count - 1);
                    graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 5);
                    twoDObject.Draw(gameTime, spriteBatch);
                    break;
                case DrawingState.Paused:
                    // when the game is paused
                    spriteBatch.DrawString(courierNew, "Game is Paused", new Vector2(WINDOW_WIDTH / 2 - 100, WINDOW_HEIGHT / 2 - 10), Color.Blue);
                    break;
                case DrawingState.Reset:
                    // game needs to be reset of quit
                    spriteBatch.DrawString(courierNew,
                        RESET_QUIT,
                        new Vector2(WINDOW_WIDTH / 2 - ((RESET_QUIT_LENGTH / 2) * 14), WINDOW_HEIGHT / 2),
                        Color.Black);
                    break;
                case DrawingState.Done:
                    // game over
                    break;
            }//end switch
            spriteBatch.End();

            base.Draw(gameTime);
        }//eom
        #endregion

        #region Other Methods
        private void SetVertices()
        {
            vertices = new VertexPositionColor[twoDObject.Points.Count * 2];
            vertices[0].Position = new Vector3(twoDObject.Points[0].PointLocation.X, twoDObject.Points[0].PointLocation.Y, 0);
            vertices[1].Position = new Vector3(twoDObject.Points[1].PointLocation.X, twoDObject.Points[1].PointLocation.Y, 0);
            vertices[2].Position = new Vector3(twoDObject.Points[1].PointLocation.X, twoDObject.Points[1].PointLocation.Y, 0);
            vertices[3].Position = new Vector3(twoDObject.Points[2].PointLocation.X, twoDObject.Points[2].PointLocation.Y, 0);
            vertices[4].Position = new Vector3(twoDObject.Points[2].PointLocation.X, twoDObject.Points[2].PointLocation.Y, 0);
            vertices[5].Position = new Vector3(twoDObject.Points[3].PointLocation.X, twoDObject.Points[3].PointLocation.Y, 0);
            vertices[6].Position = new Vector3(twoDObject.Points[3].PointLocation.X, twoDObject.Points[3].PointLocation.Y, 0);
            vertices[7].Position = new Vector3(twoDObject.Points[4].PointLocation.X, twoDObject.Points[4].PointLocation.Y, 0);
            vertices[8].Position = new Vector3(twoDObject.Points[4].PointLocation.X, twoDObject.Points[4].PointLocation.Y, 0);
            vertices[9].Position = new Vector3(twoDObject.Points[0].PointLocation.X, twoDObject.Points[0].PointLocation.Y, 0);
            for (int v = 0; v < vertices.Length; v++)
            {
                vertices[v].Color = Color.Blue;
            }//end for
        }//eom
        #endregion
    }//eoc
}//eon
