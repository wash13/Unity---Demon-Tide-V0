using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Gamekit3D
{
    public class worldControl4 : MonoBehaviour
    {
        public GameObject centerText;
        public spawnControlB4 spawnSystem;
        public PlayerControlAlt player;
        bool inDeath = false;
        bool inEnd = false;
        public Button reset;
        private StatScaling scaleControl;


        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("wc4 start");
            player = FindObjectOfType<PlayerControlAlt>();
            Debug.Log("wc4 player found");
            player.GetComponent<PlayerControlAlt>().unpackStats();
            Debug.Log("wc4 stats unpacked");
            StartCoroutine(startSequence());
            scaleControl = FindObjectOfType<StatScaling>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!player.checkAlive() && !inDeath) StartCoroutine(deathSequence());
            if (player.leveled && !inEnd)
            {
                inEnd = true;
                StartCoroutine(GetComponentInChildren<openDoor>().open(3f));
                GetComponentInChildren<spawnControlB4>().stop();
                Destroy(GetComponentInChildren<spawnControlB4>().gameObject);
            }
        }

        IEnumerator startSequence()
        {
            centerText.GetComponentInChildren<Text>().text = "The Burning Pits prepare for war, and its minions grow stronger";
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
            SceneManager.LoadScene(2);
        }

        public void nextBoard()
        {
            //Debug.Log("restart called");
            //spawnSystem.stop();
            //reset.transform.localScale = new Vector3(0, 2, 1);
            player.GetComponent<PlayerControlAlt>().packStats();
            scaleControl.scale();
            SceneManager.LoadScene(7);
        }

    }
}