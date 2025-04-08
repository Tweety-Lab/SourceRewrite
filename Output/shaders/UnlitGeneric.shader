// Unlit Object Shader

uniform sampler2D basetexture;
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
        // Default fallback values in case the uniforms are not set
        vec4 defaultColor = vec4(1.0, 1.0, 1.0, 1.0); // White color by default
        vec4 texColor = texture(basetexture, Uv);

        // Check if the color uniform is set, and use it. If not, fallback to a default color
        vec4 finalColor = texColor * (color.a > 0.0 ? color : defaultColor);

        // Output the final color
        FRAG_COLOR = finalColor;
    }
}
