using SinglePlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjective : MonoBehaviour
{

    [SerializeField] InfoText setObjective;
    [SerializeField] GameObject player;

    private void Update()
    {
        if(Vector3.Distance(gameObject.transform.position, player.transform.position) < 1)
        {
            setObjective.SetContent(" Escape the dungeon. ");
            Destroy(gameObject);
        }    
    }
}
