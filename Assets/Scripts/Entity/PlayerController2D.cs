using UnityEngine;
using UnityEngine.UI;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField]
    private GameObject levelCamera;

    [SerializeField]
    private Sprite[] playerSprite;

    [SerializeField]
    private RawImage dashCooldown;

    [SerializeField]
    private RawImage swapCooldown;

    [SerializeField]
    private float maxGroundSpeed = 8f;

    [SerializeField]
    private float maxJumpSpeed = 10f;

    [SerializeField]
    private float groundSpeed = 20f;

    [SerializeField]
    private int maxPlayerWallJump = 2;

    [SerializeField]
    private float maxBumpedTime = 0.5f;

    [SerializeField]
    private float maxDashTime = 1f;

    [SerializeField]
    private float maxSwapTime = 1f;

    private LevelManager levelManager;
    private PlayerManager playerManager;
    private SpriteRenderer spriteRenderer;
    private GroundState groundState;
    private Rigidbody2D playerBody;
    private Vector2 playerMovement;
    private DashState dashState;
    private bool playerIsJump = false;
    private bool playerIsBumped = false;
    private bool playerIsSwaped = false;
    private int playerWallJump = 0;
    private int playerState = 0;
    private float bumpedTime = 0;
    private float dashTime = 0;
    private float swapTime = 0;

    private void Awake()
    {
        groundState = new GroundState(gameObject);
        levelManager = GameObject.Find("Level Behaviour").GetComponent<LevelManager>();
        playerManager = gameObject.GetComponent<PlayerManager>();
        playerBody = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = playerSprite[playerState];
        dashCooldown.gameObject.SetActive(false);
        swapCooldown.gameObject.SetActive(false);
    }

    private void Start()
    {
        gameObject.transform.position = levelManager.playerSpawn;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (!playerManager.IsInvincible)
            {
                EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
                int newMovement = gameObject.transform.position.x > collision.gameObject.transform.position.x ? 1 : -1;
                if (dashState == DashState.Dashing && enemy.EnemyType != playerState && !playerIsBumped)
                {
                    if (playerState == 0)
                        playerState = 1;
                    else if (playerState == 1)
                        playerState = 0;
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(20 * -newMovement, 10);
                    spriteRenderer.sprite = playerSprite[playerState];
                    enemy.RemoveOneLife();
                    playerManager.AddOneLife();
                }
                else if (!playerIsBumped)
                {
                    enemy.ChangeType();
                    playerBody.velocity = new Vector2(20 * newMovement, 10);
                    if (!playerManager.RemoveOneLife())
                        gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
                playerIsBumped = true;
            }
        }
    }

    private void Update()
    {
        
        if (playerIsBumped && bumpedTime < maxBumpedTime)
            bumpedTime += Time.deltaTime * 3f;
        if (playerIsSwaped && swapTime < maxSwapTime)
            swapTime += Time.deltaTime * 2f;
        else if (playerIsSwaped && swapTime >= maxSwapTime)
        {
            swapTime = 0;
            playerIsSwaped = false;
            swapCooldown.gameObject.SetActive(false);
        }
        playerMovement = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        if (Input.GetButtonDown("Jump"))
            playerIsJump = true;
        else if (Input.GetButtonDown("Swap") && !playerIsSwaped)
            PlayerSwap();
        switch (dashState)
        {
            case DashState.Ready:
                if (Input.GetButtonDown("Dash") && playerMovement.x != 0)
                {
                    playerBody.velocity = new Vector2(playerMovement.normalized.x * 30, 15);
                    dashState = DashState.Dashing;
                }
                break;
            case DashState.Dashing:
                dashTime += Time.deltaTime * 3f;
                if (dashTime >= maxDashTime)
                {
                    dashTime = maxDashTime;
                    if (playerWallJump > 0)
                        playerWallJump--;
                    dashCooldown.gameObject.SetActive(true);
                    dashState = DashState.Cooldown;
                }
                break;
            case DashState.Cooldown:
                dashTime -= Time.deltaTime;
                if (dashTime <= 0)
                {
                    dashTime = 0;
                    dashCooldown.gameObject.SetActive(false);
                    dashState = DashState.Ready;
                }
                break;
        }
        Debug.DrawLine(playerBody.transform.position, playerBody.transform.position + new Vector3(playerBody.velocity.x, playerBody.velocity.y, 0), Color.green);
    }

    private void PlayerSwap()
    {
        playerIsSwaped = true;
        if (playerState == 0 && !playerIsBumped)
            playerState = 1;
        else if (playerState == 1 && !playerIsBumped)
            playerState = 0;
        spriteRenderer.sprite = playerSprite[playerState];
        swapCooldown.gameObject.SetActive(true);
    }

    private void FixeDashCooldownGUI()
    {
        Vector2 dashCooldownPosition = new Vector2((int)((-levelCamera.transform.position.x + 10f + gameObject.transform.position.x) * 24) - 240,
            (int)(((levelCamera.transform.position.y < 0 ? -levelCamera.transform.position.y : levelCamera.transform.position.y) + 7f + gameObject.transform.position.y) * 24) - 178);
        dashCooldown.GetComponent<RectTransform>().localPosition = dashCooldownPosition;
        dashCooldown.GetComponent<RectTransform>().sizeDelta = new Vector2(24 - (24 * dashTime), 2);
    }

    private void FixeSwapCooldownGUI()
    {
        Vector2 swapCooldownPosition = new Vector2((int)((-levelCamera.transform.position.x + 10f + gameObject.transform.position.x) * 24) - 240,
            (int)(((levelCamera.transform.position.y < 0 ? -levelCamera.transform.position.y : levelCamera.transform.position.y) + 7f + gameObject.transform.position.y) * 24) - 174);
        swapCooldown.GetComponent<RectTransform>().localPosition = swapCooldownPosition;
        swapCooldown.GetComponent<RectTransform>().sizeDelta = new Vector2((24 * swapTime), 2);
    }

    private void FixedUpdate()
    {
        if (dashState != DashState.Ready)
            FixeDashCooldownGUI();
        if (playerIsSwaped)
            FixeSwapCooldownGUI();
        if (playerIsJump && !playerIsBumped && groundState.IsGround())
            playerBody.velocity = new Vector2(playerBody.velocity.x, maxJumpSpeed);
        if (playerMovement.x == 0 && groundState.IsGround())
            playerBody.velocity = new Vector2(playerBody.velocity.x * 0.9f, playerBody.velocity.y);
        else if (playerMovement.x != 0 && playerBody.velocity.x * playerMovement.normalized.x < maxGroundSpeed)
            playerBody.AddForce(playerMovement.normalized * groundSpeed);
        if (playerIsJump && playerWallJump < maxPlayerWallJump && groundState.IsWall() && !groundState.IsGround())
        {
            playerBody.velocity = new Vector2(-groundState.WallDirection() * maxGroundSpeed * 0.80f, maxJumpSpeed);
            playerWallJump++;
        }
        if (groundState.IsTouching())
            playerBody.velocity = Vector3.ClampMagnitude(playerBody.velocity, maxGroundSpeed);
        else
            playerBody.velocity = Vector3.ClampMagnitude(playerBody.velocity, maxJumpSpeed);
        if (groundState.IsGround())
            playerWallJump = 0;
        playerIsJump = false;
        if (playerIsBumped && bumpedTime >= maxBumpedTime && groundState.IsGround())
        {
            playerIsBumped = false;
            bumpedTime = 0;
        }
    }

    private enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }
}
