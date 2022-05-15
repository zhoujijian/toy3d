#version 330 core

in vec2 frag_aTextureCoords;
out vec4 FragColor;

uniform sampler2D uScreenTexture;

uniform bool chaos;
uniform bool confuse;
uniform bool shake;

uniform vec2  offsets[9];
uniform int   edge_kernel[9];
uniform float blur_kernel[9];

void main()
{
    vec3 sample[9];
    if(chaos || shake) {
        for(int i = 0; i < 9; i++)
            sample[i] = vec3(texture(uScreenTexture, frag_aTextureCoords.st + offsets[i]));
    }

    FragColor = vec4(0.0f);
    
    if(chaos) {
        for(int i = 0; i < 9; i++)
            FragColor += vec4(sample[i] * edge_kernel[i], 0.0f);
        FragColor.a = 1.0f;
    }
    else if(confuse) {
        FragColor = vec4(1.0 - texture(uScreenTexture, frag_aTextureCoords.st).rgb, 1.0);
    }
    else if(shake) {
        for(int i = 0; i < 9; i++)
            FragColor += vec4(sample[i] * blur_kernel[i], 0.0f);
        FragColor.a = 1.0f;
    }
    else {
	FragColor = texture(uScreenTexture, frag_aTextureCoords.st);
    }

    // FragColor = vec4(offsets[2].x, offsets[2].y, 1.0, 1.0);
    // FragColor = vec4(blur_kernel[0], blur_kernel[1], 1.0, 1.0);
    // FragColor = texture(uScreenTexture, frag_aTextureCoords.st);
}