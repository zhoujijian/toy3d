#version 330 core
layout (location = 0) in vec2 aPositions;
layout (location = 1) in vec2 aTextureCoords;

out vec2 frag_aTextureCoords;

uniform bool chaos;
uniform bool confuse;
uniform bool shake;
uniform float time;

void main()
{
    gl_Position = vec4(aPositions.x, aPositions.y, 0.0, 1.0);

    if (chaos) {
        float strength = 0.3;
	frag_aTextureCoords = vec2(aTextureCoords.x + sin(time) * strength, aTextureCoords.y + cos(time) * strength);
    }
    else if (confuse) {
        frag_aTextureCoords = vec2(1 - aTextureCoords.x, 1 - aTextureCoords.y);
    }
    else {
        frag_aTextureCoords = aTextureCoords;
    }
    
    if (shake) {
        float strength = 0.01;
	gl_Position.x += cos(time * 10) * strength;
	gl_Position.y += cos(time * 15) * strength;
    }
}