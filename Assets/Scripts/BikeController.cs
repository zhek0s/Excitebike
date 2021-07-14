using System;
using UnityEngine;

public class BikeController
{
    private Vector3 position;
    private BikeAnimator animator;
    private Obstacle obstacle;

    private bool keyUp = false;
    private bool keyDown = false;
    private bool keyLeft = false;
    private bool keyRight = false;
    private bool keyRun = false;
    private bool keyRunLock = false;
    private bool keySpeedUp = false;

    private bool fly;
    private bool turnLeft = false;
    private bool turnRight = false;
    private float turnDirection = 0;
    private float turnSpeed = 5f;
    private bool cantTurn = false;
    protected private int LowestLine = 1;
    private int lowestLine = 1;
    protected private int HighestLine = 4;
    private int highestLine = 4;
    private bool cantFly = false;
    private float currentSpeed = 0f;
    private float speed;
    private float speedMulti = 1;
    private float xAx, yAx;
    private float gravity = 10f;

    private int currentLine = 2;
    float[] lines = { 1f, 0.25f, -0.55f, -1.25f };
    float[] linesOffset = { 1.5f, 0.5f, 0f, -0.5f };

    private bool obstacleCol = false;
    private bool obstacleEscape = true;
    private float obstacleColX;
    private float dX, dY;


    public BikeController(Vector3 pos, Animator anim, float s)
    {
        position = pos;
        animator = new BikeAnimator(anim);

        speed = s;

        InitMoto();
    }

    void InitMoto()
    {
        fly = false;
        xAx = yAx = 0f;
        position = (new Vector3(linesOffset[currentLine - 1], lines[currentLine - 1]));
    }

