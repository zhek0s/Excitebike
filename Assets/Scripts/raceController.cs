using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceController : MonoBehaviour
{
    public bool start = false;
    public bool win = false;
    public int toStart = 3;
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] PlayerController playerController;
    private StartController startScr;
    private float time = 0;
    
    void Start()
    {
        startScr = mapGenerator.StartControllerGO.GetComponent<StartController>();
    }

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
