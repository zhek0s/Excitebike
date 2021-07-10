using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BikeController bike;
    private Animator playerAnimation;
    public float speed = 5f;
    public bool canRun = false;
    public bool turnLeft = false;
    public bool turnRight = false;
    public bool isRunning = false;
    public bool isFall = false;
    public bool isGetUp = false;
    public bool win = false;

    void Start()
    {
        SetInitionReferences();
    }
    void SetInitionReferences()
    {
        playerAnimation = GetComponent<Animator>();
        bike = new BikeController(transform.position, playerAnimation, speed);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            bike.HandleKey("Up");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            bike.HandleKey("Down");
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            bike.HandleKey("Run");
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            bike.HandleKey("Left");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            bike.HandleKey("Right");
        }
        bike.UpdateMotion(Time.deltaTime, canRun, transform);
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
        float colX = transform.position.x;// + transform.GetComponent<BoxCollider2D>().size.x;
        Obstacle obs = collision.gameObject.GetComponent<Obstacle>();
        bike.CollideObstacle(colX, obs);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        Obstacle obs = collision.gameObject.GetComponent<Obstacle>();
        bike.EscapeCollide(obs);
    }
}
