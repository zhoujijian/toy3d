#version 330 core
layout (location = 0) in vec2 aPositions;
layout (location = 1) in vec2 aTextureCoords;

out vec2 frag_aTextureCoords;

void main()
{
    gl_Position = vec4(aPositions.x, aPositions.y, 0.0, 1.0);
    frag_aTextureCoords = aTextureCoords;
}