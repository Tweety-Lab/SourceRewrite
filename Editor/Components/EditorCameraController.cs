using System.Numerics;
using Silk.NET.Input;
using SourceRewrite.InputSystem;
using SourceRewrite.Maths;
using SourceRewrite.TimeSystem;
using SourceRewrite.Components;

namespace Editor.Components
{
    // FPS Camera Controller
    public class EditorCameraController : GameComponent
    {
        // Move at 1512 units per second
        public float MovementSpeed = 1512f;
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

        public override void Update()
        {
            // Handle camera movement input (W, A, S, D keys)
            HandleMovementInput(Time.DeltaTime);

            // Handle mouse input for camera rotation
            HandleMouseInput(Time.DeltaTime);
        }

        // Movement input (W, A, S, D)
        private void HandleMovementInput(float deltaTime)
        {
            Vector3 movement = Vector3.Zero;

            // Forward Movement (W)
            if (Input.GetKeyDown(Key.W))
            {
                movement += GameObject.Transform.Forward;
            }

            // Backward Movement (S)
            if (Input.GetKeyDown(Key.S))
            {
                movement -= GameObject.Transform.Forward;
            }

            // Left Movement (A)
            if (Input.GetKeyDown(Key.A))
            {
                movement -= GameObject.Transform.Right;
            }

            // Right Movement (D)
            if (Input.GetKeyDown(Key.D))
            {
                movement += GameObject.Transform.Right;
            }

            // Apply movement
            GameObject.Transform.Position += movement * MovementSpeed * deltaTime;
        }

        // Mouse input for camera rotation
        private void HandleMouseInput(float deltaTime)
        {
            if (Input.GetMouseButtonDown(1)) // Right Mouse Button held down
            {
                Input.LockCursor();

                // Mouse Movement (X and Y)
                float mouseX = -Input.GetMouseXMovement() * Sensitivity * deltaTime;
                float mouseY = -Input.GetMouseYMovement() * Sensitivity * deltaTime;

                // Add to Yaw and Pitch
                yaw += mouseX;
                pitch += mouseY;

                // Clamp pitch to prevent camera flipping
                pitch = Math.Clamp(pitch, -89f, 89f);

                // Update the rotation based on yaw and pitch (roll remains 0)
                GameObject.Transform.Rotation = MathsHelper.EulerToQuaternion(new Vector3(pitch, yaw, 0f));
            }
            else
            {
                Input.UnlockCursor();
            }
        }
    }
}
