/**
 *  Name:       Collisions.cs   
 *  Author:     Allan Anderson
 *  Date:       August 7, 2018
 *  Modified:   April 6, 2021
 *  Purpose:    To demonstrate collisions in 2D
 * 
 * */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#region Additional Namespaces
using System;
#endregion

namespace CollisionDemo
{
    public class Collision : Game
    {
        #region Constants
        private const int WindowWidth = 1280;
        private const int WindowHeight = 740;
        private const int WindowMargin = 30;
        private const int HudWidth = 350;
        private const int MaxBalls = 5;
        // string messages
        private const string GameOver = "Game Over";
        private const string ResetQuit = "Press R to redo or Q to quit";
        private int GameOverLength = GameOver.Length;
        private int ResetQuitLength = ResetQuit.Length;
        #endregion

        #region "Enums"
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
        Ball[] redBalls;
        Random rnd = new Random(); // used to generate all random numbers
        #endregion

        public Collision()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//eom

        #region Game Methods
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();

            gameBoundingBox = new Rectangle(0, 0, WindowWidth - HudWidth, WindowHeight);

            // create array of  red balls
            redBalls = new Ball[MaxBalls];

            // initilize the balls
            for (int b = 0; b < MaxBalls; b++)
            {
                int x, y, velocityX, velocityY;
                if (b == 0)
                {
                    SetRandomLocation(out x, out y);
                    velocityX = SetRandomVelocity();
                    velocityY = SetRandomVelocity();
                    redBalls[b] = new Ball(b, new Vector3(x, y, 0), new Vector3(velocityX, velocityY, 0), gameBoundingBox, SetRandomMass(), redBalls);
                }//end if
                else
                {
                    do
                    {
                        SetRandomLocation(out x, out y);
                        velocityX = SetRandomVelocity();
                        velocityY = SetRandomVelocity();
                        redBalls[b] = new Ball(b, new Vector3(x, y, 0), new Vector3(velocityX, velocityY, 0), gameBoundingBox, SetRandomMass(), redBalls);
                    }
                    while (BallOverlap(redBalls, b));
                }//end else
            }//end for

            drawingState = DrawingState.Initialize;
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            largeFont = Content.Load<SpriteFont>("largeFont");
            boundary = Content.Load<Texture2D>("boundary");

            // load the content for the balls
            foreach (Ball redBall in redBalls)
            {
                redBall.LoadContent(Content);
            }//end foreach
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

                    // update each ball
                    foreach (Ball redBall in redBalls)
                    {
                        redBall.Update(gameTime);
                    }//end foreach
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
            string hudText;
            int count = 1;
            switch (drawingState)
            {
                case DrawingState.Initialize:
                    // draw starting text
                    string welcomeText = "Ball Collision Demo";
                    spriteBatch.DrawString(largeFont, welcomeText, new Vector2(WindowWidth / 5 - welcomeText.Length, WindowHeight / 2 - 32), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press Space to continue...OR Q to quit", new Vector2(WindowMargin, WindowHeight - WindowMargin), Color.Blue);
                    break;
                case DrawingState.Drawing:
                    // draw graphics
                    spriteBatch.Draw(boundary, new Vector2((float)(WindowWidth - HudWidth), 0), Color.White);
                    spriteBatch.DrawString(courierNew, "Ball statistics (r = 50)", new Vector2(WindowWidth - HudWidth + boundary.Width * 2, 5), Color.Purple);
                    foreach (Ball redBall in redBalls)
                    { 
                        hudText = "P: [" + redBall.BallLocation.X + "    " + redBall.BallLocation.Y + "]";
                        spriteBatch.DrawString(courierNew, hudText, new Vector2(WindowWidth - HudWidth + boundary.Width * 2, count * 35), Color.Purple);
                        count++;
                        redBall.Draw(gameTime, spriteBatch);
                    }//end foreach
                    break;
                case DrawingState.Paused:
                    // pause drawing
                    spriteBatch.DrawString(courierNew, "Game is Paused", new Vector2(WindowWidth / 2 - 100, WindowHeight / 2 - 10), Color.Blue);
                    spriteBatch.Draw(boundary, new Vector2((float)(WindowWidth - HudWidth), 0), Color.White);
                    spriteBatch.DrawString(courierNew, "Ball statistics (r = 50)", new Vector2(WindowWidth - HudWidth + boundary.Width * 2, 5), Color.Purple);
                    foreach (Ball redBall in redBalls)
                    {
                        hudText = "P: [" + redBall.BallLocation.X + "    " + redBall.BallLocation.Y + "]";
                        spriteBatch.DrawString(courierNew, hudText, new Vector2(WindowWidth - HudWidth + boundary.Width * 2, count * 35), Color.Purple);
                        count++;
                        redBall.Draw(gameTime, spriteBatch);
                    }//end foreach
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
        #endregion

        #region Other Methods
        private bool BallOverlap(Ball[] a, int count)
        {
            // this method does not currently work
            bool overlap = false;
            for (int i = 0; i < count && !overlap; i++)
            {
                if (a[i].BoundingSphere.Intersects(a[count].BoundingSphere))
                {
                    overlap = true;
                }//end if
            }//end for
            return overlap;
        }//eom

        // methods to set velocity and location
        private int SetRandomVelocity()
        {
            int velocity = rnd.Next(0, 100);
            if (velocity % 2 == 0)
            {
                velocity *= -1;
            }//end if
            return velocity;
        }//eom

        private void SetRandomLocation(out int x, out int y)
        {
            x = rnd.Next((int)Ball.BallDimensions.X, (int)(WindowWidth - HudWidth - Ball.BallDimensions.X));
            y = rnd.Next((int)Ball.BallDimensions.Y, (int)(WindowHeight - Ball.BallDimensions.Y));
        }//eom

        private int SetRandomMass()
        {
            return rnd.Next(1, 10);
        }//eom
        #endregion
    }//eoc
}//eon
