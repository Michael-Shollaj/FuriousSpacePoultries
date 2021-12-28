using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public List<Bird> birds;
    public List<Pig> pigs;
    public static GameManager instance;

    public Vector3 originPos;//initial position
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;
    public int birdCount;
    public int pigCount;
    public GameObject[] stars;
    public int starCount = 0;//Score for the number of stars in the current level
    private int totalLevel=6;
    void Initialized() {
        for (int i = 0; i < birds.Count; i++) {
            //The Bird script of the first bird is activated, and the SpringJoint2D component is activated
            if (i == 0) {
                //Set the position of the first bird to the initial position
                birds[i].gameObject.transform.position = originPos;
                birds[i].onGround = false;
                birds[i].enabled = true;
                birds[i].sj2d.enabled = true;
            }
            else {
                birds[i].enabled = false;
                birds[i].sj2d.enabled = false;
            }
        }
    }
    void Awake() {
        instance = this;
        //If the number of birds in the scene is greater than 0, record the position of the bird at index 0 as the initial position
        if (birds.Count > 0) {
            originPos = birds[0].transform.position;
        }
    }
    // Start is called before the first frame update
    void Start() {
        Initialized();
        birdCount = birds.Count;
        pigCount = pigs.Count;
    }

    // Update is called once per frame
    void Update() {
    }

    public void NextBird() {
        if (pigs.Count > 0) {
            if (birds.Count > 0) {
                //Next bird on the slingshot frame
                Initialized();
            }
            else {
                losePanel.SetActive(true);
            }
        }
        else {
            //win
            winPanel.SetActive(true);
        }
    }

    public void DisplayStars() {
        StartCoroutine("Stars");
    }

    public IEnumerator Stars() {
        if (birds.Count==birdCount-pigCount) {
            starCount = 2;
        }
        else {
            starCount = birds.Count > birdCount - pigCount ? 3 : 1;
        }
        for (int i = 0; i < starCount; i++) {
            yield return new WaitForSeconds(0.5f);
            stars[i].SetActive(true);
        }
        //The number of stars displayed indicates that the level has been completed, and the score is stored at this time
        DataSave();
    }
    //Number of stored scores
    public void DataSave() {
        //Save the score of the level currently being played
        string currentLevel = PlayerPrefs.GetString("CurrentLevel");
        string currentMap = PlayerPrefs.GetString("CurrentMap");
        //string index = currentLevel.Substring(5);
        //PlayerPrefs.SetInt("Level"+index+"Pass",1);//Set the clearance status of the current level

        int historyScore = PlayerPrefs.GetInt(currentLevel);//Highest score 
        //Break the record, update the score display
        if (historyScore<starCount) {
            PlayerPrefs.SetInt(currentLevel, starCount);
        }
        //Calculate the number of stars in the total level in a map
        int sum = 0;
        for (int i = 1; i <= totalLevel; i++) {
            sum+=PlayerPrefs.GetInt("Level" + i+"Of"+currentMap);
        }
        //totalNumOfStarInMap1
        //Save current map
        PlayerPrefs.SetInt("TotalNumOfStarsIn"+currentMap,sum);
    }

    public void Pause() {
        pausePanel.GetComponent<Animator>().SetBool("isPause",true);
    }

    public void Retry() {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    public void Home() {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
}
