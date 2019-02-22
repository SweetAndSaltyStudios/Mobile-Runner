using UnityEngine;

public class PlayerEngine : MonoBehaviour
{
    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime);
    }
}
