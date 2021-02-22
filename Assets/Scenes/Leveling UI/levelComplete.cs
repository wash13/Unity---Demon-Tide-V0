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
        if (level > 5) level = 5;
    }

    public void complete()
    {
        SceneManager.LoadScene(level);
    }

    public void selected()
    {
        GetComponent<Button>().interactable = true;
    }
}
