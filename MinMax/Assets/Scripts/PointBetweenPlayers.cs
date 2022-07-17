using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointBetweenPlayers : MonoBehaviour
{
    Vector3 pointToFollow;
    public float furthestDistanceBetweenPlayer;
    public float[] numsToChooseFrom;

    public static PointBetweenPlayers singleton;

    private void Awake()
    {
        singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.g.Players.Count == 1)
        {
            pointToFollow = GameManager.g.Players[0].transform.position;
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(pointToFollow.x, pointToFollow.y + 25, pointToFollow.z - 15), 50 * Time.deltaTime);
            return;
        }
        foreach (GameObject player in GameManager.g.Players)
        {

            pointToFollow += player.transform.position;
            if (GameManager.g.Players.Count - 1 != numsToChooseFrom.Length)
            {
                numsToChooseFrom = new float[GameManager.g.Players.Count - 1];
            }
        }

        for (int i = 0; i < GameManager.g.Players.Count - 1; i++)
        {
            float distanceBetweenPlayer = Vector3.Distance(GameManager.g.Players[i].transform.position, GameManager.g.Players[i + 1].transform.position);
            numsToChooseFrom[i] = distanceBetweenPlayer;
            furthestDistanceBetweenPlayer = distanceBetweenPlayer;
            for (int j = 0; j < numsToChooseFrom.Length; j++)
            {
                if (numsToChooseFrom[j] > furthestDistanceBetweenPlayer)
                {
                    furthestDistanceBetweenPlayer = numsToChooseFrom[j];
                }
            }

        }


        if (GameManager.g.Players.Count > 1)
        {
            pointToFollow = pointToFollow / (GameManager.g.Players.Count + 1);

        }


        this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(pointToFollow.x, pointToFollow.y + 25, pointToFollow.z - 15), 50 * Time.deltaTime);
    }
}
