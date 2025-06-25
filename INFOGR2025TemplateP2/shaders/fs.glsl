#version 330

in vec4 positionWorld;
in vec4 normalWorld;
in vec2 uv;

uniform sampler2D diffuseTexture;
uniform vec3 materialColor;
uniform vec3 cameraPosition;

#define MAX_LIGHTS 4
uniform vec3 lightPositions[MAX_LIGHTS];
uniform vec3 lightColors[MAX_LIGHTS];
uniform float lightIntensities[MAX_LIGHTS];

out vec4 outputColor;

void main()
{
    vec3 baseColor = texture(diffuseTexture, uv).rgb;
    vec3 norm = normalize(normalWorld.xyz);
    vec3 fragPos = positionWorld.xyz;
    vec3 viewDir = normalize(cameraPosition - fragPos);

    vec3 result = vec3(0.0);
    vec3 ambient = 0.1 * baseColor;

    for (int i = 0; i < MAX_LIGHTS; ++i)
    {
        vec3 lightDir = normalize(lightPositions[i] - fragPos);
        vec3 reflectDir = reflect(-lightDir, norm);

        float diff = max(dot(norm, lightDir), 0.0);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);

        vec3 diffuse = 0.7 * diff * baseColor * lightColors[i] * lightIntensities[i];
        vec3 specular = 0.2 * spec * lightColors[i] * lightIntensities[i];

        result += diffuse + specular;
    }

    outputColor = vec4(ambient + result, 1.0);
}
