using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment5
{
    public class Bomb : GameObject
    {
        public AStarSearch search;
        List<Vector3> path;

        private float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;
        private Player player;

        private Random random { get; set; }

        private bool haveThreadRunning;

        public Bomb(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, Player player) : base()
        {
            Terrain = terrain;

            this.player = player;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            Texture2D texture = Content.Load<Texture2D>("BlackSquare");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, light, Content, graphicsDevice, 20f, texture, "SimpleShading", 2);
            renderer.ObjectModel.Meshes[0].Effects[0].Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
            renderer.ObjectModel.Meshes[0].Effects[0].Parameters["SpecularColor"].SetValue(Color.Red.ToVector3());
            (renderer.ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Red.ToVector3();
            Add<Renderer>(renderer);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            path = null;
            random = new Random();

            search = new AStarSearch(gridSize, gridSize); // size of grid 
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - Terrain.size.X / 2, 0, gridH * j + gridH / 2 - Terrain.size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 1.0) search.Nodes[j, i].Passable = false;
                }
            }

            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(NewPosition));
        }

        public override void Update()
        {
            if (path != null && path.Count > 0)
            {
                Vector3 currentPosition = Transform.Position;
                Vector3 destinationPoint = GetGridPosition(path[0]);
                // Move to the destination along the path
                currentPosition.Y = 0;
                destinationPoint.Y = 0;
                Vector3 direction = Vector3.Distance(destinationPoint, currentPosition) == 0 ? Vector3.Zero : Vector3.Normalize(destinationPoint - currentPosition);

                this.Rigidbody.Velocity = new Vector3(direction.X, 0, direction.Z) * speed;

                if (Vector3.Distance(currentPosition, destinationPoint) < 1f) // if it reaches to a point, go to the next in path
                {
                    path.RemoveAt(0);
                    if (path.Count == 0) // if it reached to the goal
                    {
                        path = null;
                        return;
                    }
                }
            }
            else
            {
                // Search again to make a new path.
                RandomPathFinding();
                Transform.LocalPosition = GetGridPosition(path[0]);
            }

            this.Transform.LocalPosition = new Vector3(this.Transform.LocalPosition.X, Terrain.GetAltitude(this.Transform.LocalPosition), this.Transform.LocalPosition.Z) + Vector3.Up;
            Transform.Update();
            base.Update();
        }

        private Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 0, gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
        }

        // GW * GP.X + GW / 2 - T.S.X / 2
        // GH * 

        private void RandomPathFinding()
        {
            while (!(search.Start = search.Nodes[random.Next(search.Rows), random.Next(search.Cols)]).Passable);

            search.Start = search.Nodes[1, 1];

            int row = (int)GetGridPosition(player.Transform.Position).X;
            int col = (int)GetGridPosition(player.Transform.Position).Z;
            Debug.WriteLine("[" + row + "," + col +"]");

            search.End = search.Nodes[search.Rows / 2, search.Cols / 2];
            search.Search();
            path = new List<Vector3>();

            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
        }

        private void NewPosition(Object obj)
        {
            while (haveThreadRunning)
            {
                System.Threading.Thread.Sleep(5000);

                if (path != null && path.Count > 0)
                {
                    path.RemoveAt(0);
                    if (path.Count == 0) // if it reached to the goal
                    {
                        path = null;
                        return;
                    }
                }
            }
        }
    }
}
