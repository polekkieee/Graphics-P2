#version 330

in vec2 uv;
in vec2 positionFromBottomLeft;
uniform sampler2D pixels;

out vec3 outputColor;

void main()
{
    float aberrationStrength = 0.008; 
    float dist = distance(uv, vec2(0.5, 0.5));
    vec2 offset = (uv - vec2(0.5)) * aberrationStrength;

    float r = texture(pixels, uv + offset).r;
    float g = texture(pixels, uv).g;
    float b = texture(pixels, uv - offset).b;
    vec3 color = vec3(r, g, b);

    float vignette = smoothstep(0.75, 0.5, dist);
    color *= vignette;

    float scanline = 0.8 + 0.2 * sin(uv.y * 1200.0); 
    color *= scanline;

    outputColor = color;
}
