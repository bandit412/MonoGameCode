/**
 * 
 *  File:       Two_D_Rotation.cs
 *  Author:     Allan Anderson
 *  Date:       August 1, 2018
 *  Purpose:    To visually demonstrate rotation of an object in 2D space
 *  
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace TwoDRotation
{
    public class Two_D_Rotation : Game
    {
        #region Constants
        private const int WindowWidth = 1024;
        private const int WindowHeight = 768;
        private const int WindowMargin = 30;
        private const int InstructionSpot = 590;
        // string messages
        private const string GameOver = "Game Over";
        private const string ResetQuit = "Press R to redo or Q to quit";
        private int GameOverLength = GameOver.Length;
        private int ResetQuitLength = ResetQuit.Length;
        // vector2 "constants"
        private Vector2 GridCenter = new Vector2(WindowWidth / 2, WindowHeight / 2);
        #endregion

        #region Enums
        protected enum DrawingState
        {
            Initialize,
            Drawing,
            Paused,
            Reset,
            Done
        }
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
        #endregion

        #region Constructor
        public Two_D_Rotation()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Game Methods
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();
            gameBoundingBox = new Rectangle(0, 0, WindowWidth, WindowHeight);
            drawingState = DrawingState.Initialize;
            twoDObject = new TwoDObject(GridCenter, GridCenter, 1.0f, 1.0f);
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                graphics.GraphicsDevice.Viewport.Height, 0,     // bottom, top
                0, 1);                                          // near, far plane 
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
        #endregion

        protected override void Update(GameTime gameTime)
        {
            InputKeyManager.Triggers currentKeys = InputKeyManager.Read();

            switch (drawingState)
            {
                case DrawingState.Initialize:
                    // initialize - set drawingState to Drawing
                    // user input to change origin of rotation
                    if ((currentKeys & InputKeyManager.Triggers.UpArrow) != 0)
                    {
                        twoDObject.RotationPoint -= new Vector2(0, 5);
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.DownArrow) != 0)
                    {
                        twoDObject.RotationPoint += new Vector2(0, 5);
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.LeftArrow) != 0)
                    {
                        twoDObject.RotationPoint -= new Vector2(5, 0);
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.RightArrow) != 0)
                    {
                        twoDObject.RotationPoint += new Vector2(5, 0);
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.CCW) != 0)
                    {
                        twoDObject.Direction = -1.0f;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.CW) != 0)
                    {
                        twoDObject.Direction = 1.0f;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.Origin) != 0)
                    {
                        twoDObject.RotationPoint = GridCenter;
                    }//end if
                    if ((currentKeys & InputKeyManager.Triggers.Center) != 0)
                    {
                        // get the current position of the 2D object
                        List<Point> points = twoDObject.Points;
                        // calculate the center
                        float cX = 0f, cY = 0f;
                        foreach (Point p in points)
                        {
                            cX += p.PointLocation.X;
                            cY += p.PointLocation.Y;
                        }//end foreach
                        cX /= points.Count;
                        cY /= points.Count;
                        // set the rotation point
                        twoDObject.RotationPoint = new Vector2(cX, cY);
                    }//end if

                    // press spacebar to start the game
                    if ((currentKeys & InputKeyManager.Triggers.Fire) != 0)
                    {
                        drawingState = DrawingState.Drawing;
                    }//end if
                    break;
                case DrawingState.Drawing:
                    // draw objects
                    // user input to rotate the twoDObject clockwise and counteclockwise
                    twoDObject.Update(gameTime);
                    SetVertices();
                    // pause

                    // reset
                    if ((currentKeys & InputKeyManager.Triggers.ExitLevel) != 0)
                    {
                        drawingState = DrawingState.Reset;
                    }//end if
                    break;
                case DrawingState.Paused:
                    // rotation is paused
                    break;
                case DrawingState.Reset:
                    // reset/clear the drawing
                    if ((currentKeys & InputKeyManager.Triggers.Reset) != 0)
                    {
                        //twoDObject.RotationPoint = GRID_CENTER;
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
                    graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 5);
                    spriteBatch.DrawString(courierNew, "Rotation in 2D", Vector2.Zero, Color.Black);
                    spriteBatch.DrawString(courierNew, "Press Up/Down arrows to +/- y", new Vector2(WindowMargin, InstructionSpot), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press Left/Right arrows to -/+ x", new Vector2(WindowMargin, InstructionSpot + WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press C to roate about object's center or O to rotate about the origin", new Vector2(WindowMargin, InstructionSpot + 2 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Press '+' for clockwise or '-' for counterclockwise", new Vector2(WindowMargin, InstructionSpot + 3 * WindowMargin), Color.Blue);
                    spriteBatch.DrawString(courierNew, "Rotation Point (" + (twoDObject.RotationPoint.X - GridCenter.X) + "," + ((twoDObject.RotationPoint.Y - GridCenter.Y) * -1) + ")", new Vector2(0, WindowMargin), Color.Red);
                    spriteBatch.Draw(gridPoint, twoDObject.RotationPoint, Color.White);
                    break;
                case DrawingState.Drawing:
                    spriteBatch.Draw(graphPaper, Vector2.Zero, Color.White);
                    twoDObject.Draw(gameTime, spriteBatch);
                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 5);
                    spriteBatch.DrawString(courierNew, "Rotation in 2D", Vector2.Zero, Color.Black);
                    spriteBatch.DrawString(courierNew, "Rotation Point (" + (twoDObject.RotationPoint.X - GridCenter.X) + "," + ((twoDObject.RotationPoint.Y - GridCenter.Y) * -1) + ")", new Vector2(0, WindowMargin), Color.Red);
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
                vertices[v].Color = Color.Red;
            }//end for
        }//eom
        #endregion
    }//eoc
}//eon
