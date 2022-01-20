using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityStats : MonoBehaviour
{

    #region Hit Points
        protected float maxHitPointsBase;
        protected float maxHitPointsMultiplier;
        protected float maxHitPointsFlatBonus;

        public virtual float getMaxHitPoints()
        {
            return maxHitPointsBase * maxHitPointsMultiplier + maxHitPointsFlatBonus;
        }
    #endregion

    #region Attack Speed
        protected float attackSpeedBase;
        protected float attackSpeedMultiplier;
        protected float attackSpeedFlatBonus;

        public virtual float getAttackSpeed()
        {
            return attackSpeedBase * attackSpeedMultiplier + attackSpeedFlatBonus;
        }
    #endregion

    #region Move Speed
        protected float moveSpeedBase;
        protected float moveSpeedMultiplier;
        protected float moveSpeedFlatBonus;

        public virtual float getMoveSpeed()
        {
            return moveSpeedBase * moveSpeedMultiplier + moveSpeedFlatBonus;
        }
    #endregion
    
    #region Defense
        protected float defenseBase;
        protected float defenseMultiplier;
        protected float defenseFlatBonus;

        public virtual float getDefense()
        {
            return defenseBase * defenseMultiplier + defenseFlatBonus;
        }
    #endregion
    
    #region Dodge Chance
        protected float dodgeChanceBase;
        protected float dodgeChanceFlatBonus;

        public virtual float getDodgeChance()
        {
            return dodgeChanceBase + dodgeChanceFlatBonus;
        }
    #endregion

    #region Crit Chance
        protected float critChanceBase;
        protected float critChanceFlatBonus;

        public virtual float getCritChance()
        {
            return critChanceBase + critChanceFlatBonus;
        }
    #endregion

    #region Crit Damage
        protected float critDamageBase;
        protected float critDamageMultiplier;
        protected float critDamageFlatBonus;

        public virtual float getCritDamage()
        {
            return critDamageBase * critDamageMultiplier + critDamageFlatBonus;
        }
    #endregion

    #region Stun Chance
        protected float stunChanceBase;
        protected float stunChanceFlatBonus;

        public virtual float getStunChance()
        {
            return stunChanceBase + stunChanceFlatBonus;
        }
    #endregion

    #region Burn Chance
        protected float burnChanceBase;
        protected float burnChanceFlatBonus;

        public virtual float getBurnChance()
        {
            return burnChanceBase + burnChanceFlatBonus;
        }
    #endregion
    
    #region Slow Chance
        protected float slowChanceBase;
        protected float slowChanceFlatBonus;

        public virtual float getSlowChance()
        {
            return slowChanceBase + slowChanceFlatBonus;
        }
    #endregion
    
    #region Status Resist Chance
        protected float statusResistChanceBase;
        protected float statusResistChanceMultiplier;
        protected float statusResistChanceFlatBonus;

        public virtual float getStatusResistChance()
        {
            return statusResistChanceBase * statusResistChanceMultiplier + statusResistChanceFlatBonus;
        }
    #endregion

}
