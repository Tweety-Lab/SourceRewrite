// Vertex Code
vertex() 
{
    #version 330 core
    layout (location = 0) in vec3 vPos;
    layout (location = 1) in vec2 vUv;

    uniform mat4 uModel;
    uniform mat4 uView;
    uniform mat4 uProjection;

    out vec2 fUv;

    void main()
    {
        //Multiplying our uniform with the vertex position, the multiplication order here does matter.
        gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
        fUv = vUv;
    }
}

// Fragment Code
fragment() 
{
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
}
