/*
 * 
 * Code adapted from http://ghoshehsoft.wordpress.com/2013/03/02/simple-spring-physics
 * 
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#region Additional Namespaces
using System;
using System.Collections.Generic;
using SpringDemo.Lab;
#endregion

namespace SpringDemo
{
    public class Springs : Game
    {
        #region Constants
        const int WINDOW_WIDTH = 1024;
        const int WINDOW_HEIGHT = 768;
        #endregion

        #region Data Members
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texHelp;  // Help contents
        Texture2D texSmall; // Small circle
        Texture2D texLarge; // Large circle
        Texture2D blank;    // Used to draw lines
        SpriteFont font, smallFont;    // Fonts (-_-)

        String linkText = "SpringLab by Hisham Ghosheh, http://ghoshehsoft.wordpress.com/2013/03/02/simple-spring-physics/";
        Rectangle linkRect;

        bool paused = false;
        bool helpVisible = true;

        Random rnd = new Random(1611);
        #endregion

        #region Game Objects
        List<Spring2D> springs = new List<Spring2D>();
        List<Node2D> nodes = new List<Node2D>();

        // Nodes selected using left and right mouse button
        Node2D nodeL, nodeR;

        // Stiffness for all springs (passed as negative)
        float stiffness = 3;

        // false means spring, true means string
        bool stringMode = false;
        #endregion

        #region Input
        KeyboardState prevKS;   // Previous keyboard state
        MouseState prevMs;      // Previous mouse state
        long clickTimeL = -99999;
        // Mouse position when the the left and right mouse button was clicked (mouse down)
        Vector2 clickPosL = new Vector2(-999999999999);
        Vector2 clickPosR = new Vector2(-999999999999);
        // Mouse position, updated each frame
        Vector2 mousePos;
        #endregion

        public Springs()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//eom

        protected override void Initialize()
        {
            base.Initialize();
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.ApplyChanges();
            Vector2 p = smallFont.MeasureString(linkText);
            float height = p.Y;
            float width = p.X;
            linkRect = new Rectangle(10, (int)(Window.ClientBounds.Height - height), (int)width, (int)height);

            IsMouseVisible = true;
        }//eom

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texHelp = Content.Load<Texture2D>("Help");
            texSmall = Content.Load<Texture2D>("PointSmall");
            texLarge = Content.Load<Texture2D>("PointLarge");
            font = Content.Load<SpriteFont>("CourierNew");
            smallFont = Content.Load<SpriteFont>("CourierNewSmall");

            // Create a texture with one pixel, used for drawing lines
            // http://www.xnawiki.com/index.php?title=Drawing_2D_lines_without_using_primitives
            blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });

            InitLab();
        }//eom

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }//eom

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            KeyboardState keyboardState = Keyboard.GetState();

            #region Handling Keyboard
            // Exit on Esc
            if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

            // Toggle isString property in all springs
            if (keyboardState.IsKeyDown(Keys.Space) && prevKS.IsKeyUp(Keys.Space))
            {
                stringMode = !stringMode;
                foreach (Spring2D spr in springs)
                    spr.StringMode = stringMode;
            }//end if

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                stiffness += 0.02f;
                stiffness = stiffness.Clamp(0.5f, 3f);

                foreach (Spring2D spr in springs)
                    spr.Stiffness = -stiffness;
            }//end if
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                stiffness -= 0.02f;
                stiffness = stiffness.Clamp(0.5f, 3f);

                foreach (Spring2D spr in springs)
                    spr.Stiffness = -stiffness;
            }//end else

            if (keyboardState.IsKeyDown(Keys.F1) && prevKS.IsKeyUp(Keys.F1))
            {
                helpVisible = !helpVisible;
            }//end if
            #endregion

            #region Handling Mouse
            MouseState mouseState = Mouse.GetState();
            mousePos = new Vector2(mouseState.X, mouseState.Y);
            #region Left Mouse Button Logic
            bool mouseDownL = (mouseState.LeftButton == ButtonState.Pressed && prevMs.LeftButton == ButtonState.Released);
            bool mouseUpL = (mouseState.LeftButton == ButtonState.Released && prevMs.LeftButton == ButtonState.Pressed);
            bool mouseDragL = (mouseState.LeftButton == ButtonState.Pressed && prevMs.LeftButton == ButtonState.Pressed);

            if (mouseDownL)
            {
                nodeL = PickNode(mousePos); // a signle click picks a node

                // Checking double clicks to remove a node
                long currentTime = gameTime.TotalGameTime.Milliseconds;

                float dt = currentTime - clickTimeL;
                float dp = Vector2.Distance(clickPosL, mousePos);
                if (nodeL != null && dt < 500 && dp < 10)
                {
                    nodes.Remove(nodeL);
                    // Remove all springs attached to the deleted node
                    for (int i = 0; i < springs.Count; i++)
                    {
                        Spring2D spr = springs[i];
                        if (spr.node1 == nodeL || spr.node2 == nodeL)
                        {
                            springs.RemoveAt(i);
                            i--;
                        }//end if
                    }//end for
                    nodeL = null;
                }//end if

                clickTimeL = currentTime;
                clickPosL = mousePos;
            }//end if
            else if (mouseDragL)
            {
                if (nodeL != null)
                {
                    // We make sure the selected node is always at the same position of the mouse
                    nodeL.a = Vector2.Zero;
                    nodeL.v = Vector2.Zero;
                    nodeL.p = mousePos;
                }//end if
            }//end else
            #endregion

            #region Right Mouse Button Logic
            bool mouseDownR = (mouseState.RightButton == ButtonState.Pressed && prevMs.RightButton == ButtonState.Released);
            bool mouseUpR = (mouseState.RightButton == ButtonState.Released && prevMs.RightButton == ButtonState.Pressed);

            if (mouseDownR)
            {
                clickPosR = mousePos;
                nodeR = PickNode(mousePos);
            }//end if
            else if (mouseUpR)
            {
                float dist = Vector2.Distance(mousePos, clickPosR);
                if (nodeR == null && dist < 10) // Simple right click
                {
                    nodes.Add(new Node2D(mousePos, 1f));
                }//end if
                else if (nodeR != null && dist > 20)  // Right button drag\drop
                {
                    // Check if dropped on an existing node, if not then create a new one, then link them
                    Node2D target = PickNode(mousePos);
                    if (target == null)
                    {
                        target = new Node2D(mousePos, 1f);
                        nodes.Add(target);
                    }//end if
                    Spring2D spr = new Spring2D(target, nodeR, Vector2.Distance(target.p, nodeR.p) * 0.80f);
                    spr.StringMode = stringMode;

                    springs.Add(spr);
                }//end else

                nodeR = null;
            }//end if
            #endregion

            #region Mouse Wheel Logic
            // Change mass from 1 to 500 using mouse wheel
            if (nodeL != null)
            {
                float wheelDelta = mouseState.ScrollWheelValue - prevMs.ScrollWheelValue;
                nodeL.Mass += wheelDelta / 50f;
                //if (mass < 1) mass = 1; else if (mass > 500) mass = 500;
            }//end if
            #endregion

            #endregion

            if (keyboardState.IsKeyDown(Keys.P) && prevKS.IsKeyUp(Keys.P)) paused = !paused;

            if (keyboardState.IsKeyDown(Keys.F5) && prevKS.IsKeyUp(Keys.F5))
            {
                InitLab();
                return;
            }//end if

            //if (prevMs.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed && linkRect.Contains(mouseState.X, mouseState.Y)) VisitBlog();

            prevKS = keyboardState;
            prevMs = mouseState;

            if (paused && keyboardState.IsKeyUp(Keys.Right)) return;

            #region Physics Update
            // First update all springs, this will only update the objects' acceleration
            foreach (Spring2D spr in springs)
                spr.Update();

            // Second update objects, this will affect acceleration, velocity and position
            foreach (Node2D node in nodes)
                node.Update();
            #endregion

            base.Update(gameTime);
        }//eom

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            {
                // Draw springs, line width is relative to the stretch amount of the spring
                foreach (Spring2D spr in springs)
                    DrawLine(spriteBatch, 6f * spr.Rest, (paused ? Color.Gray : Color.DarkGreen), spr.node1.p, spr.node2.p);

                // Draw nodes
                foreach (Node2D node in nodes)
                    spriteBatch.Draw(texSmall, node.p - new Vector2(texSmall.Width / 2f, texSmall.Height / 2f), Color.White);

                // Draw selected node as a large circle
                if (nodeL != null) spriteBatch.Draw(texLarge, nodeL.p - new Vector2(texLarge.Width / 2f, texLarge.Height / 2f), Color.White);

                if (nodeR != null)
                {
                    // Draw a line and a node when dragging with the right mouse button
                    DrawLine(spriteBatch, 3, Color.Yellow, nodeR.p, mousePos);
                    spriteBatch.Draw(texSmall, mousePos - new Vector2(texSmall.Width / 2f, texSmall.Height / 2f), Color.Yellow);
                }//end if

                // Draw info text
                String text = String.Format("Mass:{0}\nType:{1}\nStiffnes:{2}", (nodeL != null ? Math.Round(nodeL.Mass, 2).ToString() : "-"), (springs.Count > 0 && springs[0].StringMode ? "String" : "Spring"), Math.Round(stiffness, 3));
                spriteBatch.DrawString(font, text, new Vector2(29), Color.Gray);
                spriteBatch.DrawString(font, text, new Vector2(30), Color.DarkGray);

                spriteBatch.DrawString(smallFont, linkText, new Vector2(linkRect.X, linkRect.Y), Color.Blue);

                if (helpVisible) spriteBatch.Draw(texHelp, new Vector2(WINDOW_WIDTH / 2, WINDOW_HEIGHT/ 2), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }//eom

        #region Other Methods
        private void InitLab()
        {
            stiffness = 1f;
            stringMode = false;
            paused = false;
            //helpVisible = true;
            nodeL = null;
            nodeR = null;
            springs.Clear();
            nodes.Clear();

            #region Create Geometry
            var pendulum1 = GeometryGenerator.CreatePendulum(200, 180, 100, 200, 280, 10, 50, -stiffness);
            var pendulum2 = GeometryGenerator.CreatePendulum(400, 180, 100, 400, 280, 10, 50, - stiffness);
            //var mesh = GeometryGenerator.CreateMesh(200, 20, 5, 5, 60, 60, 1, 50, 50, -stiffness);
            //var mesh = GeometryGenerator.CreateMesh(200, 200, 30, 30, 4, 4, 1, -1, -1,-stiffness);
            //var circle = GeometryGenerator.CreateCircle(700, 300, 100, 6, 1, 1, 80, 60, -stiffness);

            // Add results to lists
            nodes.AddRange(pendulum1.Item1);
            nodes.AddRange(pendulum2.Item1);
            //nodes.AddRange(mesh.Item1);
            //nodes.AddRange(circle.Item1);

            springs.AddRange(pendulum1.Item2);
            springs.AddRange(pendulum2.Item2);
            //springs.AddRange(mesh.Item2);
            //springs.AddRange(circle.Item2);
            #endregion  
        }//eom

        Node2D PickNode(Vector2 mousePos, float maxDist = 20)
        {
            foreach (Node2D node in nodes)
            {
                if (Vector2.Distance(node.p, mousePos) < maxDist)
                    return node;
            }//end foreach
            return null;
        }//eom

        // Modified version of http://www.xnawiki.com/index.php?title=Drawing_2D_lines_without_using_primitives
        void DrawLine(SpriteBatch batch,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            // Small lines means two nodes near each other -> Angle changes quickly -> Missy lines            
            if (length < 10) return;

            // I added these two lines to correctly allign the line when width > 1
            point1.X += width * (float)Math.Sin(angle) / 2f;
            point1.Y -= width * (float)Math.Cos(angle) / 2f;

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }//eom
        #endregion
    }//eoc
}//eon
