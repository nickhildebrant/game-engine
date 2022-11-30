using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CPI311.GameEngine
{
    public class Player : GameObject
    {
        public TerrainRenderer Terrain { get; set; }

        public Player(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // Add other component required for Player
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, light, Content, graphicsDevice, 20f, texture, "SimpleShading", 1);
            Add<Renderer>(renderer);
        }

        public override void Update()
        {
            if (InputManager.IsKeyDown(Keys.W) && Terrain.GetAltitude(Transform.LocalPosition + Transform.Forward * 7.5f * Time.ElapsedGameTime) < 0.25) Transform.LocalPosition += Transform.Forward * Time.ElapsedGameTime * 7.5f; // move forward
            if (InputManager.IsKeyDown(Keys.S) && Terrain.GetAltitude(Transform.LocalPosition + Transform.Backward * 7.5f * Time.ElapsedGameTime) < 0.25) Transform.LocalPosition += Transform.Backward * Time.ElapsedGameTime * 7.5f; // move backward
            if (InputManager.IsKeyDown(Keys.A) && Terrain.GetAltitude(Transform.LocalPosition + Transform.Left * 7.5f * Time.ElapsedGameTime) < 0.25) Transform.LocalPosition += Transform.Left * Time.ElapsedGameTime * 7.5f; // move right
            if (InputManager.IsKeyDown(Keys.D) && Terrain.GetAltitude(Transform.LocalPosition + Transform.Right * 7.5f * Time.ElapsedGameTime) < 0.25) Transform.LocalPosition += Transform.Right * Time.ElapsedGameTime * 7.5f; // move left

            // change the Y position corresponding to the terrain (maze)
            Transform.LocalPosition = new Vector3(Transform.LocalPosition.X, Terrain.GetAltitude(Transform.LocalPosition), Transform.LocalPosition.Z) + Vector3.Up;

            base.Update();
        }
    }
}
