using UnityEngine;

public class EnemyController2D : MonoBehaviour
{
    private Rigidbody2D enemyBody;
    private Vector2 spawnPosition;
    private Vector2 movementPosition;
    private EnemyState enemyState;
    private float movementOffset = 1f;
    private float maxWaitingTime = 2f;
    private float waitingTime = 0f;
    private float maxJumpCooldown = 2.5f;
    private float jumpCooldown = 0f;
    private float maxEnemyJump = 10f;
    private float maxEnemySpeed = 10f;
    private float maxEnemyTrackingSpeed = 15f;
    private float enemySpeed = 10f;
    private bool jumpUsed = false;

    private void Awake()
    {
        enemyBody = gameObject.GetComponentInParent<Rigidbody2D>();
        spawnPosition = gameObject.transform.parent.position;
        movementPosition = new Vector2(1, 0);
    }

    private void Start()
    {
        enemyState = EnemyState.Walking;
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                return;
            case EnemyState.Walking:
                EnemyIsWalking();
                break;
            case EnemyState.Waiting:
                EnemyIsWaiting();
                break;
        }
        if (!jumpUsed && jumpCooldown < maxJumpCooldown)
            jumpCooldown += Time.deltaTime * 2f;
        else if (!jumpUsed && jumpCooldown >= maxJumpCooldown)
            jumpUsed = true;
        if (GameManager.IS_DEBUG && enemyState == EnemyState.Walking)
            Debug.DrawLine(enemyBody.transform.position, enemyBody.transform.position + new Vector3(enemyBody.velocity.x, enemyBody.velocity.y, 0), Color.red);
        else if (GameManager.IS_DEBUG && enemyState == EnemyState.Tracking)
            Debug.DrawLine(enemyBody.transform.position, enemyBody.transform.position + new Vector3(enemyBody.velocity.x, enemyBody.velocity.y, 0), Color.blue);
    }

    private void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                return;
            case EnemyState.Walking:
                enemyBody.AddForce(movementPosition * enemySpeed);
                enemyBody.velocity = Vector3.ClampMagnitude(enemyBody.velocity, maxEnemySpeed);
                break;
            case EnemyState.Tracking:
                enemyBody.AddForce(movementPosition * enemySpeed);
                enemyBody.velocity = Vector3.ClampMagnitude(enemyBody.velocity, maxEnemyTrackingSpeed);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (jumpUsed && (enemyState == EnemyState.Walking || enemyState == EnemyState.Tracking) && collision.transform.name == "Level Behaviour")
        {
            jumpUsed = true;
            jumpCooldown = 0;
            enemyBody.velocity = new Vector2(enemyBody.velocity.x, maxEnemyJump);
        }
    }

    public EnemyState EnemyControllerState
    {
        get { return enemyState; }
        set { enemyState = value; }
    }

    public Vector2 MovementPosition
    {
        get { return movementPosition; }
        set { movementPosition = value; }
    }

    public float MovementOffset
    {
        set { movementOffset = value; }
    }

    private void EnemyIsWalking()
    {
        if ((gameObject.transform.position.x <= spawnPosition.x - movementOffset && movementPosition.x == -1) ||
            gameObject.transform.position.x >= spawnPosition.x + movementOffset && movementPosition.x == 1)
        {
            movementPosition = new Vector2(0, 0);
            enemyState = EnemyState.Waiting;
        }
    }

    private void EnemyIsWaiting()
    {
        if (waitingTime < maxWaitingTime)
            waitingTime += Time.deltaTime;
        if (waitingTime >= maxWaitingTime && gameObject.transform.position.x <= spawnPosition.x - movementOffset)
        {
            waitingTime = 0;
            movementPosition = new Vector2(1, 0);
            enemyState = EnemyState.Walking;
        }
        else if (waitingTime >= maxWaitingTime && gameObject.transform.position.x >= spawnPosition.x - movementOffset)
        {
            waitingTime = 0;
            movementPosition = new Vector2(-1, 0);
            enemyState = EnemyState.Walking;
        }
    }
}
