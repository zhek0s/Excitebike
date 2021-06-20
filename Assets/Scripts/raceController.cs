using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raceController : MonoBehaviour
{
    public bool start = false;
    public bool win = false;
    public int toStart = 3;
    public GameObject startObj;
    public GameObject player;
    private startController startScr;
    private PlayerController playerController;
    private float time=0;
    void Start()
    {
        startScr = startObj.GetComponent<startController>();
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (time+1 < Time.time)
        {
            toStart -= 1;
            startScr.ChangeStart(toStart);
            time = Time.time;
        }
        if (toStart <= 0)
        {
            start = true;
            playerController.canRun = true;
        }
    }
}
