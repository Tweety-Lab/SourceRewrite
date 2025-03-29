// Unlit Object Shader

uniform sampler2D uTexture0;
uniform vec4 color; // Color to multiply with the texture

// Vertex Code
void vertex() 
{
    // Input attributes
    layout(location = 0) in vec3 vPos;    // Vertex position
    layout(location = 1) in vec3 vNormal; // Vertex normal (from location 1)
    layout(location = 2) in vec2 vUv;     // UVs (from location 2)

    out vec2 Uv;
    out vec3 VERT_NORMAL; // Pass the normal to the fragment shader
    out vec3 FragPos; // Pass the fragment position to the fragment shader

    void main()
    {
        // Transform the vertex position to clip space
        gl_Position = PROJECTION_MATRIX * VIEW_MATRIX * MODEL_MATRIX * vec4(vPos, 1.0);
    
        // Pass the UV and normal to the fragment shader
        Uv = vUv;
        VERT_NORMAL = normalize(mat3(transpose(inverse(MODEL_MATRIX))) * vNormal); // Transform normal
        FragPos = vec3(MODEL_MATRIX * vec4(vPos, 1.0)); 
    }
}

// Fragment Code
void fragment() 
{
    in vec2 Uv;

    void main()
    {
        // Sample the texture using the UV coordinates
        vec4 texColor = texture(uTexture0, Uv);

        // Apply tint by multiplying the texture color by the tint color
        vec4 finalColor = texColor * vec4(color.r / 255.0, color.g / 255.0, color.b / 255.0, color.a);

        // Handle transparency: if the alpha value is below a threshold, discard the fragment
        if (finalColor.a < 0.1)
        {
            discard;
        }

        // Output the final color
        FRAG_COLOR = finalColor;
    }
}
