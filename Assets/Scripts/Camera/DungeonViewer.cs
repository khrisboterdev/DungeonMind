using UnityEngine;

public class DungeonViewer : MonoBehaviour
{
    private Transform _target;

    private void LateUpdate()
    {
        if (_target != null)
        {
            FollowTarget();
        }
    }

    public void StartFollowAgent(Transform target)
    {
        _target = target;
    }

    private void FollowTarget()
    {
        Vector3 newPosition = _target.position;
        newPosition.z = -10;
        transform.position = newPosition;
    }
}