using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCenter : MonoBehaviour
{
    private static ControlCenter instance = null;

    public ChapterManager ChapterManager;
    public CutSceneManager CutSceneManager;
    public EffectManager EffectManager;
    public UIManager UIManager;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public static ControlCenter Instance
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
}
