/**
 * 
 *  File:       Test3DDemo.cs
 *  Author:     Allan Anderson
 *  Date:       August 8, 2018
 *  Purpose:    An object that will display as a ball bouncing in a 3D cube
 *  
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#region Additional Namespaces
using System;
#endregion

namespace _3DBallDemo
{
    public class BallCollision : Game
    {
        #region Constants
        private const int WINDOW_WIDTH = 1280;
        private const int WINDOW_HEIGHT = 720;
        private const int TEXT_LOCATION = WINDOW_WIDTH - 250;
        private const int MARGIN = 30;
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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont courierNew;
        SpriteFont largeFont;
        DrawingState drawingState;
        Random rnd = new Random(); // used to generate all random numbers

        // Camera
        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        // BasicEffect for rendering
        BasicEffect basicEffect;

        // Geometric Info
        VertexPositionColor[] face;
        VertexBuffer vertexBuffer;

        Ball ball;

        public BallCollision()
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
            camPosition = new Vector3(0f, 0f, -600f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);// Y up
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            //BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice)
            {
                Alpha = 1.0f,
                VertexColorEnabled = true,
                LightingEnabled = false
            };

            //Geometry - Face
            face = new VertexPositionColor[36];
            face = MakeCube();
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 36, BufferUsage.WriteOnly);
            vertexBuffer.SetData(face);

            // initialize the ball
            ball = new Ball(SetRandomLocation(), SetRandomVelocity());
            drawingState = DrawingState.Initialize;
            base.Initialize();
        }//eom

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            courierNew = Content.Load<SpriteFont>("CourierNew");
            largeFont = Content.Load<SpriteFont>("largeFont");
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
                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        drawingState = DrawingState.Done;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.Quit) != 0)
                    {
                        drawingState = DrawingState.Done;
                    }//end if
                    break;
                case DrawingState.Drawing:
                    // check for pause button
                    if ((currentKeys & InputKeyManager.Triggers.Pause) != 0)
                    {
                        drawingState = DrawingState.Paused;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        drawingState = DrawingState.Done;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.Quit) != 0)
                    {
                        drawingState = DrawingState.Done;
                    }//end if
                    // draw graphics

                    // update ball
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
                    string welcomeText = "3D Ball Collision Demo";
                    spriteBatch.DrawString(largeFont, welcomeText, new Vector2(MARGIN, MARGIN), Color.Black);
                    spriteBatch.DrawString(courierNew, "Press Space to continue...OR Q to quit", new Vector2(MARGIN, MARGIN * 4), Color.Blue);
                    break;
                case DrawingState.Drawing:
                    // Ball Statistics
                    spriteBatch.DrawString(courierNew, "Ball Statistics", new Vector2(TEXT_LOCATION, MARGIN), Color.Black);
                    string velocityX = "Vx = " + ball.Velocity.X;
                    string VelocityY = "Vy = " + ball.Velocity.Y;
                    string VelocityZ = "Vz = " + ball.Velocity.Z;
                    string positionX = "Px = " + ball.Location.X;
                    string positionY = "Py = " + ball.Location.Y;
                    string positionZ = "Pz = " + ball.Location.Z;

                    spriteBatch.DrawString(courierNew, velocityX, new Vector2(TEXT_LOCATION, MARGIN * 2), Color.Purple);
                    spriteBatch.DrawString(courierNew, VelocityY, new Vector2(TEXT_LOCATION, MARGIN * 3), Color.Purple);
                    spriteBatch.DrawString(courierNew, VelocityZ, new Vector2(TEXT_LOCATION, MARGIN * 4), Color.Purple);
                    spriteBatch.DrawString(courierNew, positionX, new Vector2(TEXT_LOCATION, MARGIN * 6), Color.Blue);
                    spriteBatch.DrawString(courierNew, positionY, new Vector2(TEXT_LOCATION, MARGIN * 7), Color.Blue);
                    spriteBatch.DrawString(courierNew, positionZ, new Vector2(TEXT_LOCATION, MARGIN * 8), Color.Blue);

                    basicEffect.Projection = projectionMatrix;
                    basicEffect.View = viewMatrix;
                    basicEffect.World = worldMatrix;

                    //GraphicsDevice.Clear(Color.CornflowerBlue);
                    GraphicsDevice.SetVertexBuffer(vertexBuffer);

                    //Turn off culling so we see both sides of our rendered triangle
                    RasterizerState rasterizerState = new RasterizerState
                    {
                        CullMode = CullMode.None
                    };
                    GraphicsDevice.RasterizerState = rasterizerState;

                    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
                    }//end foreach
                    ball.Draw(gameTime, viewMatrix, worldMatrix, projectionMatrix);
                    break;
                case DrawingState.Paused:
                    break;
                case DrawingState.Reset:
                    break;
                case DrawingState.Resume:
                    break;
                case DrawingState.Done:
                    break;
            }//end switch
            spriteBatch.End();
            base.Draw(gameTime);
        }//eom

        #region Other Methods
        private VertexPositionColor[] MakeCube()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[36];
            //Vector2 textCoords = new Vector2(0, 0);
            Vector3[] face = new Vector3[6];
            Matrix rotY90 = Matrix.CreateRotationY(-MathHelper.Pi / 2);
            Matrix rotX90 = Matrix.CreateRotationX(-MathHelper.Pi / 2);
            // Top Left
            face[0] = new Vector3(WINDOW_HEIGHT / -4, WINDOW_HEIGHT / 4, 0);
            // Bottom Left
            face[1] = new Vector3(WINDOW_HEIGHT / -4, WINDOW_HEIGHT / -4, 0);
            // Top Right
            face[2] = new Vector3(WINDOW_HEIGHT / 4, WINDOW_HEIGHT / 4, 0);
            // Bottom Left
            face[3] = new Vector3(WINDOW_HEIGHT / -4, WINDOW_HEIGHT / -4, 0);
            // Bottom Right
            face[4] = new Vector3(WINDOW_HEIGHT / 4, WINDOW_HEIGHT / -4, 0);
            // Top Right
            face[5] = new Vector3(WINDOW_HEIGHT / 4, WINDOW_HEIGHT / 4, 0);

            // Front Face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i] = new VertexPositionColor(face[i] + new Vector3(0, 0, WINDOW_HEIGHT / 4), Color.Yellow);
                vertices[i + 3] = new VertexPositionColor(face[i + 3] + new Vector3(0, 0, WINDOW_HEIGHT / 4), Color.Yellow);
            }//end for

            // Back Face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 6] = new VertexPositionColor(face[2 - i] - new Vector3(0, 0, WINDOW_HEIGHT / 4), Color.Green);
                vertices[i + 6 + 3] = new VertexPositionColor(face[5 - i] - new Vector3(0, 0, WINDOW_HEIGHT / 4), Color.Green);
            }//end for

            // Left Face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 12] = new VertexPositionColor(Vector3.Transform(face[i], rotY90) - new Vector3(WINDOW_HEIGHT / 4, 0, 0), Color.Blue);
                vertices[i + 12 + 3] = new VertexPositionColor(Vector3.Transform(face[i + 3], rotY90) - new Vector3(WINDOW_HEIGHT / 4, 0, 0), Color.Blue);
            }//end for

            // Right Face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 18] = new VertexPositionColor(Vector3.Transform(face[2 - i], rotY90) + new Vector3(WINDOW_HEIGHT / 4, 0, 0), Color.Purple);
                vertices[i + 18 + 3] = new VertexPositionColor(Vector3.Transform(face[5 - i], rotY90) + new Vector3(WINDOW_HEIGHT / 4, 0, 0), Color.Purple);
            }//end for

            // Top Face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 24] = new VertexPositionColor(Vector3.Transform(face[i], rotX90) + new Vector3(0, WINDOW_HEIGHT / 4, 0), Color.Red);
                vertices[i + 24 + 3] = new VertexPositionColor(Vector3.Transform(face[i + 3], rotX90) + new Vector3(0, WINDOW_HEIGHT / 4, 0), Color.Red);
            }//end for

            // Bottom Face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 30] = new VertexPositionColor(Vector3.Transform(face[i], rotX90) - new Vector3(0, WINDOW_HEIGHT / 4, 0), Color.Orange);
                vertices[i + 30 + 3] = new VertexPositionColor(Vector3.Transform(face[i + 3], rotX90) - new Vector3(0, WINDOW_HEIGHT / 4, 0), Color.Orange);
            }//end for

            return vertices;
        }//eom

        private Vector3 SetRandomVelocity()
        {
            Vector3 speed = new Vector3();
            speed.X = rnd.Next(-50, 50);
            speed.Y = rnd.Next(-50, 50);
            speed.Z = rnd.Next(-50, 50);
            return speed;
        }//eom

        private Vector3 SetRandomLocation()
        {
            Vector3 spot = new Vector3();
            spot.X = rnd.Next(-1 * (WINDOW_HEIGHT / 4) + 50, WINDOW_HEIGHT / 4 - 50);
            spot.Y = rnd.Next(-1 * (WINDOW_HEIGHT / 4) + 50, WINDOW_HEIGHT / 4 - 50);
            spot.Z = rnd.Next(-1 * (WINDOW_HEIGHT / 4) + 50, WINDOW_HEIGHT / 4 - 50);
            return spot;
        }//eom

        private int SetRandomMass()
        {
            return rnd.Next(1, 10);
        }//eom

        private bool BallOverlap(Ball[] a, int count)
        {
            // this method does not currently work
            bool overlap = false;
            for (int i = 0; i < count && !overlap; i++)
            {
                if (a[i].Sphere.Intersects(a[count].Sphere))
                {
                    overlap = true;
                }//end if
            }//end for
            return overlap;
        }//eom
        #endregion
    }//eoc
}//eon
