using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance = null;

    public GameObject BulletPrefab;
    public Transform BulletList;
    public GameObject Characters;
    List<Bullet> Bullet_List;
    List<Monster> Monster_List;
    List<Vector3> Monster_Patrol_Positions;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        InitBullet();
        InitMonster();
    }

    public static PoolManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    void InitBullet()
    {
        Bullet_List = new List<Bullet>();
        for (int i = 0; i < 10; ++i)
        {
            AddBullet(false, Vector3.zero, Quaternion.identity);
        }
    }

    void InitMonster()
    {
        Monster_List = new List<Monster>();
        Monster_List = Characters.GetComponentsInChildren<Monster>().ToList();
    }

    public void InitMonsterPatrol()
    {
        Monster_Patrol_Positions = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(2, 0, 2),
            new Vector3(-2, 0, -2),
            new Vector3(2, 0, -2),
            new Vector3(-2, 0, 2),
            new Vector3(3, 0, 0),
            new Vector3(-3, 0, 0)
        };

        int numberOfPosToSelect = 3;
        foreach (Monster monster in Monster_List)
        {
            if (monster.m_State == Character.State.Idle)
                continue;

            List<Vector3> positionsCopy = new List<Vector3>(Monster_Patrol_Positions);
            List<Vector3> selectedPositions = new List<Vector3>();

            while (selectedPositions.Count < numberOfPosToSelect && positionsCopy.Count > 0)
            {
                int randomIndex = Random.Range(0, positionsCopy.Count);
                selectedPositions.Add(positionsCopy[randomIndex]);
                positionsCopy.RemoveAt(randomIndex);
            }

            monster.SetPatrolWaypoints(selectedPositions);
            monster.StartPatrol();
        }
    }

    public void EnableBullet(Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < Bullet_List.Count; ++i)
        {
            if (!Bullet_List[i].gameObject.activeSelf)
            {
                Bullet_List[i].transform.position = position;
                Bullet_List[i].transform.rotation = rotation;
                Bullet_List[i].gameObject.SetActive(true);
                return;
            }
        }

        // when all the bullets are enabled already
        AddBullet(true, position, rotation);
    }

    void AddBullet(bool isActive, Vector3 position, Quaternion rotation)
    {
        Bullet bullet = Instantiate(BulletPrefab).GetComponent<Bullet>();
        bullet.transform.parent = BulletList;
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.gameObject.SetActive(isActive);
        Bullet_List.Add(bullet);
    }

    public void RestartMonsters()
    {
        foreach (Monster monster in Monster_List)
        {
            monster.Restart();
            monster.Init();
        }
    }
}
