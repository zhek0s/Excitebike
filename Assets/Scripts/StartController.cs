using System.Collections;
using UnityEngine;

public class StartController : MonoBehaviour
{
    enum START_STATE
    {
        READY = 0,
        STEADY = 1,
        GO = 2
    }
    
    public GameObject start1;
    public GameObject start2;
    public GameObject start3;

    [SerializeField] private float secondsToStart = 2;
    [SerializeField] private bool isStarted = false;
    [SerializeField] private START_STATE currentState;
    private PlayerController playerController;
    
    private void Awake()
    {
        DisableAll();
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>(); // forgive me god – i have sinned
        if (isStarted)
        {
            ChangeState(START_STATE.GO);
        }
        else
        {
            StartCoroutine(CountdownCoroutine());
        }
    }
    
    private void DisableAll()
    {
        start1.SetActive(false);
        start2.SetActive(false);
        start3.SetActive(false);
    }
    
    private IEnumerator CountdownCoroutine()
    {
        float startTimePartition = secondsToStart / 2;
        ChangeState(START_STATE.READY);
        yield return new WaitForSeconds(startTimePartition);
        ChangeState(START_STATE.STEADY);
        yield return new WaitForSeconds(startTimePartition);
        ChangeState(START_STATE.GO);
    }
    
    private void ChangeState(START_STATE state)
    {
        //This can be redone in 3 SpriteRenderers instead of start1 start2 start3 GOs
        currentState = state;
        switch (state)
        {
            case START_STATE.READY:
                start1.SetActive(true);
                break;
            case START_STATE.STEADY:
                start1.SetActive(false);
                start2.SetActive(true);
                break;
            case START_STATE.GO:
                start2.SetActive(false);
                start3.SetActive(true);
                isStarted = true;
                if (playerController != null) 
                    playerController.canRun = true;
                break;
        }
    }
}
