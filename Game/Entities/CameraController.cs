using Silk.NET.Input;
using Silk.NET.Windowing;
using SourceRewrite;
using SourceRewrite.Attributes;
using SourceRewrite.Entities;
using SourceRewrite.Entities.Point;
using SourceRewrite.InputSystem;
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

        private float pitch = 0f;  // rotation around Right (X)
        private float yaw = 0f;    // rotation around Up (Z)

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
            Vector3 currentEuler = SourceRewrite.Math.QuaternionToEuler(Transform.Rotation);
            pitch = currentEuler.X;
            yaw = currentEuler.Z;
        }

        public override void Update()
        {
            HandleMovementInput(Time.DeltaTime);
            HandleMouseInput(Time.DeltaTime);
        }

        private void HandleMovementInput(float deltaTime)
        {
            Vector3 movement = Vector3.Zero;

            // Forward/Backward
            if (Input.GetDown("forward"))
            {
                movement += Transform.Forward;
            }

            if (Input.GetDown("back"))
            {
                movement -= Transform.Forward;
            }

            // Left/Right
            if (Input.GetDown("left"))
            {
                movement -= Transform.Right;
            }

            if (Input.GetDown("right"))
            {
                movement += Transform.Right;
            }

            // Up/Down (optional — jump or fly cam)
            if (Input.GetDown("up"))
            {
                movement += Transform.Up;
            }

            if (Input.GetDown("down"))
            {
                movement -= Transform.Up;
            }

            // Apply movement
            Transform.Position += movement * MovementSpeed * deltaTime;
        }

        private void HandleMouseInput(float deltaTime)
        {
            if (Input.GetMouseButtonDown(1)) // RMB held
            {
                Input.LockCursor();

                float mouseX = -Input.GetMouseXMovement() * Sensitivity * deltaTime;
                float mouseY = -Input.GetMouseYMovement() * Sensitivity * deltaTime;

                yaw += mouseX;    // Horizontal rotation around Z (yaw)
                pitch += mouseY;  // Vertical rotation around Right (pitch)

                // Clamp pitch to avoid flipping
                pitch = System.Math.Clamp(pitch, -89f, 89f);

                // Convert euler angles (pitch X, yaw Z, roll Y stays zero)
                Transform.Rotation = SourceRewrite.Math.EulerToQuaternion(new Vector3(pitch, 0f, yaw));
            }
            else
            {
                Input.UnlockCursor();
            }
        }
    }
}
