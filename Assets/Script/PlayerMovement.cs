using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro


public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isCarryingSoldier = false;
    private int soldiersDelivered = 0;
    private bool gameWon = false;  // Track if the game is won

    public TextMeshProUGUI soldierText; // UI text reference
    public TextMeshProUGUI carriedText; // UI text reference
    public GameObject winText;         // Reference to the "You WIN" text UI

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

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
        else if (collision.CompareTag("Soldier") && !isCarryingSoldier)
        {
            Debug.Log("Soldier picked up!");
            isCarryingSoldier = true;
            Destroy(collision.gameObject);
            UpdateUI();
        }
        else if (collision.CompareTag("Base") && isCarryingSoldier)
        {
            Debug.Log("Soldier delivered to base!");
            isCarryingSoldier = false;
            soldiersDelivered++;
            UpdateUI();

            // Check if the player has delivered all 3 soldiers
            if (soldiersDelivered >= 3)
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
            carriedText.text = "Carrying: " + (isCarryingSoldier ? "1 Soldier" : "None");
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
        isCarryingSoldier = false;  // Reset carry state
        UpdateUI();  // Update UI after restart
        winText.SetActive(false);  // Hide the "You WIN" message
        gameWon = false;  // Reset game state
    }
}

