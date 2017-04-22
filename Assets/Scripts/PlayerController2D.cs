using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField]
    private float maxGroundSpeed = 8f;

    [SerializeField]
    private float maxJumpSpeed = 10f;

    [SerializeField]
    private float groundSpeed = 20f;

    [SerializeField]
    private int maxPlayerWallJump = 2;

    private LevelManager levelManager;
    private GroundState groundState;
    private Rigidbody2D playerBody;
    private Vector2 playerMovement;
    private bool playerIsJump = false;
    private int playerWallJump = 0;

    private void Awake()
    {
        levelManager = GameObject.Find("Level Behaviour").GetComponent<LevelManager>();
        groundState = new GroundState(gameObject);
        playerBody = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gameObject.transform.position = levelManager.playerSpawn;
    }

    private void Update()
    {
        playerMovement = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        if (Input.GetButtonDown("Jump"))
            playerIsJump = true;
    }

    private void FixedUpdate()
    {
        if (playerIsJump && groundState.IsGround())
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
    }
}
