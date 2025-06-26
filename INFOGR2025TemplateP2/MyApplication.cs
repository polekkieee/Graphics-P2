using System.Diagnostics;
using System.Windows.Markup;
using INFOGR2025TemplateP2;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        Mesh? teapotMesh, floorMesh, teacupMesh, gorillaMesh, giraffeMesh, pandaMesh, bearMesh; // meshes to draw using OpenGL
        float a = 0, b = 0, c = 0, d = 0;       // teapot rotation angle
        readonly Stopwatch timer = new();       // timer for measuring frame duration
        Shader? shader;                         // shader to use for rendering
        Shader? postproc;                       // shader to use for post processing
        Texture? wood, vase, cat_text, gorilla_text, giraffe_text, panda_text, bear_text; // texture to use for rendering
        RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                       // screen filling quad for post processing
        readonly bool useRenderTarget = true;   // required for post processing

        public SceneGraph sceneGraph = new();          // scene graph for managing objects
        public SceneNode? teapot, floor, teacup1, teacup2, teacup3, teacup4, gorilla, gorilla1, gorilla2, gorilla3, gorilla11, gorilla22, gorilla33, giraffe, giraffe1, giraffe2, giraffe3, giraffe4, panda, panda1, panda11, bear, bear1, bear11;   // scene nodes for the meshes

        public Matrix4 worldToCamera;
        public Matrix4 cameraToScreen;
        public float angle90degrees = MathF.PI / 2;

        public List<Light> lights = new List<Light>();
        public List<SpotLight> spotLights = new List<SpotLight>();


        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            // load teapot
            teapotMesh = new Mesh("../../../assets/objects/teapot.obj");
            floorMesh = new Mesh("../../../assets/objects/floor.obj");
            teacupMesh = new Mesh("../../../assets/objects/teacup.obj");            
            gorillaMesh = new Mesh("../../../assets/objects/gorilla.obj");
            giraffeMesh = new Mesh("../../../assets/objects/giraffe.obj");
            pandaMesh = new Mesh("../../../assets/objects/panda.obj");           
            bearMesh = new Mesh("../../../assets/objects/bear.obj");
            

            // initialize stopwatch
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            // load a texture
            wood = new Texture("../../../assets/textures/wood.jpg");
            vase = new Texture("../../../assets/textures/vase.jpg");        
            gorilla_text = new Texture("../../../assets/textures/gorilla.png");
            giraffe_text = new Texture("../../../assets/textures/giraffe.png");
            panda_text = new Texture("../../../assets/textures/panda.png");               
            bear_text = new Texture("../../../assets/textures/bear.png");
           
            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            // prepare the scene graph
            teapot = new SceneNode { Mesh = teapotMesh, Texture = vase, Shader = shader };
            floor = new SceneNode { Mesh = floorMesh, Texture = wood, Shader = shader };
            teacup1 = new SceneNode { Mesh = teacupMesh, Texture = vase, Shader = shader };
            teacup2 = new SceneNode { Mesh = teacupMesh, Texture = vase, Shader = shader };
            teacup3 = new SceneNode { Mesh = teacupMesh, Texture = vase, Shader = shader };
            teacup4 = new SceneNode { Mesh = teacupMesh, Texture = vase, Shader = shader };
            gorilla = new SceneNode { Mesh = gorillaMesh, Texture = gorilla_text, Shader = shader };
            gorilla1 = new SceneNode { Mesh = gorillaMesh, Texture = gorilla_text, Shader = shader };
            gorilla2 = new SceneNode { Mesh = gorillaMesh, Texture = gorilla_text, Shader = shader };
            gorilla3 = new SceneNode { Mesh = gorillaMesh, Texture = gorilla_text, Shader = shader };
            gorilla11 = new SceneNode { Mesh = gorillaMesh, Texture = gorilla_text, Shader = shader };
            gorilla22 = new SceneNode { Mesh = gorillaMesh, Texture = gorilla_text, Shader = shader };
            gorilla33 = new SceneNode { Mesh = gorillaMesh, Texture = gorilla_text, Shader = shader };
            giraffe = new SceneNode { Mesh = giraffeMesh, Texture = giraffe_text, Shader = shader };
            giraffe1 = new SceneNode { Mesh = giraffeMesh, Texture = giraffe_text, Shader = shader };
            giraffe2 = new SceneNode { Mesh = giraffeMesh, Texture = giraffe_text, Shader = shader };
            giraffe3 = new SceneNode { Mesh = giraffeMesh, Texture = giraffe_text, Shader = shader };
            giraffe4 = new SceneNode { Mesh = giraffeMesh, Texture = giraffe_text, Shader = shader };           
            panda = new SceneNode { Mesh = pandaMesh, Texture = panda_text, Shader = shader };
            panda1 = new SceneNode { Mesh = pandaMesh, Texture = panda_text, Shader = shader };
            panda11 = new SceneNode { Mesh = pandaMesh, Texture = panda_text, Shader = shader };
            bear = new SceneNode { Mesh = bearMesh, Texture = bear_text, Shader = shader };
            bear1 = new SceneNode { Mesh = bearMesh, Texture = bear_text, Shader = shader };
            bear11 = new SceneNode { Mesh = bearMesh, Texture = bear_text, Shader = shader };
 


            sceneGraph.Root.AddChild(teapot);
            teapot.AddChild(teacup1);
            teapot.AddChild(teacup2);
            teapot.AddChild(teacup3);
            teapot.AddChild(teacup4);

            sceneGraph.Root.AddChild(floor);          
            
            sceneGraph.Root.AddChild(gorilla);
            gorilla.AddChild(gorilla1);
            gorilla1.AddChild(gorilla11);
            gorilla.AddChild(gorilla2);
            gorilla2.AddChild(gorilla22);
            gorilla.AddChild(gorilla3);
            gorilla3.AddChild(gorilla33);
                        
            sceneGraph.Root.AddChild(giraffe);
            giraffe.AddChild(giraffe1); 
            giraffe.AddChild(giraffe2);
            giraffe.AddChild(giraffe3);
            giraffe.AddChild(giraffe4);
      
            sceneGraph.Root.AddChild(panda);  
            panda.AddChild(panda1);
            panda1.AddChild(panda11);
           
            sceneGraph.Root.AddChild(bear);
            bear.AddChild(bear1);
            bear1.AddChild(bear11);
   

            // Camera and projection
            worldToCamera = Matrix4.CreateTranslation(new Vector3(-50, -2, 0)) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 30, 0), -angle90degrees);
            cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60.0f),
                (float)screen.width / screen.height, 0.1f, 1000);

            // Light setup
            lights.Add(new Light(new Vector3(5, 10, 0), new Vector3(1, 1, 1), 1.2f));
            lights.Add(new Light(new Vector3(-5, 10, 0), new Vector3(1, 0, 0), 1.0f));
            lights.Add(new Light(new Vector3(0, 10, 5), new Vector3(0, 1, 0), 0.8f));
            lights.Add(new Light(new Vector3(0, 10, -5), new Vector3(0, 0, 1), 0.8f));

            //spotlights
            spotLights.Add(new SpotLight(
                new Vector3(0, 10, 10),
                new Vector3(0, -1, -1),
                MathHelper.DegreesToRadians(20),
                new Vector3(1, 1, 1),
                1.5f
            ));
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
                                    Matrix4.CreateTranslation(0, 3*MathF.Sin(b) -8, 0);
            teacup1.LocalTransform = Matrix4.CreateScale(1f) *
                                     Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -a) *
                                     Matrix4.CreateTranslation(6f, MathF.Cos(d) - 3f, 6f);
            teacup2.LocalTransform = Matrix4.CreateScale(1f) *
                                     Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -a) *
                                     Matrix4.CreateTranslation(-6f, MathF.Sin(d) - 3f, 6f);
            teacup3.LocalTransform = Matrix4.CreateScale(1f) *
                                     Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -a) *
                                     Matrix4.CreateTranslation(6f, -MathF.Sin(d) - 3f, -6f);
            teacup4.LocalTransform = Matrix4.CreateScale(1f) *
                                     Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -a) *
                                     Matrix4.CreateTranslation(-6f, -MathF.Cos(d) - 3f, -6f);

            floor.LocalTransform = Matrix4.CreateScale(8.0f) *
                                   Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 1) *
                                   Matrix4.CreateTranslation(0, -1.5f, 0);

            
            gorilla.LocalTransform = Matrix4.CreateScale(10f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), b) *
                                    Matrix4.CreateTranslation(20, -15 + Math.Abs(MathF.Sin(a))*10, 20);            
            gorilla1.LocalTransform = Matrix4.CreateScale(0.2f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -c) *
                                    Matrix4.CreateTranslation(0, 1.7f, 0);
            gorilla11.LocalTransform = Matrix4.CreateScale(0.5f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), d) *
                                    Matrix4.CreateTranslation(0, 1.65f, 0);
            gorilla2.LocalTransform = Matrix4.CreateScale(0.2f) *                                    
                                    Matrix4.CreateTranslation(0.4f, 0, 0.4f) * 
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -c);
            gorilla22.LocalTransform = Matrix4.CreateScale(0.5f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), d) *
                                    Matrix4.CreateTranslation(0, 1.65f, 0);
            gorilla3.LocalTransform = Matrix4.CreateScale(0.2f) *
                                    Matrix4.CreateTranslation(-0.4f,0, -0.4f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -c); ;
            gorilla33.LocalTransform = Matrix4.CreateScale(0.5f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), d) *
                                    Matrix4.CreateTranslation(0, 1.65f, 0);


            giraffe.LocalTransform = Matrix4.CreateScale(5f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 1) *
                                    Matrix4.CreateTranslation(-20 + 10*MathF.Sin(b), -15f, -20 + 5*MathF.Sin(c));
            giraffe1.LocalTransform = Matrix4.CreateScale(0.2f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), d) *
                                    Matrix4.CreateTranslation(1.5f, 0, 1.5f);
            giraffe2.LocalTransform = Matrix4.CreateScale(0.2f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -d) *
                                    Matrix4.CreateTranslation(1.5f, 0, -1.5f);
            giraffe3.LocalTransform = Matrix4.CreateScale(0.2f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), d) *
                                    Matrix4.CreateTranslation(-1.5f, 0, 1.5f);
            giraffe4.LocalTransform = Matrix4.CreateScale(0.2f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -d) *
                                    Matrix4.CreateTranslation(-1.5f, 0, -1.5f);


            panda.LocalTransform = Matrix4.CreateScale(10f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), MathF.Sin(b)*0.5f) *
                                    Matrix4.CreateTranslation(20, -15, -20);
            panda1.LocalTransform = Matrix4.CreateScale(0.2f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -c) *
                                    Matrix4.CreateTranslation(0, 0.8f, 0);
            panda11.LocalTransform = Matrix4.CreateScale(0.5f) *
                               Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), d) *
                               Matrix4.CreateTranslation(0, 0.8f, 0);


            bear.LocalTransform = Matrix4.CreateScale(10f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -(b + MathF.PI)) *
                                    Matrix4.CreateTranslation(-20 - MathF.Cos(b)*10, -15f, 20 - MathF.Sin(b)*10);           
            bear1.LocalTransform = Matrix4.CreateScale(0.2f) *
                                    Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -c) *
                                    Matrix4.CreateTranslation(0, 1.2f, 0);
            bear11.LocalTransform = Matrix4.CreateScale(0.5f) *
                                   Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), d) *
                                   Matrix4.CreateTranslation(0, 1.2f, 0);



            // update rotation
            a += 0.0005f * frameDuration;
            b += 0.001f * frameDuration;
            c += 0.002f * frameDuration;
            d += 0.004f * frameDuration;

            if (a > 2 * MathF.PI) a -= 2 * MathF.PI;
            if (b > 2 * MathF.PI) b -= 2 * MathF.PI;
            if (c > 2 * MathF.PI) c -= 2 * MathF.PI;
            if (d > 2 * MathF.PI) d -= 2 * MathF.PI;

            //apply vignetting and chromatic aberration
            target.Bind();


            // Render the scene graph
            sceneGraph.Render(worldToCamera, cameraToScreen, lights, spotLights, shader, wood);

            target.Unbind();

            postproc.Use();

            int vignetteLoc = GL.GetUniformLocation(postproc.programID, "vignetteStrength");
            int chromAbLoc = GL.GetUniformLocation(postproc.programID, "chromAbOffset");

            GL.Uniform1(vignetteLoc, 1.5f);        
            GL.Uniform1(chromAbLoc, 0.003f);     
            quad.Render(postproc, target.GetTexture());
        }
    }
}