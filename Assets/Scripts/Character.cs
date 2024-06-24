using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum Type
    {
        Player,
        Monster,
    }
    
    public enum State
    {
        Idle,
        Aim,
        Reloading,
        Patrol,
        Chase,
        Return,
        Attack,
        Dead,
    }
    
    public class Stat
    {
        public float HP;
        public float MaxHP;
        public float Damage;
        public float SpeedWalk;
        public float SpeedRun;
        public float SpeedRotate;
        public float SpeedAttack;
        public float SpeedDamage;
        public float ForceJump;
        public bool IsGrounded;
    }
    
    public Type m_Type;
    public State m_State;
    public Stat m_Stat;

    public virtual void Init()
    {
        //Debug.Log(this.gameObject + " Init");

        m_Stat = new Stat();
        m_Stat.HP = 50;
        m_Stat.MaxHP = 100;
        m_Stat.Damage = 10;
        m_Stat.SpeedWalk = 5;
        m_Stat.SpeedRun = 10;
        m_Stat.SpeedRotate = 5;
        m_Stat.SpeedAttack = 2;
        m_Stat.SpeedDamage = 5;
        m_Stat.ForceJump = 5;
    }

    public virtual void Move()
    {
        //Debug.Log(this.gameObject + " Move");
    }

    public virtual void Attack()
    {
        //Debug.Log(this.gameObject + " Attack");
    }

    public virtual void Jump()
    {
        //Debug.Log(this.gameObject + " Jump");
    }

    public virtual void GetDamage(float damage)
    {
        //Debug.Log(this.gameObject + " Get Damage : " + damage);
    }

    public virtual void GetDamage(Monster.Part part, float damage)
    {
        //Debug.Log(this.gameObject + " Part : " + part + ", Get Damage : " + damage);
    }

    public virtual void Die()
    {
        //Debug.Log(this.gameObject + " Die");
        m_State = State.Dead;
    }
}