// Global Uniforms
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

uniform sampler2D uTexture0;
uniform vec4 tint; // Color to multiply with the texture

// Vertex Code
void vertex() 
{
    layout (location = 0) in vec3 vPos;
    layout (location = 1) in vec2 vUv;

    out vec2 fUv;

    void main()
    {
        // Multiplying our uniform with the vertex position, the multiplication order here does matter.
        gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
        fUv = vUv;
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