    public void UpdateMotion(float dt, bool canMove, Transform transform)
    {
        //#controll animation up bike
        if (keyUp)
            animator.HandleKey("Up");
        if (keyDown)
            animator.HandleKey("Down");
        if (keyLeft && obstacleEscape)
        {
            animator.SetBool("turnLeft", true);
            if (animator.CanChangeLine())
            {
                turnLeft = true;
                keyRunLock = true;
            }
        }
        else
        {
            turnLeft = false;
            animator.SetBool("turnLeft", false);
            animator.DeactivateAction();
            keyRunLock = false;
        }
        if (keyRight && obstacleEscape)
        {
            animator.SetBool("turnRight", true);
            if (animator.CanChangeLine())
            {
                turnRight = true;
                keyRunLock = true;
            }
        }
        else
        {
            animator.SetBool("turnRight", false);
            animator.DeactivateAction();
            keyRunLock = false;
        }

        //#Movement controll
        if (canMove)
        {
            if (!fly)
            {
                if (keyRun && !keyRunLock && currentSpeed < speed * speedMulti)
                {
                    currentSpeed += speed * speedMulti * dt;
                    animator.SetBool("isRunning", true);
                }
                else
                {
                    if (currentSpeed > 0)
                    {
                        float k = 2;
                        if (currentSpeed > speed * speedMulti)
                        {
                            k = 1 / speedMulti;
                        }
                        currentSpeed -= speed * dt * k;
                    }
                    else
                    {
                        currentSpeed = 0;
                        animator.SetBool("isRunning", false);
                    }
                }
                if (obstacleCol)
                {
                    if (transform.position.x - obstacleColX <= 1)
                    {
                        animator.SetProcess(Mathf.Max(0, Mathf.Min(0.5f, transform.position.x - obstacleColX) * 2));
                    }
                    if (!cantFly && obstacle.direction == -1 && transform.position.y > (Mathf.Lerp(Mathf.Abs(obstacle.endY), 0, ((transform.position.x - obstacleColX) / obstacle.length)) + lines[currentLine - 1]))
                    {
                        fly = true;
                        animator.SetBool("isRunning", false);
                    }
                    else
                    {
                        yAx = obstacle.direction * dY * currentSpeed;
                        xAx = dX * currentSpeed;
                    }
                }
                else
                {
                    xAx = currentSpeed;
                    if (!turnLeft && !turnRight && turnDirection == 0)
                    {
                        if (!obstacleEscape)
                        {
                            if (transform.position.y <= lines[currentLine - 1] + Mathf.Abs(obstacle.endY))
                            {
                                transform.position = new Vector2(transform.position.x, lines[currentLine - 1] + Mathf.Abs(obstacle.endY));
                            }
                            else
                            {
                                fly = true;
                                animator.SetBool("isRunning", false);
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
                                animator.SetBool("isRunning", false);
                            }
                        }
                    }
                    else
                    {
                        if (turnLeft)
                            turnDirection = 1;
                        if (turnRight)
                            turnDirection = -1;
                        if (turnDirection > 0)
                        {
                            if (currentLine == lowestLine)
                            {
                                turnDirection = 0;
                            }
                            else
                            {
                                if (transform.position.y < lines[currentLine - 2])
                                {
                                    transform.Translate(new Vector3(0, turnSpeed * dt, 0));
                                }
                                else
                                {
                                    currentLine -= 1;
                                    turnDirection = 0;
                                }
                            }
                        }
                        else
                        {
                            if (currentLine == highestLine)
                            {
                                turnDirection = 0;
                            }
                            else
                            {
                                if (transform.position.y > lines[currentLine])
                                {
                                    transform.Translate(new Vector3(0, -turnSpeed * dt, 0));
                                }
                                else
                                {
                                    currentLine += 1;
                                    turnDirection = 0;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                yAx -= gravity * dt;
                if (currentSpeed > 0 && xAx > speed * speedMulti / 4)
                {
                    currentSpeed -= speed * dt / 2;
                    xAx -= speed * dt / 2;
                }
                if (!obstacleEscape)
                {
                    if (obstacle.direction == -1)
                    {
                        if (transform.position.y <= (Mathf.Lerp(Mathf.Abs(obstacle.endY), 0, ((transform.position.x - obstacleColX) / obstacle.length)) + lines[currentLine - 1]))
                        {
                            transform.position = new Vector2(transform.position.x, (Mathf.Lerp(Mathf.Abs(obstacle.endY), 0, ((transform.position.x - obstacleColX) / obstacle.length)) + lines[currentLine - 1]));
                            yAx = 0;
                            fly = false;
                            cantFly = true;
                        }
                    }
                    else
                    {
                        if (transform.position.y <= (Mathf.Lerp(0, Mathf.Abs(obstacle.endY), ((transform.position.x - obstacleColX) / obstacle.length)) + lines[currentLine - 1]))
                        {
                            transform.position = new Vector2(transform.position.x, (Mathf.Lerp(0, Mathf.Abs(obstacle.endY), ((transform.position.x - obstacleColX) / obstacle.length)) + lines[currentLine - 1]));
                            yAx = 0;
                            fly = false;
                            cantFly = true;
                        }
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
            //
            int i = 1;
            if (transform.position.y + yAx * dt > 6.5) i = 0;
            //
            transform.Translate(new Vector3(xAx * dt, yAx * dt * i, 0));
        }
        animator.SetFly(fly);
        animator.UpdateAnimation(dt);
        keyUp = keyDown = keyLeft = keyRight = keyRun = keySpeedUp = false;
        turnLeft = turnRight = false;
    }

    public void HandleKey(string key)
    {
        switch (key)
        {
            case "Up":
                keyUp = true;
                break;
            case "Down":
                keyDown = true;
                break;
            case "Left":
                keyLeft = true;
                break;
            case "Right":
                keyRight = true;
                break;
            case "Run":
                keyRun = true;
                break;
            case "SpeedUp":
                keySpeedUp = true;
                break;
            default:
                Debug.LogError("Bike controller: Key " + key + " is not usable!");
                break;
        }
    }

    public void CollideObstacle(float colX, Obstacle obs)
    {
        obstacle = obs;
        dX = dY = 0;
        if (obs.interactLines[currentLine - 1])
        {
            currentLine += Convert.ToInt32(turnDirection);
            currentLine = Mathf.Max(LowestLine, Mathf.Min(HighestLine, currentLine));
            turnDirection = 0;

            obstacleCol = true;
            obstacleColX = colX;
            speedMulti = obstacle.force / 10;
            if (obstacle.direction == 1)
            {
                obstacleEscape = false;
                cantFly = false;
                //animator.HandleAction(BikeAnimator.action.wheelUp, 1);
                animator.SetMinUpFrame(obstacle.minUpAnim);
            }
            else
            {
                //animator.HandleAction(BikeAnimator.action.wheelDown, 1);
                animator.SetMinDownFrame(obstacle.minDownAnim);
            }
            float q = Mathf.Atan2(Mathf.Abs(obstacle.endY), obstacle.length);
            dX = Mathf.Cos(q);
            dY = Mathf.Sin(q);
        }
        else
        {
            if (currentLine != LowestLine)
            {
                if (obs.interactLines[currentLine - 1])
                {
                    lowestLine = currentLine - 1;
                }
                else
                {
                    lowestLine = currentLine;
                }
            }
            if (currentLine != HighestLine)
            {
                if (obs.interactLines[currentLine + 1])
                {
                    highestLine = currentLine + 1;
                }
                else
                {
                    highestLine = currentLine;
                }
            }
        }
    }

    public void EscapeCollide(Obstacle obs)
    {
        if (obs.direction == obstacle.direction)
        {
            if (obstacle.endRamp)
            {
                lowestLine = LowestLine;
                highestLine = HighestLine;
                obstacleEscape = true;
                cantFly = false;
            }
            obstacleCol = false;
            speedMulti = 1;
        }
        if (obs.direction == -1)
        {
            animator.SetMinDownFrame(0);
        }
        else
        {
            animator.SetMinUpFrame(0);
        }
    }
}
