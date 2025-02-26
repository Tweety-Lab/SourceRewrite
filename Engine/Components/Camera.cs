using SourceRewrite.Rendering;
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

        // Update Shader uniforms with Camera data
        private Vector3 lastCameraPos;
        public override void Update(float deltaTime)
        {
            // Check if the camera's position has changed
            Vector3 currentCameraPos = GameObject.Transform.Position;
            if (currentCameraPos != lastCameraPos)
            {
                // If it has changed, update the uniform in all shaders
                foreach (Shader shader in Shader.Shaders)
                {
                    shader.SetParameter("viewPos", currentCameraPos);
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
