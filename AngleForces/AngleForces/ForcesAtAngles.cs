/**
 * 
 *  File:       ForcesAtAngles.cs
 *  Author:     Allan Anderson
 *  Date:       August 4, 208
 *  Purpose:    To visually demonstrate forces applied in 2D
 *  
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AngleForces
{
    public class ForcesAtAngles : Game
    {
        #region Constants
        private const int WINDOW_WIDTH = 1024;
        private const int WINDOW_HEIGHT = 700;
        private const int WINDOW_MARGIN = 30;
        private const int BOTTOM_BORDER = 647;
        private const int INSTRUCTION_SPOT = 590;
        // string messages
        private const string GAME_OVER = "Game Over";
        private const string RESET_QUIT = "Press R to reset or Q to quit";
        private int GAME_OVER_LENGTH = GAME_OVER.Length;
        private int RESET_QUIT_LENGTH = RESET_QUIT.Length;
        #endregion

        #region Enums
        protected enum GameState
        {
            Initialize,
            Playing,
            Paused,
            GameOver,
            Quit
        }//end enum
        #endregion

        #region Data Members
        // required for game
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont courierNew;
        Texture2D background;
        Rectangle gameBoundingBox;
        float handleAngle = 0.0f;
        float forceMagnitude = 0.0f;
        float mass = 100.0f;
        float frictionCoefficient = 0.01f;
        GameState gameState;

        // external references
        internal Wagon wagon;
        #endregion

        #region Constructor
        public ForcesAtAngles()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//eom
        #endregion

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.ApplyChanges();
            gameBoundingBox = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
            gameState = GameState.Initialize;
            wagon = new Wagon(
                new Vector2((WINDOW_WIDTH - Wagon.HandleDimensions.X - Wagon.WagonDimensions.X) / 8, BOTTOM_BORDER - Wagon.WagonDimensions.Y),
                handleAngle,
                forceMagnitude,
                mass,
                frictionCoefficient,
                gameBoundingBox);
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            background = Content.Load<Texture2D>("background");

            // load external content
            wagon.LoadContent(Content);
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
                    // initialize the game
                    // change the mass
                    if ((currentKeys & InputKeyManager.Triggers.UpArrow) != 0)
                    {
                        wagon.Mass += 5.0f;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.DownArrow) != 0)
                    {
                        wagon.Mass -= 5.0f;
                    }//end if

                    // change the friction coefficient
                    if ((currentKeys & InputKeyManager.Triggers.LeftArrow) != 0)
                    {
                        wagon.FrictionCoefficient -= 0.01f;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.RightArrow) != 0)
                    {
                        wagon.FrictionCoefficient += 0.01f;
                    }//end if

                    // press spacebar to start the game
                    if ((currentKeys & InputKeyManager.Triggers.Fire) != 0)
                    {
                        gameState = GameState.Playing;
                    }//end if
                    break;
                case GameState.Playing:
                    // puase the game
                    if ((currentKeys & InputKeyManager.Triggers.Pause) != 0)
                    {
                        gameState = GameState.Paused;
                    }//end if

                    // reset
                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        gameState = GameState.GameOver;
                    }//end if

                    // play the game
                    // check if the up arrow was pressed
                    if ((currentKeys & InputKeyManager.Triggers.UpArrow) != 0)
                    {
                        // move the handle angle up in increments of 5 degrees to a maximum of 85 degrees
                        wagon.HandleAngle -= 5;
                    }//end if
                    // check if the down arrow was pressed
                    if ((currentKeys & InputKeyManager.Triggers.DownArrow) != 0)
                    {
                        // move the handle down in increments of 5 degreees to a minimum of 0 degrees
                        wagon.HandleAngle += 5;
                    }//end if
                    // check if the left arrow was pressed
                    if ((currentKeys & InputKeyManager.Triggers.LeftArrow) != 0)
                    {
                        // decrease the applied force in increments of 5N to a minimum of 0N
                        wagon.ForceMagnitude -= 5;
                    }//end if
                    // check if the right arrow was pressed
                    if ((currentKeys & InputKeyManager.Triggers.RightArrow) != 0)
                    {
                        // increase the applied force in increments of 5N to a maximum of 5000N
                        wagon.ForceMagnitude += 5;
                    }//end if

                    // update the wagon
                    wagon.Update(gameTime);
                    break;
                case GameState.Paused:
                    // game is paused
                    if ((currentKeys & InputKeyManager.Triggers.Pause) != 0)
                    {
                        gameState = GameState.Playing;
                    }//end if
                    break;
                case GameState.GameOver:
                    // end the game
                    if ((currentKeys & InputKeyManager.Triggers.Reset) != 0)
                    {
                        wagon.Mass = mass;
                        wagon.FrictionCoefficient = frictionCoefficient;
                        wagon.ForceMagnitude = forceMagnitude;
                        wagon.HandleAngle = handleAngle;
                        wagon.Location = new Vector2((WINDOW_WIDTH - Wagon.HandleDimensions.X - Wagon.WagonDimensions.X) / 8, BOTTOM_BORDER - Wagon.WagonDimensions.Y);
                        gameState = GameState.Initialize;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.Quit) != 0)
                    {
                        gameState = GameState.Quit;
                    }//end if
                    break;
                case GameState.Quit:
                    // close the game completely
                    this.Exit();
                    break;
            }//end switch
            base.Update(gameTime);
        }//eom

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            switch (gameState)
            {
                case GameState.Initialize:
                    spriteBatch.DrawString(courierNew, "Forces At Angles in 2D", Vector2.Zero, Color.Black);
                    spriteBatch.DrawString(courierNew, "Press Up/Down arrows to increas/decrease mass", new Vector2(WINDOW_MARGIN, INSTRUCTION_SPOT), Color.Black);
                    spriteBatch.DrawString(courierNew, "Press Left/Right arrows to decrease/increase friction coefficient", new Vector2(WINDOW_MARGIN, INSTRUCTION_SPOT + WINDOW_MARGIN), Color.Black);
                    spriteBatch.DrawString(courierNew, "Wagon Mass = " + wagon.Mass + " kg", new Vector2(WINDOW_MARGIN, WINDOW_HEIGHT - WINDOW_MARGIN), Color.Yellow);
                    spriteBatch.DrawString(courierNew, "Friction Coefficient = " + wagon.FrictionCoefficient, new Vector2(WINDOW_WIDTH / 2, WINDOW_HEIGHT - WINDOW_MARGIN), Color.Yellow);
                    break;
                case GameState.Playing:
                    wagon.Draw(gameTime, spriteBatch);
                    // display forces and values
                    spriteBatch.DrawString(courierNew, "  Handle Angle = " + wagon.HandleAngle * -1 + " degrees", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN), Color.Black);
                    spriteBatch.DrawString(courierNew, "        Weight = [" + wagon.Weight.X + ", " + wagon.Weight.Y + "]N", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 2), Color.Black);
                    spriteBatch.DrawString(courierNew, "  Force Normal = [" + wagon.ForceNormal.X + "," + wagon.ForceNormal.Y + "]N", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 3), Color.Black);
                    spriteBatch.DrawString(courierNew, "         Force = " + wagon.ForceMagnitude + "N", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 4), Color.Black);
                    spriteBatch.DrawString(courierNew, " Force Applied = [" + wagon.ForceApplied.X + "," + wagon.ForceApplied.Y + "]N", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 5), Color.Black);
                    spriteBatch.DrawString(courierNew, "  Max Friction = [" + wagon.ForceFrictionMax.X + "," + wagon.ForceFrictionMax.Y + "]N", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 6), Color.Black);
                    spriteBatch.DrawString(courierNew, "Force Friction = [" + wagon.ForceFriction.X + "," + wagon.ForceFriction.Y + "]N", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 7), Color.Black);
                    spriteBatch.DrawString(courierNew, "     Force Net = [" + wagon.ForceNet.X + "," + wagon.ForceNet.Y + "]N", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 8), Color.Black);
                    spriteBatch.DrawString(courierNew, "  Acceleration = [" + wagon.Acceleration.X + "," + wagon.Acceleration.Y + "]m/s^2", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 9), Color.Black);
                    spriteBatch.DrawString(courierNew, "      Velocity = [" + wagon.Velocity.X + "," + wagon.Velocity.Y + "]m/s", new Vector2(WINDOW_MARGIN, WINDOW_MARGIN * 10), Color.Black);
                    // display mass and friction coefficient in lower portion of window
                    spriteBatch.DrawString(courierNew, "Wagon Mass = " + wagon.Mass + " kg", new Vector2(WINDOW_MARGIN, WINDOW_HEIGHT - WINDOW_MARGIN), Color.Yellow);
                    spriteBatch.DrawString(courierNew, "Friction Coefficient = " + wagon.FrictionCoefficient, new Vector2(WINDOW_WIDTH / 2, WINDOW_HEIGHT - WINDOW_MARGIN), Color.Yellow);
                    break;
                case GameState.Paused:
                    // game is paused
                    spriteBatch.DrawString(courierNew, "Game is Paused", new Vector2(WINDOW_WIDTH / 2 - 100, WINDOW_HEIGHT / 2 - 10), Color.Black);
                    break;
                case GameState.GameOver:
                    //game is over
                    spriteBatch.DrawString(courierNew,
                        GAME_OVER,
                        new Vector2(WINDOW_WIDTH / 2 - ((GAME_OVER_LENGTH / 2) * 14), WINDOW_HEIGHT / 2 - WINDOW_MARGIN),
                        Color.Black);
                    spriteBatch.DrawString(courierNew,
                        RESET_QUIT,
                        new Vector2(WINDOW_WIDTH / 2 - ((RESET_QUIT_LENGTH / 2) * 14), WINDOW_HEIGHT / 2),
                        Color.Black);
                    break;
            }//end switch
            spriteBatch.End();
            base.Draw(gameTime);
        }//eom
    }//eoc
}//eon
