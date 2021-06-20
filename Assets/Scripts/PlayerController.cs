using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator playerAnimation;
    public float speed = 10f;
    float currentSpeed = 0f;
    float gravity = 20;
    float flyForce = 0f;
    public bool canRun = false;
    public bool turnLeft = false;
    public bool turnRight = false;
    public bool isRunning = false;
    public bool isFall = false;
    public bool isGetUp = false;
    public bool win = false;
    public bool fly = false;
    float[] lines = { 1f, 0.25f, -0.55f, -1.25f };
    float[] linesOffset = { 1.5f, 0.5f, 0f, -0.5f };
    public int currentLine = 2;
    public bool obstacleCol = false;
    float offsetY = 0f;
    bool obstacleEscape = true;
    Obstacle obstacle;
    float obstacleColX;

    float xAx = 0;
    float yAx = 0;
    void Start()
    {
        SetInitionReferences();
        transform.position = (new Vector3(linesOffset[currentLine - 1], lines[currentLine - 1]));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            playerAnimation.SetBool("isUp", true);
            playerAnimation.SetFloat("DirectionUp", 1);
        }
        else
        {
            playerAnimation.SetFloat("DirectionUp", -1);
            playerAnimation.SetBool("isUp", false);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {

        }
        bool s = Input.GetKey(KeyCode.Space);
        if (canRun)
        {
            isRunning = s;
            if (!fly)
            {
                if (s)
                {
                    if (currentSpeed < speed)
                    {
                        currentSpeed += speed * Time.deltaTime;
                        playerAnimation.SetBool("isRunning", true);
                    }
                }
                else
                {
                    if (currentSpeed > 0)
                    {
                        currentSpeed -= speed * Time.deltaTime;
                    }
                    else
                    {
                        currentSpeed = 0;
                        playerAnimation.SetBool("isRunning", false);
                    }
                }
                if (obstacleCol)
                {
                    if (obstacle.direction == 1)
                    {
                        playerAnimation.Play("Up", -1, obstacle.minUpAnim / 6f * Mathf.Min(1, transform.position.x - obstacleColX + 1));
                    }
                    else
                    {
                        playerAnimation.Play("Down", -1, obstacle.minDownAnim / 4f * Mathf.Min(1, transform.position.x - obstacleColX + 1));
                    }
                    if (obstacle.direction == -1 && yAx > 0)
                    {
                        fly = true;
                    }
                    else
                    {
                        yAx = obstacle.endY / obstacle.length * currentSpeed;
                        xAx = obstacle.length / Mathf.Abs(obstacle.endY) * currentSpeed;
                    }
                }
                else
                {
                    xAx = currentSpeed;
                    if (!obstacleEscape)
                    {
                        if (transform.position.y <= lines[currentLine - 1] + obstacle.endY)
                        {
                            transform.position = new Vector2(transform.position.x, lines[currentLine - 1] + obstacle.endY);
                        }
                        else
                        {
                            fly = true;
                        }
                    }
                    else
                    {
                        if (transform.position.y <= lines[currentLine - 1])
                        {
                            transform.position = new Vector2(transform.position.x, lines[currentLine - 1]);
                            yAx = 0;
                        }
                        else
                        {
                            fly = true;
                        }
                    }
                }
            }
            else
            {
                yAx -= gravity * Time.deltaTime;
                if (!obstacleEscape)
                {
                    if (transform.position.y < lines[currentLine - 1] + obstacle.endY)
                    {
                        transform.position = new Vector2(transform.position.x, lines[currentLine - 1] + obstacle.endY);
                        fly = false;
                        yAx = 0;
                    }
                }
                else
                {
                    if (transform.position.y < lines[currentLine - 1])
                    {
                        transform.position = new Vector2(transform.position.x, lines[currentLine - 1]);
                        fly = false;
                        yAx = 0;
                    }
                    else
                    {
                        fly = true;
                    }
                }
            }
            transform.Translate(new Vector3(xAx*Time.deltaTime, yAx*Time.deltaTime, 0));
        }

        //if (canRun)
        // {
        //     isRunning = s;
        //     if (!fly)
        //     {
        //         if (s)
        //         {
        //             if (currentSpeed < speed)
        //             {
        //                 currentSpeed += speed * Time.deltaTime;
        //                 playerAnimation.SetBool("isRunning", true);
        //             }
        //         }
        //         else
        //         {
        //             if (currentSpeed > 0)
        //             {
        //                 currentSpeed -= speed * Time.deltaTime;
        //             }
        //             else
        //             {
        //                 currentSpeed = 0;
        //                 playerAnimation.SetBool("isRunning", false);
        //             }
        //         }
        //     }
        //     transform.position += new Vector3(currentSpeed * Time.deltaTime, 0, 0);
        //     if (obstacleCol)
        //     {
        //         if (!fly)
        //         {
        //             if (obstacle.direction == 1)
        //             {
        //                 playerAnimation.Play("Up", -1, obstacle.minUpAnim / 6f * Mathf.Min(1, transform.position.x - obstacleColX + 1));
        //             }
        //             else
        //             {
        //                 playerAnimation.Play("Down", -1, obstacle.minDownAnim / 4f * Mathf.Min(1, transform.position.x - obstacleColX + 1));
        //             }
        //             float y = offsetY + lines[currentLine - 1] + obstacle.direction * (obstacle.endY) * Mathf.Min((Mathf.Max(transform.position.x - obstacleColX, 0)) / (obstacle.length - 1), 1);
        //             if ((y == lines[currentLine - 1] + obstacle.endY) && currentSpeed * 2 > speed)
        //             {
        //                 fly = true;
        //                 flyForce = (currentSpeed - speed / 2) * obstacle.force;
        //             }
        //             transform.position = new Vector3(transform.position.x, y, transform.position.z);
        //         }
        //     }
        //     else
        //     {
        //         if (!obstacleEscape)
        //         {
        //             offsetY = obstacle.endY;
        //         }
        //     }
        //     if (fly)
        //     {
        //         float y;
        //         if (transform.position.y < offsetY + lines[currentLine - 1])
        //         {
        //             y = offsetY + lines[currentLine - 1];
        //             fly = false;
        //         }
        //         else
        //         {
        //             y = transform.position.y + flyForce/10 * Time.deltaTime;
        //             flyForce = flyForce - speed*15 * Time.deltaTime;
        //         }
        //         transform.position = new Vector3(transform.position.x, y, transform.position.z);
        //     }
        // }



        playerAnimation.SetBool("turnLeft", turnLeft);
        playerAnimation.SetBool("turnRight", turnRight);
        playerAnimation.SetBool("win", win);
    }

    void SetInitionReferences()
    {
        playerAnimation = GetComponent<Animator>();
    }

    public void SetMinUp(int v)
    {

    }

    public void SetMinDown(int v)
    {

    }

    public void Jump(int force)
    {

    }

    public void Fall()
    {
        playerAnimation.SetBool("isFall", true);
    }

    public void GetUp()
    {
        playerAnimation.SetBool("isFall", false);
        playerAnimation.SetBool("isGetUp", false);
        isFall = false;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        obstacleCol = true;
        obstacleColX = transform.position.x + transform.GetComponent<BoxCollider2D>().size.x;
        obstacle = collision.gameObject.GetComponent<Obstacle>();
        if (obstacle.direction == 1)
        {
            obstacleEscape = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (obstacle.direction == -1)
        {
            obstacleEscape = true;
            playerAnimation.SetFloat("DirectionDown", -1);
            playerAnimation.Play("Down");
        }
        else
        {
            playerAnimation.SetFloat("DirectionUp", -1);
            playerAnimation.Play("Up");
        }
        obstacleCol = false;
        //playerAnimation.Play("Run");
    }
}
