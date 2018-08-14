/**
 * 
 *  File:       BouncingBall.cs
 *  Author:     Allan Anderson
 *  Date:       August 14, 2018
 *  Purpose:    An demo of a ball bouncing in 2D
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
        private const int WINDOW_WIDTH = 1280;
        private const int WINDOW_HEIGHT = 720;
        private const int TEXT_LOCATION = WINDOW_WIDTH - 250;
        private const int WINDOW_MARGIN = 50;
        private const int HUD_WIDTH = 350;
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
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.ApplyChanges();
            gameBoundingBox = new Rectangle(0, 0, WINDOW_WIDTH - HUD_WIDTH, WINDOW_HEIGHT);
            drawingState = DrawingState.Initialize;
            ball = new Ball(new Vector3((WINDOW_WIDTH - HUD_WIDTH - WINDOW_MARGIN) / 2, WINDOW_MARGIN, 0),Vector3.Zero,gameBoundingBox);
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
                    spriteBatch.DrawString(largeFont, welcomeText, new Vector2(WINDOW_WIDTH / 5 - welcomeText.Length, WINDOW_HEIGHT / 2 - 32), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press Space to continue...OR Q to quit", new Vector2(WINDOW_MARGIN, WINDOW_HEIGHT - WINDOW_MARGIN), Color.Blue);
                    break;
                case DrawingState.Drawing:
                    spriteBatch.Draw(boundary, new Vector2((float)(WINDOW_WIDTH - HUD_WIDTH), 0), Color.White);
                    spriteBatch.DrawString(courierNew, "Ball Statistics", new Vector2(WINDOW_WIDTH - HUD_WIDTH + boundary.Width * 2, 5), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Vy = " + ball.BallVelocity.Y, new Vector2(WINDOW_WIDTH - HUD_WIDTH + boundary.Width * 2, WINDOW_MARGIN), Color.Red);
                    spriteBatch.DrawString(courierNew, "Py = " + (WINDOW_HEIGHT - ball.BallLocation.Y - ball.BallDimensions.Y), new Vector2(WINDOW_WIDTH - HUD_WIDTH + boundary.Width * 2, WINDOW_MARGIN * 2), Color.Red);
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
