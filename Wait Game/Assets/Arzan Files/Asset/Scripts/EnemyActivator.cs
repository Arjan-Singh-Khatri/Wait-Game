using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [SerializeField] EnemyAI enemy;
    
    void Activate()
    {
        enemy.Activate();
    }
}
