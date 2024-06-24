using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public partial class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public static GameManager Instance
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

    public enum State
    {
        Ready,
        Playing,
        Win,
        Lose,
        Paused,
    }

    public State m_State;
    public Button StartButton;
    public Button RestartButton;
    public GameObject Characters;
    public GameObject Coins;

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

        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (m_State == State.Playing)
            {
                PauseGame();
                UIManager.Instance.ShowUI(UIManager.Instance.InventoryUI);
            }
            else if (m_State == State.Paused)
            {
                UIManager.Instance.HideUI(UIManager.Instance.InventoryUI);
                ResumeGame();
            }
        }
    }

    void Init()
    {
        m_State = State.Ready;
        StartButton.onClick.AddListener(StartGame);
        RestartButton.onClick.AddListener(RestartGame);
        ItemUseButton.onClick.AddListener(UseItem);
        ItemDumpButton.onClick.AddListener(RemoveItem);
        InitInventoryUI();
        InitItemData();
        InitItemInfoList();
        UIManager.Instance.HideAllUI();
        UIManager.Instance.ShowUI(UIManager.Instance.ReadyUI);
    }

    public void StartGame()
    {
        m_State = State.Playing;
        UIManager.Instance.Init();
        UIManager.Instance.HideAllUI();
        UIManager.Instance.ShowUI(UIManager.Instance.PlayUI);
        PoolManager.Instance.InitMonsterPatrol();
    }

    public void LoseGame()
    {
        m_State = State.Lose;
        UIManager.Instance.HideAllUI();
        UIManager.Instance.ShowUI(UIManager.Instance.GameOverUI);
    }

    public void PauseGame()
    {
        m_State = State.Paused;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        m_State = State.Playing;
        Time.timeScale = 1;
    }

    void RestartGame()
    {
        StartGame();
        RestartPlayer();
        PoolManager.Instance.RestartMonsters();
        PoolManager.Instance.InitMonsterPatrol();
    }

    void RestartPlayer()
    {
        Player.Instance.Restart();
        Player.Instance.Init();
    }
}
