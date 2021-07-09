using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator playerAnimation;
    private Obstacle obstacle;
    public float speed = 10f;
    float currentSpeed = 0f;
    float gravity = 20;
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
    bool obstacleEscape = true;
    public float obstacleColX;

    float xAx = 0;
    float yAx = 0;
    void Start()
    {
        SetInitionReferences();
        transform.position = (new Vector3(linesOffset[currentLine - 1], lines[currentLine - 1]));
    }
    void SetInitionReferences()
    {
        playerAnimation = GetComponent<Animator>();
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
                    if (obstacle.direction == -1 && yAx > (-1*Mathf.Lerp(obstacle.endY,0,((transform.position.x-obstacleColX)/obstacle.length))+lines[currentLine-1]))
                    {
                        fly = true;
                    }
                    else
                    {
                        float k = 1f;
                        if (Mathf.Abs(obstacle.endY / obstacle.length) != 1)
                        {
                            if (obstacle.endY > obstacle.length)
                            {
                                k = obstacle.length / obstacle.endY;
                            }
                            else
                            {
                                k = obstacle.endY / obstacle.length;
                            }
                        }
                        k = Mathf.Abs(k);
                        yAx = k * obstacle.endY * currentSpeed * (obstacle.force/10);
                        xAx = k * obstacle.length * currentSpeed * (obstacle.force/10);
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
            Debug.Log(new Vector3(xAx*Time.deltaTime, yAx*Time.deltaTime, 0));
            transform.Translate(new Vector3(xAx*Time.deltaTime, yAx*Time.deltaTime, 0));
        }

        playerAnimation.SetBool("turnLeft", turnLeft);
        playerAnimation.SetBool("turnRight", turnRight);
        playerAnimation.SetBool("win", win);
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
    void OnTriggerExit2D(Collider2D collision)
    {
        if (obstacle.direction == -1)
        {
            obstacleEscape = true;
            if (!fly)
            {
                playerAnimation.SetFloat("DirectionDown", -1);
                playerAnimation.Play("Down");
            }
        }
        else
        {
            //playerAnimation.SetBool("isUp",true);
            playerAnimation.SetFloat("DirectionUp", -1);
            playerAnimation.Play("Up");
        }
        obstacleCol = false;
    }
}
