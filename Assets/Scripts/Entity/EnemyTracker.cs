using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    private EnemyController2D enemyController;
    private Rigidbody2D enemyBody;

    private void Awake()
    {
        var parenObject = gameObject.transform.parent.gameObject;
        enemyController = parenObject.GetComponentInChildren<EnemyController2D>();
        enemyBody = parenObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player Behaviour" && enemyController.EnemyControllerState != EnemyState.Idle)
        {
            int newMovement = gameObject.transform.position.x < collision.gameObject.transform.position.x ? 1 : -1;
            if (newMovement != enemyController.MovementPosition.x)
            {
                enemyBody.AddForce(Vector2.zero);
                enemyBody.velocity = Vector2.zero;
                enemyController.MovementPosition = new Vector2(newMovement, 0);
            }
            enemyController.EnemyControllerState = EnemyState.Tracking;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player Behaviour" && enemyController.EnemyControllerState != EnemyState.Idle)
            enemyController.EnemyControllerState = EnemyState.Walking;
    }
}
