/**
 * 
 *  File:       Rotation.cs
 *  Author:     Allan Anderson
 *  Date:       August 10, 2018
 *  Purpose:    Demo showing 3-axis rotations using Matrix and Quaternion
 *  
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#region Additional Namespaces
using System;
#endregion

namespace Rotation3D
{
    public class Rotation : Game
    {
        #region Constants
        private const int WINDOW_WIDTH = 1280;
        private const int WINDOW_HEIGHT = 720;
        private const int WINDOW_DEPTH = 1280;
        private const int TEXT_LOCATION = 50;
        private const int MARGIN = 30;
        #endregion

        #region Enums
        protected enum GameState
        {
            Initialize,
            Drawing,
            Quit
        }//end enum

        protected enum RotationMode
        {
            Matrix,
            Quaternion
        }//end enum
        #endregion

        #region Initial Settings
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont courierNew;
        GameState gameState;
        RotationMode rotationMode;
        // Camera
        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;
        Matrix rotation;
        Quaternion quaternion;
        // Model
        Airplane airplane;
        float roll = 0.0f, pitch = 0.0f, yaw = 0.0f;
        float rollStep, pitchStep, yawStep;
        int maxAngle, step = 1;
        #endregion

        public Rotation()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//eom

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
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
            gameState = GameState.Initialize;
            rotationMode = RotationMode.Matrix;
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            airplane.LoadContent(Content);
        }//eom

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }//eom

        protected override void Update(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Initialize:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Exit();
                    // Roll Keys
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        roll -= 1.0f;
                        if (roll <= -360)
                            roll = 0;
                    }//end if
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        roll += 1.0f;
                        if (roll >= 360)
                            roll = 0;
                    }//end if

                    // Pitch Keys
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        pitch += 1.0f;
                        if (pitch >= 360)
                            pitch = 0;
                    }//end if

                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        pitch -= 1.0f;
                        if (pitch <= -360)
                            pitch = 0;
                    }//end if

                    // Yaw Keys
                    if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
                    {
                        yaw -= 1.0f;
                        if (yaw <= -360)
                            yaw = 0;
                    }//end if
                    if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
                    {
                        yaw += 1.0f;
                        if (yaw >= 360)
                            yaw = 0;
                    }//end if

                    // Use Matrix
                    if (Keyboard.GetState().IsKeyDown(Keys.M))
                    {
                        rotationMode = RotationMode.Matrix;
                    }//eom
                    // Use Quaternion
                    if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    {
                        rotationMode = RotationMode.Quaternion;
                    }//eom

                    // ready to draw
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        maxAngle = MaxAngle(roll, pitch, yaw);
                        rollStep = roll / maxAngle;
                        pitchStep = pitch / maxAngle;
                        yawStep = yaw / maxAngle;
                        gameState = GameState.Drawing;
                    }//end if
                    break;
                case GameState.Drawing:

                    if (step <= maxAngle)
                    {
                        if (rotationMode == RotationMode.Matrix)
                        {
                            rotation = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(yawStep * -1), MathHelper.ToRadians(pitchStep * -1), MathHelper.ToRadians(rollStep));
                        }//end if
                        else
                        {
                            quaternion = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(yawStep * -1), MathHelper.ToRadians(pitchStep * -1), MathHelper.ToRadians(rollStep));
                            rotation = Matrix.CreateFromQuaternion(quaternion);
                        }//end else
                        airplane.Update(gameTime, rotation);
                        rollStep = rollStep + roll / maxAngle;
                        pitchStep = pitchStep + pitch / maxAngle;
                        yawStep = yawStep + yaw / maxAngle;
                        step++;
                    }//end if
                    break;
                case GameState.Quit:
                    this.Exit();
                    break;
            }//end switch

            base.Update(gameTime);
        }//eom

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Initialize:
                    //string welcomeText = "3D Rotation Demo";
                    //spriteBatch.DrawString(largeFont, welcomeText, new Vector2(MARGIN, MARGIN), Color.Black);
                    spriteBatch.DrawString(courierNew, "Press Space to rotate...OR Esc to quit", new Vector2(MARGIN, WINDOW_HEIGHT - MARGIN), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Rotation Parameters", new Vector2(MARGIN, MARGIN), Color.Black);
                    spriteBatch.DrawString(courierNew, " Roll = " + roll, new Vector2(MARGIN, MARGIN * 2), Color.Red);
                    spriteBatch.DrawString(courierNew, "Pitch = " + pitch, new Vector2(MARGIN, MARGIN * 3), Color.Purple);
                    spriteBatch.DrawString(courierNew, "  Yaw = " + yaw, new Vector2(MARGIN, MARGIN * 4), Color.Yellow);
                    spriteBatch.DrawString(courierNew, " Mode = " + rotationMode.ToString(), new Vector2(MARGIN, MARGIN * 5), Color.Green);
                    airplane.Draw(gameTime, viewMatrix, worldMatrix, projectionMatrix);
                    break;
                case GameState.Drawing:
                    spriteBatch.DrawString(courierNew, "Rotation Parameters", new Vector2(MARGIN, MARGIN), Color.Black);
                    spriteBatch.DrawString(courierNew, " Roll = " + roll, new Vector2(MARGIN, MARGIN * 2), Color.Red);
                    spriteBatch.DrawString(courierNew, "Pitch = " + pitch, new Vector2(MARGIN, MARGIN * 3), Color.Purple);
                    spriteBatch.DrawString(courierNew, "  Yaw = " + yaw, new Vector2(MARGIN, MARGIN * 4), Color.Yellow);
                    spriteBatch.DrawString(courierNew, " Mode = " + rotationMode.ToString(), new Vector2(MARGIN, MARGIN * 5), Color.Green);
                    airplane.Draw(gameTime, viewMatrix, worldMatrix, projectionMatrix);
                    break;
                case GameState.Quit:
                    break;
            }//end switch
            spriteBatch.End();
            base.Draw(gameTime);
        }//eom

        #region Other Methods
        private int MaxAngle(float roll, float pitch, float yaw)
        {
            float max = 0;
            roll = Math.Abs(roll);
            pitch = Math.Abs(pitch);
            yaw = Math.Abs(yaw);
            max = Math.Max(roll, pitch);
            max = Math.Max(max, yaw);
            return (int)max;
        }//eom
        #endregion
    }//eoc
}//eon
