using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour {
    //If the bird's speed is greater than 10, the green-skin pig will die, and if the speed is greater than 5 and less than 10, the pig will be injured.
    public float maxSpeed = 10;
    public float minSpeed = 5;

    public SpriteRenderer sr;
    public Sprite hurtSprite;

    public bool isHurt;
    public GameObject boom;
    public GameObject score;

    public bool isPig=false;

    public AudioClip dead;
    public AudioClip hurt;
    public AudioClip birdCollision;
    // Start is called before the first frame update
    void Start() {
        sr = GetComponent<SpriteRenderer>();
        isHurt = false;
    }

    // Update is called once per frame
    void Update() {

    }
    //Impact checking
    private void OnCollisionEnter2D(Collision2D collision) {
        //collision.relativeVelocity represents the relative speed (vector), and magnitude represents the modulus length of the vector
        //Death
        if (collision.relativeVelocity.magnitude >= maxSpeed) {
            Dead();
        }
        //Injuried
        else if (collision.relativeVelocity.magnitude > minSpeed && collision.relativeVelocity.magnitude < maxSpeed) {
            sr.sprite = hurtSprite;
            isHurt = true;
            AudioPlay(hurt);
        }

        if (collision.transform.tag=="Player") {
            AudioPlay(birdCollision);
        }

        if(collision.transform.tag == "Space")
        {
            Dead();
            AudioPlay(birdCollision);
        }

    }
    //Operations after the death of a green pig
    public void Dead() {
        if (isPig) {
            GameManager.instance.pigs.Remove(this);
            AudioPlay(dead);
        }
        Instantiate(boom,transform.position,Quaternion.identity);
        GameObject s=Instantiate(score, transform.position+new Vector3(0,0.8f,0), Quaternion.identity);
        Destroy(s,1f);
        Destroy(this.gameObject);
    }
    public void AudioPlay(AudioClip ac) {
        AudioSource.PlayClipAtPoint(ac, transform.position);
    }
}
