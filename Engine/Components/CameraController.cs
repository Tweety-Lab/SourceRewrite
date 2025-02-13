using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Silk.NET.Input;
using SourceRewrite.InputSystem;

namespace SourceRewrite.Components
{
    // FPS Camera Controller
    public class CameraController : GameComponent
    {
        public int MovementSpeed = 4;
        public int Sensitivity = 30;

        private Camera camera;

        public override void Start()
        {
            // Get the camera thats attached to the same game object as the controller
            camera = GameObject.GetComponentFromType<Camera>();
        }

        public override void Update(double deltaTime)
        {
            // Forward Movement
            if (Input.GetKeyDown(Key.W))
            {
                GameObject.Transform.Position = GameObject.Transform.Position + MovementSpeed * (float)deltaTime * GameObject.Transform.Forward;
            }

            // Backward Movement
            if (Input.GetKeyDown(Key.S))
            {
                GameObject.Transform.Position = GameObject.Transform.Position - MovementSpeed * (float)deltaTime * GameObject.Transform.Forward;
            }

            // Left Movement
            if (Input.GetKeyDown(Key.A))
            {
                GameObject.Transform.Position = GameObject.Transform.Position - MovementSpeed * (float)deltaTime * GameObject.Transform.Right;
            }

            // Right Movement
            if (Input.GetKeyDown(Key.D))
            {
                GameObject.Transform.Position = GameObject.Transform.Position + MovementSpeed * (float)deltaTime * GameObject.Transform.Right;
            }

            // Mouse Movement
            float mouseX = -Input.GetMouseXMovement() * Sensitivity * (float)deltaTime;
            float mouseY = -Input.GetMouseYMovement() * Sensitivity * (float)deltaTime;

            GameObject.Transform.RotateBy(mouseY, mouseX, 0);
        }
    }
}
