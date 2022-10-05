using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class Material
    {
        // Variables
        public Effect effect;
        public Texture2D DiffuseTexture;

        // Camera properties
        public Matrix World { get; set; }
        public Camera Camera { get; set; }
        public Light Light { get; set; }

        // Material properties
        public Vector3 Diffuse { get; set; }
        public Vector3 Ambient { get; set; }
        public Vector3 Specular { get; set; }
        public float Shininess { get; set; }

        // Rendering properties
        public int Passes { get { return effect.CurrentTechnique.Passes.Count; } }
        public int CurrentTechnique { get; set; }

        public Material(Matrix world, Camera camera, Light light, ContentManager content, string filename, int currTechnique, float shininess, Texture2D texture)
        {
            effect = content.Load<Effect>(filename);

            World = world;
            Camera = camera;
            Light = light;

            CurrentTechnique = currTechnique;

            Shininess = shininess;
            DiffuseTexture = texture;
            Diffuse = Color.Gray.ToVector3();
            Ambient = Color.Gray.ToVector3();
            Specular = Color.Gray.ToVector3();
        }

        public virtual void Apply(int currentPass)
        {
            // Set technique
            effect.CurrentTechnique = effect.Techniques[CurrentTechnique];

            // Set Parameters
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(Camera.View);
            effect.Parameters["Projection"].SetValue(Camera.Projection);
            effect.Parameters["LightPosition"].SetValue(Light.Transform.Position);
            effect.Parameters["CameraPosition"].SetValue(Camera.Transform.Position);
            effect.Parameters["Shininess"].SetValue(Shininess);
            effect.Parameters["AmbientColor"].SetValue(Ambient);
            effect.Parameters["DiffuseColor"].SetValue(Diffuse);
            effect.Parameters["SpecularColor"].SetValue(Specular);
            effect.Parameters["DiffuseTexture"].SetValue(DiffuseTexture);

            // Apply to GPU
            effect.CurrentTechnique.Passes[currentPass].Apply();
        }
    }
}
