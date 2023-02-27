using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Kunai : MonoBehaviour
{
    public GameObject hitVfx;

    public Rigidbody2D rb;

    private void Start()
    {
        OnInit();
    }

    public void OnInit()
    {
        rb.velocity = transform.right * 5f;
        Invoke(nameof(OnDespawn), 4f);
    }

    public void OnDespawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.GetComponent<Character>().OnHit(30f);
            Instantiate(hitVfx, transform.position, transform.rotation);
            OnDespawn();
        }
    }
}
