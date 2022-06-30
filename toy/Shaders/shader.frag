#version 330 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};

out vec4 outputColor;

uniform vec3 lightColor;
uniform vec3 objectColor;
uniform vec3 lightWorldPosition;
uniform vec3 viewWorldPosition;

in vec3 fragWorldNormal;
in vec3 fragWorldPosition;

void main()
{
    float ambientStrength = 0.5;
    vec3 ambient = ambientStrength * lightColor;
    vec3 lightDirection = normalize(lightWorldPosition - fragWorldPosition);
    vec3 diffuse = max(dot(lightDirection, normalize(fragWorldNormal)), 0.0) * lightColor;

    float specularStrength = 0.5;
    vec3 viewDirection = normalize(viewWorldPosition - fragWorldPosition);
    vec3 reflectLightDirection = reflect(-lightDirection, fragWorldNormal);
    float spec = pow(max(dot(viewDirection, reflectLightDirection), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;
    
    outputColor = vec4((ambient + diffuse + specular) * objectColor, 1.0);
    // outputColor = vec4(normalize(fragWorldNormal), 1.0);
}