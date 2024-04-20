using UnityEngine;

namespace Interfaces.Control
{
    /// <summary>
    /// Interface for movement behavior.
    /// </summary>
    public interface IMovement
    {
        /// <summary>
        /// Moves an object in a specified direction relative to a camera's orientation.
        /// </summary>
        /// <param name="direction">The direction in which to move the object.</param>
        /// <param name="cameraTransform">The transform of the camera.</param>
        /// <returns>The movement vector.</returns>
        Vector3 Move(Vector3 direction, Transform cameraTransform);
        /// <summary>
        /// Rotates an object towards a specified direction.
        /// </summary>
        /// <param name="direction">The direction to rotate towards.</param>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <returns>The rotation quaternion.</returns>
        Quaternion Rotate(Vector3 direction,Transform transform);
    }
}