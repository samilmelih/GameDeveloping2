using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character 
{
	public float health = 100f;
    public float mana = 100;
	public int money;			// This should be in additional parameters

    /// <summary>
    /// If we have level system then we need to increase some variables
    /// depend on this
    /// </summary>
    int CurrentLevel;

	public Direction direction;

    //bu şimdilik public buna daha iyi çözümler üretebiliriz
    public bool isAlive=true;
	// how fast my character moves right to left 2 fps

	// FIXME: bunun public olma konusunda düşün
	public Weapon currentWeapon;

	public string Type{ get; set; }

	// This is just speed, no direction. We use this to get velocity
	public Vector2 speed;

	// For updating velocity of our character
	public Vector2 velocity;

	// For updating scale of our character
	public Vector3 scale;
    

	// old callbacks
	Action<Character> cbOnAttack;
	Action<Character> cbOnJump;
	Action<Character> cbOnCrouch;
	Action<Character> cbOnWalk;


	// new callbacks
	Action<Character> cbOnDestroyed;
	Func<Character, bool> canCharacterJump;

	World world;

	public Character()
	{
		world = WorldController.Instance.world;
	}

	public void Update()
	{
		if (health <= 0)
		{
			if(cbOnDestroyed != null)
				cbOnDestroyed(this);
			
			if(this.Type == "Main Character")
			{
				isAlive = false;
			}
			else if(this.Type == "Enemy")
			{
				world.enemies.Remove(this);
			}
		}
	}

	public void PhysicUpdates()
	{
		if(this.Type == "Enemy")
			this.Walk(this.direction);
	}




	public void Attack()
	{
		if(cbOnAttack != null)
			cbOnAttack(this);
	}

	public void Walk(float axis)
	{
		velocity.x = speed.x * axis;

		if (axis < 0)
		{
			scale = new Vector2(-1f, 1f);
			direction = Direction.Left;
		}
		else if (axis > 0)
		{
			scale = new Vector2(1f, 1f);
			direction = Direction.Right;
		}

		if(cbOnWalk != null)
			cbOnWalk(this);
	}

	// Cause of enemy characters are managed by CharacterAI, we don't have
	// an axis info. We need to translate direction info to axis info.
	public void Walk(Direction direction)
	{
		if(direction == Direction.Left)
			Walk(-1f);
		else
			Walk(1f);
	}

	public void Jump()
	{
		if(canCharacterJump != null && canCharacterJump(this) == true)
		{
			velocity.y = speed.y;
			if(cbOnJump != null)
				cbOnJump(this);
		}
	}

	public void Crouch()
	{
		
	}

	public void RegisterOnAttackCallback(Action<Character> cb)
	{
		cbOnAttack += cb;
	}

	public void RegisterOnJumpCallback(Action<Character> cb)
	{
		cbOnJump += cb;
	}

	public void RegisterOnCrouchCallback(Action<Character> cb)
	{
		cbOnCrouch += cb;
	}

	public void RegisterOnWalkCallback(Action<Character> cb)
	{
		cbOnWalk += cb;
	}

	public void RegisterOnDestroyedCallback(Action<Character> cb)
	{
		cbOnDestroyed += cb;
	}

	public void RegisterCanCharacterJumpCallback(Action<Character, bool> cb)
	{
		canCharacterJump += cb;
	}
}
