using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterAI
{
	

	Func<Character, EnemyImpact> cbOnWatch;

	World world;

	public CharacterAI()
	{
		world = WorldController.Instance.world;
	}

	public void Update(Character ch)
	{
		
	}

	public void PhysicUpdates(Character ch)
	{
		Walk(ch);
	}

	void Walk(Character ch)
	{

		// TODO: burayı çok genel yapabiliriz

		// Eğer karakteri görmüyorsa ona yeni gidecek pozisyon tanımlamaları yaparız kısa aralıklarda destPos şeklinde veirriz oraya yürür
		// Karakteri görüyorsa destPos karakterin konumu olur
		// karakter görüşünden çıktığı anda yeni bir destPos oluştururuz

		//eğer karakter görüş açımızda ise ona doğru yürü
		EnemyImpact impact = cbOnWatch(ch);

		Vector2 destPos;
		float direction;
		float scaleX;

		if(ch.direction == Direction.Left)
			direction = scaleX = -1f;
		else
			direction = scaleX = 1f;

		switch (impact)
		{
		case EnemyImpact.Enemy:

			//Debug.Log(EnemyImpact.Enemy);
			direction *= -1;
			scaleX    *= -1;
			if(ch.direction==Direction.Left)
				ch.direction = Direction.Right;
			else
				ch.direction = Direction.Left;

			break;
		case EnemyImpact.Player:
			// Debug.Log(EnemyImpact.Player);


			break;
		case EnemyImpact.Wall:
			//  Debug.Log(EnemyImpact.Wall);
			break;
		case EnemyImpact.None:


			LayerMask whatIsGround = 127;

			// FIXME: I think we need a better approach for checking if we on a ground or not.
			Transform groundCheck = enemy_go.transform.Find("GroundCheck");

			float groundRadius = 0.2f;

			bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

			// FIXME: Düşman spawn edildiğinde havada olduğu için bu kod yüzünden saçmalıyor.
			// Çok büyük bir hata olmadığı için şimdilik not olarak buraya bırakıyorum.
			if (grounded == false)
			{
				direction *= -1;
				scaleX    *= -1;
				if(ch.direction==Direction.Left)
					ch.direction = Direction.Right;
				else
					ch.direction = Direction.Left;
			}

			break;
		}   


		enemy_go.transform.localScale = new Vector3(scaleX, 1, 0);

		Rigidbody2D rgbd2D = enemy_go.GetComponent<Rigidbody2D>();
		rgbd2D.velocity = new Vector2(ch.speed.x * direction, rgbd2D.velocity.y);


	}





	public void RegisterOnWatchCallback(Func<EnemyImpact, Character> cb)
	{
		cbOnWatch += cb;
	}
}
