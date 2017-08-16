using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	void Update()
	{
		// This code is not nice. not comfortable.
		Character mainCharacter = WorldController.Instance.world.character;
		GameObject go_mainCharacter = CharacterController.Instance.characterGoMap[mainCharacter];

        if (go_mainCharacter != null)
        {
        	Vector3 new_position = new Vector3(
         		go_mainCharacter.transform.position.x,
                go_mainCharacter.transform.position.y,
                Camera.main.transform.position.z
			);

			Camera.main.transform.position = new_position;
		}
	}
}
