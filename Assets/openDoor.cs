using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openDoor : MonoBehaviour
{
    public GameObject left;
    public GameObject right;
    private bool rotating = false;
    public float leftTurn = 90f;
    public float rightTurn = -90f;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(open(3f));
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void genericOpen(float duration)
    {
        StartCoroutine(open(duration));
    }

    public IEnumerator open(float duration)
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;

        Quaternion leftRot = left.transform.rotation;
        Quaternion rightRot = right.transform.rotation;

        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            left.transform.rotation = Quaternion.Lerp(leftRot, Quaternion.Euler(0f, leftTurn, 0f), counter / duration);
            right.transform.rotation = Quaternion.Lerp(rightRot, Quaternion.Euler(0f, rightTurn, 0f), counter / duration);
            yield return null;
        }
        rotating = false;
    }
}
