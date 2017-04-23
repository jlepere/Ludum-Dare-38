using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lifeObject;

    [SerializeField]
    private int maxPlayerLife = 3;

    [SerializeField]
    private float maxInvincibleTime = 2f;

    private int playerLife = 3;
    private float invincibleTime = 0;
    private bool isInvincible = false;

    private void Awake()
    {
        playerLife = maxPlayerLife;
    }

    public bool IsInvincible
    {
        get { return isInvincible; }
    }

    private void Update()
    {
        if (isInvincible && invincibleTime < maxInvincibleTime)
            invincibleTime += Time.deltaTime * 3f;
        else if (isInvincible && invincibleTime >= maxInvincibleTime)
        {
            invincibleTime = 0;
            isInvincible = false;
        }
    }

    public bool RemoveOneLife()
    {
        playerLife--;
        isInvincible = true;
        RawImage test = lifeObject[playerLife].GetComponent<RawImage>();
        test.color = new Color(176f / 255f, 39f / 255f, 39f / 255f);
        test.uvRect = new Rect(0, -0.5f, 0.5f, 0.5f);
        if (playerLife <= 0)
            return false;
        return true;
    }

    public void AddOneLife()
    {
        if (playerLife >= maxPlayerLife)
            return;
        playerLife++;
        RawImage test = lifeObject[playerLife - 1].GetComponent<RawImage>();
        test.color = new Color(39f / 255f, 176f / 255f, 46f / 255f);
        test.uvRect = new Rect(0.5f, -0.5f, 0.5f, 0.5f);
    }
}
