// Screenspace GUI Shader

uniform sampler2D uTexture0;
uniform vec4 tint; // Color to multiply with the texture

// Vertex Code
void vertex() 
{
    layout(location = 0) in vec2 vPos;   // Vertex position (2D for screenspace)
    layout(location = 1) in vec2 vUv;    // UVs

    out vec2 Uv;

    void main()
    {
        // For GUI, we don't use any transformation matrices - directly use NDC coordinates
        gl_Position = vec4(vPos, 0.0, 1.0);
    
        // Pass UVs to fragment shader
        Uv = vUv;
    }
}

// Fragment Code
void fragment() 
{
    in vec2 Uv;
    out vec4 FragColor;

    void main()
    {
        // Sample the texture using the UV coordinates
        vec4 texColor = texture(uTexture0, Uv);
    
        // Apply tint by multiplying the texture color by the tint color
        vec4 finalColor = texColor * tint / 255.0;
    
        // Handle transparency: if the alpha value is below a threshold, discard the fragment
        if (finalColor.a < 0.001)
        {
            discard; // Discard fragments with low alpha
        }
    
        // Output the final color
        FRAG_COLOR = finalColor;
    }
}
