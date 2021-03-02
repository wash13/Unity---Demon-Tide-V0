using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class levelComplete : MonoBehaviour
{

    playerStats source;
    // Start is called before the first frame update
    void Start()
    {
        source = FindObjectOfType<playerStats>();
        if (source.level > 5) source.level = 5;
    }

    public void complete()
    {
        source.hp = Mathf.RoundToInt(source.hp * 1.02f);
        source.damage = Mathf.RoundToInt(source.damage * 1.02f);
        SceneManager.LoadScene(source.level);
    }

    public void selected()
    {
        GetComponent<Button>().interactable = true;
    }
}
