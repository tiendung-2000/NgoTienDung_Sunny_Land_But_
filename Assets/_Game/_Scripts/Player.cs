using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;

    private bool isGround = true;
    private bool isJumping = false;
    private bool isAttack;
 
    private float horizontal;

    private int coin = 0;

    private Vector3 savePoint;

    private void Awake()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
    }

    private void Update()
    {
        if (isDeath) return;

        isGround = CheckGrounded();

       // horizontal = Input.GetAxisRaw("Horizontal");

        if (isAttack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isGround)
        {
            if (isJumping)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGround)
            {
                Jump();
            }

            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket) && isGround)
            {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.RightBracket) && isGround)
            {
                Throw();
            }
        }

        if (!isGround && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }

        if (Mathf.Abs(horizontal) > 0.1f)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            transform.rotation = Quaternion.Euler(new Vector3(0f, horizontal > 0 ? 0f : 180f, 0f));
        }
        else if (isGround)
        {
            rb.velocity = Vector2.zero;
            ChangeAnim("idle");
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        isAttack = false;

        transform.position = savePoint;

        ChangeAnim("idle");

        DeActiveAttack();

        SavePoint();

        UIManager.Instance.SetCoin(coin);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

    public void Attack()
    {
        ChangeAnim("attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 1f);

        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 0.5f);
    }

    public void Throw()
    {
        ChangeAnim("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 1f);

        Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);
    }

    public void Jump()
    {
        isJumping = true;
        ChangeAnim("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }

    private void ResetAttack()
    {
        isAttack = false;
        ChangeAnim("idle");
    }

    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        return hit.collider != null;
    }

    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeActiveAttack()
    {
        attackArea.SetActive(false);
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            coin++;
            PlayerPrefs.SetInt("coin", coin);
            UIManager.Instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }
        if (collision.tag == "DeadZone")
        {
            ChangeAnim("die");
            Invoke(nameof(OnInit), 1f);
        }
    }
}
