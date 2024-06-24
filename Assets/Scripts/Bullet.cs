using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public class Stat
    {
        public float Damage;
        public float Speed;
    }

    public Stat m_Stat;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * m_Stat.Speed * Time.deltaTime);
    }

    void Init()
    {
        m_Stat = new Stat();
        m_Stat.Damage = 10; // temp
        m_Stat.Speed = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        if (other.GetComponent<MonsterPart>())
        {
            Monster monster = other.transform.parent.GetComponent<Monster>();
            if (monster != null)
            {
                monster.GetDamage(other.GetComponent<MonsterPart>().part, m_Stat.Damage);
            }
        }
    }
}
