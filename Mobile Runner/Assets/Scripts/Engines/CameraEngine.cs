using UnityEngine;

public class CameraEngine : Singelton<CameraEngine>
{
    private Transform currentCameraTarget;
    private float camerasZOffset;

    public Transform CameraTarget
    {
        get
        {
            return currentCameraTarget = currentCameraTarget ?? transform;
        }
        set
        {
            currentCameraTarget = value ?? null;
        }
    }

    private void Awake()
    {
        camerasZOffset = transform.position.z;
    }

    private void LateUpdate()
    {
        FollowTarget(CameraTarget);      
    }

    private void FollowTarget(Transform target)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, currentCameraTarget.position.z + camerasZOffset);
    }
}
