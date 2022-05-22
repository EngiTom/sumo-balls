using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SpawnPlayers : MonoBehaviour
{

    public GameObject playerPrefab;
    
    public float minX, maxX, minZ, maxZ;

    public ArrayList players;
    GameManager gameManagerScript;
    public GameObject gameManagerObject;

    int idCounter = 0;

    public TextMeshProUGUI deathCounter;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = gameManagerObject.GetComponent<GameManager>();

        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 0.5f, Random.Range(minZ, maxZ));
        //spawns player prefab at position with no rotation
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        player.GetComponent<PlayerController>().id = idCounter;
        gameManagerScript.players.Add(player);

        PlayerController playerControllerScript = player.GetComponent<PlayerController>();
        playerControllerScript.deathCounter = deathCounter;

        idCounter++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
