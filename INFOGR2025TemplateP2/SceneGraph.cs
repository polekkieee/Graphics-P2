using OpenTK.Mathematics;

namespace Template
{
    public class SceneGraph
    {
        // The root node of the scene graph. It doesn't typically have a mesh itself
        // but acts as a parent for all top-level objects in the scene.
        public SceneNode Root { get; private set; } = new SceneNode();

        public void Render(Matrix4 worldToCamera, Matrix4 cameraToScreen, List<Light> lights, Shader defaultShader, Texture defaultTexture)
        {
            // Start the recursive rendering from the root node.
            // The initial parent transform is the identity matrix.
            Root.Render(Matrix4.Identity, worldToCamera, cameraToScreen, lights, defaultShader, defaultTexture);
        }
    }
}
