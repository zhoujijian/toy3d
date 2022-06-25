#version 330 core

out vec4 outputColor;

uniform vec3 lightColor;
uniform vec3 objectColor;
uniform vec3 lightWorldPosition;

in vec3 fragWorldNormal;
in vec3 fragWorldPosition;

void main()
{
    float ambientStrength = 0.5;
    vec3 ambient = ambientStrength * lightColor;
    vec3 lightDirection = normalize(lightWorldPosition - fragWorldPosition);
    vec3 diffuse = max(dot(lightDirection, normalize(fragWorldNormal)), 0.0) * lightColor;
    outputColor = vec4((ambient + diffuse) * objectColor, 1.0);
}