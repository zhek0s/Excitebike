using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    public bool start = false;
    public bool win = false;
    public int toStart = 3;
    public GameObject startController;
    public GameObject player;
    private StartController startScr;
    private PlayerController playerController;
    private float time=0;
    void Start()
    {
        startScr = startController.GetComponent<StartController>();
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!start)
        {
            if (time + 1 < Time.time)
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
}
