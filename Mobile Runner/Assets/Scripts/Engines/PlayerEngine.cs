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
        private float horizontalMovementDirection;
        
        private readonly float horizontalMovementSpeed = 10f;
        private float currentForwardMovementSpeed;
        private readonly float startingForwardMovementSpeed = 20f;

        private readonly float respawnOffset = -5f;

        private bool canMove;

        private readonly float angularVelocityHitBoost = 8f;
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            CameraEngine.Instance.CameraTarget = transform;
            currentForwardMovementSpeed = startingForwardMovementSpeed;
        }

        private void OnDisable()
        {
            currentForwardMovementSpeed = 0;
            CameraEngine.Instance.CameraTarget = null;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

        }

        private void Update()
        {
            canMove = LevelManager.Instance.CurrentLevelState.Equals(LEVEL_STATE.RUNNING);

            if (canMove)
            {
                horizontalMovementDirection = InputManager.Instance.GetHorizontalAxis;
             
                LevelManager.Instance.UpdateScoreModifier(currentForwardMovementSpeed - startingForwardMovementSpeed);
            }

            if (PlayerPosition.y <= respawnOffset)
            {
                Die();
            }
        }

        private void FixedUpdate()
        {
            if (canMove)
            {
                rigidbody.position = rigidbody.position + (new Vector3(horizontalMovementDirection * horizontalMovementSpeed, 0, currentForwardMovementSpeed)) * Time.deltaTime;
                rigidbody.position = new Vector3(Mathf.Clamp(rigidbody.position.x, -4, 4), rigidbody.position.y, rigidbody.position.z);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer.Equals(10))
            {
                AudioManager.Instance.PlaySfxAtPoint("Crash", PlayerPosition);

                Die();
            }          
        }

        public void Die()
        {
            canMove = false;

            rigidbody.angularVelocity *= angularVelocityHitBoost;

            LevelManager.Instance.EndGame();
        }

        public void IncreaseMovementSpeed()
        {
            var newForwardSpeed = currentForwardMovementSpeed + FORWARD_SPEED_INCREMENT_MODIFIER;
            currentForwardMovementSpeed = newForwardSpeed < MAX_SPEED_LIMIT ? newForwardSpeed : MAX_SPEED_LIMIT;
        }
    }
}