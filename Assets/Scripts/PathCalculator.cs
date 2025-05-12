using UnityEngine;

public static class PathCalculator
{
    public static Vector3 CalculateRequiredVelocity(Vector3 start, Vector3 target, float timeToTarget)
    {
        Vector3 displacement = target - start;
        Vector3 horizontalDisplacement = new(displacement.x, 0, displacement.z);

        float horizontalDistance = horizontalDisplacement.magnitude;
        float verticalDistance = displacement.y;

        // Horizontal speed needed to reach the target in time
        float horizontalSpeed = horizontalDistance / timeToTarget;

        // Vertical speed needed to reach the height in time (accounting for gravity)
        float verticalSpeed = (verticalDistance - 0.5f * Physics.gravity.y * timeToTarget * timeToTarget) / timeToTarget;

        // Final velocity vector
        Vector3 direction = horizontalDisplacement.normalized;
        Vector3 velocity = direction * horizontalSpeed;
        velocity.y = verticalSpeed;

        return velocity;
    }
}
