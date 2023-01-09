using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public Transform rocketSocket;
    public Transform obstacleSocket;

    public List<AudioClip> explosionSounds;

    public Camera mainCam;

    public Text scoreText;

    private int[] heightThresholds = new int[] { 30, 60, 90 };

    private Dictionary<Rocket, float> rockets = new Dictionary<Rocket, float>();
    int nExploded = 0;

    bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in rocketSocket)
        {
            Rocket rocket = t.GetComponent<Rocket>();

            if (rocket != null)
            {
                rockets.Add(rocket, 0);
                rocket.OnExploded += RocketExploded;
            }
        }
    }

    private void RocketExploded(Rocket r, float height)
    {
        
        nExploded++;

        int idxSound = Random.Range(0, explosionSounds.Count);

        // play random explosion sound
        this.GetComponent<AudioSource>().clip = explosionSounds[idxSound];
        this.GetComponent<AudioSource>().Play();

        //if game over, sum up all points
        if (nExploded >= rockets.Count)
        {
            // game over
            int totalScore = (int) rockets.Values.ToList().Sum();

            Debug.Log("Total score: " + totalScore);


            string highscorePrefs = PlayerPrefs.GetString("Highscore");

            if (highscorePrefs == null)
            {
                Debug.Log("no highscore yet");
                highscorePrefs = "";
            }
            else if (highscorePrefs != "")
            {
                highscorePrefs += "\n";
            }

            highscorePrefs += "Me;" + totalScore.ToString();

            PlayerPrefs.SetString("Highscore", highscorePrefs);

            PlayerPrefs.Save();


            gameOver = true;

            // TODO switch over to highscore after 2 sec

            StartCoroutine(SwitchScene());
        //
        }
    }

    IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(2);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            return;

        // instantiate obstacle X units over current camera height
        Vector3 camPos = mainCam.transform.position;
        camPos.z = 0;

        float currentHeight = 0;

        foreach (Transform t in rocketSocket)
        {
            Rocket r = t.GetComponent<Rocket>();

            if (r != null)
            {
                if (r.CurrentHeight > rockets[r])
                    rockets[r] = r.CurrentHeight;

                if (r.CurrentHeight > currentHeight)
                {
                    currentHeight = r.CurrentHeight;
                }
            }
        }

        int heightIdx = 0;

        for (int i = 0; i < heightThresholds.Length; i++)
        {
            heightIdx = i;

            if (currentHeight < heightThresholds[i])
            {
                break;
            }
        }

        //Debug.Log("Height: " + currentHeight);

        int totalScore = (int)rockets.Values.ToList().Sum();


        scoreText.text = "Score: " + (int) totalScore;

        foreach (Transform t in obstacleSocket)
        {
            Obstacles obs = t.GetComponent<Obstacles>();

            if (obs != null)
            {
                if (t.position.y < camPos.y - 8 || t.position.y > camPos.y + 10)
                {
                    Vector3 newPos = camPos + new Vector3(0, 8, 0);

                    t.position = newPos;

                    obs.Respawn();
                }
            }
        }

        while (obstacleSocket.childCount <= heightIdx)
        {
            Vector3 spawnPos = camPos + new Vector3(0, 2 * (1 + obstacleSocket.childCount * 5), 0);

            GameObject newSpawn = Instantiate(Resources.Load("Prefabs/Bird") as GameObject, spawnPos, Quaternion.identity);

            newSpawn.transform.SetParent(obstacleSocket);
        }
    }
}
