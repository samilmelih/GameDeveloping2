using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    // FIXME: If we want to spawn enemies into scene at any time, we need a new approach.
    

    // Update is called once per frame
    void Update()
    {
		
    }

	void FixedUpdate()
	{
		
	}



    

    void Walk(KeyValuePair<Character, GameObject> enemyGOPair)
    {
		
        // TODO: burayı çok genel yapabiliriz

        // Eğer karakteri görmüyorsa ona yeni gidecek pozisyon tanımlamaları yaparız kısa aralıklarda destPos şeklinde veirriz oraya yürür
        // Karakteri görüyorsa destPos karakterin konumu olur
        // karakter görüşünden çıktığı anda yeni bir destPos oluştururuz

        Character enemy = enemyGOPair.Key;
        GameObject enemy_go = enemyGOPair.Value;
		        
		GameObject go_mainCharacter = CharacterController.Instance.go_mainCharacter;
        Vector3 mainCharacterPosition;
        
		//eğer karakter görüş açımızda ise ona doğru yürü
        EnemyImpact impact = Watch(enemyGOPair);
      
		Vector2 destPos;
		float direction;
		float scaleX;

		if(enemyGOPair.Key.direction == Direction.Left)
			direction = scaleX = -1f;
		else
			direction = scaleX = 1f;

		switch (impact)
        {
            case EnemyImpact.Enemy:

                //Debug.Log(EnemyImpact.Enemy);
                direction *= -1;
                scaleX    *= -1;
                if(enemy.direction==Direction.Left)
                    enemy.direction = Direction.Right;
                else
                    enemy.direction = Direction.Left;

                break;
            case EnemyImpact.Player:
               // Debug.Log(EnemyImpact.Player);

                mainCharacterPosition = go_mainCharacter.transform.position;

                destPos = new Vector2(mainCharacterPosition.x, mainCharacterPosition.y);
                break;
            case EnemyImpact.Wall:
              //  Debug.Log(EnemyImpact.Wall);
                break;
            case EnemyImpact.None:
              
               
                LayerMask whatIsGround = 127;

                Transform groundCheck = enemy_go.transform.Find("GroundCheck");

                float groundRadius = 0.2f;

                bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

				// FIXME: Düşman spawn edildiğinde havada olduğu için bu kod yüzünden saçmalıyor.
				// Çok büyük bir hata olmadığı için şimdilik not olarak buraya bırakıyorum.
                if (grounded == false)
                {
                    direction *= -1;
                    scaleX    *= -1;
                    if(enemy.direction==Direction.Left)
                        enemy.direction = Direction.Right;
                    else
                        enemy.direction = Direction.Left;
                }
               
                break;
        }   
        

		enemy_go.transform.localScale = new Vector3(scaleX, 1, 0);

		Rigidbody2D rgbd2D = enemy_go.GetComponent<Rigidbody2D>();
		rgbd2D.velocity = new Vector2(enemy.speed.x * direction, rgbd2D.velocity.y);

        
    }

	EnemyImpact OnWatch(Character enemy)
    {
		GameObject go_enemy = enemyGOMap[enemy];

        Vector2 direction;

        //baktığın yönde doğrusal olarak bir raycast yap ve gördüğün nesne Player ise ateş et


        if (enemy.direction == Direction.Left)
            direction = Vector2.left;
        else
            direction = Vector2.right;

        Transform gunPosition = go_enemy.transform.Find("Gun");

		// FIXME: Distance of the ray is hardcoded.
        RaycastHit2D hit = Physics2D.Raycast(gunPosition.position, direction, 10);
        
		// Debug.DrawRay(gunPosition.position,new Vector3(direction.x,direction.y,0) * 10 , Color.red);

        // Player dışında menzili düşürmemiz lazım küçük bir hesap yap

        // Duvar ile yada enemy ile arasında 10 birim mesafe olmasın 1 2 olsa yeter o yüzden tekrar bi pozisyon farkı al
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            Fire(enemy);
            return EnemyImpact.Player;
        }

		// FIXME: "hit.distance < 1f" is harcoded.
        if (hit.collider != null && hit.collider.tag == "Enemy" && hit.distance < 1f)
        {   
            return EnemyImpact.Enemy;
        }

		if (hit.collider != null && hit.collider.tag == "Wall" && hit.distance < 1f)
        {
            return EnemyImpact.Wall;
        }
       
        return EnemyImpact.None;
    }

    void Fire(Character enemy)
    {
        enemy.currentWeapon.cbAttack(enemy);
    }
}
