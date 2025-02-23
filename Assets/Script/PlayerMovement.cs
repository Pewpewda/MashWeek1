using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro


public class PlayerMovement : MonoBehaviour
{
    public float initialMoveSpeed = 5f;  // Set an initial speed
    private float moveSpeed;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isCarryingSoldier = false;
    private int soldiersDelivered = 0;
    private int soldiersCarried = 0;  // Track how many soldiers are being carried
    private bool gameWon = false;  // Track if the game is won

    public TextMeshProUGUI soldierText; // UI text reference
    public TextMeshProUGUI carriedText; // UI text reference
    public GameObject winText;         // Reference to the "You WIN" text UI

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        moveSpeed = initialMoveSpeed; // Set initial speed
        // Initialize UI
        UpdateUI();
        winText.SetActive(false);  // Hide the win message initially
    }

    void Update()
    {
        // Check for restart key (R) at any time
        if (Input.GetKey(KeyCode.R))
        {
            RestartGame(); // Call the restart function
            return;  // Prevent further actions when restarting
        }

        if (gameWon)
        {
            // Stop movement and actions after the game is won
            return;
        }

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveX < 0) spriteRenderer.flipX = true;
        else if (moveX > 0) spriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        if (!gameWon)
            rb.velocity = moveDirection * moveSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tree"))
        {
            Debug.Log("Game Over! You hit a tree.");
            RestartGame();  // Restart when hitting a tree
        }
        else if (collision.CompareTag("Soldier") && soldiersCarried < 3)
        {
            Debug.Log("Soldier picked up!");
            soldiersCarried++;  // Increase the number of soldiers carried
            moveSpeed = Mathf.Max(initialMoveSpeed - soldiersCarried * 1f, 1f);  // Reduce speed after picking up a soldier (speed can't go below 1)
            Destroy(collision.gameObject);
            UpdateUI();
        }
        else if (collision.CompareTag("Base") && soldiersCarried > 0)
        {
            Debug.Log("Soldier delivered to base!");
            soldiersDelivered += soldiersCarried;  // Add the carried soldiers to delivered
            soldiersCarried = 0;  // Reset carried soldiers
            moveSpeed = initialMoveSpeed;  // Reset speed to initial value
            UpdateUI();

            // Check if the player has delivered all 3 soldiers
            if (soldiersDelivered >= 7)
            {
                WinGame();
            }
        }
    }

    void UpdateUI()
    {
        // Update UI text
        if (soldierText != null)
            soldierText.text = "Soldiers Delivered: " + soldiersDelivered;

        if (carriedText != null)
            carriedText.text = "Carrying: " + soldiersCarried + " Soldier" + (soldiersCarried != 1 ? "s" : "");
    }

    // Call this method when the game is won
    void WinGame()
    {
        gameWon = true;  // Set gameWon to true
        winText.SetActive(true);  // Show the "You WIN" message
        Time.timeScale = 0f;  // Stop the game (pause)
    }

    // Restart the game
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload the current scene
        Time.timeScale = 1f;  // Reset the time scale to normal
        soldiersDelivered = 0;  // Reset soldier count
        soldiersCarried = 0;  // Reset carried soldier count
        isCarryingSoldier = false;  // Reset carry state
        moveSpeed = initialMoveSpeed;  // Reset speed to initial value
        UpdateUI();  // Update UI after restart
        winText.SetActive(false);  // Hide the "You WIN" message
        gameWon = false;  // Reset game state
    }
}


