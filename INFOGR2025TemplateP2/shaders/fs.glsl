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

#define MAX_SPOTLIGHTS 4
uniform vec3 spotPositions[MAX_SPOTLIGHTS];
uniform vec3 spotDirections[MAX_SPOTLIGHTS];
uniform float spotAngles[MAX_SPOTLIGHTS];
uniform vec3 spotColors[MAX_SPOTLIGHTS];
uniform float spotIntensities[MAX_SPOTLIGHTS];

out vec4 outputColor;

void main()
{
    vec3 baseColor = texture(diffuseTexture, uv).rgb;
    vec3 norm = normalize(normalWorld.xyz);
    vec3 fragPos = positionWorld.xyz;
    vec3 viewDir = normalize(cameraPosition - fragPos);

    vec3 result = vec3(0.0);

    // Ambient light
    vec3 ambient = 0.1 * baseColor;
    result += ambient;

    // Point lights
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

    // Spotlights
    for (int i = 0; i < MAX_SPOTLIGHTS; ++i)
    {
        vec3 lightToFrag = fragPos - spotPositions[i];
        vec3 lightDir = normalize(-lightToFrag);
        float theta = dot(lightDir, normalize(spotDirections[i]));

        if (theta > spotAngles[i])  // hard cutoff
        {
            float diff = max(dot(norm, lightDir), 0.0);
            vec3 reflectDir = reflect(-lightDir, norm);
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);

            float attenuation = smoothstep(spotAngles[i], 1.0, theta);
            vec3 diffuse = diff * baseColor * spotColors[i] * spotIntensities[i] * attenuation;
            vec3 specular = spec * spotColors[i] * spotIntensities[i] * attenuation;

            result += diffuse + specular;
        }
    }

    outputColor = vec4(result, 1.0);
}
