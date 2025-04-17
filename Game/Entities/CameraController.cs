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
        public static float MovementSpeed { get; set; } = 216f;

        [ConVar("sensitivity")]
        public static float Sensitivity { get; set; } = 1f;

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

            // Physics
            PhysicsBody.BodyType = BodyType.Dynamic;
            PhysicsBody.BoundingBox = new Vector3(32f, 32f, 64f);
            PhysicsBody.FreezeRotationX = true;
            PhysicsBody.FreezeRotationY = true;
            PhysicsInitNormal();
        }

        public override void Update()
        {
            HandleMovementInput(Time.DeltaTime);
            HandleMouseInput(Time.DeltaTime);
        }

        private void HandleMovementInput(float deltaTime)
        {
            Vector3 movementDirection = Vector3.Zero;

            // Forward/Backward
            if (Input.GetDown("forward")) movementDirection += Transform.Forward;
            if (Input.GetDown("back")) movementDirection -= Transform.Forward;

            // Left/Right
            if (Input.GetDown("left")) movementDirection -= Transform.Right;
            if (Input.GetDown("right")) movementDirection += Transform.Right;

            // Up/Down (optional — jump or fly cam)
            if (Input.GetDown("up")) movementDirection += Transform.Up;
            if (Input.GetDown("down")) movementDirection -= Transform.Up;

            // Normalize if moving diagonally
            if (movementDirection != Vector3.Zero)
            {
                movementDirection = Vector3.Normalize(movementDirection);
            }

            // Calculate target velocity
            Vector3 targetVelocity = movementDirection * MovementSpeed;

            // Apply velocity directly
            SetAbsVelocity(targetVelocity);
        }

        private void HandleMouseInput(float deltaTime)
        {
            if (Input.GetMouseButtonDown(1)) // RMB held
            {
                Input.LockCursor();

                float mouseX = -Input.GetMouseXMovement() * Sensitivity;
                float mouseY = -Input.GetMouseYMovement() * Sensitivity;

                yaw += mouseX;    // Horizontal rotation around Z (yaw)
                pitch += mouseY;  // Vertical rotation around Right (pitch)

                // Clamp pitch to avoid flipping
                pitch = System.Math.Clamp(pitch, -89f, 89f);

                // Convert euler angles to quaternion
                Quaternion newRotation = SourceRewrite.Math.EulerToQuaternion(new Vector3(pitch, 0f, yaw));

                // Set Rotation of camera
                PointCamera.ActiveCamera.LocalTransform.Rotation = newRotation;
            }
            else
            {
                Input.UnlockCursor();
            }
        }
    }
}
