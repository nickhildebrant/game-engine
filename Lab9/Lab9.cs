using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CPI311.Labs
{
    public class Lab9 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        int size = 80; // 100 x 100 grid
        Random random = new Random();

        Model cube;
        Model sphere;
        AStarSearch search;
        List<Vector3> path;

        Camera camera;
        Transform cameraTransform;

        public Lab9()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);

            search = new AStarSearch(size, size); // size of grid

            foreach (AStarNode node in search.Nodes)
            {
                // if 20% of cells are blocks
                if (random.NextDouble() < 0.2) search.Nodes[random.Next(size), random.Next(size)].Passable = false;
            }

            search.Start = search.Nodes[0, 0];
            search.Start.Passable = true;
            search.End = search.Nodes[size - 1, size - 1];
            search.End.Passable = true;

            search.Search(); // A search is made here.
            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            sphere = Content.Load<Model>("Sphere");
            cube = Content.Load<Model>("cube");

            cameraTransform = new Transform();
            camera = new Camera();
            camera.Transform = cameraTransform;
            camera.Transform.LocalPosition = Vector3.Right * size/2 + Vector3.Backward * size/2 + Vector3.Up * (int)(size/1.5);
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Time.Update(gameTime);
            InputManager.Update();

            if(InputManager.IsKeyPressed(Keys.Space))
            {
                // While loops fix the search to avoid inpassable nodes
                while (!(search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ; // assign a random start node (passable)
                while (!(search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;   // assign a random end node (passable)
                search.Search();
                path.Clear();
                AStarNode current = search.End;
                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (AStarNode node in search.Nodes)
                if (!node.Passable) cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) * Matrix.CreateTranslation(node.Position), camera.View, camera.Projection);

            foreach (Vector3 position in path) sphere.Draw(Matrix.CreateScale(0.2f, 0.2f, 0.2f) * Matrix.CreateTranslation(position), camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}