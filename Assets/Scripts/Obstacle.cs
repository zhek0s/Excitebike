using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float endY;
    public float length;
    public float direction;
    public float force=1;
    public float minUpAnim;
    public float minDownAnim;
    public bool endRamp = false;
    public bool canFall = false;
    public bool[] interactLines = {true, true, true, true};
}
