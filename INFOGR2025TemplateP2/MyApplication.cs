using System.Diagnostics;
using OpenTK.Mathematics;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        Mesh? teapotMesh, floorMesh;            // meshes to draw using OpenGL
        float a = 0;                            // teapot rotation angle
        readonly Stopwatch timer = new();       // timer for measuring frame duration
        Shader? shader;                         // shader to use for rendering
        Shader? postproc;                       // shader to use for post processing
        Texture? wood;                          // texture to use for rendering
        RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                       // screen filling quad for post processing
        readonly bool useRenderTarget = true;   // required for post processing

        SceneGraph sceneGraph = new();          // scene graph for managing objects
        SceneNode? teapot, floor;               // scene nodes for the meshes


        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            // load teapot
            teapotMesh = new Mesh("../../../assets/teapot.obj");
            floorMesh = new Mesh("../../../assets/floor.obj");

            // initialize stopwatch
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            // load a texture
            wood = new Texture("../../../assets/wood.jpg");
            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            // prepare the scene graph
            teapot = new SceneNode { Mesh = teapotMesh, Texture = wood, Shader = shader };
            floor = new SceneNode { Mesh = floorMesh, Texture = wood, Shader = shader };


            sceneGraph.Root.AddChild(teapot);
            sceneGraph.Root.AddChild(floor);

        }

        // tick for background surface
        public void Tick()
        {
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            float angle90degrees = MathF.PI / 2;

            // Compose object-to-world transforms for each node
            teapot.LocalTransform = Matrix4.CreateScale(0.5f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a) *
                                    Matrix4.CreateTranslation(0, 0, 0);

            floor.LocalTransform = Matrix4.CreateScale(4.0f) *
                                   Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 1) *
                                   Matrix4.CreateTranslation(0, -1.5f, 0);

            // Camera and projection
            Matrix4 worldToCamera = Matrix4.CreateTranslation(new Vector3(0, -14.5f, 0)) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), angle90degrees);
            Matrix4 cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60.0f),
                (float)screen.width / screen.height, 0.1f, 1000);

            // update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * MathF.PI) a -= 2 * MathF.PI;

            // Render the scene graph
            sceneGraph.Render(worldToCamera, cameraToScreen, shader, wood);
        }
    }
}