using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Template
{
    // Mesh and MeshLoader based on work by JTalton; https://web.archive.org/web/20160123042419/www.opentk.com/node/642
    // Only triangles and quads with vertex positions, normals, and texture coordinates are supported
    public class Mesh
    {
        // data members
        public readonly string filename;        // for improved error reporting
        public List<ObjVertex> vertices = new();// vertices (positions and normals in Object Space, and texture coordinates)
        public List<ObjTriangle> triangles = new(); // triangles (3 indices into the vertices array)
        public List<ObjQuad> quads = new();     // quads (4 indices into the vertices array)
        int vertexBufferId;                     // vertex buffer object (VBO) for vertex data
        int triangleBufferId;                   // element buffer object (EBO) for triangle vertex indices
        int quadBufferId;                       // element buffer object (EBO) for quad vertex indices (not in Modern OpenGL)

        // constructor
        public Mesh(string filename)
        {
            this.filename = filename;
            Util.ImportMesh(this, filename);
        }

        // initialization; called during first render
        public void Prepare()
        {
            if (vertexBufferId == 0)
            {
                // generate interleaved vertex data array (uv/normal/position per vertex)
                GL.GenBuffers(1, out vertexBufferId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
                if (!OpenTKApp.isMac) GL.ObjectLabel(ObjectLabelIdentifier.Buffer, vertexBufferId, 8 + filename.Length, "VBO for " + filename);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Count * Marshal.SizeOf(typeof(ObjVertex))), ref CollectionsMarshal.AsSpan(vertices)[0], BufferUsageHint.StaticDraw);

                // generate triangle index array
                GL.GenBuffers(1, out triangleBufferId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
                if (!OpenTKApp.isMac) GL.ObjectLabel(ObjectLabelIdentifier.Buffer, triangleBufferId, 17 + filename.Length, "triangle EBO for " + filename);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Count * Marshal.SizeOf(typeof(ObjTriangle))), ref CollectionsMarshal.AsSpan(triangles)[0], BufferUsageHint.StaticDraw);

                if (OpenTKApp.allowPrehistoricOpenGL)
                {
                    // generate quad index array
                    GL.GenBuffers(1, out quadBufferId);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
                    if (!OpenTKApp.isMac) GL.ObjectLabel(ObjectLabelIdentifier.Buffer, quadBufferId, 13 + filename.Length, "quad EBO for " + filename);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Count * Marshal.SizeOf(typeof(ObjQuad))), ref CollectionsMarshal.AsSpan(quads)[0], BufferUsageHint.StaticDraw);
                }
            }
        }

        // render the mesh using the supplied shader and matrix
        public void Render(Shader shader, Matrix4 objectToScreen, Matrix4 objectToWorld, Texture texture)
        {
            // on first run, prepare buffers
            Prepare();

            // enable shader
            GL.UseProgram(shader.programID);

            // enable texture
            int textureLocation = GL.GetUniformLocation(shader.programID, "diffuseTexture");    // get the location of the shader variable
            int textureUnit = 0;                                                                // choose a texture unit
            GL.Uniform1(textureLocation, textureUnit);                                          // set the value of the shader variable to that texture unit
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);                               // make that the active texture unit
            GL.BindTexture(TextureTarget.Texture2D, texture.id);                                // bind the texture as a 2D image texture to the active texture unit

            // pass transforms to vertex shader
            GL.UniformMatrix4(shader.uniform_objectToScreen, false, ref objectToScreen);
            GL.UniformMatrix4(shader.uniform_objectToWorld, false, ref objectToWorld);

            // enable position, normal and uv attribute arrays corresponding to the shader "in" variables
            GL.EnableVertexAttribArray(shader.in_vertexPositionObject);
            GL.EnableVertexAttribArray(shader.in_vertexNormalObject);
            GL.EnableVertexAttribArray(shader.in_vertexUV);

            // bind vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);

            // link vertex attributes to shader parameters 
            GL.VertexAttribPointer(shader.in_vertexUV, 2, VertexAttribPointerType.Float, false, 32, 0);
            GL.VertexAttribPointer(shader.in_vertexNormalObject, 3, VertexAttribPointerType.Float, true, 32, 2 * 4);
            GL.VertexAttribPointer(shader.in_vertexPositionObject, 3, VertexAttribPointerType.Float, false, 32, 5 * 4);

            // bind triangle index data and render
            if (triangles != null && triangles.Count > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
                GL.DrawArrays(PrimitiveType.Triangles, 0, triangles.Count * 3);
            }

            // bind quad index data and render
            if (quads != null && quads.Count > 0)
            {
                if (OpenTKApp.allowPrehistoricOpenGL)
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
                    GL.DrawArrays(PrimitiveType.Quads, 0, quads.Count * 4);
                }
                else throw new Exception("Quads not supported in Modern OpenGL");
            }

            // restore previous OpenGL state
            GL.UseProgram(0);
        }

        // layout of a single vertex
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjVertex
        {
            public Vector2 TexCoord;
            public Vector3 Normal;
            public Vector3 Vertex;
        }

        // layout of a single triangle
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjTriangle
        {
            public int Index0, Index1, Index2;
        }

        // layout of a single quad
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjQuad
        {
            public int Index0, Index1, Index2, Index3;
        }
    }
}