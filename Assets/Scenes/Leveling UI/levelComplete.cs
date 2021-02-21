using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class levelComplete : MonoBehaviour
{
    int level;
    // Start is called before the first frame update
    void Start()
    {
        level = FindObjectOfType<playerStats>().level;
        if (level > 6) level = 6;
    }

    public void complete()
    {
        SceneManager.LoadScene(level + 1);
    }

    public void selected()
    {
        GetComponent<Button>().interactable = true;
    }
}
