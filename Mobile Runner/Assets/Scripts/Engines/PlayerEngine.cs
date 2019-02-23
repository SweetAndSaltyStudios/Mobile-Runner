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
        
        private readonly float moveSpeed = 2000;

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
            //rigidbody.MovePosition(Vector3.forward * moveSpeed);

            if (isMovingHorizontaly)
            {
                //var newPosition = rigidbody.position + Vector3.right * movementDirection;
                //newPosition.x = Mathf.Clamp(newPosition.x, -5, 5);
                //rigidbody.MovePosition(newPosition);
                rigidbody.AddForce(Vector3.right * movementDirection * moveSpeed);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer.Equals(9))
            {
                other.gameObject.SetActive(false);
                LevelManager.Instance.AddCollectable(1);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            canMove = false;
            GameMaster.Instance.RestartScene();
        }
    }
}