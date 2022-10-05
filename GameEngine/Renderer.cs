using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class Renderer : Component, IRenderable
    {
        public Material Material { get; set; }
        public Model ObjectModel;
        public Transform ObjectTransform;
        public int CurrentTechnique;
        public GraphicsDevice g;

      public Renderer(Model objModel, Transform objTransform, Camera camera, ContentManager content, GraphicsDevice graphicsDevice, ..., string filename, ...)
      {
            if (filename != null) Material = new Material(objTransfrom.World, ...);
            else Material = null;
            ... // set the other properties;
}
        public virtual void Draw()
        {
            if (Material != null)
            {
                Material.Camera = Camera; // Update Material's properties
                
                for (int i = 0; i < Material.Passes; i++)
                {
                    Material.Apply(i); // Look at the Material's Apply method
                    foreach (ModelMesh mesh in objModel.Meshes)
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            g.SetVertexBuffer(...)
                            g.Indices = ...
                            g.DrawIndexedPrimitives(...);
                        }
                }
            }
            else objModel.Draw(Transform.World, Camera.View, Camera.Projection);
        }
    }
}
