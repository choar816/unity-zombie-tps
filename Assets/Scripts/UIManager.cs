using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    public static UIManager Instance
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

    public GameObject ReadyUI;
    public GameObject PlayUI;
    public GameObject GameOverUI;
    public GameObject InventoryUI;
    public List<GameObject> AllUI;
    public TMP_Text HPText;
    public TMP_Text KillText;
    public TMP_Text CoinText;
    public TMP_Text ScoreText;
    public TMP_Text BulletText;
    private float MaxHP;
    private float CurrentHP;
    private int KillValue;
    private int CoinValue;
    private int ScoreValue;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        AllUI.Add(ReadyUI);
        AllUI.Add(PlayUI);
        AllUI.Add(GameOverUI);
        AllUI.Add(InventoryUI);
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        MaxHP = Player.Instance.m_Stat.MaxHP;
        KillValue = 0;
        CoinValue = 0;
        ScoreValue = 0;
        Player.Instance.gun.m_Stat.currentBullet = Player.Instance.gun.m_Stat.maxBullet;

        UpdateHPText();
        UpdateKillText();
        UpdateCoinText();
        UpdateScoreText();
        UpdateBulletText();
    }

    public void AddKillValue(int add)
    {
        KillValue += add;
        UpdateKillText();
        AddScoreValue(300);
    }

    public void AddCoinValue(int add)
    {
        CoinValue += add;
        UpdateCoinText();
        AddScoreValue(100);
    }

    public void AddScoreValue(int add)
    {
        ScoreValue += add;
        UpdateScoreText();
    }

    private void UpdateKillText()
    {
        KillText.text = KillValue.ToString();
    }

    public void UpdateHPText()
    {
        CurrentHP = Player.Instance.m_Stat.HP;
        HPText.text = $"{Mathf.FloorToInt(Mathf.Clamp(CurrentHP, 0, MaxHP))} / {MaxHP}";
        if (CurrentHP == MaxHP)
        {
            HPText.color = Color.green;
        }
        else if (CurrentHP < MaxHP / 3)
        {
            HPText.color = Color.red;
        }
        else
        {
            HPText.color = Color.white;
        }
    }

    public void UpdateCoinText()
    {
        CoinText.text = CoinValue.ToString();
    }

    public void UpdateScoreText()
    {
        ScoreText.text = ScoreValue.ToString();
    }

    public void UpdateBulletText()
    {
        int currentBullet = Player.Instance.gun.m_Stat.currentBullet;
        int maxBullet = Player.Instance.gun.m_Stat.maxBullet;
        BulletText.text = $"{currentBullet} / {maxBullet}";
        if (currentBullet == maxBullet)
        {
            BulletText.color = Color.green;
        }
        else if (currentBullet == 0)
        {
            BulletText.color = Color.red;
        }
        else
        {
            BulletText.color = Color.white;
        }
    }

    public void ShowUI(GameObject UI)
    {
        UI.SetActive(true);
    }

    public void HideUI(GameObject UI)
    {
        UI.SetActive(false);
    }

    public void HideAllUI()
    {
        foreach (GameObject UI in AllUI)
            UI.SetActive(false);
    }
}
