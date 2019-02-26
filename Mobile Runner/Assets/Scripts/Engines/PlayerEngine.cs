using UnityEngine;

namespace SweetAndSaltyStudios
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerEngine : MonoBehaviour
    {
        private const float MAX_SPEED_LIMIT = 200f;
        private const float FORWARD_SPEED_INCREMENT_MODIFIER = 0.1f;

        public Vector3 PlayerPosition
        {
            get
            {
                return transform.position;
            }
        }

        private new Rigidbody rigidbody;
        private bool canMove;
        private float movementDirection;
        // private readonly float movementTreshold = 0.1f;
        
        private readonly float horizontalMovementSpeed = 10f;
        private float forwardMovementSpeed = 50f;

        private readonly float respawnOffset = -5f;

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
            forwardMovementSpeed = 0;
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
         
            if (PlayerPosition.y <= respawnOffset)
            {
                Die();
            }
        }

        private void FixedUpdate()
        {
            if (canMove == false)
                return;

            rigidbody.MovePosition(transform.position + (new Vector3(movementDirection * horizontalMovementSpeed, 0, forwardMovementSpeed)) * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer.Equals(10))
            {
                Die();
            }          
        }

        public void Die()
        {
            canMove = false;
            rigidbody.constraints = RigidbodyConstraints.None;
            GameMaster.Instance.RestartScene();
        }

        public void IncreaseMovementSpeed()
        {
            var newForwardSpeed = forwardMovementSpeed + FORWARD_SPEED_INCREMENT_MODIFIER;
            forwardMovementSpeed = newForwardSpeed < MAX_SPEED_LIMIT ? newForwardSpeed : MAX_SPEED_LIMIT;
        }
    }
}