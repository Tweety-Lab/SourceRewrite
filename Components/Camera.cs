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
        // Setup Camera Data
        public Vector3 CameraFront = new Vector3(0f, 0f, 1f);
        public Vector3 CameraUp = new Vector3(0f, 1f, 0f);

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
            // Normalize the front vector
            CameraFront = MathsHelper.QuaternionToEulerDegrees(GameObject.Transform.Rotation);
            CameraFront = Vector3.Normalize(CameraFront);
        }

        // This method will be automatically called to update the camera's front each frame
        // Dumb.
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
