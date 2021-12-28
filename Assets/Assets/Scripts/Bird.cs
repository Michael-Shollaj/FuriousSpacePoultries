using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bird : MonoBehaviour {
    public Vector3 currPos;
    public bool isClick;
    public Vector3 launchPos;  //Slingshot launch position
    public float maxDis;   //Maximum distance of rubber band
    public SpriteRenderer sr;
    public SpringJoint2D sj2d;
    public Rigidbody2D r2d;

    public LineRenderer lrRight;
    public LineRenderer lrLeft;
    public Transform rightPos;//Right bracket
    public Transform leftPos;//Left bracket
    public GameObject boom;//Explosion effect
    public bool isFly;
    public bool canMove = true;

    public WeaponTrail trail;
    public float smooth = 3;

    public AudioClip select;
    public AudioClip fly;
    public Sprite yellowSpeedUp;
    public bool launch = false;

    public Sprite redHurt;
    public Sprite yellowHurt;
    public Sprite greenHurt;
    public Sprite blackHurt;
    public bool onGround = true;
    public enum BirdType {
        Red, Yellow, Black, Green
    }
    public BirdType bt;
    public bool isReleased ;
    
    public void Awake() {
        isReleased = false;
        onGround = true;
        sr = GetComponent<SpriteRenderer>();
        trail = GetComponent<Trails>().trail;
        sj2d = GetComponent<SpringJoint2D>();
        r2d = GetComponent<Rigidbody2D>();
        sj2d.connectedBody = GameObject.Find("Right").GetComponent<Rigidbody2D>();
        lrRight = GameObject.Find("Right").GetComponent<LineRenderer>();
        lrLeft = GameObject.Find("Left").GetComponent<LineRenderer>();
        rightPos = GameObject.Find("RightPos").transform;
        leftPos = GameObject.Find("LeftPos").transform;

    }
    // Start is called before the first frame update
    public void Start() {
        isFly = false;
        isClick = false;
        launchPos = GameObject.Find("LaunchPos").transform.position;
        //No tailing by default
        trail.SetTime(0.0f, 0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update() {
        //Whether the UI is clicked
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        currPos = transform.position;
        if (isClick) {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);//Convert mouse coordinates to screen space coordinate system


            //1.Limit the z-axis of the coordinates to 0
            //transform.position = new Vector3(transform.position.x,transform.position.y,0);
            //2.Coordinate minus the camera's z coordinate
            transform.position -= new Vector3(0, 0, Camera.main.transform.position.z);

            //Location limited
            //If the object's position is greater than maxDos from the launch position
            if (Vector3.Distance(transform.position, launchPos) > maxDis) {
                //The direction vector of the emission position pointing to the object position (normalized)
                Vector3 dir = (transform.position - launchPos).normalized;
                dir *= maxDis;//The direction is multiplied by the distance to get the length vector
                transform.position = dir + launchPos;//The direction is multiplied by the distance to get the length vector
            }
            //Draw rubber band
            DrawLine();
        }
        //Camera follow
        float posX = transform.position.x;//Bird position
        //Debug.Log(transform.name+" "+posX);
        //Target position, x range is limited between 0-15


        Vector3 tarPos = new Vector3(Mathf.Clamp(posX, 0, 15), Camera.main.transform.position.y, Camera.main.transform.position.z);
        //Smooth position
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, tarPos, Time.deltaTime * smooth);

        //Bird skills
        if (isFly && Input.GetMouseButtonDown(0)) {
            if (bt == BirdType.Yellow) {
                DirectionalSpeedUpSkill();
            }
            if (bt == BirdType.Green) {
                BoomerangSkill();
            }
            if (bt == BirdType.Black) {
                BoomSkill();
            }
        }
    }
    //Black bird's explosive skills
    public virtual void BoomSkill() {

    }
    //Green bird's maneuver skills
    public void BoomerangSkill() {
        isFly = false;
        GameObject bo = Instantiate(boom, transform.position, Quaternion.identity);
        bo.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        r2d.velocity = new Vector2(-r2d.velocity.x * 1.5f, r2d.velocity.y * 0.5f);
    }

    //Yellow bird’s directional acceleration skills
    public void DirectionalSpeedUpSkill() {
        isFly = false;
        GameObject bo = Instantiate(boom, transform.position, Quaternion.identity);
        bo.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        sr.sprite = yellowSpeedUp;
        r2d.velocity *= 2.2f;
    }
    //Mouse down
    public void OnMouseDown() {
        if (Input.GetMouseButtonDown(0) && onGround == false) {
            AudioPlay(@select);
            if (canMove) {
                isClick = true;
                //Accept physical influence
                r2d.isKinematic = true;
            }
        }
    }
    //Mouse up
    public void OnMouseUp() {

        if (Input.GetMouseButtonUp(0) && onGround == false) {
            launch = true;
            if (canMove) {
                isClick = false;
                //Not accept physical influence
                r2d.isKinematic = false;
                //Delay the call and wait for the physical calculation to complete before invalidating the spring joint
                Invoke("Fly", 0.1f);

                //Disable drawing rubber bands
                lrRight.enabled = false;
                lrLeft.enabled = false;
                canMove = false;

                //Debug.Log(currTime);
            }
        }



    }

    public void Fly() {
        isReleased = true;
        AudioPlay(fly);

        isFly = true;
        //Set the trailing duration
        trail.SetTime(0.2f, 0.0f, 1.0f);
        //Start tailing
        trail.StartTrail(0.5f, 0.4f);

        //springJoint dissapear
        sj2d.enabled = false;

    }
    //RigidBody's Angular Drag value represents rotation attenuation, resistance (air resistance)

    //Draw a rubber band
    public void DrawLine() {

        //Activate drawing rubber band
        lrRight.enabled = true;
        lrLeft.enabled = true;
        //draw
        lrRight.SetPositions(new[] { rightPos.position, transform.position });
        lrLeft.SetPositions(new[] { leftPos.position, transform.position });
        //lrRight.SetPosition(0,rightPos.position);
        //lrRight.SetPosition(1,transform.position);
        //lrLeft.SetPosition(0,leftPos.position);
        //lrLeft.SetPosition(1,transform.position);
    }

    //Destroy itself
    public void DestroyMyself() {
        GameManager.instance.birds.Remove(this);
        Instantiate(boom, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        
        GameManager.instance.NextBird();
    }
    //When the bird hits an object, it cancels the trailing
    public void OnCollisionEnter2D(Collision2D collision) {
        trail.ClearTrail();
        if (collision.transform.tag == "Enemy" || collision.transform.tag == "Ground" && launch) {
            isFly = false;
            switch (bt) {
                case BirdType.Red: sr.sprite = redHurt; break;
                case BirdType.Yellow: sr.sprite = yellowHurt; break;
                case BirdType.Green: sr.sprite = greenHurt; break;
                case BirdType.Black: sr.sprite = blackHurt; break;
            }
            Invoke("DestroyMyself", 3);
        }
    }
    public void AudioPlay(AudioClip ac) {
        AudioSource.PlayClipAtPoint(ac, transform.position);
    }

}
