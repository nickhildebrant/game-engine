using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using CPI311.GameEngine;

public class Prize : GameObject
{
    public AStarSearch search;
    List<Vector3> path;

    private int gridSize = 20; //grid size
    private TerrainRenderer Terrain;

    private Random random { get; set; }

    public Prize(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
    {
        Terrain = terrain;

        Rigidbody rigidbody = new Rigidbody();
        rigidbody.Transform = Transform;
        rigidbody.Mass = 1;
        Add<Rigidbody>(rigidbody);

        Texture2D texture = Content.Load<Texture2D>("taxes");
        Renderer renderer = new Renderer(Content.Load<Model>("paper2"), Transform, camera, light, Content, graphicsDevice, 20f, texture, "SimpleShading", 3);
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
                Vector3 pos = new Vector3(gridW * i + gridW / 2 - Terrain.size.X / 2, 1, gridH * j + gridH / 2 - Terrain.size.Y / 2);
                if (Terrain.GetAltitude(pos) > 1.0) search.Nodes[j, i].Passable = false;
            }
        }
    }

    public override void Update()
    {
        Transform.Rotate(Transform.Up, 5 * Time.ElapsedGameTime);

        if (path == null)
        {
            // Search again to make a new path.
            RandomPathFinding();
            Transform.LocalPosition = GetGridPosition(path[0]);
        }

        this.Transform.LocalPosition = new Vector3(this.Transform.LocalPosition.X, (float)(2 + Math.Sin(Time.TotalGameTime.Milliseconds / 200.0f)), this.Transform.LocalPosition.Z) + Vector3.Up;
        Transform.Update();
        base.Update();
    }

    private Vector3 GetGridPosition(Vector3 gridPos)
    {
        float gridW = Terrain.size.X / search.Cols;
        float gridH = Terrain.size.Y / search.Rows;
        return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 1, gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
    }

    private void RandomPathFinding()
    {
        if (path == null) while (!(search.Start = search.Nodes[random.Next(search.Rows), random.Next(search.Cols)]).Passable) ;
        else search.Start = search.End;

        while (!(search.End = search.Nodes[random.Next(search.Rows), random.Next(search.Cols)]).Passable) ;
        search.Search();
        path = new List<Vector3>();

        AStarNode current = search.End;
        while (current != null)
        {
            path.Insert(0, current.Position);
            current = current.Parent;
        }
    }
}