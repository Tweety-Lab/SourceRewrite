using SourceRewrite.Components;
using SourceRewrite.Maps;
using SourceRewrite.Maths;
using SourceRewrite.Objects;
using SourceRewrite.TimeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components
{
    public class CameraController : GameComponent
    {
        [MapProperty("angles")]
        public Vector3 Angles;

        private float rotationAmount = 10f; // Degrees per update

        public override void Start()
        {
            // Create a new Camera 
            GameObject gameObject = new GameObject();
            Camera cameraObject = new Camera();
            gameObject.AddComponent(cameraObject);

            // Start the gameobject
            gameObject.GameObjectStart();  

            // Set the active cameras position and rotation
            Camera.ActiveCamera.GameObject.Transform.Position = GameObject.Transform.Position;
            Camera.ActiveCamera.GameObject.Transform.Rotation = MathsHelper.EulerToQuaternion(Angles);
        }

        // Spin the camera 10 degrees per frame
        public override void Update()
        {
            // Rotate the camera by 10 degrees around the Y-axis (the up axis)
            Angles.Y += rotationAmount;

            // Ensure the angles stay within the 0-360 degree range (optional)
            if (Angles.Y >= 360f)
            {
                Angles.Y -= 360f;
            }

            // Update the camera's rotation based on the new angles
            Camera.ActiveCamera.GameObject.Transform.Rotation = MathsHelper.EulerToQuaternion(Angles);
        }
    }
}
