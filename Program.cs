using System;
using Silk.NET.Maths;
using SourceRewrite.Windowing;

// Entry Point
class Program
{
    static void Main()
    {
        // Create our main Game Window
        new GameWindow(new Vector2D<int>(800, 500), "Game Window");
        Console.WriteLine(GameWindow.CurrentWindow);
    }
}
