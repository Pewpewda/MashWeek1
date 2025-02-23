using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro


public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isCarryingSoldier = false;
    private int soldiersDelivered = 0;

    public TextMeshProUGUI soldierText; // UI text reference
    public TextMeshProUGUI carriedText; // UI text reference

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        // Initialize UI
        UpdateUI();
    }

    void Update()
    {
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
        rb.velocity = moveDirection * moveSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tree"))
        {
            Debug.Log("Game Over! You hit a tree.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
