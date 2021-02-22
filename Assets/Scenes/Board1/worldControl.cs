using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Gamekit3D
{
    public class worldControl : MonoBehaviour
    {
        public GameObject centerText;
        public spawnControl spawnSystem;
        public PlayerControlAlt player;
        bool inDeath = false;
        bool inEnd = false;
        public Button reset;
        public StatScaling scaleControl;


        // Start is called before the first frame update
        void Start()
        {
            player = FindObjectOfType<PlayerControlAlt>();
            StartCoroutine(startSequence());
        }

        // Update is called once per frame
 

        private void FixedUpdate()
        {
            if (!player.checkAlive() && !inDeath) StartCoroutine(deathSequence());
            if (player.leveled && !inEnd)
            {
                inEnd = true;
                StartCoroutine(GetComponentInChildren<openDoor>().open(3f));
                GetComponentInChildren<spawnControl>().stop();
                //yield return new WaitForSeconds(.1f);
                Destroy(GetComponentInChildren<spawnControl>().gameObject);

                //StartCoroutine(endSequence());
            }
        }

        IEnumerator endSequence()
        {
            StartCoroutine(GetComponentInChildren<openDoor>().open(3f));
            GetComponentInChildren<spawnControl>().stop();
            yield return new WaitForSeconds(.1f);
            Destroy(GetComponentInChildren<spawnControl>().gameObject);
        }

        IEnumerator startSequence()
        {
            centerText.GetComponentInChildren<Text>().text = "The gates have closed.  The path is set.  \nYour only way is forward.";
            centerText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            yield return new WaitForSeconds(3f);
            centerText.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);

            yield return new WaitForSeconds(3f);
            StartCoroutine(spawnSystem.spawn());
        }

        IEnumerator deathSequence()
        {
            inDeath = true;
            reset.transform.localScale = new Vector3(2, 2, 1);
            centerText.GetComponentInChildren<Text>().text = "Arthur has fallen.  \nHis deeds shall be remembered.  \nLevel: "
                + player.GetComponent<PlayerControlAlt>().level + "      XP: " + player.GetComponent<PlayerControlAlt>().xp;

            yield return null;
            centerText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        }

        public void restartBoard()
        {
            //Debug.Log("restart called");
            spawnSystem.stop();
            reset.transform.localScale = new Vector3(0, 2, 1);
            scaleControl.resetScale();
            SceneManager.LoadScene(0);
        }

        public void nextBoard()
        {
            //Debug.Log("restart called");
            //spawnSystem.stop();
            //reset.transform.localScale = new Vector3(0, 2, 1);
            player.GetComponent<PlayerControlAlt>().packStats();
            scaleControl.scale();
            SceneManager.LoadScene(6);
        }

    }
}