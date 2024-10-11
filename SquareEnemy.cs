using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareEnemy : Enemy
{
    private void FixedUpdate()
    {
        Move(player.position);
    }
}
