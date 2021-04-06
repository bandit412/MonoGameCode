﻿/**
 * 
 *  File:       Transform.cs
 *  Author:     Allan Anderson
 *  Date:       August 13, 2018
 *  Modified:   April 6, 2021
 *  Purpose:    To visually demonstrate transformation of an object in 2D space
 *  
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Transform2D
{
    public class Transform : Game
    {
        #region Constants
        private const int WindowWidth = 1024;
        private const int WindowHeight = 768;
        private const int WindowMargin = 30;
        private const int InstructionSpot = 540;
        // string messages
        private const string GameOver = "Game Over";
        private const string ResetQuit = "Press R to redo or Q to quit";
        private int GameOverLength = GameOver.Length;
        private int ResetQuitLength = ResetQuit.Length;
        // vector2 "constants"
        private Vector2 Grid_Center = new Vector2(WindowWidth / 2, WindowHeight / 2);
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

        protected enum TransformToggle
        {
            Shift,
            Scale
        }//end enum
        #endregion

        #region Data Members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont courierNew;
        Texture2D graphPaper;
        Texture2D gridPoint;
        DrawingState drawingState;
        Rectangle gameBoundingBox;

        // used for line drawing
        BasicEffect basicEffect;
        VertexPositionColor[] vertices;

        //external references
        internal TwoDObject twoDObject;

        // transforms
        float scaleX = 1;
        float scaleY = 1;
        float shiftX = 0;
        float shiftY = 0;
        TransformToggle transformToggle;
        #endregion

        #region Constructor
        public Transform()
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
            transformToggle = TransformToggle.Shift;
            twoDObject = new TwoDObject(Grid_Center, Vector2.Zero, Vector2.Zero);
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                Projection = Matrix.CreateOrthographicOffCenter
                (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                graphics.GraphicsDevice.Viewport.Height, 0,     // bottom, top
                0, 1)                                          // near, far plane 
            };
            SetVertices();
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            graphPaper = Content.Load<Texture2D>("GraphBackground");
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
                    // initialize - set drawingState to Drawing
                    // user input to change shift and scale
                    if ((currentKeys & InputKeyManager.Triggers.UpArrow) != 0)
                    {
                        if(transformToggle == TransformToggle.Shift)
                        {
                            shiftY += 1.0f;
                        }//end if
                        else
                        {
                            scaleY += 1.0f;
                            if (scaleY == 0)
                                scaleY = 1;
                        }//end else
                        
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.DownArrow) != 0)
                    {
                        if (transformToggle == TransformToggle.Shift)
                        {
                            shiftY -= 1.0f;
                        }//end if
                        else
                        {
                            scaleY -= 1.0f;
                            if (scaleY == 0)
                                scaleY = 1;
                        }//end else
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.LeftArrow) != 0)
                    {
                        if (transformToggle == TransformToggle.Shift)
                        {
                            shiftX -= 1.0f;
                        }//end if
                        else
                        {
                            scaleX -= 1.0f;
                            if (scaleX == 0)
                                scaleX = 1;
                        }//end else
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.RightArrow) != 0)
                    {
                        if (transformToggle == TransformToggle.Shift)
                        {
                            shiftX += 1.0f;
                        }//end if
                        else
                        {
                            scaleX += 1.0f;
                            if (scaleX == 0)
                                scaleX = 1;
                        }//end else
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.Toggle) != 0)
                    {
                        if (transformToggle == TransformToggle.Shift)
                        {
                            transformToggle = TransformToggle.Scale;
                        }//end if
                        else
                        {
                            transformToggle = TransformToggle.Shift;
                        }//end else
                    }//end if
                    
                    // press spacebar to start the game
                    if ((currentKeys & InputKeyManager.Triggers.Fire) != 0)
                    {
                        twoDObject.Scale = new Vector2(scaleX, scaleY);
                        twoDObject.Shift = new Vector2(shiftX, shiftY);
                        drawingState = DrawingState.Drawing;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        drawingState = DrawingState.Reset;
                    }//end if
                    break;
                case DrawingState.Drawing:
                    // draw objects
                    // user input to transform the twoDObject
                    twoDObject.Update(gameTime);
                    SetVertices();
                    // pause
                    drawingState = DrawingState.Initialize;
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
                    // reset the drawing
                    if ((currentKeys & InputKeyManager.Triggers.Reset) != 0)
                    {
                        twoDObject = new TwoDObject(Grid_Center, Vector2.Zero, Vector2.Zero);
                        twoDObject.LoadContent(Content);
                        shiftX = 0;
                        shiftY = 0;
                        scaleX = 1;
                        scaleY = 1;
                        transformToggle = TransformToggle.Shift;
                        SetVertices();
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
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            switch (drawingState)
            {
                case DrawingState.Initialize:
                    spriteBatch.Draw(graphPaper, Vector2.Zero, Color.White);
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 5);
                    spriteBatch.DrawString(courierNew, "Shift and Scale in 2D", Vector2.Zero, Color.Black);
                    spriteBatch.DrawString(courierNew, "Press Up/Down arrows to +/- y", new Vector2(WindowMargin, InstructionSpot), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press Left/Right arrows to -/+ x", new Vector2(WindowMargin, InstructionSpot + WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press T to toggle between shift and scale", new Vector2(WindowMargin, InstructionSpot + 2 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Mode: " + transformToggle.ToString(), new Vector2(WindowMargin, InstructionSpot + 3 * WindowMargin), Color.Black);
                    spriteBatch.DrawString(courierNew, "    | " + scaleX + " 0 " + shiftX + " |", new Vector2(WindowMargin, InstructionSpot + 4 * WindowMargin), Color.Black);
                    spriteBatch.DrawString(courierNew, "T = | 0 " + scaleY + " " + shiftY + " |", new Vector2(WindowMargin, InstructionSpot + 5 * WindowMargin), Color.Black);
                    spriteBatch.DrawString(courierNew, "    | 0 0 1 |", new Vector2(WindowMargin, InstructionSpot + 6 * WindowMargin), Color.Black);
                    break;
                case DrawingState.Drawing:
                    spriteBatch.Draw(graphPaper, Vector2.Zero, Color.White);
                    twoDObject.Draw(gameTime, spriteBatch);
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 5);
                    spriteBatch.DrawString(courierNew, "    | " + scaleX + " 0 " + shiftX + " |", new Vector2(WindowMargin, InstructionSpot + 4 * WindowMargin), Color.Black);
                    spriteBatch.DrawString(courierNew, "T = | 0 " + scaleY + " " + shiftY + " |", new Vector2(WindowMargin, InstructionSpot + 5 * WindowMargin), Color.Black);
                    spriteBatch.DrawString(courierNew, "    | 0 0 1 |", new Vector2(WindowMargin, InstructionSpot + 6 * WindowMargin), Color.Black);
                    break;
                case DrawingState.Paused:
                    // game is paused
                    spriteBatch.DrawString(courierNew, "Game is Paused", new Vector2(WindowWidth / 2 - 100, WindowHeight / 2 - 10), Color.Blue);
                    break;
                case DrawingState.Reset:
                    spriteBatch.DrawString(courierNew,
                        ResetQuit,
                        new Vector2(WindowWidth / 2 - ((ResetQuitLength / 2) * 14), WindowHeight / 2),
                        Color.Black);
                    break;
                case DrawingState.Done:
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
            vertices[0].Position = new Vector3(Grid_Center.X + (twoDObject.Points[0].PointLocation.X), Grid_Center.Y - (twoDObject.Points[0].PointLocation.Y), 0);
            vertices[1].Position = new Vector3(Grid_Center.X + (twoDObject.Points[1].PointLocation.X), Grid_Center.Y - (twoDObject.Points[1].PointLocation.Y), 0);
            vertices[2].Position = new Vector3(Grid_Center.X + (twoDObject.Points[1].PointLocation.X), Grid_Center.Y - (twoDObject.Points[1].PointLocation.Y), 0);
            vertices[3].Position = new Vector3(Grid_Center.X + (twoDObject.Points[2].PointLocation.X), Grid_Center.Y - (twoDObject.Points[2].PointLocation.Y), 0);
            vertices[4].Position = new Vector3(Grid_Center.X + (twoDObject.Points[2].PointLocation.X), Grid_Center.Y - (twoDObject.Points[2].PointLocation.Y), 0);
            vertices[5].Position = new Vector3(Grid_Center.X + (twoDObject.Points[3].PointLocation.X), Grid_Center.Y - (twoDObject.Points[3].PointLocation.Y), 0);
            vertices[6].Position = new Vector3(Grid_Center.X + (twoDObject.Points[3].PointLocation.X), Grid_Center.Y - (twoDObject.Points[3].PointLocation.Y), 0);
            vertices[7].Position = new Vector3(Grid_Center.X + (twoDObject.Points[4].PointLocation.X), Grid_Center.Y - (twoDObject.Points[4].PointLocation.Y), 0);
            vertices[8].Position = new Vector3(Grid_Center.X + (twoDObject.Points[4].PointLocation.X), Grid_Center.Y - (twoDObject.Points[4].PointLocation.Y), 0);
            vertices[9].Position = new Vector3(Grid_Center.X + (twoDObject.Points[0].PointLocation.X), Grid_Center.Y - (twoDObject.Points[0].PointLocation.Y), 0);
            for (int v = 0; v < vertices.Length; v++)
            {
                vertices[v].Color = Color.Red;
            }//end for
        }//eom
        #endregion
    }//eoc
}//eon
