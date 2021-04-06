/**
 * 
 *     File:    Transform.cs
 *   Author:    Allan Anderson
 *     Date:    August 21, 2018
 * Modified:    April 6, 2021
 *  Purpose:    To visually demonstrate transformation of an object in 3D space
 *  
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Transform3D
{
    public class Transform : Game
    {
        #region Enums
        protected enum GameState
        {
            Initialize,
            Drawing,
            Paused,
            Reset,
            Quit
        }//end enum

        protected enum TransformToggle
        {
            Shift,
            Scale
        }//end enum
        #endregion

        #region Constants
        private const int WindowWidth = 1024;
        private const int WindowHeight = 768;
        private const int WindowMargin = 30;
        private const int InstructionSpot = 570;
        // string messages
        private const string GameOver = "Game Over";
        private const string ResetQuit = "Press R to redo or Q to quit";
        private int GameOverLength = GameOver.Length;
        private int ResetQuitLength = ResetQuit.Length;
        //Vector3 "constants"

        #endregion

        #region Initial Settings
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont courierNew;
        GameState gameState;
        // Camera
        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        // used for line drawing
        BasicEffect basicEffect;
        VertexPositionColor[] vertices;
        VertexPositionColor[] gridPoisitve;
        VertexPositionColor[] gridNegative;

        //external references
        internal ThreeDObject threeDObject;

        //transform
        float scaleX = 1;
        float scaleY = 1;
        float scaleZ = 1;
        float shiftX = 0;
        float shiftY = 0;
        float shiftZ = 0;

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
            //Setup Camera
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(100f, 100f, 100f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(60f),
                               GraphicsDevice.Viewport.AspectRatio, 1f, 200f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);// Y up
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
            gameState = GameState.Initialize;
            transformToggle = TransformToggle.Shift;
            threeDObject = new ThreeDObject(camTarget, new Vector3(shiftX, shiftY, shiftZ), new Vector3(scaleX, scaleY, scaleZ));
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            SetVertices();
            SetPoitiveGrid();
            SetNegativeGrid();
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            threeDObject.LoadContent(Content);
        }//eom

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }//eom

        protected override void Update(GameTime gameTime)
        {
            InputKeyManager.Triggers currentKeys = InputKeyManager.Read();
            switch (gameState)
            {
                case GameState.Initialize:
                    // initialize - set drawingState to Drawing
                    #region User input to change shift and scale
                    if ((currentKeys & InputKeyManager.Triggers.UpArrow) != 0)
                    {
                        if (transformToggle == TransformToggle.Shift)
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

                    if ((currentKeys & InputKeyManager.Triggers.Minus) != 0)
                    {
                        if (transformToggle == TransformToggle.Shift)
                        {
                            shiftZ -= 1.0f;
                        }//end if
                        else
                        {
                            scaleZ -= 1.0f;
                            if (scaleZ == 0)
                                scaleZ = 1;
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

                    if ((currentKeys & InputKeyManager.Triggers.Plus) != 0)
                    {
                        if (transformToggle == TransformToggle.Shift)
                        {
                            shiftZ += 1.0f;
                        }//end if
                        else
                        {
                            scaleZ += 1.0f;
                            //if (scaleZ == 0)
                            //    scaleZ = 1;
                        }//end else
                    }//end if
                    #endregion

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
                        threeDObject.Scale = new Vector3(scaleX, scaleY, scaleZ);
                        threeDObject.Shift = new Vector3(shiftX, shiftY, shiftZ);
                        gameState = GameState.Drawing;
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.Reset) != 0)
                    {
                        gameState = GameState.Reset;
                    }//end if

                    #region Move Camera
                    if ((currentKeys & InputKeyManager.Triggers.CamLeft) != 0)
                    {
                        camPosition.X -= 5.0f;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.CamRight) != 0)
                    {
                        camPosition.X += 5.0f;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.CamUp) != 0)
                    {
                        camPosition.Y += 5.0f;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.CamDown) != 0)
                    {
                        camPosition.Y -= 5.0f;
                    }//end if

                    viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
                    basicEffect.View = viewMatrix;
                    #endregion


                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        gameState = GameState.Quit;
                    }//end if
                    break;
                case GameState.Drawing:
                    // draw objects
                    // user input to transform the twoDObject
                    threeDObject.Update(gameTime);
                    SetVertices();
                    // pause
                    gameState = GameState.Initialize;
                    // reset
                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        gameState = GameState.Reset;
                    }//end if
                    break;
                case GameState.Reset:
                    shiftX = 0;
                    shiftY = 0;
                    shiftZ = 0;
                    scaleX = 1;
                    scaleY = 1;
                    scaleZ = 1;
                    threeDObject = new ThreeDObject(camTarget, new Vector3(shiftX, shiftY, shiftZ), new Vector3(scaleX, scaleY, scaleZ));
                    threeDObject.LoadContent(Content);

                    transformToggle = TransformToggle.Shift;
                    SetVertices();

                    //rest camera and view
                    camPosition = new Vector3(100f, 100f, 100f);
                    basicEffect.View = viewMatrix;

                    if ((currentKeys & InputKeyManager.Triggers.Quit) != 0)
                    {
                        gameState = GameState.Quit;
                    }//end if

                    if ((currentKeys & InputKeyManager.Triggers.Reset) != 0)
                    {
                        gameState = GameState.Initialize;
                    }//end if
                    break;
                case GameState.Quit:
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
            switch (gameState)
            {
                case GameState.Initialize:
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, gridPoisitve, 0, 3);
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, gridNegative, 0, 3);
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 15);
                    spriteBatch.DrawString(courierNew, "Shift and Scale in 3D", new Vector2(WindowMargin, 0), Color.Black);
                    spriteBatch.DrawString(courierNew, "Press Up/Down arrows to +/- y", new Vector2(WindowMargin, InstructionSpot), Color.Purple);
                    spriteBatch.DrawString(courierNew, "Press Left/Right arrows to -/+ x", new Vector2(WindowMargin, InstructionSpot + WindowMargin), Color.Purple);
                    spriteBatch.DrawString(courierNew, "Press - / +  to -/+ z", new Vector2(WindowMargin, InstructionSpot + WindowMargin * 2), Color.Purple);
                    spriteBatch.DrawString(courierNew, "Press T to toggle between shift and scale", new Vector2(WindowMargin, InstructionSpot + 3 * WindowMargin), Color.Purple);
                    spriteBatch.DrawString(courierNew, "To move camera press A/S = Left/Right and W/X = Up/Down", new Vector2(WindowMargin, InstructionSpot + 4 * WindowMargin), Color.Purple);
                    spriteBatch.DrawString(courierNew, "Press Esc to quit", new Vector2(WindowMargin, InstructionSpot + 5 * WindowMargin), Color.Purple);
                    // display transform matrix
                    spriteBatch.DrawString(courierNew, "Mode: " + transformToggle.ToString(), new Vector2(WindowMargin, 2 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "    | " + scaleX + " 0 0 " + shiftX + " |", new Vector2(WindowMargin, 3 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "T = | 0 " + scaleY + " 0 " + shiftY + " |", new Vector2(WindowMargin, 4 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "    | 0 0 " + scaleZ + " " + shiftZ + " |", new Vector2(WindowMargin, 5 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "    | 0 0 0 1 |", new Vector2(WindowMargin, 6 * WindowMargin), Color.Blue);
                    break;
                case GameState.Drawing:
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, gridPoisitve, 0, 3);
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, gridNegative, 0, 3);
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 15);
                    spriteBatch.DrawString(courierNew, "Mode: " + transformToggle.ToString(), new Vector2(WindowMargin, 2 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "    | " + scaleX + " 0 0 " + shiftX + " |", new Vector2(WindowMargin, 3 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "T = | 0 " + scaleY + " 0 " + shiftY + " |", new Vector2(WindowMargin, 4 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "    | 0 0 " + scaleZ + " " + shiftZ + " |", new Vector2(WindowMargin, 5 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "    | 0 0 0 1 |", new Vector2(WindowMargin, 6 * WindowMargin), Color.Blue);
                    break;
                case GameState.Paused:
                    // game is paused
                    spriteBatch.DrawString(courierNew, "Game is Paused", new Vector2(WindowWidth / 2 - 100, WindowHeight / 2 - 10), Color.Purple);
                    break;
                case GameState.Reset:
                    spriteBatch.DrawString(courierNew,
                        ResetQuit,
                        new Vector2(WindowWidth / 2 - ((ResetQuitLength / 2) * 14), WindowHeight / 2),
                        Color.Purple);
                    break;
                case GameState.Quit:
                    break;
            }//end switch
            spriteBatch.End();

            base.Draw(gameTime);
        }//eom
        #endregion

        #region Other Methods
        private void SetVertices()
        {
            vertices = new VertexPositionColor[threeDObject.Points.Count * 3];
            // back face
            vertices[0].Position = new Vector3(
                camTarget.X + (threeDObject.Points[0].PointLocation.X),
                camTarget.Y + (threeDObject.Points[0].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[0].PointLocation.Z));
            vertices[1].Position = new Vector3(
                camTarget.X + (threeDObject.Points[1].PointLocation.X),
                camTarget.Y + (threeDObject.Points[1].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[1].PointLocation.Z));
            vertices[2].Position = new Vector3(
                camTarget.X + (threeDObject.Points[1].PointLocation.X),
                camTarget.Y + (threeDObject.Points[1].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[1].PointLocation.Z));
            vertices[3].Position = new Vector3(
                camTarget.X + (threeDObject.Points[2].PointLocation.X),
                camTarget.Y + (threeDObject.Points[2].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[2].PointLocation.Z));
            vertices[4].Position = new Vector3(
                camTarget.X + (threeDObject.Points[2].PointLocation.X),
                camTarget.Y + (threeDObject.Points[2].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[2].PointLocation.Z));
            vertices[5].Position = new Vector3(
                camTarget.X + (threeDObject.Points[3].PointLocation.X),
                camTarget.Y + (threeDObject.Points[3].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[3].PointLocation.Z));
            vertices[6].Position = new Vector3(
                camTarget.X + (threeDObject.Points[3].PointLocation.X),
                camTarget.Y + (threeDObject.Points[3].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[3].PointLocation.Z));
            vertices[7].Position = new Vector3(
                camTarget.X + (threeDObject.Points[4].PointLocation.X),
                camTarget.Y + (threeDObject.Points[4].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[4].PointLocation.Z));
            vertices[8].Position = new Vector3(
                camTarget.X + (threeDObject.Points[4].PointLocation.X),
                camTarget.Y + (threeDObject.Points[4].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[4].PointLocation.Z));
            vertices[9].Position = new Vector3(
                camTarget.X + (threeDObject.Points[0].PointLocation.X),
                camTarget.Y + (threeDObject.Points[0].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[0].PointLocation.Z));
            // front face
            vertices[10].Position = new Vector3(
                camTarget.X + (threeDObject.Points[5].PointLocation.X),
                camTarget.Y + (threeDObject.Points[5].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[5].PointLocation.Z));
            vertices[11].Position = new Vector3(
                camTarget.X + (threeDObject.Points[6].PointLocation.X),
                camTarget.Y + (threeDObject.Points[6].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[6].PointLocation.Z));
            vertices[12].Position = new Vector3(
                camTarget.X + (threeDObject.Points[6].PointLocation.X),
                camTarget.Y + (threeDObject.Points[6].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[6].PointLocation.Z));
            vertices[13].Position = new Vector3(
                camTarget.X + (threeDObject.Points[7].PointLocation.X),
                camTarget.Y + (threeDObject.Points[7].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[7].PointLocation.Z));
            vertices[14].Position = new Vector3(
                camTarget.X + (threeDObject.Points[7].PointLocation.X),
                camTarget.Y + (threeDObject.Points[7].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[7].PointLocation.Z));
            vertices[15].Position = new Vector3(
                camTarget.X + (threeDObject.Points[8].PointLocation.X),
                camTarget.Y + (threeDObject.Points[8].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[8].PointLocation.Z));
            vertices[16].Position = new Vector3(
                camTarget.X + (threeDObject.Points[8].PointLocation.X),
                camTarget.Y + (threeDObject.Points[8].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[8].PointLocation.Z));
            vertices[17].Position = new Vector3(
                camTarget.X + (threeDObject.Points[9].PointLocation.X),
                camTarget.Y + (threeDObject.Points[9].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[9].PointLocation.Z));
            vertices[18].Position = new Vector3(
                camTarget.X + (threeDObject.Points[9].PointLocation.X),
                camTarget.Y + (threeDObject.Points[9].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[9].PointLocation.Z));
            vertices[19].Position = new Vector3(
                camTarget.X + (threeDObject.Points[5].PointLocation.X),
                camTarget.Y + (threeDObject.Points[5].PointLocation.Y),
                camTarget.Z + (threeDObject.Points[5].PointLocation.Z));
            // joining lines
            vertices[20].Position = new Vector3(
               camTarget.X + (threeDObject.Points[0].PointLocation.X),
               camTarget.Y + (threeDObject.Points[0].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[0].PointLocation.Z));
            vertices[21].Position = new Vector3(
               camTarget.X + (threeDObject.Points[5].PointLocation.X),
               camTarget.Y + (threeDObject.Points[5].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[5].PointLocation.Z));
            vertices[22].Position = new Vector3(
               camTarget.X + (threeDObject.Points[1].PointLocation.X),
               camTarget.Y + (threeDObject.Points[1].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[1].PointLocation.Z));
            vertices[23].Position = new Vector3(
               camTarget.X + (threeDObject.Points[6].PointLocation.X),
               camTarget.Y + (threeDObject.Points[6].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[6].PointLocation.Z));
            vertices[24].Position = new Vector3(
               camTarget.X + (threeDObject.Points[2].PointLocation.X),
               camTarget.Y + (threeDObject.Points[2].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[2].PointLocation.Z));
            vertices[25].Position = new Vector3(
               camTarget.X + (threeDObject.Points[7].PointLocation.X),
               camTarget.Y + (threeDObject.Points[7].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[7].PointLocation.Z));
            vertices[26].Position = new Vector3(
               camTarget.X + (threeDObject.Points[3].PointLocation.X),
               camTarget.Y + (threeDObject.Points[3].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[3].PointLocation.Z));
            vertices[27].Position = new Vector3(
               camTarget.X + (threeDObject.Points[8].PointLocation.X),
               camTarget.Y + (threeDObject.Points[8].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[8].PointLocation.Z));
            vertices[28].Position = new Vector3(
               camTarget.X + (threeDObject.Points[4].PointLocation.X),
               camTarget.Y + (threeDObject.Points[4].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[4].PointLocation.Z));
            vertices[29].Position = new Vector3(
               camTarget.X + (threeDObject.Points[9].PointLocation.X),
               camTarget.Y + (threeDObject.Points[9].PointLocation.Y),
               camTarget.Z + (threeDObject.Points[9].PointLocation.Z));
            //set colour
            for (int v = 0; v < vertices.Length; v++)
            {
                vertices[v].Color = Color.Red;
            }//end for
        }//eom

        private void SetPoitiveGrid()
        {
            gridPoisitve = new VertexPositionColor[6];
            gridPoisitve[0].Position = camTarget;
            gridPoisitve[1].Position = new Vector3(50, camTarget.Y, camTarget.Z);
            gridPoisitve[2].Position = camTarget;
            gridPoisitve[3].Position = new Vector3(camTarget.X, 50, camTarget.Z);
            gridPoisitve[4].Position = camTarget;
            gridPoisitve[5].Position = new Vector3(camTarget.X, camTarget.Y, 50);
            //set colour
            for (int v = 0; v < gridPoisitve.Length; v++)
            {
                gridPoisitve[v].Color = Color.Blue;
            }//end for
        }//eom

        private void SetNegativeGrid()
        {
            gridNegative= new VertexPositionColor[6];
            gridNegative[0].Position = camTarget;
            gridNegative[1].Position = new Vector3(-100, camTarget.Y, camTarget.Z);
            gridNegative[2].Position = camTarget;
            gridNegative[3].Position = new Vector3(camTarget.X, -100, camTarget.Z);
            gridNegative[4].Position = camTarget;
            gridNegative[5].Position = new Vector3(camTarget.X, camTarget.Y, -100);
            //set colour
            for (int v = 0; v < gridPoisitve.Length; v++)
            {
                gridNegative[v].Color = Color.LightGray;
            }//end for
        }//eom
        #endregion
    }//eoc
}//eon
;