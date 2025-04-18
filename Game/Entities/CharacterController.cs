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
        // Speed of Movement
        public float MovementSpeed { get; set; } = 256f;

        // Power of Jump
        public float JumpPower { get; set; } = 256f;


        [ConVar("sensitivity")]
        public static float Sensitivity { get; set; } = 0.4f;

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

                // Set the Camera offset
                // Eye level = 64 units offset from center of origin
                camEntity.LocalTransform.Position = new Vector3(0f, 0f, 28f);

                PointCamera.SetActiveCamera(camEntity);
            }

            // Initialize rotation angles from current transform
            Vector3 currentEuler = SourceRewrite.Math.QuaternionToEuler(Transform.Rotation);
            pitch = currentEuler.X;
            yaw = currentEuler.Z;

            // Physics
            PhysicsBody.BodyType = BodyType.Dynamic;
            PhysicsBody.BoundingBox = new Vector3(16f, 16f, 36f); // Bounding box is half of desired dimensions
            PhysicsBody.FreezeAllRotations = true;
            PhysicsInitNormal();
        }

        public override void Update()
        {
            // Jumping
            if (Input.GetPressed("jump"))
                Jump();

            HandleMovementInput();
            HandleMouseInput();

        }

        private void Jump()
        {
            if (IsGrounded())
                Velocity = new Vector3(Velocity.X, Velocity.Y, JumpPower);
        }

        private bool IsGrounded()
        {
            float groundCheckDistance = 8f;
            float skinWidth = 1f; // Small offset

            Vector3 footPosition = Transform.Position - new Vector3(0f, 0f, PhysicsBody.BoundingBox.Z - skinWidth);
            Vector3 groundCheckPosition = footPosition - new Vector3(0f, 0f, groundCheckDistance);

            var hit = Physics.RayCast(new PhysicsRay(footPosition, groundCheckPosition));
            return hit != null;
        }


        private void HandleMovementInput()
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

            // Normalize the movement direction
            if (movementDirection != Vector3.Zero)
            {
                movementDirection = Vector3.Normalize(movementDirection);
            }

            Vector3 targetVelocity = movementDirection * MovementSpeed;
            Velocity = new Vector3(targetVelocity.X, targetVelocity.Y, Velocity.Z);
        }

        private void HandleMouseInput()
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
