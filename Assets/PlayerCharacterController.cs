using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    public string JumpButton = "Jump";
    public string MovementAxis = "Horizontal";
    public string SetGravityButton = "Fire2";
    public CharacterController characterController;
    public GravityModifier gravityModifier;

	void Update ()
    {
		if (Input.GetButtonDown(JumpButton))
        {
            characterController.StartJump();
        }
        else if (Input.GetButtonUp(JumpButton))
        {
            characterController.EndJump();
        }

        characterController.HorizontalMovement = Input.GetAxis(MovementAxis);

        if (Input.GetButtonDown(SetGravityButton))
        {
            gravityModifier.ChangeGravityDirection();
        }
	}
}
