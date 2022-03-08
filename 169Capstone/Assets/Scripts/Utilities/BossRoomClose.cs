using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomClose : MonoBehaviour
{
    [SerializeField] private Room room;
    [SerializeField] private Color newColor;
    [SerializeField] private Light sceneLight;

    private Color oldColor;
   
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
           // Enables the force field object
           transform.GetChild(0).gameObject.SetActive(true);
           
           foreach(EntityHealth e in room.enemies)
           {
               e.OnDeath.AddListener(RoomOpen);
               if( e.isBossEnemy ){
                   InGameUIManager.instance.bossHealthBar.SetBossHealthBarActive(true, e.enemyID);
               }
           }

           oldColor = sceneLight.color;
           sceneLight.color = newColor;
        }
    }

    private void RoomOpen(EntityHealth _)
    {
        if (!room.hasEnemies())
        {
            transform.GetChild(0).gameObject.SetActive(false);
            sceneLight.color = oldColor;
            Debug.Log("Room Open");
        }
    }

}
