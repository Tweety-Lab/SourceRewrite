using System.Numerics;

namespace SourceRewrite.Components
{
    /// <summary>
    /// Camera Component and Interaction class.
    /// </summary>
    public class Camera : GameComponent
    {
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

        public override void Start()
        {
            SetActiveCamera(this);
        }
    }
}
