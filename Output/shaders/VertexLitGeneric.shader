// Lit Mesh Shader

// Global Uniforms
uniform sampler2D basetexture;

struct Light {
    vec3 position;
    vec4 color; // RGB = color, A = intensity
    vec3 attenuation; // Constant, Linear, Quadratic

    // SpotLights
    vec3 direction;
    float cutOff;      // Cosine of inner cutoff angle
    float outerCutOff; // Cosine of outer cutoff angle

    int lightType;     // 0 = point, 1 = spotlight, 2 = directional
};

uniform Light lights[10];
uniform int activeLights; // Number of active lights in the scene

// Vertex Code
void vertex() 
{
    // Input attributes
    layout(location = 0) in vec3 vPos;    // Vertex position
    layout(location = 1) in vec3 vNormal; // Vertex normal
    layout(location = 2) in vec2 vUv;     // UV coordinates

    out vec2 Uv;
    out vec3 VERT_NORMAL;
    out vec3 FragPos;

    void main()
    {
        mat4 model = MODEL_MATRIX;
        vec4 worldPos = model * vec4(vPos, 1.0);
    
        FragPos = worldPos.xyz;
        Uv = vUv;
        VERT_NORMAL = normalize(mat3(transpose(inverse(model))) * vNormal);
    
        gl_Position = PROJECTION_MATRIX * VIEW_MATRIX * worldPos;
    }
}

// Fragment Code
void fragment() 
{
    in vec2 Uv;
    in vec3 VERT_NORMAL;
    in vec3 FragPos;

    uniform float specular = 0.5;
    uniform float ambientStrength = 0.4;

    void main()
    {
        // Sample base texture using mesh UV
        vec4 texColor = texture(basetexture, Uv);

        vec3 norm = normalize(VERT_NORMAL);
        vec3 viewDir = normalize(VIEW_POS - FragPos);
        
        vec3 totalLight = vec3(0.0);
        
        // Iterate through all active lights
        for (int i = 0; i < activeLights; i++) {
            // Light properties
            vec3 lightDir;
            float intensity = lights[i].color.a;
            vec3 color = lights[i].color.rgb;
            
            // Calculate light direction and attenuation based on light type
            float attenuation = 1.0;
            if (lights[i].lightType == 2) { // Directional light
                lightDir = normalize(-lights[i].direction);
            } else { // Point or spotlight
                lightDir = normalize(lights[i].position - FragPos);
                float distance = length(lights[i].position - FragPos);
                attenuation = 1.0 / (lights[i].attenuation.x + 
                                     lights[i].attenuation.y * distance + 
                                     lights[i].attenuation.z * (distance * distance));
            }
            
            // Spotlight effect
            float spotlightEffect = 1.0;
            if (lights[i].lightType == 1) {
                float theta = dot(lightDir, normalize(-lights[i].direction));
                float epsilon = lights[i].cutOff - lights[i].outerCutOff;
                spotlightEffect = clamp((theta - lights[i].outerCutOff) / epsilon, 0.0, 1.0);
                attenuation *= spotlightEffect;
            }
            
            // Ambient
            vec3 ambient = ambientStrength * color * intensity;
            
            // Diffuse
            float diff = max(dot(norm, lightDir), 0.0);
            vec3 diffuse = diff * color * intensity;
            
            // Specular
            vec3 reflectDir = reflect(-lightDir, norm);
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);
            vec3 specularOutput = specular * spec * color * intensity;
            
            // Combine components
            totalLight += (ambient + diffuse + specularOutput) * attenuation;
        }
        
        // Final color = texture modulated by lighting
        vec3 result = texColor.rgb * totalLight;
        FRAG_COLOR = vec4(result, texColor.a);
    }
}
