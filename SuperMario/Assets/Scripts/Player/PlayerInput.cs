using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && player.isGrounded)
        {
            player.Jump();
            player.canDoubleJump = true;
        }
        else if(Input.GetButtonDown("Jump") && !player.isGrounded && player.canDoubleJump)
        {
            player.DoubleJump();
            player.canDoubleJump = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            player.ShootProjectile();
        }
    }
}
