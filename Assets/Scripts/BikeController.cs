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
	private bool keySpeedUp = false;

	private bool fly;
	private bool cantFly = false;
	private float currentSpeed = 0f;
	private float speed;
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
		{
			animator.HandleKey("Up");
		}
		if (keyDown)
		{
			animator.HandleKey("Down");
		}

		//#Movement controll
		if (canMove)
		{
			if (!fly)
			{
				if (keyRun)
				{
					if (currentSpeed < speed)
					{
						currentSpeed += speed * dt;
						animator.SetBool("isRunning", true);
					}
				}
				else
				{
					if (currentSpeed > 0)
					{
						currentSpeed -= speed * dt;
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
						animator.SetProcess(Mathf.Max(0, Mathf.Min(0.5f, transform.position.x - obstacleColX)*2));
					}
                    if (!cantFly && obstacle.direction == -1 && transform.position.y > (Mathf.Lerp(Mathf.Abs(obstacle.endY), 0, ((transform.position.x - obstacleColX) / obstacle.length)) + lines[currentLine - 1]))
                    {
                        fly = true;
						animator.SetBool("isRunning", false);
					}
                    else
					{
						yAx = obstacle.direction * dY * currentSpeed * (obstacle.force / 10);
						xAx = dX * currentSpeed * (obstacle.force / 10);
					}
				}
                else
				{
					xAx = currentSpeed;
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
			}
			else
			{
				yAx -= gravity * dt;
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
			transform.Translate(new Vector3(xAx * dt, yAx * dt, 0));
		}
		animator.SetFly(fly);
		animator.UpdateAnimation(dt);
		keyUp = keyDown = keyLeft = keyRight = keyRun = keySpeedUp = false;
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
				Debug.LogError("Bike controller: Key "+key+" is not usable!");
				break;
		}
	}

	public void CollideObstacle(float colX, Obstacle obs)
	{
		dX = dY = 0;
		obstacleCol = true;
		obstacleColX = colX;
		obstacle = obs;
		if (obstacle.direction == 1)
		{
			obstacleEscape = false;
			cantFly = false;
			animator.HandleAction(BikeAnimator.action.wheelUp, 1);
			animator.SetMinUpFrame(obstacle.minUpAnim);
        }
        else
        {
			animator.HandleAction(BikeAnimator.action.wheelDown, 1);
			animator.SetMinDownFrame(obstacle.minDownAnim);
        }
		float q = Mathf.Atan2(Mathf.Abs(obstacle.endY), obstacle.length);
		dX = Mathf.Cos(q);
		dY = Mathf.Sin(q);

	}

	public void EscapeCollide(Obstacle obs)
	{
		if (obs.direction == obstacle.direction)
		{
			if (obstacle.direction == -1)
			{
				obstacleEscape = true;
				cantFly = false;
				animator.SetMinDownFrame(0);
			}
			else
			{
				animator.SetMinUpFrame(0);
			}
			obstacleCol = false;
		}
	}
}
