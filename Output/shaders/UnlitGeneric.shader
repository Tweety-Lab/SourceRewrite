// Unlit Object Shader

// Global Uniforms
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

uniform sampler2D uTexture0;
uniform vec4 tint; // Color to multiply with the texture

// Vertex Code
void vertex() 
{
    // Input attributes
    layout(location = 0) in vec3 vPos;    // Vertex position
    layout(location = 1) in vec3 vNormal; // Vertex normal (from location 1)
    layout(location = 2) in vec2 vUv;     // UVs (from location 2)

    out vec2 Uv;
    out vec3 Normal; // Pass the normal to the fragment shader
    out vec3 FragPos; // Pass the fragment position to the fragment shader

    void main()
    {
        // Transform the vertex position to clip space
        gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
    
        // Pass the UV and normal to the fragment shader
        Uv = vUv;
        Normal = normalize(mat3(transpose(inverse(uModel))) * vNormal); // Transform normal
        FragPos = vec3(uModel * vec4(vPos, 1.0)); // Transform position to world space
    }
}

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
