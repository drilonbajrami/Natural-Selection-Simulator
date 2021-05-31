﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrimaryState : State
{
	private enum SubState { ROAMING, IDLE }
	private SubState _currentSubState = SubState.ROAMING;

	private GameObject prey;
	//private float hungerTimer = 30.0f;
	//private float timer;
	private bool chasing = false;
	//private bool hungry = false;

	public PrimaryState() : base()
	{
		//_entity.isOnReproducingState = false;
		//_entity.fleeing = false;
		_stateName = "Primary State";
		//_entity.isMating = false;
		//timer = hungerTimer;
	}

	public override void HandleState(Entity entity)
	{
		switch (_currentSubState)
		{
			case SubState.ROAMING:
				RoamingSubState(entity);
				break;
			case SubState.IDLE:
				IdleSubState(entity);
				break;
			default:
				break;
		}
	}

	private void RoamingSubState(Entity entity)
	{
		if (entity.IsHungry())
		{
			if (entity.order == Order.CARNIVORE)
			{
				entity.ChangeState(new ChaseState());
			}
			else
			{
				entity.ChangeState(new HungryThirstyState());
			}
		}

		if ((entity.IsStuck() || !entity.HasPath()))
		{
			entity.Sight.AssessUtilities();
			float difference = Mathf.Abs(entity.Sight.SightArea.HighestUtilityValue - entity.Sight.SightArea.LowestUtilityValue) + 1.5f;
			entity.IncreaseMaxSpeed(difference);
			bool doNotSocialize = false;
			if(entity.Sight.SightArea.LowestUtilityValue >= 0)
				doNotSocialize = Random.Range(0.0f, 100.0f) >= entity.genome.Behavior.IdealAllele.SocializingChance;
			entity.SetDestination(entity.GetIdealRandomDestination(doNotSocialize));
		}

		//if (entity.Velocity() > 0.01f)
		//{
		//	//entity.thirstiness += Time.deltaTime * entity.Velocity() / 5;
		//	entity.hungriness += Time.deltaTime * entity.Velocity() / 10;
		//}
	}

	private void IdleSubState(Entity entity)
	{
		entity.thirstiness += Time.deltaTime * 5 / 20;
		entity.hungriness += Time.deltaTime * 5 / 20;

		if (entity.thirstiness > 30.0f || entity.hungriness > 30.0f)
			_currentSubState = SubState.ROAMING;
	}

	private bool ResourcesAreSufficient(Entity entity, float threshold)
	{
		return entity.thirstiness < threshold && entity.hungriness < threshold;
	}

	private bool IsHungryOrThirsty(Entity entity, float threshold)
	{
		return entity.thirstiness > threshold || entity.hungriness > threshold;
	}
}