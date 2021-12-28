using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour {
    public Animator anim;
    public GameObject pauseButton;
    void Awake() {
        anim = GetComponent<Animator>();
        //The UI that covers the whole world does not interact with the mouse
        transform.Find("All").GetComponent<Image>().raycastTarget = false;
        transform.Find("All").transform.Find("LeftPopWindow").GetComponent<Image>().raycastTarget = false;
    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }
    public void Pause() {
        anim.SetBool("isPause", true);
    }

    public void Resume() {
        Time.timeScale = 1;
        anim.SetBool("isPause", false);
        //Are there any birds in the scene
        if (GameManager.instance.birds.Count > 0) {
            //If the bird on the slingshot has not yet flown (unlaunched)
            if (GameManager.instance.birds[0].isReleased == false) {
                //Unmovable
                GameManager.instance.birds[0].canMove = true;
            }
        }
    }

    public void Retry() {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    //Animation complete event
    //pause animation event
    public void PauseAnimStart() {
        pauseButton.SetActive(false);
    }
    public void PauseAnimEnd() {
        //Are there any birds in the scene
        if (GameManager.instance.birds.Count > 0) {
            //If the bird on the slingshot has not yet flown (unlaunched)
            if (GameManager.instance.birds[0].isReleased == false) {
                //Unmovable
                GameManager.instance.birds[0].canMove = false;
            }
        }
        Time.timeScale = 0;
    }
    //resume animation event
    public void ResumeAnimEnd() {
        pauseButton.SetActive(true);
    }
}