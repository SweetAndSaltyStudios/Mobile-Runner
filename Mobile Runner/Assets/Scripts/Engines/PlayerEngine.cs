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
        private float horizontalMovementDirection;
        // private readonly float movementTreshold = 0.1f;
        
        private readonly float horizontalMovementSpeed = 10f;
        private float currentForwardMovementSpeed;
        private float startingForwardMovementSpeed = 20f;

        private readonly float respawnOffset = -5f;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            CameraEngine.Instance.CameraTarget = transform;
            canMove = true;
            currentForwardMovementSpeed = startingForwardMovementSpeed;
        }

        private void OnDisable()
        {
            currentForwardMovementSpeed = 0;
            CameraEngine.Instance.CameraTarget = null;
        }

        private void Update()
        {
            if (canMove == false)
                return;

            if (!GameMaster.Instance.CurrentGamestate.Equals(GAMESTATE.RUNNING))
            {
                return;
            }

#if UNITY_EDITOR

              horizontalMovementDirection = InputManager.Instance.GetHorizontalAxis;
#else
              horizontalMovementDirection = InputManager.Instance.GetHorizontalAxisTilt;
#endif

            if (PlayerPosition.y <= respawnOffset)
            {
                Die();
            }

            LevelManager.Instance.UpdateScoreModifier(currentForwardMovementSpeed - startingForwardMovementSpeed);
        }

        private void FixedUpdate()
        {
            if (canMove == false)
                return;

            if (!GameMaster.Instance.CurrentGamestate.Equals(GAMESTATE.RUNNING))
            {
                return;
            }

            rigidbody.MovePosition(transform.position + (new Vector3(horizontalMovementDirection * horizontalMovementSpeed, 0, currentForwardMovementSpeed)) * Time.deltaTime);
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
            var newForwardSpeed = currentForwardMovementSpeed + FORWARD_SPEED_INCREMENT_MODIFIER;
            currentForwardMovementSpeed = newForwardSpeed < MAX_SPEED_LIMIT ? newForwardSpeed : MAX_SPEED_LIMIT;
        }
    }
}