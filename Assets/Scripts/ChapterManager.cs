using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public GameObject[] maps;
    int currentChapter;

    // Start is called before the first frame update
    void Start()
    {
        currentChapter = 0;
        UpdateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateMap()
    {
        for (int i=0; i<maps.Length; ++i)
        {
            if (i == currentChapter)
            {
                maps[i].SetActive(true);
            }
            else
            {
                maps[i].SetActive(false);
            }
        }
    }
}
