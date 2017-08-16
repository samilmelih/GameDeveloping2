using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterController : MonoBehaviour
{
	public Dictionary<Character, GameObject> characterGoMap;
	public Dictionary<GameObject,Character > GoCharacterMap;

	// FIXME: This shouldn't be here!
	public Transform[] spawnPos;

	World world;

	public static CharacterController Instance;

    void Start()
    {
		Instance = this;

		world = WorldController.Instance.world;

		characterGoMap = new Dictionary<Character, GameObject>();
		GoCharacterMap = new Dictionary<GameObject, Character>();

		CreateMainCharacter();
		CreateEnemies();
	}

	// This just creates our main character.
	void CreateMainCharacter()
	{
		// Create our characters into scene. For now,
		// we only have one character.

		// FIXME: This need to be change in the future.
		GameObject chr_prefab = (GameObject) Resources.Load("Prefabs/Character");

		GameObject go_mainCharacter = Instantiate(chr_prefab, chr_prefab.transform);

		go_mainCharacter.name = "Main Character";
		go_mainCharacter.tag = "Player";
		go_mainCharacter.transform.position = transform.position;
		go_mainCharacter.transform.SetParent(this.transform, true);

		world.character.RegisterOnAttackCallback(OnCharacterAttack);
		world.character.RegisterOnCrouchCallback(OnCharacterCrouch);
		world.character.RegisterOnJumpCallback(OnCharacterJump);
		world.character.RegisterOnWalkCallback(OnCharacterWalk);

		characterGoMap.Add(world.character, go_mainCharacter);
		GoCharacterMap.Add(go_mainCharacter, world.character);
	}

	void CreateEnemies()
	{
		int numberOfEnemy = 0;
		foreach (Character enemy in world.enemies)
		{
			GameObject enemy_prefab = (GameObject)Resources.Load("Prefabs/Enemy");		// FIXME: This need to be change in the future.

			//TODO : burada bir transform list içince spawn positions belirlenecek
			// bu posizsyonlar ne olursa olsun bu şekilde yapılabilir
			GameObject enemy_go = (GameObject)Instantiate(enemy_prefab, spawnPos[numberOfEnemy], false);

			enemy_go.name = "Enemy_" + (++numberOfEnemy);
			enemy_go.tag = "Enemy";

			// FIXME: We need to randomize positions later. We can't instantiate
			// enemies at the same position.

			enemy_go.transform.SetParent(this.transform);

			characterGoMap.Add(enemy, enemy_go);
			GoCharacterMap.Add(enemy_go, enemy);
		}

	}

    // Update is called once per frame
    void Update()
    {
		// Update enemies
		List<Character> keys = new List<Character>(characterGoMap.Keys);
		foreach (Character characters in keys)
		{
			// We will update every frame.
			characters.Update();
		}
	}

	void FixedUpdate()
	{
		// Update physics for enemies
		List<Character> keys = new List<Character>(characterGoMap.Keys);
		foreach (Character characters in keys)
		{
			characters.PhysicUpdates();
		}
	}

	// This method will be called by Character class.
	void OnCharacterDestroyed(Character ch)
	{
		if(characterGoMap.ContainsKey(ch))
		{
			Destroy(characterGoMap[ch]);
			GoCharacterMap.Remove(characterGoMap[ch]);
			characterGoMap.Remove(ch);
		}
		else
		{
			Debug.LogError("OnCharacterDestroyed() -- GameObject of the character to be destroyed is not found.");
		}
	}


	bool canCharacterJump(Character ch)
	{
		// 01111111, ilk 7 layer dahil, 8 ve sonrası yok. Bunu karakter
		// özelliği olarak yapmak istersek sanırım <Character, LayerMask>
		// şekline tutmamız gerekecek bu sınıfta.

		// FIXME: In fact this mask is not general. We need to fix later.
		// FIXME: Maybe a <character.Type, LayerMask>.
		LayerMask whatIsGround;

		if(ch.Type == "Main Character")
		{
			// 98 7654 3210		layers
			// 10 1111 1111
			whatIsGround = 767;
		}
		else if(ch.Type == "Enemy")
		{
			whatIsGround = 511;
		}
		else
		{
			Debug.LogError("canCharacterJump() -- Character type could not be identified.");
			return false;
		}

		Transform groundCheck = characterGoMap[ch].transform.Find("GroundCheck");

		// FIXME: This is hardcoded.
		float groundRadius = 0.2f;

		Collider2D canJump = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

		// If character can jump return true;
		if(canJump != null)
			return true;
		else
			return false;
	}

	void OnCharacterJump(Character ch)
	{
		if(characterGoMap.ContainsKey(ch) == false)
		{
			Debug.LogError("OnCharacterJump() -- GameObject reference is not found.");
			return;
		}

		Rigidbody2D rgbd2D = characterGoMap[ch].GetComponent<Rigidbody2D>();
		rgbd2D.velocity = new Vector2(rgbd2D.velocity.x, ch.velocity.y);
	}
		
	void OnCharacterWalk(Character ch)
	{
		if(characterGoMap.ContainsKey(ch) == false)
		{
			Debug.LogError("OnCharacterWalk() -- GameObject reference is not found.");
			return;
		}

		Transform chr_transform = characterGoMap[ch].GetComponent<Transform>();
		Rigidbody2D chr_rgbd2D  = characterGoMap[ch].GetComponent<Rigidbody2D>();

		chr_transform.localScale = new Vector3(ch.scale.x, ch.scale.y, 1f);
		chr_rgbd2D.velocity 	 = new Vector2(ch.velocity.x, chr_rgbd2D.velocity.y);
	}

	void OnCharacterAttack(Character ch)
	{
		ch.currentWeapon.cbAttack(ch);
	}

	void OnCharacterCrouch(Character ch)
	{
	}
}
