using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Silk.NET.Input;
using SourceRewrite.InputSystem;
using Silk.NET.Assimp;

namespace SourceRewrite.Components
{
    // FPS Camera Controller
    public class CameraController : GameComponent
    {
        public float MovementSpeed = 4f;
        public float Sensitivity = 30f;

        private Camera camera;

        public override void Start()
        {
            // Get the camera thats attached to the same game object as the controller
            camera = GameObject.GetComponentFromType<Camera>();
        }

        public override void Update(float deltaTime)
        {
            // Forward Movement
            if (Input.GetKeyDown(Key.W))
            {
                camera.Transform.Position = camera.Transform.Position + MovementSpeed * deltaTime * camera.Transform.Forward;
            }

            // Backward Movement
            if (Input.GetKeyDown(Key.S))
            {
                camera.Transform.Position = camera.Transform.Position - MovementSpeed * deltaTime * camera.Transform.Forward;
            }

            // Left Movement
            if (Input.GetKeyDown(Key.A))
            {
                camera.Transform.Position = camera.Transform.Position - MovementSpeed * deltaTime * camera.Transform.Right;
            }

            // Right Movement
            if (Input.GetKeyDown(Key.D))
            {
                camera.Transform.Position = camera.Transform.Position + MovementSpeed * deltaTime * camera.Transform.Right;
            }

            // If right mouse held down move Camera view
            if (Input.GetMouseButtonDown(1))
            {
                Input.LockCursor();

                // Mouse Movement
                float mouseX = -Input.GetMouseXMovement() * Sensitivity * deltaTime;
                float mouseY = -Input.GetMouseYMovement() * Sensitivity * deltaTime;

                Console.WriteLine("Right: " + camera.Transform.Right);
                Console.WriteLine("Forward: " + camera.Transform.Forward);

                camera.Transform.RotateBy(mouseY, mouseX, 0);
            } else
            {
                Input.UnlockCursor();
            }
        }
    }
}
