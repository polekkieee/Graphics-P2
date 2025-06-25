#version 330

in vec2 uv;
out vec4 fragColor;

uniform sampler2D screenTexture;
uniform float vignetteStrength = 1.5;
uniform float chromAbOffset = 0.003;

void main()
{
    vec2 center = vec2(0.5, 0.5);
    float dist = distance(uv, center);

    float vignette = 1.0 - smoothstep(0.4, 1.0, dist * vignetteStrength);

    vec2 offset = normalize(uv - center) * chromAbOffset * dist;

    vec3 col;
    col.r = texture(screenTexture, uv + offset).r;
    col.g = texture(screenTexture, uv).g;
    col.b = texture(screenTexture, uv - offset).b;

    col *= vignette;

    fragColor = vec4(col, 1.0);
}
