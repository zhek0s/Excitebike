using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeAnimator
{
	private Animator animator;
	public enum action
	{
		wheelUp = 0,
		wheelDown = 1,
		speedUp = 2,
		turnLeft = 3,
		turnRight = 4,
		none = 5
	}
	//private action currentAction;
	private action needAction = action.none;
	private float process = 0f;
	//private float currentProcess = 0f;
	//private int currentDirection;
	//private int needDirection;
	private float minUp = 0f;
	private float minDown = 0f;
	private float maxUp = 7f;
	private float maxDown = 5f;

	private bool fly = false;
	private bool running = true;
	private float wheelSpeed = 1;
	private float wheelAccelerate = 0;
	private float wheelVal = 0;

	public BikeAnimator(Animator anim)
	{
		animator = anim;
	}

	public void HandleAction(action a, int direction)
	{
		needAction = a;
		//needDirection = direction;
	}

	public void DeactivateAction()
	{
		needAction = action.none;
	}

	public void HandleKey(string key)
	{
		switch (key)
		{
			case "Up":
				running = false;
				wheelAccelerate = 1;
				break;
			case "Down":
				if (!running)
					wheelAccelerate = -1;
				break;
		}
	}

	public void SetProcess(float p)
	{
		process = p;
	}

	public void SetFly(bool f)
	{
		fly = f;
	}

	public void SetMinUpFrame(float f)
	{
		minUp = f;
		if (wheelVal < 0)
		{
			Debug.Log("You dumb!!!");
		}
	}

	public void SetMinDownFrame(float f)
	{
		minDown = f;
		if (wheelVal > 0)
		{
			Debug.Log("You dumb!!!");
		}
	}

	public void UpdateAnimation(float dt)
	{
		if (needAction != action.none)
		{
			running = false;
			switch (needAction)
			{
				case action.turnLeft:
					animator.Play("Left", -1, 1);
					break;
				case action.turnRight:
					animator.Play("Right", -1, 1);
					break;
			}
		}
		else
		{
			if (fly)
			{
				running = false;
				wheelVal += wheelAccelerate * wheelSpeed * dt;
				wheelVal = Mathf.Max(Mathf.Min(wheelVal, 1), -1);
				if (wheelVal > 0)
				{
					animator.Play("Up", -1, wheelVal);
				}
				else
				{
					animator.Play("Down", -1, Mathf.Abs(wheelVal));
				}
			}
			else
			{
				if (minUp == 0f && minDown == 0f)
				{
					if (running)
					{
						wheelVal = 0;
						//animator.Play("Run", -1, 1);
					}
					else
					{
						if (wheelAccelerate == 0)
						{
							if (Mathf.Abs(wheelVal) < 0.1)
							{
								running = true;
							}
							else
							{
								wheelAccelerate = -1 * wheelSpeed / 2 * (Mathf.Abs(wheelVal) / wheelVal);
							}
						}
						wheelVal += wheelAccelerate * wheelSpeed * dt;
						wheelVal = Mathf.Max(Mathf.Min(wheelVal, 1), -1);
						if (wheelVal >= 0)
						{
							animator.Play("Up", -1, wheelVal);
						}
						else
						{
							Debug.Log("You dumb!!!");
							wheelVal = 0;
							running = true;
							//animator.Play("Down", -1, Mathf.Abs(wheelVal));
						}
					}
				}
				else
				{
					if (minUp > 0)
					{
						wheelVal = minUp / maxUp * process;
						animator.Play("Up", -1, wheelVal);
					}
					else if (minDown > 0)
					{
						wheelVal = -minDown / maxDown * process;
						animator.Play("Down", -1, -wheelVal);
					}
				}
			}
		}
		wheelAccelerate = 0;
	}

	public bool CanChangeLine()
	{
		return running;
	}

	public void SetBool(string key, bool value)
	{
		animator.SetBool(key, value);
	}

	public void SetFloat(string key, float value)
	{
		animator.SetFloat(key, value);
	}
}
