using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public class Stat
    {
        public float FireCoolTime;
        public float LastFireTime;
        public int maxBullet;
        public int currentBullet;
    }

    public GameObject pivot;
    public Stat m_Stat;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        m_Stat = new Stat();
        m_Stat.FireCoolTime = 0.5f;
        m_Stat.LastFireTime = -m_Stat.FireCoolTime;
        m_Stat.maxBullet = 10;        
        m_Stat.currentBullet = 10;
    }

    public void TryFire()
    {
        if (Time.time - m_Stat.LastFireTime >= m_Stat.FireCoolTime && m_Stat.currentBullet > 0)
        {
            Fire();
        }
    }

    void Fire()
    {
        PoolManager.Instance.EnableBullet(pivot.transform.position, pivot.transform.rotation);
        m_Stat.currentBullet -= 1;
        UIManager.Instance.UpdateBulletText();
    }
}
