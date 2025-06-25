using System.Diagnostics;
using System.Windows.Markup;
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
        Texture? wood, vase;                          // texture to use for rendering
        RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                       // screen filling quad for post processing
        readonly bool useRenderTarget = true;   // required for post processing

        public SceneGraph sceneGraph = new();          // scene graph for managing objects
        public SceneNode? teapot, floor;               // scene nodes for the meshes

        public Matrix4 worldToCamera;
        public Matrix4 cameraToScreen;
        public float angle90degrees = MathF.PI / 2;

        public List<Light> lights = new List<Light>();


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
            vase = new Texture("../../../assets/vase.jpg");


            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            // prepare the scene graph
            teapot = new SceneNode { Mesh = teapotMesh, Texture = vase, Shader = shader };
            floor = new SceneNode { Mesh = floorMesh, Texture = wood, Shader = shader };


            sceneGraph.Root.AddChild(teapot);
            sceneGraph.Root.AddChild(floor);

            // Camera and projection
            worldToCamera = Matrix4.CreateTranslation(new Vector3(18, -3f, 0)) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(1, 30, 0), angle90degrees);
            cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60.0f),
                (float)screen.width / screen.height, 0.1f, 1000);

            // Light setup
            // Define multiple lights
            lights.Add(new Light(new Vector3(-5, 10, 0), new Vector3(1, 1, 1), 1.2f));
            lights.Add(new Light(new Vector3(5, 10, 0), new Vector3(1, 0, 0), 1.0f));
            lights.Add(new Light(new Vector3(0, 10, 5), new Vector3(0, 1, 0), 0.8f));
            lights.Add(new Light(new Vector3(0, 10, -5), new Vector3(0, 0, 1), 0.8f));
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


            // Compose object-to-world transforms for each node
            teapot.LocalTransform = Matrix4.CreateScale(0.5f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a) *
                                    Matrix4.CreateTranslation(0, 0, 0);

            floor.LocalTransform = Matrix4.CreateScale(4.0f) *
                                   Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 1) *
                                   Matrix4.CreateTranslation(0, -1.5f, 0);

            // update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * MathF.PI) a -= 2 * MathF.PI;


            // Render the scene graph
            sceneGraph.Render(worldToCamera, cameraToScreen, lights, shader, wood);
        }
    }
}