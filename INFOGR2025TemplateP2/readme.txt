Team members: (names and student IDs)
* Dagmar Smid 9660925
* Sam Polak 5467365
* Naomi van Drimmelen 6880576

Tick the boxes below for the implemented features. Add a brief note only if necessary, e.g., if it's only partially working, or how to turn it on.

Formalities:
[x] This readme.txt
[x] Cleaned (no obj/bin folders)
[x] Demonstration scene(s) with all implemented features
[ ] (Optional) Screenshots: make it clear which feature is demonstrated in which screenshot

Minimum requirements implemented:
[x] Camera: position and orientation controls
Controls: ...
[x] Model matrix: for each mesh, stored as part of the scene graph
[x] Scene graph data structure: tree hierarchy, no limitation on breadth or depth or size
[x] Rendering: recursive scene graph traversal, correct model matrix concatenation
[x] Shading in fragment shader: diffuse, glossy, uniform variable for ambient light color
[x] Point light: at least 1, position/color may be hardcoded

Bonus features implemented:
[x] Multiple point lights: at least 4, uniform variables to change position and color at runtime
[x] Spot lights: position, center direction, opening angle, color
[ ] Environment mapping: cube or sphere mapping, used in background and/or reflections
[ ] Frustum culling: in C# code, using scene graph node bounds, may be conservative
[ ] Bump or normal mapping
[ ] Shadow mapping: render depth map to texture, only hard shadows required, some artifacts allowed
[x] Vignetting and chromatic aberrations: darker corners, color channels separated more near corners
[ ] Color grading: color cube lookup table
[ ] Blur: separate horizontal and vertical blur passes, variable blur size
[ ] HDR glow: HDR render target, blur in HDR, tone-mapping
[ ] Depth of field: blur size based on distance from camera, some artifacts allowed
[ ] Ambient occlusion: darker in tight corners, implemented as screen-space post process
[ ] ...

Notes:
source used for the chromatic aberration and vignetting; https://github.com/kylemcdonald/ofxCameraFilter
source used for the start up of the scene graph structure; https://openai.com/chatgpt/overview/
source used for the textures of the objects; https://gctrader.com
source used for the spotlights feature; https://github.com/opentk/LearnOpenTK/blob/master/Chapter2/5-LightCasters-Spotlight/Shaders/lighting.frag

The controls of our demo work with the aswdqe keys and the arrow keys. 
The W rotates around the x-axis, pitched up
The S rotates around the x-axis, pitched down
The A rotates around the y-axis, yaws left
The D rotates around the y-axis, yaws right
The Q rotates around the z-axis, rolls counter-clockwise
The E rotates around the z-axis, rolls clockwise

The Arrow Up moves forward (along z)
The Arrow Down moves backwards (along z)
The Arrow Left straves right (along x)
The Arrow Right straves left (along x)
Page Up moves up (along y)
Page Down moves down (along y)