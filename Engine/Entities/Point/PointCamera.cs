using SourceRewrite.AssetTypes;
using SourceRewrite.Attributes;
using SourceRewrite.Editor;
using SourceRewrite.Maps;
using System.Numerics;

namespace SourceRewrite.Entities.Point
{
    [Entity("point_camera")]
    public class PointCamera : BaseEntity
    {
        // Camera properties like field of view, aspect ratio, near and far clipping planes
        private float _fieldOfView = MathF.PI / 4f; // Default FOV 45 degrees (in radians)

        [EntityProperty("FOV")]
        public float FieldOfView
        {
            get => _fieldOfView;
            set => _fieldOfView = value * (MathF.PI / 180f);
        }

        public float AspectRatio { get; set; } = 16f / 9f; // Default 16:9 aspect ratio
        public float NearPlane { get; set; } = 0.1f; // Default near plane
        public float FarPlane { get; set; } = 99999f; // Default far plane

        /// <summary>
        /// Currently active Camera Component.
        /// </summary>
        public static PointCamera ActiveCamera { get; private set; }

        public PointCamera()
        {
            MapSystem.OnMapUnloaded += () => ActiveCamera = null;
        }

        /// <summary>
        /// Set the active Camera.
        /// </summary>
        public static void SetActiveCamera(PointCamera camera)
        {
            ActiveCamera = camera;
        }

        public Matrix4x4 GetViewMatrix()
        {
            Vector3 position = Transform.Position;
            return Matrix4x4.CreateLookAt(
                position,                    // Camera position
                position + Transform.Forward,      // Look target (position + direction)
                Transform.Up                     // Up vector
            );
        }

        /// <summary>
        /// Get the projection matrix (Perspective or Orthographic).
        /// </summary>
        /// <returns>Projection matrix.</returns>
        public Matrix4x4 GetProjectionMatrix()
        {
            // TODO: Orthographic projection

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
        public override void Update()
        {
            // Check if the camera's position has changed
            Vector3 currentCameraPos = Transform.Position;
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
            if (ActiveCamera != null)
                return;

            SetActiveCamera(this);
        }

#if EDITOR
        public override void DrawGizmos()
        {
            Gizmos.Color = new Vector4(255, 255, 255, 1);
            Gizmos.DrawSprite(Transform.Position, "sprites/point_camera", 2f, this);
        }
#endif
    }
}
