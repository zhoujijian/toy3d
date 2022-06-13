#version 330 core

out vec4 outputColor;

uniform vec3 lightColor;
uniform vec3 objectColor;
uniform vec3 lightPosition;

in vec3 fragWorldNormal;
in vec3 fragWorldPosition;

void main()
{
    float ambientStrength = 0.5;
    vec3 ambient = ambientStrength * lightColor;
    outputColor = vec4(ambient * objectColor, 1.0);
}