#version 330 core

out vec4 outputColor;

uniform vec3 lightColor;
uniform vec3 objectColor;

void main()
{
    // outputColor = vec4(1.0, 0.5, 0.8, 1.0);
    outputColor = vec4(objectColor * lightColor, 1.0);
}