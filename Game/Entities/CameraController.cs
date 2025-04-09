using Silk.NET.Input;
using SourceRewrite;
using SourceRewrite.Attributes;
using SourceRewrite.Entities;
using SourceRewrite.Entities.Point;
using SourceRewrite.InputSystem;
using SourceRewrite.Maths;
using SourceRewrite.PhysicsSystem;
using SourceRewrite.TimeSystem;
using SourceRewrite.Windowing;
using SourceRewrite.Windowing.Modules;
using System.Numerics;

namespace Game.Entities
{
    [Entity("info_player_start")]
    public class CameraController : PointEntity
    {

        [ConVar("movement_speed")]
        public static float MovementSpeed { get; set; } = 1512f;

        [ConVar("sensitivity")]
        public static float Sensitivity { get; set; } = 20f;

        private float pitch = 0f;
        private float yaw = 0f;

        public override void Start()
        {
            DeveloperConsole.Msg("Started Camera Controller");

            if (PointCamera.ActiveCamera == null)
            {
                PointCamera camEntity = new PointCamera();
                camEntity.Start();

                camEntity.Parent = this;

                PointCamera.SetActiveCamera(camEntity);
            }

            // Initialize rotation angles from current transform
            Vector3 currentEuler = MathsHelper.QuaternionToEuler(Transform.Rotation);
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

        // Movement input
        private void HandleMovementInput(float deltaTime)
        {
            Vector3 movement = Vector3.Zero;

            // Forward Movement
            if (Input.GetDown("forward"))
            {
                movement += PointCamera.ActiveCamera.Transform.Forward;
            }

            // Backward Movement
            if (Input.GetDown("back"))
            {
                movement -= PointCamera.ActiveCamera.Transform.Forward;
            }

            // Left Movement
            if (Input.GetDown("left"))
            {
                movement -= PointCamera.ActiveCamera.Transform.Right;
            }

            // Right Movement
            if (Input.GetDown("right"))
            {
                movement += PointCamera.ActiveCamera.Transform.Right;
            }

            // Apply movement
            PointCamera.ActiveCamera.Transform.Position += movement * MovementSpeed * deltaTime;
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
                PointCamera.ActiveCamera.Transform.Rotation = MathsHelper.EulerToQuaternion(new Vector3(pitch, yaw, 0f));
            }
            else
            {
                Input.UnlockCursor();
            }

            if (Input.GetMouseButtonDown(0))
            {
                // Get the mouse position
                BaseEntity hitEnt = Physics.RayCast(new PhysicsRay(PointCamera.ActiveCamera.Transform.Position, PointCamera.ActiveCamera.Transform.Forward));

                if (hitEnt != null)
                    hitEnt.DestroyDeferred();
            }
        }
    }
}
