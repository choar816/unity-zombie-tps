using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public partial class Monster : Character
{
    public enum Part
    {
        Head,
        Body,
    }

    private Animator animator;
    private Rigidbody rb;
    private MonsterPart[] monsterParts;
    private float lastAttackTime;
    private float lastDamageTime;
    
    NavMeshAgent agent;
    int waypointIndex;
    Vector3 waypointTarget;
    Vector3 initPosition;
    List<Vector3> patrolWaypoints;

    [SerializeField] bool DebugMode = false;
    [Range(0f, 360f)] [SerializeField] float ViewAngle = 0f;
    [SerializeField] float ViewRadius = 1f;
    [SerializeField] LayerMask TargetMask;
    [SerializeField] LayerMask ObstacleMask;

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if (m_State == State.Dead)
            return;

        Move();
        UpdateState();
    }

    void UpdateState()
    {
        //Debug.Log(m_State);

        if (GameManager.Instance.m_State != GameManager.State.Playing)
        {
            //m_State = State.Idle;
            return;
        }

        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        float lookingAngle = transform.eulerAngles.y;  // the angle character looks at
        Vector3 lookDir = AngleToDir(lookingAngle);

        Vector3 targetPos = Player.Instance.transform.position;
        Vector3 targetDir = (targetPos - myPos).normalized;
        float targetAngle = Vector3.Angle(lookDir, targetDir);

        // if already chase -> check distance only
        if (m_State == State.Chase || m_State == State.Attack)
        {
            if (Vector3.Distance(myPos, targetPos) < ViewRadius + 1)
            {
                if (DebugMode) Debug.DrawLine(myPos, targetPos, Color.red);
                if (Vector3.Distance(myPos, targetPos) > 1.5f)
                {
                    m_State = State.Chase;
                }
                else if (Time.time - lastAttackTime > m_Stat.SpeedAttack)
                {
                    Attack();
                }
            }
            else
            {
                m_State = State.Return;
            }
        }
        // chase start (idle, patrol -> chase)
        else if ((m_State == State.Patrol || m_State == State.Idle) &&
            targetAngle <= ViewAngle * 0.5f && Physics.Raycast(myPos, targetDir, ViewRadius, TargetMask))
        {
            m_State = State.Chase;
        }
    }

    private void OnDrawGizmos()
    {
        if (!DebugMode) return;

        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);

        float lookingAngle = transform.eulerAngles.y;  // the angle character looks at
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.cyan);
    }

    public override void Init()
    {
        base.Init();
        m_Type = Type.Monster;
        //m_State = State.Patrol;
        m_Stat.SpeedWalk = 0.1f;
        m_Stat.SpeedRotate = 50;
        m_Stat.Damage = 100;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        monsterParts = GetComponentsInChildren<MonsterPart>();
        initPosition = transform.position;
        patrolWaypoints = new List<Vector3>();
    }

    public override void Move()
    {
        //base.Move();

        switch (m_State)
        {
            case State.Chase:
                agent.SetDestination(Player.Instance.transform.position);
                animator.SetBool("walk", true);
                break;
            case State.Patrol:
                if (Vector3.Distance(transform.position, waypointTarget) < 1)
                {
                    IterateWaypointIndex();
                    UpdateDestination();
                    animator.SetBool("walk", true);
                }
                break;
            case State.Return:
                ReturnToPatrol();
                break;
            default:
                animator.SetBool("walk", false);
                break;
        }
    }

    void UpdateDestination()
    {
        if (patrolWaypoints.Count <= waypointIndex)
            return;
        waypointTarget = patrolWaypoints[waypointIndex];
        agent.SetDestination(waypointTarget);
    }

    void IterateWaypointIndex()
    {
        waypointIndex++;
        waypointIndex %= patrolWaypoints.Count;
    }

    void ReturnToPatrol()
    {
        // CASE 1: idle -> chase -> return
        if (patrolWaypoints.Count == 0)
        {
            agent.SetDestination(initPosition);
            if (Vector3.Distance(transform.position, initPosition) < 1)
            {
                m_State = State.Idle;
            }
            return;
        }

        // CASE 2: patrol -> chase -> return
        Vector3 currentPosition = transform.position;
        float nearestDistance = Mathf.Infinity;

        for (int i = 0; i < patrolWaypoints.Count; i++)
        {
            float distance = Vector3.Distance(currentPosition, patrolWaypoints[i]);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                waypointIndex = i;
            }
        }
        UpdateDestination();
        if (Vector3.Distance(transform.position, waypointTarget) < 1)
        {
            m_State = State.Patrol;
        }
    }

    public override void Attack()
    {
        base.Attack();
        m_State = State.Attack;
        animator.SetTrigger("attackTrigger");
        lastAttackTime = Time.time;
    }

    void MakeDamage()
    {
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Vector3 targetPos = Player.Instance.transform.position;
        if (Vector3.Distance(myPos, targetPos) < 1.5f)
        {
            Player.Instance.GetDamage(m_Stat.Damage);
        }
    }

    public override void GetDamage(Part part, float damage)
    {
        if (m_State == State.Dead)
            return;

        base.GetDamage(part, damage);
        
        bool isCritical = false;
        int randomInt = UnityEngine.Random.Range(0, 10);
        if (randomInt >= 5)
            isCritical = true;
        
        if (isCritical)
        {
            damage *= 2;
            animator.SetTrigger("damageTrigger");
        }

        switch (part)
        {
            case Part.Head:
                m_Stat.HP -= damage * 2;
                break;
            case Part.Body:
                m_Stat.HP -= damage * 0.5f;
                break;
        }
        Debug.Log("HP: " + m_Stat.HP);

        if (m_Stat.HP <= 0)
            Die();
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("dieTrigger");
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        foreach (MonsterPart part in monsterParts)
        {
            part.GetComponent<BoxCollider>().enabled = false;
        }
        UIManager.Instance.AddKillValue(1);
    }

    public void Restart()
    {
        transform.position = initPosition;
        agent.SetDestination(initPosition);
        animator.SetTrigger("restartTrigger");
    }

    public void SetPatrolWaypoints(List<Vector3> posDiffs)
    {
        foreach (Vector3 posDiff in posDiffs)
        {
            patrolWaypoints.Add(transform.position + posDiff);
        }
    }

    public void StartPatrol()
    {
        waypointIndex = 0;
        UpdateDestination();
    }

    Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }
}
