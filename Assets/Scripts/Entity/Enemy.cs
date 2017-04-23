using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Sprite[] enemySprites;

    [SerializeField]
    private float maxDestroyTime = 1f;

    [SerializeField]
    private int maxEnemyLife = 2;

    private SpriteRenderer spriteRenderer;
    private float destroyTime = 0f;
    private int enemyLife = 0;
    private int enemyType = 0;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemySprites[enemyType];
        enemyLife = maxEnemyLife;
    }

    private void Update()
    {
        if (enemyLife <= 0 && destroyTime < maxDestroyTime)
            destroyTime += Time.deltaTime * 3f;
        else if (enemyLife <= 0 && destroyTime >= maxDestroyTime)
            gameObject.SetActive(false);
    }

    public int EnemyType
    {
        get { return enemyType; }
    }

    public void RemoveOneLife()
    {
        enemyLife--;
    }

    public void SwapType()
    {
        if (enemyType == 0)
            enemyType = 1;
        else
            enemyType = 0;
        spriteRenderer.sprite = enemySprites[enemyType];
    }
}
