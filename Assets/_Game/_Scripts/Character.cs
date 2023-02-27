using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected CombatText combatTextPrefab;
    private float hp;
    private string currentAnim;

    public bool isDeath => hp <= 0;

    private void Start()
    {
        OnInit();
    }

    public virtual void OnInit()
    {
        hp = 100f;
        healthBar.OnInit(100, transform);
    }

    public virtual void OnDespawn()
    {

    }
    protected virtual void OnDeath()
    {
        ChangeAnim("die");
        Invoke(nameof(OnDespawn), 2f);
    }

    protected void ChangeAnim(string animName)
    {
        if (currentAnim != animName)
        {
            anim.SetTrigger(animName);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
        }
    }

    public void OnHit(float damage)
    {
        if (!isDeath)
        {
            hp -= damage;

            if (isDeath)
            {
                hp = 0;
                OnDeath();
            }

            healthBar.SetNewHp(hp);
            Instantiate(combatTextPrefab, transform.position + Vector3.up, Quaternion.identity).OnInit(damage);
        }
    }
}
