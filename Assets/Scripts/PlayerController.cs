using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks
{

    public Vector3 bounceForce = Vector3.zero;
    public Rigidbody rb;
    public float maxVel = 5f;
    public float maxAcc = 20f;

    public int deaths = 0;
    public TextMeshProUGUI deathCounter;

    public int id;

    Camera cam;
    Collider floorCollider;
    RaycastHit hit;
    Ray ray;

    PhotonView view;

    public GameObject gameManagerObject;

    public GameObject createJoinRoom;
    CreateAndJoinRooms createJoinScript;

    public Vector3 currentLocation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        floorCollider = GameObject.Find("InfiniteFloor").GetComponent<Collider>();

        view = GetComponent<PhotonView>();

        //createJoinScript = createJoinRoom.GetComponent<CreateAndJoinRooms>();
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine) {
            deathCounter.text = "DEATHS - " + deaths;
            // gravity
            rb.AddForce(new Vector3(0, -0.5f, 0));
            // Time to resolve falling through floor by considering two cases: player is outside arena in x, z vs inside
            Vector3 loc = new Vector3(rb.position.x, 0, rb.position.z);
            if (loc.magnitude <= 10 && rb.position.y < 0)
                rb.position = loc;
            //caps speed at max velocity
            if (rb.velocity.magnitude > maxVel) rb.velocity = rb.velocity.normalized * maxVel;
            //Camera stuff
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider == floorCollider) {
                    if (loc.magnitude <= 10 && rb.position.y <= 0) {
                        Vector3 moveVector = hit.point - transform.position;
                        moveVector = new Vector3(moveVector.x, 0, moveVector.z);
                        //cap acceleration
                        if (moveVector.magnitude > maxAcc) moveVector = moveVector.normalized * maxAcc;
                        rb.AddForce(moveVector);
                        // should also have maximum for acceleration
                    }
                }
            }
            currentLocation = transform.position;
        }
        
    }

    //the first time it collides with something
    void OnTriggerEnter(Collider other) {
        // }
        //if it's colliding with something that isn't the floor
        if (other.gameObject.tag != "Floor" && other.gameObject.tag != "InfiniteFloor" && view.IsMine) {
            rb.velocity = Vector3.zero;
            // way that ball bounces will be based on current pos of enemy and self
            Vector3 gappy = rb.position - other.transform.position;
            bounceForce = gappy.normalized*200;
            bounceForce = new Vector3(bounceForce.x, 50, bounceForce.z);
            rb.AddForce(bounceForce);
        }
    }

    //while it collides with something
    void OnTriggerStay(Collider other) {
        // Collision with other player
        if (other.gameObject.tag != "Floor" && other.gameObject.tag != "InfiniteFloor" && view.IsMine) {
            // way that ball bounces will be based on current pos of enemy and self
            Vector3 gappy = rb.position - other.transform.position;
            bounceForce = gappy.normalized*200;
            bounceForce = new Vector3(bounceForce.x, 50, bounceForce.z);
            rb.AddForce(bounceForce);
        }
    }

    public void OnClickLeaveRoom() {
        GameObject gameManagerObject;
        gameManagerObject = GameObject.Find("GameManager");

        GameManager script = gameManagerObject.GetComponent<GameManager>();

        float minDist = 999999f;
        int currentID = 0;

        foreach (GameObject p in script.players) {
            Vector3 diff = currentLocation - p.transform.position;
            if (diff.magnitude < minDist) {
                minDist = diff.magnitude;
                currentID = p.GetComponent<PlayerController>().id;
            }
        }
        
        script.players.Remove(script.players[currentID]);  
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Loading");
    }
}
