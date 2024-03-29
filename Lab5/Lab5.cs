﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace CPI311.Labs
{
    public class Lab5 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Lab 5
        Model model;
        Effect effect;
        Transform modelTransform;

        Camera camera;
        Transform cameraTransform;

        SpriteFont font;
        int shadingTechnique = 0;

        public Lab5()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");

            model = Content.Load<Model>("Torus");
            modelTransform = new Transform();
            effect = Content.Load<Effect>("SimpleShading");

            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 5;
            camera.Transform = cameraTransform;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.W)) cameraTransform.LocalPosition += cameraTransform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A)) cameraTransform.Rotate(cameraTransform.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.S)) cameraTransform.LocalPosition += cameraTransform.Backward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) cameraTransform.Rotate(cameraTransform.Down, Time.ElapsedGameTime);

            // Changes the shading technique
            if(InputManager.IsKeyPressed(Keys.Tab))
            {
                if (shadingTechnique == 3) shadingTechnique = 0;
                else shadingTechnique++;
            }

            base.Update(gameTime);
        }

        [System.Obsolete]
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix view = camera.View;
            Matrix projection = camera.Projection;

            effect.CurrentTechnique = effect.Techniques[shadingTechnique]; //"0" is the first technique
            effect.Parameters["World"].SetValue(modelTransform.World);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
            effect.Parameters["CameraPosition"].SetValue(cameraTransform.Position);
            effect.Parameters["Shininess"].SetValue(2f);
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0, 0));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.5f));
            //effect.Parameters["DiffuseTexture"].SetValue(texture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (ModelMesh mesh in model.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
            }

            _spriteBatch.Begin();
            switch (shadingTechnique)
            {
                case 0:
                    _spriteBatch.DrawString(font, "TAB: Technique = Gouraud", new Vector2(5, 10), Color.Black);
                    break;

                case 1:
                    _spriteBatch.DrawString(font, "TAB: Technique = Phong", new Vector2(5, 10), Color.Black);
                    break;

                case 2:
                    _spriteBatch.DrawString(font, "TAB: Technique = Phong-Blinn", new Vector2(5, 10), Color.Black);
                    break;

                case 3:
                    _spriteBatch.DrawString(font, "TAB: Technique = Schlick", new Vector2(5, 10), Color.Black);
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}