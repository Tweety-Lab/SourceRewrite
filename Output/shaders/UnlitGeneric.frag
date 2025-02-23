#version 330 core
in vec2 fUv;

uniform sampler2D uTexture0;
uniform int uBrightness; // Color to multiply with the texture

out vec4 FragColor;

void main()
{
    // Sample the texture using the UV coordinates
    vec4 texColor = texture(uTexture0, fUv);
    
    // Apply brightness by multiplying the texture color by the integer value
    FragColor = texColor * float(uBrightness);
}