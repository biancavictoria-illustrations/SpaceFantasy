using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossHealthBar : EnemyHealthBar
{
    public TMP_Text bossName;

    public void SetBossHealthBarActive(bool set, EnemyID enemyID = EnemyID.enumSize)
    {
        slider.gameObject.SetActive(set);
        bossName.gameObject.SetActive(set);

        if(set){
            switch(enemyID){
                case EnemyID.BeetleBoss:
                    bossName.text = "The Brute Pest";
                    break;
                case EnemyID.TimeLich:
                    bossName.text = "The Time Lich";
                    break;
                case EnemyID.Harvester:
                    bossName.text = "The Harvester";
                    break;
            }
        }
    }
}
