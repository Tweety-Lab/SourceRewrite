uniform sampler2D uTexture0;
uniform vec4 tint; // Color to multiply with the texture

// Fragment Code
void fragment() 
{
    in vec2 fUv;
    out vec4 FragColor;

    void main()
    {
        // Sample the texture using the UV coordinates
        vec4 texColor = texture(uTexture0, fUv);

        // Apply tint by multiplying the texture color by the tint color
        FragColor = texColor * tint / 255;
    }
}
