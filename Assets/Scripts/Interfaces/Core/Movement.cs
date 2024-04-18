using UnityEngine;

namespace Interfaces.Core
{
    public class Movement : IMovement
    {
        public Vector3 Move(Vector3 direction, Transform cameraTransform)
        {
            direction = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * direction;
            direction.Normalize();
            return direction;
        }
        
        public Quaternion Rotate(Vector3 direction,Transform transform)
        {
            if (direction.magnitude == 0) return transform.rotation;

            var rotation = Quaternion.LookRotation(direction);
            return rotation;
        }
    }
}