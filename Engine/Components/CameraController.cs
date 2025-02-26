using System.Numerics;
using Silk.NET.Input;
using SourceRewrite.InputSystem;
using SourceRewrite.Maths;

namespace SourceRewrite.Components
{
    // FPS Camera Controller
    public class CameraController : GameComponent
    {
        public float MovementSpeed = 4f;
        public float Sensitivity = 20f;

        private float pitch = 0f;
        private float yaw = 0f;

        public override void Start()
        {
            // Initialize rotation angles from current transform
            Vector3 currentEuler = MathsHelper.QuaternionToEuler(GameObject.Transform.Rotation);
            pitch = currentEuler.X;
            yaw = currentEuler.Y;
        }

        public override void Update(float deltaTime)
        {
            // Forward Movement
            if (Input.GetKeyDown(Key.W))
            {
                GameObject.Transform.Position = GameObject.Transform.Position + MovementSpeed * deltaTime * GameObject.Transform.Forward;
            }

            // Backward Movement
            if (Input.GetKeyDown(Key.S))
            {
                GameObject.Transform.Position = GameObject.Transform.Position - MovementSpeed * deltaTime * GameObject.Transform.Forward;
            }

            // Left Movement
            if (Input.GetKeyDown(Key.A))
            {
                GameObject.Transform.Position = GameObject.Transform.Position - MovementSpeed * deltaTime * GameObject.Transform.Right;
            }

            // Right Movement
            if (Input.GetKeyDown(Key.D))
            {
                GameObject.Transform.Position = GameObject.Transform.Position + MovementSpeed * deltaTime * GameObject.Transform.Right;
            }

            // If right mouse held down move Camera view
            if (Input.GetMouseButtonDown(1))
            {
                Input.LockCursor();

                // Mouse Movement
                float mouseX = -Input.GetMouseXMovement() * Sensitivity * deltaTime;
                float mouseY = -Input.GetMouseYMovement() * Sensitivity * deltaTime;

                // Add to Yaw and Pitch
                yaw += mouseX;
                pitch += mouseY;

                // Clamp pitch to prevent camera flipping
                pitch = Math.Clamp(pitch, -89f, 89f);

                // Create rotation quaternion from Euler angles
                // Note: We only use pitch and yaw, keeping roll at 0
                GameObject.Transform.Rotation = MathsHelper.EulerToQuaternion(
                    new Vector3(pitch, yaw, 0f)
                );

            } else
            {
                Input.UnlockCursor();
            }
        }
    }
}
