using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Maths;

namespace SourceRewrite.Components
{
    /// <summary>
    /// Camera Component and Interaction class.
    /// </summary>
    public class Camera : GameComponent
    {
        // Setup Camera Data with OpenGL coordinate system (-Z forward)
        public Vector3 CameraFront = new Vector3(0f, 0f, -1f);
        public Vector3 CameraUp = Vector3.UnitY;
        public Vector3 CameraRight => Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp));

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

        public void UpdateCameraFront()
        {
            // Get the rotation from the transform
            Quaternion rotation = GameObject.Transform.Rotation;

            // Update camera direction vectors based on transform's rotation
            CameraFront = Vector3.Transform(-Vector3.UnitZ, rotation);
            CameraUp = Vector3.Transform(Vector3.UnitY, rotation);

            // Ensure vectors are normalized
            CameraFront = Vector3.Normalize(CameraFront);
            CameraUp = Vector3.Normalize(CameraUp);
        }

        public Matrix4x4 GetViewMatrix()
        {
            Vector3 position = GameObject.Transform.Position;
            return Matrix4x4.CreateLookAt(
                position,                    // Camera position
                position + CameraFront,      // Look target (position + direction)
                CameraUp                     // Up vector
            );
        }

        // This method will be automatically called to update the camera's front each frame
        // Dumb. Should Be in OnRender instead of Update.
        public override void Update(double deltaTime)
        {
            // Call the method to update the front direction based on the rotation
            UpdateCameraFront();
        }


        public Camera()
        {
            ActiveCamera = this;
        }
    }
}
