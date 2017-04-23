using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private Sprite[] enemySprites;

    [SerializeField]
    private int maxEnemyLife = 2;

    [SerializeField]
    private float movementOffset = 3f;

    [SerializeField]
    private float groundSpeed = 10f;

    [SerializeField]
    private float maxGroundSpeed = 10f;

    [SerializeField]
    private float maxTrackingSpeed = 15f;

    [SerializeField]
    private float waitingTime = 2f;

    [SerializeField]
    private float maxDestroyTime = 1f;

    [SerializeField]
    private int enemyType = 0;

    private SpriteRenderer spriteRenderer;
    private EnemyState enemyState;
    private Rigidbody2D enemyBody;
    private Vector2 startPosition;
    private Vector2 enemyMovement;
    private float waitSince = 0f;
    private float destroyTime = 0f;
    private int enemyLife;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemySprites[enemyType];
        enemyBody = gameObject.GetComponent<Rigidbody2D>();
        startPosition = gameObject.transform.position;
        enemyMovement = new Vector2(1, 0);
        enemyLife = maxEnemyLife;
    }

    private void Update()
    {
        if (enemyState == EnemyState.Destroyed)
            return;
        switch (enemyState)
        {
            case EnemyState.Walking:
                EnemyIsWalking();
                break;
            case EnemyState.Waiting:
                EnemyIsWaiting();
                break;
        }
        if (enemyLife <= 0 && destroyTime < maxDestroyTime)
            destroyTime += Time.deltaTime * 3f;
        else if (enemyLife <= 0 && destroyTime >= maxDestroyTime)
            gameObject.SetActive(false);
        Debug.DrawLine(enemyBody.transform.position, enemyBody.transform.position + new Vector3(enemyBody.velocity.x, enemyBody.velocity.y, 0), Color.red);
    }

    public int EnemyType
    {
        get { return enemyType; }
    }

    public void RemoveOneLife()
    {
        enemyLife--;
    }

    public void ChangeType()
    {
        if (enemyType == 0)
            enemyType = 1;
        else
            enemyType = 0;
        spriteRenderer.sprite = enemySprites[enemyType];
    }

    private void EnemyIsWalking()
    {
        if ((gameObject.transform.position.x <= startPosition.x - movementOffset && enemyMovement.x == -1) ||
            gameObject.transform.position.x >= startPosition.x + movementOffset && enemyMovement.x == 1)
        {
            enemyMovement = new Vector2(0, 0);
            enemyState = EnemyState.Waiting;
        }
    }

    private void EnemyIsWaiting()
    {
        if (waitSince < waitingTime)
            waitSince += Time.deltaTime;
        if (waitSince >= waitingTime && gameObject.transform.position.x <= startPosition.x - movementOffset)
        {
            waitSince = 0;
            enemyMovement = new Vector2(1, 0);
            enemyState = EnemyState.Walking;
        }
        else if (waitSince >= waitingTime && gameObject.transform.position.x >= startPosition.x - movementOffset)
        {
            waitSince = 0;
            enemyMovement = new Vector2(-1, 0);
            enemyState = EnemyState.Walking;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player Behaviour")
        {
            
            int newMovement = gameObject.transform.position.x < collision.gameObject.transform.position.x ? 1 : -1;
            if (newMovement != enemyMovement.x)
            {
                enemyBody.AddForce(Vector2.zero);
                enemyBody.velocity = Vector2.zero;
                enemyMovement = new Vector2(newMovement, 0);
            }
            enemyState = EnemyState.Tracking;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player Behaviour")
            enemyState = EnemyState.Walking;
    }

    private void FixedUpdate()
    {
        if (enemyState == EnemyState.Destroyed)
            return;
        enemyBody.AddForce(enemyMovement * groundSpeed);
        if (enemyState == EnemyState.Tracking)
            enemyBody.velocity = Vector3.ClampMagnitude(enemyBody.velocity, maxTrackingSpeed);
        else
            enemyBody.velocity = Vector3.ClampMagnitude(enemyBody.velocity, maxGroundSpeed);
    }

    private enum EnemyState
    {
        Walking,
        Tracking,
        Waiting,
        Destroyed
    }
}
