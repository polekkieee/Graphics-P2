using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Template
{
    public class SceneNode
    {
        // Member variables
        public Mesh? Mesh { get; set; }
        public Texture? Texture { get; set; }
        public Shader? Shader { get; set; }
        public Matrix4 LocalTransform { get; set; } = Matrix4.Identity;
        public List<SceneNode> Children { get; private set; } = new List<SceneNode>();

        public void Render(Matrix4 parentTransform, Matrix4 worldToCamera, Matrix4 cameraToScreen, Light light, Shader defaultShader, Texture defaultTexture)
        {
            // Combine the parent's world transform with this node's local transform
            // to get the final world transform for this node.
            Matrix4 worldTransform = LocalTransform * parentTransform;

            // If this node has a mesh, render it.
            if (Mesh != null)
            {
                // Determine which shader and texture to use.
                // Prioritize the node's own components, otherwise use the defaults passed down.
                Shader shaderToUse = this.Shader ?? defaultShader;
                Texture textureToUse = this.Texture ?? defaultTexture;

                // Calculate the final transformation matrices required by the shader.
                Matrix4 objectToWorld = worldTransform;
                Matrix4 objectToScreen = objectToWorld * worldToCamera * cameraToScreen;

                // Render the mesh with the calculated transformations and materials.
                Mesh.Render(shaderToUse, objectToScreen, objectToWorld, worldToCamera, light, textureToUse);
            }

            // Recursively render all child nodes.
            // Pass down this node's world transform as the parent transform for its children.
            foreach (var child in Children)
            {
                child.Render(worldTransform, worldToCamera, cameraToScreen, light, defaultShader, defaultTexture);
            }
        }

        public void AddChild(SceneNode child)
        {
            Children.Add(child);
        }
    }
}
