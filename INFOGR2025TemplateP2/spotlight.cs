using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2025TemplateP2
{
    public class SpotLight
    {
        public Vector3 Position;
        public Vector3 Direction;
        public float CutoffAngle; // in radians
        public Vector3 Color;
        public float Intensity;

        //spotlight constructor
        public SpotLight(Vector3 pos, Vector3 dir, float cutoffAngle, Vector3 color, float intensity)
        {
            Position = pos;
            Direction = dir.Normalized();
            CutoffAngle = cutoffAngle;
            Color = color;
            Intensity = intensity;
        }
    }
}
