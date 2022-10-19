using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using CPI311.GameEngine;
using System;

namespace CPI311.Labs
{
    public class Lab8 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Lab 8 sounds
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;

        Model model;
        Transform modelTransform;
        //Collider sphereCollider;
        Texture texture;
        Camera camera, topDownCamera;

        Effect effect;

        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;

        public Lab8()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);

            transforms = new List<Transform>();
            colliders = new List<Collider>();
            cameras = new List<Camera>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.Setup(true, 1920, 1080);

            gunSound = Content.Load<SoundEffect>("Gun");

            effect = Content.Load<Effect>("SimpleShading");

            model = Content.Load<Model>("Sphere");
            modelTransform = new Transform();
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * modelTransform.LocalScale.Y;
            sphereCollider.Transform = modelTransform;

            transforms.Add(modelTransform);
            colliders.Add(sphereCollider);

            texture = Content.Load<Texture2D>("Square");

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0f, 0f);
            camera.Size = new Vector2(0.5f, 1f);
            camera.AspectRatio = camera.Viewport.AspectRatio;

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.5f, 0f);
            topDownCamera.Size = new Vector2(0.5f, 1f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;

            cameras.Add(topDownCamera);
            cameras.Add(camera);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            
            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsMouseLeftClicked())
            {
                SoundEffectInstance instance = gunSound.CreateInstance();
                instance.Play();
            }

            Ray ray = camera.ScreenPointToWorldRay(InputManager.GetMousePosition());

            foreach (Collider collider in colliders)
            {
                if (collider.Intersects(ray) != null)
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Blue.ToVector3();
                }
                else
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Red.ToVector3();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Camera camera in cameras)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState();
                GraphicsDevice.Viewport = camera.Viewport;
                Matrix view = camera.View;
                Matrix projection = camera.Projection;
                effect.CurrentTechnique = effect.Techniques[1];
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
                effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
                effect.Parameters["Shininess"].SetValue(2f);
                effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
                effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.5f));
                effect.Parameters["DiffuseTexture"].SetValue(texture);

                foreach (Transform transform in transforms)
                {
                    effect.Parameters["World"].SetValue(transform.World);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        foreach (ModelMesh mesh in model.Meshes)
                        {
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                                GraphicsDevice.Indices = part.IndexBuffer;
                                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                            }
                        }
                    }
                }
            }

            base.Draw(gameTime);
        }
    }
}