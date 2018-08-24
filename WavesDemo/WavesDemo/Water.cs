using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace WavesDemo
{
    class Water
    {
        struct WaterColumn
        {
            public float TargetHeight;
            public float Height;
            public float Speed;

            public void Update(float dampening, float tension)
            {
                float x = TargetHeight - Height;
                Speed += tension * x - Speed * dampening;
                Height += Speed;
            }//eom
        }//eos

        PrimitiveBatch pb;
        WaterColumn[] columns = new WaterColumn[201];
        static Random rand = new Random();

        public float Tension = 0.025f;
        public float Dampening = 0.025f;
        public float Spread = 0.25f;

        RenderTarget2D metaballTarget, particlesTarget;
        SpriteBatch spriteBatch;
        AlphaTestEffect alphaTest;
        Texture2D particleTexture;

        private float Scale { get { return spriteBatch.GraphicsDevice.Viewport.Width / (columns.Length - 1f); } }

        List<Particle> particles = new List<Particle>();
        class Particle
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Orientation;

            public Particle(Vector2 position, Vector2 velocity, float orientation)
            {
                Position = position;
                Velocity = velocity;
                Orientation = orientation;
            }//eom
        }//eoc

        public Water(GraphicsDevice device, Texture2D particleTexture)
        {
            pb = new PrimitiveBatch(device);
            this.particleTexture = particleTexture;
            spriteBatch = new SpriteBatch(device);
            metaballTarget = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height);
            particlesTarget = new RenderTarget2D(device, device.Viewport.Width, device.Viewport.Height);
            alphaTest = new AlphaTestEffect(device);
            alphaTest.ReferenceAlpha = 175;

            var view = device.Viewport;
            alphaTest.Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) *
                Matrix.CreateOrthographicOffCenter(0, view.Width, view.Height, 0, 0, 1);

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new WaterColumn()
                {
                    Height = 240,
                    TargetHeight = 240,
                    Speed = 0
                };
            }//end for
        }//eom

        // Returns the height of the water at a given x coordinate.
        public float GetHeight(float x)
        {
            if (x < 0 || x > 800)
                return 240;

            return columns[(int)(x / Scale)].Height;
        }//eom

        void UpdateParticle(Particle particle)
        {
            const float Gravity = 0.3f;
            particle.Velocity.Y += Gravity;
            particle.Position += particle.Velocity;
            particle.Orientation = GetAngle(particle.Velocity);
        }//eom

        public void Splash(float xPosition, float speed)
        {
            int index = (int)MathHelper.Clamp(xPosition / Scale, 0, columns.Length - 1);
            for (int i = Math.Max(0, index - 0); i < Math.Min(columns.Length - 1, index + 1); i++)
                columns[index].Speed = speed;

            CreateSplashParticles(xPosition, speed);
        }

        private void CreateSplashParticles(float xPosition, float speed)
        {
            float y = GetHeight(xPosition);

            if (speed > 120)
            {
                for (int i = 0; i < speed / 8; i++)
                {
                    Vector2 pos = new Vector2(xPosition, y) + GetRandomVector2(40);
                    Vector2 vel = FromPolar(MathHelper.ToRadians(GetRandomFloat(-150, -30)), GetRandomFloat(0, 0.5f * (float)Math.Sqrt(speed)));
                    CreateParticle(pos, vel);
                }//end for
            }//end if
        }//eom

        private void CreateParticle(Vector2 pos, Vector2 velocity)
        {
            particles.Add(new Particle(pos, velocity, 0));
        }//eom

        private Vector2 FromPolar(float angle, float magnitude)
        {
            return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }//eom

        private float GetRandomFloat(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }//eom

        private Vector2 GetRandomVector2(float maxLength)
        {
            return FromPolar(GetRandomFloat(-MathHelper.Pi, MathHelper.Pi), GetRandomFloat(0, maxLength));
        }//eom

        private float GetAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }//eom

        public void Update()
        {
            for (int i = 0; i < columns.Length; i++)
                columns[i].Update(Dampening, Tension);

            float[] lDeltas = new float[columns.Length];
            float[] rDeltas = new float[columns.Length];

            // do some passes where columns pull on their neighbours
            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    if (i > 0)
                    {
                        lDeltas[i] = Spread * (columns[i].Height - columns[i - 1].Height);
                        columns[i - 1].Speed += lDeltas[i];
                    }//end if
                    if (i < columns.Length - 1)
                    {
                        rDeltas[i] = Spread * (columns[i].Height - columns[i + 1].Height);
                        columns[i + 1].Speed += rDeltas[i];
                    }//end if
                }//end for

                for (int i = 0; i < columns.Length; i++)
                {
                    if (i > 0)
                        columns[i - 1].Height += lDeltas[i];
                    if (i < columns.Length - 1)
                        columns[i + 1].Height += rDeltas[i];
                }//end for
            }//end for

            foreach (var particle in particles)
                UpdateParticle(particle);

            particles = particles.Where(x => x.Position.X >= 0 && x.Position.X <= 800 && x.Position.Y - 5 <= GetHeight(x.Position.X)).ToList();
        }//eom

        public void DrawToRenderTargets()
        {
            GraphicsDevice device = spriteBatch.GraphicsDevice;
            device.SetRenderTarget(metaballTarget);
            device.Clear(Color.Transparent);

            // draw particles to the metaball render target
            spriteBatch.Begin(0, BlendState.Additive);
            foreach (var particle in particles)
            {
                Vector2 origin = new Vector2(particleTexture.Width, particleTexture.Height) / 2f;
                spriteBatch.Draw(particleTexture, particle.Position, null, Color.White, particle.Orientation, origin, 2f, 0, 0);
            }
            spriteBatch.End();

            // draw a gradient above the water so the metaballs will fuse with the water's surface.
            pb.Begin(PrimitiveType.TriangleList);

            const float thickness = 20;
            float scale = Scale;
            for (int i = 1; i < columns.Length; i++)
            {
                Vector2 p1 = new Vector2((i - 1) * scale, columns[i - 1].Height);
                Vector2 p2 = new Vector2(i * scale, columns[i].Height);
                Vector2 p3 = new Vector2(p1.X, p1.Y - thickness);
                Vector2 p4 = new Vector2(p2.X, p2.Y - thickness);

                pb.AddVertex(p2, Color.White);
                pb.AddVertex(p1, Color.White);
                pb.AddVertex(p3, Color.Transparent);

                pb.AddVertex(p3, Color.Transparent);
                pb.AddVertex(p4, Color.Transparent);
                pb.AddVertex(p2, Color.White);
            }//end for

            pb.End();

            // save the results in another render target (in particlesTarget)
            device.SetRenderTarget(particlesTarget);
            device.Clear(Color.Transparent);
            spriteBatch.Begin(0, null, null, null, null, alphaTest);
            spriteBatch.Draw(metaballTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // switch back to drawing to the backbuffer.
            device.SetRenderTarget(null);
        }//eom

        public void Draw()
        {
            Color lightBlue = new Color(0.2f, 0.5f, 1f);

            // draw the particles 3 times to create a bevelling effect
            spriteBatch.Begin();
            spriteBatch.Draw(particlesTarget, -Vector2.One, new Color(0.8f, 0.8f, 1f));
            spriteBatch.Draw(particlesTarget, Vector2.One, new Color(0f, 0f, 0.2f));
            spriteBatch.Draw(particlesTarget, Vector2.Zero, lightBlue);
            spriteBatch.End();

            // draw the waves
            pb.Begin(PrimitiveType.TriangleList);
            Color midnightBlue = new Color(0, 15, 40) * 0.9f;
            lightBlue *= 0.8f;

            float bottom = spriteBatch.GraphicsDevice.Viewport.Height;
            float scale = Scale;
            for (int i = 1; i < columns.Length; i++)
            {
                Vector2 p1 = new Vector2((i - 1) * scale, columns[i - 1].Height);
                Vector2 p2 = new Vector2(i * scale, columns[i].Height);
                Vector2 p3 = new Vector2(p2.X, bottom);
                Vector2 p4 = new Vector2(p1.X, bottom);

                pb.AddVertex(p1, lightBlue);
                pb.AddVertex(p2, lightBlue);
                pb.AddVertex(p3, midnightBlue);

                pb.AddVertex(p1, lightBlue);
                pb.AddVertex(p3, midnightBlue);
                pb.AddVertex(p4, midnightBlue);
            }//end for

            pb.End();
        }//eom
    }//eoc
}//eon
