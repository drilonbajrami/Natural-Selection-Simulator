﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
	private GameObject prey;
	private bool chasing;

	private float maxChaseTime;
	private float currentChaseTime;

	public ChaseState() : base()
	{
		_stateName = "Chase State";
		prey = null;
		chasing = false;
		maxChaseTime = 10.0f;
		currentChaseTime = maxChaseTime;
	}

	public override void HandleState(Entity entity)
	{
		if (!chasing)
		{
			entity.SetDestination(entity.GetIdealRandomDestination(true));
			prey = entity.Smell.ChoosePrey();

			if (prey != null)
			{
				chasing = true;
				entity.Run();
			}
			else
				entity.Walk();
		}
		else
		{
			ChasePrey(entity);
		}
	}

	private void ChasePrey(Entity entity)
	{
		if (chasing)
		{
			currentChaseTime -= Time.deltaTime;
			if (currentChaseTime <= 0.0f)
			{
				prey = null;
				chasing = false;
				currentChaseTime = maxChaseTime;
			}

			if (prey != null)
			{
				entity.SetDestination(prey.transform.position);

				if (entity.CheckIfClose(prey, 0.75f))
				{
					entity.Smell.RemoveFromSmellZone(prey.gameObject.GetInstanceID());
					prey.gameObject.GetComponent<Entity>().Die();
					prey = null;
					chasing = false;
					entity.Vitals.RecoverEnergy(50.0f);
					entity.Fitness += 0.01f;
					entity.ChangeState(new PrimaryState());
				}
			}
		}
		else
		{
			prey = null;
			chasing = false;	
			currentChaseTime = maxChaseTime;
		}
	}
}
