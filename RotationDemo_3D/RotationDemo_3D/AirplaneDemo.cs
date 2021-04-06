/**
 * 
 *  File:       AirplaneDemo.cs
 *  Author:     Allan Anderson
 *  Date:       August 7, 2018
 *  Modified:   April 6, 2021
 *  Purpose:    An object that will demo 3D rotations
 *  
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RotationDemo_3D
{
    public class AirplaneDemo : Game
    {
        #region Constants
        private const int WindowWidth = 1280;
        private const int WindowHeight = 720;
        private const int WindowDepth = 1280;
        #endregion

        #region Initial Settings
        GraphicsDeviceManager graphics;
        // Camera
        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;
        Matrix rotation;
        // Model
        Airplane airplane;
        float roll = 0.0f, pitch = 0.0f, yaw = 0.0f;
        #endregion

        public AirplaneDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//eom

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();
            //Setup Camera
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(-10f, 10f, -10f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               GraphicsDevice.DisplayMode.AspectRatio, 1f, 100f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);// Y up
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
            airplane = new Airplane(Vector3.Zero, Matrix.Identity);
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            airplane.LoadContent(Content);
        }//eom

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }//eom

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                // roll left
                roll += MathHelper.ToRadians(-0.5f);
            }//end if

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                // roll right
                roll += MathHelper.ToRadians(0.5f);
            }//end if

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                // pitch up
                pitch += MathHelper.ToRadians(-0.5f);
            }//end if

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                // pitch down
                pitch += MathHelper.ToRadians(0.5f);
            }//end if

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                // yaw left
                yaw += MathHelper.ToRadians(0.5f);
            }//end if

            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                // yaw right
                yaw += MathHelper.ToRadians(-0.5f);
            }//end if
            rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            airplane.Update(gameTime, rotation);
            base.Update(gameTime);
        }//eom

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AliceBlue);

            airplane.Draw(gameTime, viewMatrix, worldMatrix, projectionMatrix);

            base.Draw(gameTime);
        }//eom
    }//eoc
}//eon
