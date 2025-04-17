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
    public class CharacterController : PointEntity
    {
        [ConVar("movement_speed")]
        public static float MovementSpeed { get; set; } = 256f;

        [ConVar("sensitivity")]
        public static float Sensitivity { get; set; } = 0.6f;

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
            PhysicsBody.FreezeAllRotations = true;
            PhysicsBody.Mass = 128f;
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

            // Get the camera's forward/right vectors
            Vector3 camForward = PointCamera.ActiveCamera.Transform.Forward;
            Vector3 camRight = PointCamera.ActiveCamera.Transform.Right;

            // Flatten them onto the horizontal plane (zero out Z)
            camForward.Z = 0;
            camRight.Z = 0;

            // Re-normalize since we changed the length by removing Z
            camForward = Vector3.Normalize(camForward);
            camRight = Vector3.Normalize(camRight);

            // Move relative to camera's horizontal orientation
            if (Input.GetDown("forward")) movementDirection += camForward;
            if (Input.GetDown("back")) movementDirection -= camForward;

            if (Input.GetDown("left")) movementDirection -= camRight;
            if (Input.GetDown("right")) movementDirection += camRight;

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
