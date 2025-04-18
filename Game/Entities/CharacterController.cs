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
        public int MovementSpeed { get; set; } = 256;

        // Speed of Movement when crouching
        public int CrouchSpeed { get; set; } = 75;

        // Power of Jump
        public int JumpPower { get; set; } = 230;

        // Range of Interaction
        public int InteractionRange { get; set; } = 64;

        public float AirDrag { get; set; } = 0.99f; // Higher = less air drag
        public float AirControl { get; set; } = 0.025f; // Higher = more air control


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
            Vector3 currentEuler = SourceRewrite.SourceMath.QuaternionToEuler(Transform.Rotation);
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

            if (Input.GetPressed("use"))
                Interact();

            HandleCrouchInput();
            HandleMovementInput();
            HandleMouseInput();
        }

        private void Jump()
        {
            if (IsGrounded())
                Velocity = new Vector3(Velocity.X, Velocity.Y, JumpPower);
        }

        private void Interact()
        {
            // Raycast out of camera
            var hit = Physics.RayCast(
                new PhysicsRay(
                    PointCamera.ActiveCamera.Transform.Position,
                    PointCamera.ActiveCamera.Transform.Position + PointCamera.ActiveCamera.Transform.Forward * 64f
                )
            );

            // Check if hit is usable and trigger OnUse
            if (hit is IUsable usable)
                usable.OnUse(this);
        }

        bool isCrouching = false;
        private int _preCrouchSpeed;
        private void HandleCrouchInput()
        {
            if (Input.GetPressed("duck"))
            {
                isCrouching = true;

                // Save movement speed
                _preCrouchSpeed = MovementSpeed;

                // Change bounding box
                PhysicsDestroyObject();
                PhysicsBody.BoundingBox = new Vector3(PhysicsBody.BoundingBox.X, PhysicsBody.BoundingBox.Y, PhysicsBody.BoundingBox.Z / 3);
                PhysicsInitNormal();

                // Slow down movement speed
                MovementSpeed = CrouchSpeed;
            }
            if (Input.GetUp("duck"))
            {
                if (isCrouching)
                {
                    // Revert bounding box
                    PhysicsDestroyObject();
                    PhysicsBody.BoundingBox = new Vector3(PhysicsBody.BoundingBox.X, PhysicsBody.BoundingBox.Y, PhysicsBody.BoundingBox.Z * 3);
                    PhysicsInitNormal();

                    // Restore saved movement speed
                    MovementSpeed = _preCrouchSpeed;
                }

                isCrouching = false;
            }
        }

        private bool IsGrounded()
        {
            float groundCheckDistance = 8f;
            float skinWidth = 1f;
            float halfWidthX = PhysicsBody.BoundingBox.X;
            float halfWidthY = PhysicsBody.BoundingBox.Y;
            float characterRadius = Math.Max(PhysicsBody.BoundingBox.X, PhysicsBody.BoundingBox.Y);

            Vector3 footPosition = Transform.Position - new Vector3(0f, 0f, PhysicsBody.BoundingBox.Z - skinWidth);

            // Create multiple ray origins
            Vector3[] rayOrigins = new Vector3[]
            {
                // Middle
                footPosition,

                // Four corners
                footPosition + new Vector3(halfWidthX, halfWidthY, 0f),
                footPosition + new Vector3(-halfWidthX, halfWidthY, 0f),
                footPosition + new Vector3(halfWidthX, -halfWidthY, 0f),
                footPosition + new Vector3(-halfWidthX, -halfWidthY, 0f),
        
                // Midpoints of each side
                footPosition + new Vector3(halfWidthX, 0f, 0f),
                footPosition + new Vector3(-halfWidthX, 0f, 0f),
                footPosition + new Vector3(0f, halfWidthY, 0f),
                footPosition + new Vector3(0f, -halfWidthY, 0f)
            };

            int hitCount = 0;
            int requiredHits = 1; // Minimum number of hits to consider the character grounded

            foreach (var origin in rayOrigins)
            {
                Vector3 groundCheckPosition = origin - new Vector3(0f, 0f, groundCheckDistance);
                var hit = Physics.RayCast(new PhysicsRay(origin, groundCheckPosition));

                if (hit != null)
                {
                    hitCount++;
                    if (hitCount >= requiredHits)
                        return true;
                }
            }

            return false;
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

            // Re-normalize since we changed length
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

            // Get current horizontal velocity
            Vector3 currentVelocity = Velocity;
            Vector2 currentHorizontalVelocity = new Vector2(currentVelocity.X, currentVelocity.Y);

            // Calculate target velocity
            Vector2 targetVelocity = new Vector2(movementDirection.X, movementDirection.Y) * MovementSpeed;

            if (IsGrounded())
            {
                // On ground, immediate response
                currentHorizontalVelocity = targetVelocity;
            }
            else
            {
                // In air, apply air control with some acceleration/deceleration
                // If there's input accelerate toward target velocity
                if (movementDirection != Vector3.Zero)
                {
                    currentHorizontalVelocity = Vector2.Lerp(
                        currentHorizontalVelocity,
                        targetVelocity,
                        AirControl * Time.DeltaTime * 60f
                    );
                }
                else
                {
                    // No input, slowly decelerate
                    currentHorizontalVelocity *= AirDrag;
                }
            }

            // Apply the new horizontal velocity while preserving vertical velocity (Z)
            Velocity = new Vector3(currentHorizontalVelocity.X, currentHorizontalVelocity.Y, currentVelocity.Z);
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
                Quaternion newRotation = SourceRewrite.SourceMath.EulerToQuaternion(new Vector3(pitch, 0f, yaw));

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
