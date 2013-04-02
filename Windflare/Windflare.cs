using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Windflare.Core;

namespace Windflare {
    public class Windflare : Game {
        ProgressiveNode nodeA;
        BasicEffect effect;
        GraphicsDeviceManager graphics;

        public Windflare() {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 800;
        }

        protected override void Initialize() {
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.World = Matrix.Identity;
            effect.DiffuseColor = new Vector3(1f, 1f, 1f);
            effect.LightingEnabled = true;
            effect.AmbientLightColor = new Vector3(.1f, .1f, .1f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = new Vector3(-1, -1, -1);
            effect.DirectionalLight0.DiffuseColor = new Vector3(.2f, .2f, .2f);
            effect.DirectionalLight0.SpecularColor = new Vector3(.2f, .2f, .2f);
            base.Initialize();


            var image = Content.Load<Texture2D>("hatemap");
            var generator = new TerrainGenerator();
            var heightmap = new Terrain(generator.FromImage(image), 16);

            var surface = new SphericalSubsurface(
                45.Rad(), 0.Rad(), 90.Rad(), 90.Rad(),
                100000, Vector3.Zero, Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -90.Rad()));
            nodeA = new ProgressiveNode(surface, heightmap);
        }

        float v = 180.Rad(), u = 45.Rad();
        Vector3 pos = new Vector3(120000, 0, 0);

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            var state = Keyboard.GetState();
            float speed = 0;
            if (state[Keys.Left] == KeyState.Down) { v -= (float)gameTime.ElapsedGameTime.TotalSeconds * 4; }
            if (state[Keys.Right] == KeyState.Down) { v += (float)gameTime.ElapsedGameTime.TotalSeconds * 4; }
            if (state[Keys.Up] == KeyState.Down) { u -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2; }
            if (state[Keys.Down] == KeyState.Down) { u += (float)gameTime.ElapsedGameTime.TotalSeconds * 2; }
            if (state[Keys.W] == KeyState.Down) { speed = 50f; }
            if (state[Keys.S] == KeyState.Down) { speed = -50f; }

            u = (float)Math.Min(Math.PI - .01, Math.Max(u, .01));
            Vector3 cameraDir = MathExt.SphericalToCartesian(u, v, 1);

            pos += cameraDir * speed;

            var projectionMatrix = Matrix.CreatePerspectiveFieldOfView(50.Rad(),
                GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height,
                1f, 1000000f);
            var viewMatrix = Matrix.CreateLookAt(
                pos,
                pos + cameraDir,
                new Vector3(0, 1, 0));
            var cameraFrustum = new BoundingFrustum(viewMatrix * projectionMatrix);

            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                nodeA.Render(GraphicsDevice, pos);
            }

            base.Draw(gameTime);
        }
    }
}
