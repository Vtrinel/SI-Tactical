using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessAnimEnemyDamaged : PostProcessAnimTrigger
{
    public static PostProcessAnimEnemyDamaged instance;

    private void Awake()
    {
        instance = this;
    }
}
