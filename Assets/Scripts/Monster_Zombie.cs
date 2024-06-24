using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Zombie : Monster
{
    public class Addtional_Stat
    {
        public float Speed_Crawing;
    }

    public Addtional_Stat m_Add_Stat;

    public override void Init()
    {
        base.Init();
        m_Add_Stat.Speed_Crawing = 10;
    }

    public void Growl()
    {
        Debug.Log("Zombie Growls");
    }
}
