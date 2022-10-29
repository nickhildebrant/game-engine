using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class Renderer : Component, IRenderable
    {
        public Material Material { get; set; }
        public Model ObjectModel;
        public Transform ObjectTransform;
        public GraphicsDevice graphicsDevice;
        public Camera Camera;
        public Light Light;
        public int CurrentTechnique;

        public Renderer(Model objModel, Transform objTransform, Camera camera, Light light, ContentManager content, 
              GraphicsDevice gfxDevice, float shininess, Texture2D texture, string filename, int currTechnique)
        {
            if (filename != null) Material = new Material(objTransform.World, camera, light, content, filename, currTechnique, shininess, texture);
            else Material = null;

            // set the other properties;
            ObjectModel = objModel;
            ObjectTransform = objTransform;
            graphicsDevice = gfxDevice;
            Camera = camera;
            Light = light;
            CurrentTechnique = currTechnique;
        }
       
        public virtual void Draw()
        {
            if (Material != null)
            {
                Material.World = ObjectTransform.World;
                Material.Camera = Camera; // Update Material's properties
                
                for (int i = 0; i < Material.Passes; i++)
                {
                    Material.Apply(i); // Look at the Material's Apply method
                    foreach (ModelMesh mesh in ObjectModel.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            graphicsDevice.SetVertexBuffer(part.VertexBuffer);
                            graphicsDevice.Indices = part.IndexBuffer;
                            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.PrimitiveCount);
                        }
                    }
                }
            }
            else ObjectModel.Draw(Transform.World, Camera.View, Camera.Projection);
        }
    }
}
