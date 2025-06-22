using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Template
{
    public class Light
    {
        // Member variables
        public Vector3 Position { get; set; } = new Vector3(0, 10, 0); // Default position of the light
        public Vector3 Color { get; set; } = new Vector3(1, 0, 1); // Default color 
        public float Intensity { get; set; } = 1.0f; // Default intensity

        // Constructor
        public Light(Vector3 position, Vector3 color, float intensity)
        {
            Position = position;
            Color = color;
            Intensity = intensity;
        }
    }
}