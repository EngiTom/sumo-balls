using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public ArrayList players = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        foreach (GameObject p in players) {
            if (p.transform.position.y < -10) {
                p.transform.position = new Vector3(Random.Range(-5, 5), 0.5f, Random.Range(-5, 5));
                PlayerController script = p.GetComponent<PlayerController>();
                script.deaths++;
            }
        }
    }
}
