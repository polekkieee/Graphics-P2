#version 330

in vec4 positionWorld;
in vec4 normalWorld;
in vec2 uv;

uniform sampler2D diffuseTexture;
uniform vec3 materialColor;
uniform vec3 lightPosition;
uniform vec3 lightColor;
uniform float lightIntensity;
uniform vec3 cameraPosition;

out vec4 outputColor;

void main()
{
    vec3 baseColor = texture(diffuseTexture, uv).rgb;

    vec3 norm = normalize(normalWorld.xyz);
    vec3 fragPos = positionWorld.xyz;
    vec3 lightDir = normalize(lightPosition - fragPos);
    vec3 viewDir = normalize(cameraPosition - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);

    float diff = max(dot(norm, lightDir), 0.0);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);

    vec3 ambient = 0.1 * baseColor;
    vec3 diffuse = 0.7 * diff * baseColor * lightColor * lightIntensity;
    vec3 specular = 0.2 * spec * lightColor * lightIntensity;

    vec3 result = ambient + diffuse + specular;
    outputColor = vec4(result, 1.0);
}
