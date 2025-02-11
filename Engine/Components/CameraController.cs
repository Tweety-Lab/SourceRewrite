using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SourceRewrite.Input;
using Silk.NET.Input;

namespace SourceRewrite.Components
{
    // FPS Camera Controller
    public class CameraController : GameComponent
    {
        public int MovementSpeed = 4;
        public int Sensitivity = 20;

        public override void Start()
        {
            // Get the camera thats attached to the same game object as the controller
            Camera camera = GameObject.GetComponentFromType<Camera>();
        }

        public override void Update(double deltaTime)
        {
            // Forward Movement
            if (Input.Input.GetKeyDown(Key.W))
            {
                GameObject.Transform.Position = GameObject.Transform.Position + new Vector3(0, 0, MovementSpeed * (float)deltaTime);
            }

            // Backward Movement
            if (Input.Input.GetKeyDown(Key.S))
            {
                GameObject.Transform.Position = GameObject.Transform.Position - new Vector3(0, 0, MovementSpeed * (float)deltaTime);
            }

            // Left Movement
            if (Input.Input.GetKeyDown(Key.A))
            {
                GameObject.Transform.Position = GameObject.Transform.Position + new Vector3(MovementSpeed * (float)deltaTime, 0, 0);
            }

            // Right Movement
            if (Input.Input.GetKeyDown(Key.D))
            {
                GameObject.Transform.Position = GameObject.Transform.Position - new Vector3(MovementSpeed * (float)deltaTime, 0, 0);
            }

            // Mouse Movement
            float mouseX = -Input.Input.GetMouseXMovement() * Sensitivity * (float)deltaTime;
            float mouseY = -Input.Input.GetMouseYMovement() * Sensitivity * (float)deltaTime;

            GameObject.Transform.RotateBy(mouseX, mouseY, 0);
        }
    }
}
