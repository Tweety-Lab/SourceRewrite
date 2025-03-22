uniform vec4 tint; // Color to display

// Fragment Code
void fragment() 
{

    void main()
    {
        // Output the final color
        FRAG_COLOR = tint / 255;
    }
}