using UnityEngine;

public class Obstacle : MonoBehaviour
{
    //private Vector3 defaultPosition;
    //private Quaternion defaultRotation;
    //private new Rigidbody rigidbody;

    private void Awake()
    {
        //rigidbody = GetComponent<Rigidbody>();
        //defaultPosition = transform.position;
        //defaultRotation = transform.rotation;
    }

    private void OnEnable()
    {
        //rigidbody.transform.SetPositionAndRotation(defaultPosition, defaultRotation);
        //rigidbody.velocity = Vector3.zero;
        //rigidbody.angularVelocity = Vector3.zero;
    }

    private void OnDisable()
    {
        //transform.SetPositionAndRotation(defaultPosition, defaultRotation);
        //rigidbody.velocity = Vector3.zero;
        //rigidbody.angularVelocity = Vector3.zero;
    }
}
