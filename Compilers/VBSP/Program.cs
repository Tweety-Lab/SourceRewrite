using System;
class Program
{
    static void Main(string[] args)
    {
        // Entry point of the application
        Console.WriteLine("Hello, World!");

        // Command-line arguments using the 'args' parameter
        if (args.Length > 0)
        {
            Console.WriteLine("Command-line arguments:");
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
        }
    }
}
