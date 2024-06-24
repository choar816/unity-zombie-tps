using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : Character
{
    private static Player instance = null;
    public static Player Instance
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

    Character.State prevState;
    private Rigidbody rb;
    private BoxCollider boxCollider;
    public Animator animator;
    private Camera mainCamera;
    public Transform cameraPivot;
    public Gun gun;
    Vector3 initPosition;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        Init();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void Update()
    {

        if (m_State == State.Dead ||
            GameManager.Instance.m_State != GameManager.State.Playing)
            return;

        //Debug.Log(m_State);
        RotateHorizontal();
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
        ZoomInOrOut();
        if (m_State == State.Aim && Input.GetMouseButtonDown(0))
            gun.TryFire();
        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    public override void Init()
    {
        base.Init();
        m_Type = Type.Player;
        m_State = State.Idle;
        initPosition = transform.position;
    }

    bool CheckHitWall(Vector3 movement)
    {
        movement = transform.TransformDirection(movement);
        float scope = 1f;

        List<Vector3> rayPositions = new List<Vector3>();
        rayPositions.Add(transform.position + Vector3.up * 0.1f);
        rayPositions.Add(transform.position + Vector3.up * boxCollider.size.y * 0.5f);
        rayPositions.Add(transform.position + Vector3.up * boxCollider.size.y);

        foreach (Vector3 pos in rayPositions)
        {
            Debug.DrawRay(pos, movement * scope, Color.red);
        }
        foreach (Vector3 pos in rayPositions)
        {
            if (Physics.Raycast(pos, movement, out RaycastHit hit, scope))
            {
                if (hit.collider.CompareTag("Wall"))
                    return true;
            }
        }
        return false;
    }

    public override void Move()
    {
        //base.Move();

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        if (CheckHitWall(movement))
            movement = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("run", true);
            animator.SetBool("walk", false);
            transform.Translate(movement * m_Stat.SpeedRun * Time.deltaTime);
        }
        else
        {
            animator.SetBool("run", false);
            animator.SetBool("walk", true);
            transform.Translate(movement * m_Stat.SpeedWalk * Time.deltaTime);
        }

        if (movement == Vector3.zero)
        {
            animator.SetBool("run", false);
            animator.SetBool("walk", false);
        }
    }

    void RotateHorizontal()
    {
        Vector3 horizontalRotation = new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.Rotate(horizontalRotation * m_Stat.SpeedRotate);
    }

    public override void Jump()
    {
        base.Jump();
        rb.AddForce(Vector3.up * m_Stat.ForceJump, ForceMode.Impulse);
    }

    public override void Attack()
    {
        base.Attack();
    }

    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);
        m_Stat.HP -= damage;
        StartCoroutine(DamageAnimation());
        UIManager.Instance.UpdateHPText();

        if (m_Stat.HP <= 0)
            Die();
    }

    private IEnumerator DamageAnimation()
    {
        animator.SetBool("damage", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("damage", false);
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("dieTrigger");
        GameManager.Instance.LoseGame();
    }

    void ZoomInOrOut()
    {
        float zoomInFOV = 40f;
        float zoomOutFOV = 60f;
        float zoomFactor = 0.2f;

        if (m_State == State.Reloading)
            return;

        if (m_State != State.Aim && Input.GetMouseButton(1))
        {
            m_State = State.Aim;
            animator.SetBool("aim", true);
        }
        if (m_State == State.Aim && !Input.GetMouseButton(1))
        {
            m_State = State.Idle;
            animator.SetBool("aim", false);
        }

        if (m_State == State.Aim && Math.Abs(mainCamera.fieldOfView - zoomInFOV) > 0.1f)
        {
            mainCamera.fieldOfView -= zoomFactor;
        }
        else if (m_State != State.Aim && Math.Abs(mainCamera.fieldOfView - zoomOutFOV) > 0.1f)
        {
            mainCamera.fieldOfView += zoomFactor;
        }
    }

    public void Reload()
    {
        if (gun.m_Stat.currentBullet == gun.m_Stat.maxBullet ||
            m_State == State.Reloading)
            return;
        prevState = m_State;
        m_State = State.Reloading;
        animator.SetTrigger("reloadTrigger");
    }

    void FinishReload()
    {
        m_State = prevState;
        gun.m_Stat.currentBullet = gun.m_Stat.maxBullet;
        UIManager.Instance.UpdateBulletText();
    }

    public void Restart()
    {
        transform.position = initPosition;
        animator.SetTrigger("restartTrigger");
    }

    public void GainHP(int hp)
    {
        m_Stat.HP = Mathf.Min(m_Stat.HP + hp, 100);
        UIManager.Instance.UpdateHPText();
    }
}
