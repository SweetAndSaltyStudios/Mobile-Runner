using System.Collections;
using UnityEngine;

public class CameraEngine : Singelton<CameraEngine>
{
    private Transform currentCameraTarget;
    public float offset;
    private float camerasZOffset;

    public bool IsShaking
    {
        get;
        private set;
    }

    public Transform CameraTarget
    {
        get
        {
            return currentCameraTarget = currentCameraTarget ?? null;
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
        if(CameraTarget == null)
        {
            return;
        }

        FollowTarget(CameraTarget);      
    }

    private void FollowTarget(Transform target)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, currentCameraTarget.position.z + camerasZOffset);
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(IShake(duration, magnitude));
    }

    private IEnumerator Foo(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 20f * Time.deltaTime);
        yield return null;
    }

    private IEnumerator IShake(float duration, float magnitude)
    {
        IsShaking = true;

        var originalPosition = transform.localPosition;

        var elapsed = 0.0f;

        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y + originalPosition.y, originalPosition.z);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;

        IsShaking = false;
    }
}
