using UnityEngine;

namespace SweetAndSaltyStudios
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerEngine : MonoBehaviour
    {
        private bool canMove;

        public Vector3 PlayerPosition
        {
            get
            {
                return transform.position;
            }
        }

        private new Rigidbody rigidbody;
        private float movementDirection;
        private readonly float movementTreshold = 0.1f;
        
        private readonly float moveSpeed = 10;

        private bool isMovingHorizontaly;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            CameraEngine.Instance.CameraTarget = transform;
            canMove = true;
        }

        private void OnDisable()
        {
            CameraEngine.Instance.CameraTarget = null;
        }

        private void Update()
        {
            if (canMove == false)
                return;

#if UNITY_EDITOR
              movementDirection = InputManager.Instance.GetHorizontalAxis;
#else
              movementDirection = InputManager.Instance.GetHorizontalAxisTilt;
#endif

            isMovingHorizontaly = Mathf.Abs(movementDirection) > movementTreshold;

            if(PlayerPosition.y <= -5f)
            {
                transform.position = new Vector3(0, 0.6f, transform.position.z);
            }
        }

        private void FixedUpdate()
        {
            if(rigidbody.velocity.z < 20)
            {
                rigidbody.AddForce(Vector3.forward * moveSpeed);
            }

            if (isMovingHorizontaly)
            {             
                rigidbody.AddForce(Vector3.right * movementDirection * moveSpeed);
            }    
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer.Equals(10))
            {
                canMove = false;
                rigidbody.constraints = RigidbodyConstraints.None;
                GameMaster.Instance.RestartScene();
            }          
        }
    }
}