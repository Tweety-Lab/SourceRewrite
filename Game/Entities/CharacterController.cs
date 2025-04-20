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
        // Player
        public int MovementSpeed { get; set; } = 256;
        public int CrouchSpeed { get; set; } = 128;
        public int JumpPower { get; set; } = 230;
        public int InteractionRange { get; set; } = 64;

        // Physics
        public float GroundAcceleration { get; set; } = 0.2f; 
        public float AirDrag { get; set; } = 0.99f;
        public float AirControl { get; set; } = 0.025f;

        // Camera

        [ConVar("sensitivity", ConVarFlag.Archive)]
        public static float Sensitivity { get; set; } = 0.4f;

        [ConVar("fov", ConVarFlag.Archive)]
        public static float FieldOfView { get; set; } = 70f;

        // The camera offset from the middle of the player
        public Vector3 CameraOffset { get; set; } = new Vector3(0f, 0f, 32f);

        private float pitch = 0f;
        private float yaw = 0f;

        private bool wasGroundedLastFrame = false;

        public override void Start()
        {
            DeveloperConsole.Msg("Started Camera Controller");

            if (PointCamera.ActiveCamera == null)
            {
                PointCamera camEntity = new PointCamera();
                camEntity.FieldOfView = FieldOfView;
                camEntity.Start();
                camEntity.Parent = this;
                camEntity.LocalTransform.Position = CameraOffset;
                PointCamera.SetActiveCamera(camEntity);
            }

            Vector3 currentEuler = EngineMaths.QuaternionToEuler(Transform.Rotation);
            pitch = currentEuler.X;
            yaw = currentEuler.Z;

            PhysicsBody.BodyType = BodyType.Dynamic;
            PhysicsBody.BoundingBox = new Vector3(16f, 16f, 36f);
            PhysicsBody.Friction = 1.75f;
            PhysicsBody.Mass = 5f;
            PhysicsBody.FreezeAllRotations = true;
            PhysicsInitNormal();
        }

        public override void Update()
        {
            // Jumping
            if (Input.GetPressed("jump"))
                Jump();

            // Interacting
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
            var hit = Physics.RayCast(
                new PhysicsRay(
                    PointCamera.ActiveCamera.Transform.Position,
                    PointCamera.ActiveCamera.Transform.Position + PointCamera.ActiveCamera.Transform.Forward * 64f
                )
            );

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
                _preCrouchSpeed = MovementSpeed;

                PhysicsDestroyObject();
                PhysicsBody.BoundingBox = new Vector3(PhysicsBody.BoundingBox.X, PhysicsBody.BoundingBox.Y, PhysicsBody.BoundingBox.Z / 3);
                PhysicsInitNormal();

                MovementSpeed = CrouchSpeed;
            }
            if (Input.GetUp("duck"))
            {
                if (isCrouching)
                {
                    PhysicsDestroyObject();
                    PhysicsBody.BoundingBox = new Vector3(PhysicsBody.BoundingBox.X, PhysicsBody.BoundingBox.Y, PhysicsBody.BoundingBox.Z * 3);
                    PhysicsInitNormal();

                    MovementSpeed = _preCrouchSpeed;
                }

                isCrouching = false;
            }
        }

        private bool IsGrounded()
        {
            float groundCheckDistance = 4f;
            float skinWidth = 1f;
            float halfWidthX = PhysicsBody.BoundingBox.X;
            float halfWidthY = PhysicsBody.BoundingBox.Y;

            Vector3 footPosition = Transform.Position - new Vector3(0f, 0f, PhysicsBody.BoundingBox.Z - skinWidth);

            Vector3[] rayOrigins = new Vector3[]
            {
                footPosition,
                footPosition + new Vector3(halfWidthX, halfWidthY, 0f),
                footPosition + new Vector3(-halfWidthX, halfWidthY, 0f),
                footPosition + new Vector3(halfWidthX, -halfWidthY, 0f),
                footPosition + new Vector3(-halfWidthX, -halfWidthY, 0f),
                footPosition + new Vector3(halfWidthX, 0f, 0f),
                footPosition + new Vector3(-halfWidthX, 0f, 0f),
                footPosition + new Vector3(0f, halfWidthY, 0f),
                footPosition + new Vector3(0f, -halfWidthY, 0f)
            };

            int hitCount = 0;
            int requiredHits = 1;

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
            bool grounded = IsGrounded();

            if (grounded && !wasGroundedLastFrame)
            {
                Velocity = new Vector3(0f, 0f, Velocity.Z);
            }

            wasGroundedLastFrame = grounded;

            Vector3 movementDirection = Vector3.Zero;

            Vector3 camForward = PointCamera.ActiveCamera.Transform.Forward;
            Vector3 camRight = PointCamera.ActiveCamera.Transform.Right;
            camForward.Z = 0;
            camRight.Z = 0;
            camForward = Vector3.Normalize(camForward);
            camRight = Vector3.Normalize(camRight);

            if (Input.GetDown("forward")) movementDirection += camForward;
            if (Input.GetDown("back")) movementDirection -= camForward;
            if (Input.GetDown("left")) movementDirection -= camRight;
            if (Input.GetDown("right")) movementDirection += camRight;

            if (movementDirection != Vector3.Zero)
                movementDirection = Vector3.Normalize(movementDirection);

            Vector3 currentVelocity = Velocity;
            Vector2 currentHorizontalVelocity = new Vector2(currentVelocity.X, currentVelocity.Y);
            Vector2 targetVelocity = new Vector2(movementDirection.X, movementDirection.Y) * MovementSpeed;

            if (grounded)
            {
                if (movementDirection != Vector3.Zero)
                {
                    currentHorizontalVelocity = Vector2.Lerp(
                        currentHorizontalVelocity,
                        targetVelocity,
                        GroundAcceleration * Time.DeltaTime * 60f
                    );
                }
            }
            else
            {
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
                    currentHorizontalVelocity *= AirDrag;
                }
            }

            Velocity = new Vector3(currentHorizontalVelocity.X, currentHorizontalVelocity.Y, currentVelocity.Z);
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Input.LockCursor();

                float mouseX = -Input.GetMouseXMovement() * Sensitivity;
                float mouseY = -Input.GetMouseYMovement() * Sensitivity;

                yaw += mouseX;
                pitch += mouseY;

                pitch = System.Math.Clamp(pitch, -89f, 89f);

                Quaternion newRotation = EngineMaths.EulerToQuaternion(new Vector3(pitch, 0f, yaw));
                PointCamera.ActiveCamera.LocalTransform.Rotation = newRotation;
            }
            else
            {
                Input.UnlockCursor();
            }
        }
    }
}
