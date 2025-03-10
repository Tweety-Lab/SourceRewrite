using Silk.NET.Assimp;
using SourceRewrite.Rendering;
using System.Numerics;

namespace SourceRewrite.Components
{
    /// <summary>
    /// Camera Component and Interaction class.
    /// </summary>
    public class Camera : GameComponent
    {

        // Camera properties like field of view, aspect ratio, near and far clipping planes
        public float FieldOfView { get; set; } = MathF.PI / 4f; // Default FOV 45 degrees
        public float AspectRatio { get; set; } = 16f / 9f; // Default 16:9 aspect ratio
        public float NearPlane { get; set; } = 0.1f; // Default near plane
        public float FarPlane { get; set; } = 99999f; // Default far plane

        /// <summary>
        /// Currently active Camera Component.
        /// </summary>
        public static Camera ActiveCamera { get; private set; }

        /// <summary>
        /// Set the active Camera.
        /// </summary>
        public static void SetActiveCamera(Camera camera)
        {
            ActiveCamera = camera;
        }



        public Matrix4x4 GetViewMatrix()
        {
            Vector3 position = GameObject.Transform.Position;
            return Matrix4x4.CreateLookAt(
                position,                    // Camera position
                position + GameObject.Transform.Forward,      // Look target (position + direction)
                GameObject.Transform.Up                     // Up vector
            );
        }

        /// <summary>
        /// Get the projection matrix (Perspective or Orthographic).
        /// </summary>
        /// <returns>Projection matrix.</returns>
        public Matrix4x4 GetPerspectiveProjectionMatrix()
        {
            // Perspective Projection Matrix
            return Matrix4x4.CreatePerspectiveFieldOfView(
                FieldOfView,   // Field of View
                AspectRatio,   // Aspect ratio (width/height)
                NearPlane,     // Near clipping plane
                FarPlane       // Far clipping plane
            );
        }

        // Update Shader uniforms with Camera data
        private Vector3 lastCameraPos;
        public override void Update(float deltaTime)
        {
            // Check if the camera's position has changed
            Vector3 currentCameraPos = GameObject.Transform.Position;
            if (currentCameraPos != lastCameraPos)
            {
                // If it has changed, update the engine uniform
                foreach (Shader shader in Shader.Shaders)
                {
                    shader.SetParameter("VIEW_POS", currentCameraPos);
                }

                // Store the new camera position for future comparisons
                lastCameraPos = currentCameraPos;
            }
        }

        public override void Start()
        {
            SetActiveCamera(this);
        }
    }
}
