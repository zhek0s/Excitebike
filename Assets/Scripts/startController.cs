using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartController : MonoBehaviour
{
    public GameObject start1;
    public GameObject start2;
    public GameObject start3;
    void Start()
    {
        start1.SetActive(true);
        start2.SetActive(false);
        start3.SetActive(false);
    }
    public void ChangeStart(int state)
    {
        switch (state)
        {
            case 1:
                start1.SetActive(false);
                start2.SetActive(true);
                break;
            case 0:
                start2.SetActive(false);
                start3.SetActive(true);
                break;
        }
    }
}
