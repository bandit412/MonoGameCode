/**
 * 
 *  File:       BouncingBall.cs
 *  Author:     Allan Anderson
 *  Date:       August 14, 2018
 *  Modified:   April 6, 2021
 *  Purpose:    A demo of a ball bouncing in 2D
 *  
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BouncingBallDemo
{
    public class BouncingBall : Game
    {
        #region Constants
        private const int WindowWidth = 1280;
        private const int WindowHeight = 720;
        private const int TextLocation = WindowWidth - 250;
        private const int WindowMargin = 50;
        private const int HudWidth = 350;
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
            Resume,
            Done
        }//end enum
        #endregion

        #region Data Members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawingState drawingState;
        SpriteFont courierNew;
        SpriteFont largeFont;
        Texture2D boundary;
        Rectangle gameBoundingBox;
        
        Ball ball;
        #endregion

        public BouncingBall()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//eom

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();
            gameBoundingBox = new Rectangle(0, 0, WindowWidth - HudWidth, WindowHeight);
            drawingState = DrawingState.Initialize;
            ball = new Ball(new Vector3((WindowWidth - HudWidth - WindowMargin) / 2, WindowMargin, 0),Vector3.Zero,gameBoundingBox);
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            largeFont = Content.Load<SpriteFont>("largeFont");
            boundary = Content.Load<Texture2D>("boundary");
            ball.LoadContent(Content);
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
                    // initialize game values
                    if ((currentKeys & InputKeyManager.Triggers.Resume) != 0)
                    {
                        drawingState = DrawingState.Drawing;
                    }//end if
                    break;
                case DrawingState.Drawing:
                    // check for pause button
                    if ((currentKeys & InputKeyManager.Triggers.Pause) != 0)
                    {
                        drawingState = DrawingState.Paused;
                    }//end if
                    // draw graphics
                    ball.Update(gameTime);
                    break;
                case DrawingState.Paused:
                    // pause drawing
                    if ((currentKeys & InputKeyManager.Triggers.Resume) != 0)
                    {
                        drawingState = DrawingState.Drawing;
                    }//end if
                    break;
                case DrawingState.Reset:
                    // reset for another demo
                    break;
                case DrawingState.Resume:
                    // resume demo
                    drawingState = DrawingState.Drawing;
                    break;
                case DrawingState.Done:
                    // finished
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
                    // draw starting text
                    string welcomeText = "Dropping Ball Demo";
                    spriteBatch.DrawString(largeFont, welcomeText, new Vector2(WindowWidth / 5 - welcomeText.Length, WindowHeight / 2 - 32), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press Space to continue...OR Q to quit", new Vector2(WindowMargin, WindowHeight - WindowMargin), Color.Blue);
                    break;
                case DrawingState.Drawing:
                    spriteBatch.Draw(boundary, new Vector2((float)(WindowWidth - HudWidth), 0), Color.White);
                    spriteBatch.DrawString(courierNew, "Ball Statistics", new Vector2(WindowWidth - HudWidth + boundary.Width * 2, 5), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Vy = " + ball.BallVelocity.Y, new Vector2(WindowWidth - HudWidth + boundary.Width * 2, WindowMargin), Color.Red);
                    spriteBatch.DrawString(courierNew, "Py = " + (WindowHeight - ball.BallLocation.Y - ball.BallDimensions.Y), new Vector2(WindowWidth - HudWidth + boundary.Width * 2, WindowMargin * 2), Color.Red);
                    ball.Draw(gameTime, spriteBatch);
                    break;
                case DrawingState.Paused:
                    break;
                case DrawingState.Reset:
                    // reset for another demo
                    break;
                case DrawingState.Done:
                    // finished
                    break;
            }//end switch
            spriteBatch.End();
            base.Draw(gameTime);
        }//eom
    }//eoc
}//eon
