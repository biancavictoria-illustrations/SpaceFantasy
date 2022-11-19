using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DamageSourceType{
    Player,

    // Enemies
    Slime,
    Robert,

    // Bosses
    BeetleBoss,
    // Harvester,
    TimeLich,
    DefeatedTimeLichEndRunDeath,

    // Traps
    DeathPit,   // Intentionally does not contain "trap" because we check if the string contains "trap" to deal with trap damage
    TurretTrap,
    CoilTrap,
    FlameTrap,
    FloorSpikeTrap,

    DeathPitTimeLichArena,

    enumSize
}

public struct DamageData{
    public float damageValue;
    public bool isCrit;

    public DamageData(float _damageValue, bool _isCrit){
        damageValue = _damageValue;
        isCrit = _isCrit;
    }
}

public class EntityHealth : MonoBehaviour
{
    #region OnDeath
        public class OnDeathEvent : UnityEvent<EntityHealth> {}

        public OnDeathEvent OnDeath
        {
            get 
            { 
                if(onDeath == null) 
                    onDeath = new OnDeathEvent(); 
                
                return onDeath;
            }

            set { onDeath = value; }
        }
        private OnDeathEvent onDeath;
    #endregion

    #region OnHit
        public class OnHitEvent : UnityEvent<EntityHealth, float> {}

        public OnHitEvent OnHit
        {
            get 
            { 
                if(onHit == null) 
                    onHit = new OnHitEvent(); 
                
                return onHit;
            }

            set { onHit = value; }
        }
        private OnHitEvent onHit;
    #endregion

    // #region OnCrit
    //     public class OnCritEvent : UnityEvent<EntityHealth, float> {}

    //     public OnCritEvent OnCrit
    //     {
    //         get
    //         {
    //             if (onCrit == null)
    //                 onCrit = new OnCritEvent();

    //             return onCrit;
    //         }

    //         set { onCrit = value; }
    //     }
    //     private OnCritEvent onCrit;
    // #endregion

    public float maxHitpoints;
    public float currentHitpoints;

    [Tooltip("All enemies need this (except technically those in the lich fight arena)")]
    [SerializeField] private EnemyDropGenerator enemyDropGenerator;
    [Tooltip("ONLY FOR MINI BOSSES - the single item this boss drops that connects to the NPC")]
    [SerializeField] private EquipmentBaseData dropOverride;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject starShardPrefab;

    [FMODUnity.EventRef] public string hitSFX;

    private EnemyHealthBar enemyHealthUI;
    public bool isBossEnemy {get; private set;}
    public EnemyID enemyID {get; private set;}

    private DamageSourceType damageSourceCausedPlayerDeath;

    private const int TEMPCOINDROPAMOUNT = 5;
    private const float TEMPSTARSHARDDROPCHANCE = 0.25f;

    private const float LOW_HP_THRESHOLD_FOR_UI = 0.2f;

    public bool tempPlayerGodModeToggle = false;    // FOR TESTING - REMOVE THIS FOR FINAL BUILD

    void Awake()
    {
        isBossEnemy = false;
        enemyID = EnemyID.enumSize;
        damageSourceCausedPlayerDeath = DamageSourceType.enumSize;
        
        if(gameObject.tag != "Player"){
            enemyID = GetComponent<EnemyStats>().enemyID;
            // If a boss enemy (update int if we add more bosses)
            if( enemyID == EnemyID.TimeLich || enemyID == EnemyID.BeetleBoss || enemyID == EnemyID.Harvester ){
                isBossEnemy = true;
            }
            // If normal enemy
            else{
                enemyHealthUI = gameObject.GetComponentInChildren<EnemyHealthBar>();
                if(enemyHealthUI == null){
                    Debug.LogError("No enemy health UI found for enemy unit: " + enemyID + "  Transform: " + gameObject.transform.position);
                    return;
                }
            }
            SetMaxHealthUI();
            SetCurrentHealthUI();
        }
    }

    void Start()
    {
        OnDeath.AddListener(onEntityDeath);
        // OnCrit.AddListener(onPlayerCrit);
    }

    public void SetStartingHealthUI()
    {
        SetMaxHealthUI();
        SetCurrentHealthUI();
    }

    // Need a bool to check for slime pit otherwise you could DODGE falling in a slime pit and fall for eternity
    public bool Damage(DamageData damageData, DamageSourceType damageSource)
    {
        float damage = damageData.damageValue;

        if( gameObject.tag == "Player" && damageSource != DamageSourceType.DeathPit && damageSource != DamageSourceType.DeathPitTimeLichArena && damageSource != DamageSourceType.DefeatedTimeLichEndRunDeath ){
            // TEMP for dev
            if( tempPlayerGodModeToggle ){
                return currentHitpoints <= 0;
            }

            // Check for dodge
            float dodgeChance = Player.instance.stats.getDodgeChance();
            float rolledChance = Random.Range(0.0f, 1f);
            if( rolledChance <= dodgeChance ){
                InGameUIManager.instance.ShowFloatingText(null, 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1f, gameObject, "dodge", false);
                return currentHitpoints <= 0;
            }

            // If we got to this point, lower damage according to defense before enacting the damage to the player
            float defense = Player.instance.stats.getDefense();
            damage *= 1 - defense;

            // IF A TRAP, account for trap damage resist
            if( damageSource.ToString().Contains("Trap") ){
                float trapDamageResist = Player.instance.stats.getTrapDamageResist();
                damage *= 1 - trapDamageResist;
            }

            // Enable hit UI
            InGameUIManager.instance.damageUIOverlay.StartDamageOverlayRoutine();
        }

        currentHitpoints -= damage;
        OnHit.Invoke(this, damage);

        if (gameObject.tag == "Player")
            InGameUIManager.instance.ShowFloatingText(UIUtils.GetTruncatedDecimalForUIDisplay(damage), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1f, gameObject, "damage-player", false);
        else
            InGameUIManager.instance.ShowFloatingText(UIUtils.GetTruncatedDecimalForUIDisplay(damage), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1f, gameObject, "damage-enemy", damageData.isCrit);

        SetCurrentHealthUI();

        if(currentHitpoints <= 0)
        {
            currentHitpoints = 0;

            // If the player died in the lich arena but NOT to the lich himself OR after killing him OR to a death pit, set damageSource to lich instead
            if( GameManager.instance.currentSceneName == GameManager.LICH_ARENA_STRING_NAME && damageSource != DamageSourceType.DeathPitTimeLichArena && damageSource != DamageSourceType.DefeatedTimeLichEndRunDeath ){
                damageSourceCausedPlayerDeath = DamageSourceType.TimeLich;
            }
            else{   // Otherwise, set it to whatever it registered as
                damageSourceCausedPlayerDeath = damageSource;
            }
            
            OnDeath.Invoke(this);
        }

        // If player fell in a pit, play that SFX
        if(gameObject.tag == "Player" && damageSource == DamageSourceType.DeathPit || damageSource == DamageSourceType.DeathPitTimeLichArena){
            AudioManager.Instance.PlaySFX(AudioManager.SFX.FellInSlimePit, gameObject);
        }
        else if( !damageSource.ToString().Contains("Trap") ){   // Otherwise, play this entity's unique hit SFX (if it has one) (unless it's trap damage cuz that gets weird)
            AudioManager.Instance.PlaySFX(hitSFX, gameObject);
        }

        ManageLowHealthOverlay();

        return currentHitpoints <= 0;
    }

    public void Heal(float health)
    {
        currentHitpoints += health;
        InGameUIManager.instance.ShowFloatingText(UIUtils.GetTruncatedDecimalForUIDisplay(health), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1f, gameObject, "health", false);

        if(currentHitpoints > maxHitpoints)
        {
            currentHitpoints = maxHitpoints;
        }
        
        ManageLowHealthOverlay();

        SetCurrentHealthUI();
    }

    public void UpdateHealthOnUpgrade()
    {
        float oldMax = maxHitpoints;

        // Calculate new max from bonuses
        maxHitpoints = Player.instance.stats.getMaxHitPoints();

        // Calculate new current by adding the same amount you gained or lost
        currentHitpoints += maxHitpoints - oldMax;

        // Don't show floating text when you upgrade max health (Chase designer decision)
        // InGameUIManager.instance.ShowFloatingText(UIUtils.GetTruncatedDecimalForUIDisplay(maxHitpoints), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1.5f, gameObject, "health");

        // If you unequipped something that gave you a HP buff and now you would have <= 0 HP, instead set HP to 1
        if(currentHitpoints <= 0){
            currentHitpoints = 1;
        }

        ManageLowHealthOverlay();

        SetMaxHealthUI();
        SetCurrentHealthUI();
    }

    public void ManageLowHealthOverlay()
    {
        if(gameObject.tag != "Player"){
            return;
        }

        // If the player's HP is > the low HP threshold, tell the low health overlay to go away IF it's
        // currently active
        if( currentHitpoints > maxHitpoints * LOW_HP_THRESHOLD_FOR_UI ){
            InGameUIManager.instance.damageUIOverlay.EnableLowHealthOverlay(false);
        }
        else{ // if( currentHitpoints <= maxHitpoints * LOW_HP_THRESHOLD_FOR_UI ){
            // If the player is <= 20% health, enable crit HP UI
            InGameUIManager.instance.damageUIOverlay.EnableLowHealthOverlay(true);
        }
    }

    public void SetCurrentHealthUI()
    {
        if(gameObject.tag == "Player"){
            InGameUIManager.instance.SetCurrentHealthValue(currentHitpoints);   
        }
        else if(!isBossEnemy){
            enemyHealthUI.SetCurrentHealth(currentHitpoints);
        }
        else{
            InGameUIManager.instance.bossHealthBar.SetCurrentHealth(currentHitpoints);
        }
    }

    public void SetMaxHealthUI()
    {
        if(gameObject.tag == "Player"){
            InGameUIManager.instance.SetMaxHealthValue(maxHitpoints);
        }
        else if(!isBossEnemy){
            enemyHealthUI.SetMaxHealth(maxHitpoints);
        }
        else{
            InGameUIManager.instance.bossHealthBar.SetMaxHealth(maxHitpoints);
        }
    }

    private void onEntityDeath(EntityHealth health)
    {
        if(health != this)
            return;

        // Debug.Log("Death");
        if(gameObject.tag == "Player")
        {
            // Tell the story manager what the player was killed by
            StoryManager.instance.KilledEventOccurred(damageSourceCausedPlayerDeath, StoryBeatType.KilledBy);            
            GameManager.instance.playerDeath = true;

            AudioManager.Instance.stopMusic(true);
            AudioManager.Instance.PlaySFX(AudioManager.SFX.TimelineResetDeath, gameObject);
        }
        else
        {
            // Tell the story manager that this creature was killed
            StoryManager.instance.KilledEventOccurred(enemyID, StoryBeatType.EnemyKilled);

            // Update total # enemies killed this run and maybe increase gear tier
            GameManager.instance.UpdateGearTierValuesOnEnemyKilled(enemyID == EnemyID.BeetleBoss);

            if(isBossEnemy){
                InGameUIManager.instance.bossHealthBar.SetBossHealthBarActive(false);

                if(enemyID == EnemyID.TimeLich){
                    // Story state things
                    // If this is your first clear, make note of that
                    if( !GameManager.instance.hasKilledTimeLich ){
                        GameManager.instance.DocumentFirstClearInfo();
                    }

                    DialogueManager.instance.SetTimeLichDeathDialogueTriggered(true);
                    if( PermanentUpgradeManager.instance.GetSkillLevel(PermanentUpgradeType.TimeLichKillerThing) > 0 ){
                        GameManager.instance.epilogueTriggered = true;
                    }

                    // Trigger auto dialogue from player since this script is about to get destroyed
                    Player.instance.StartAutoDialogueFromPlayer();
                }
            }
            
            // If we're NOT in the lich fight, maybe drop something
            if(GameManager.instance.currentSceneName != GameManager.LICH_ARENA_STRING_NAME){
                if(enemyDropGenerator){
                    // If it's a mini boss, drop the designated item & a bunch of star shards
                    if(enemyID == EnemyID.BeetleBoss){
                        enemyDropGenerator.GetDrop(GameManager.instance.gearTier, transform, rarity: ItemRarity.Legendary, dropOverride);
                        DropStarShard(15);
                    }
                    // Otherwise, randomly generate a new drop
                    else{
                        enemyDropGenerator.GetDrop(GameManager.instance.gearTier, transform);
                    }
                }
                else
                    Debug.LogWarning("No enemy drop generator found for enemy: " + enemyID);
            }
            
            // Currency drops
            for(int i = 0; i < TEMPCOINDROPAMOUNT; ++i)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }
            if(Random.value <= TEMPSTARSHARDDROPCHANCE)
            {
                DropStarShard();
            }

            // If you killed the mini boss, trigger Stellan's comm to tell you to go to the elevator
            if(enemyID == EnemyID.BeetleBoss){
                DialogueManager.instance.SetStellanCommTriggered(true);
                // Have to do this in the player since this script is about to get destroyed and everything will get upset
                Player.instance.StartAutoDialogueFromPlayer();
            }

            Destroy(gameObject);
        }   
    }

    private void DropStarShard(int numberOfStarShards = 1)
    {
        // Spawn the first star shard right where the enemy died
        GameObject firstStarShard = Instantiate(starShardPrefab, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(AudioManager.SFX.StarShardDrop, firstStarShard);

        numberOfStarShards--;

        // TODO: BOUNDS CHECK -> don't let stuff spawn outside room bounds (which can happen with the offset)
        // If we're spawning more than one, loop through the rest with random positions
        if(numberOfStarShards > 0){
            // Set offset value for randomly generating offset vector positions
            // (if we spawn all star shards in a single spot, they explode cuz physics)
            float offsetValue = 4;

            // Loop through the remaining # of star shards to spawn until we've spawned all of them
            while(numberOfStarShards > 0){
                // Randomly generate a new offset
                Vector3 offset = new Vector3( Random.Range(-offsetValue, offsetValue), Random.Range(-offsetValue, offsetValue), Random.Range(-offsetValue, offsetValue) );

                // Instantiate a ss at the enemies position with offset
                Instantiate(starShardPrefab, transform.position + offset, Quaternion.identity);
                numberOfStarShards--;
            }
        }     
    }

    // private void onPlayerCrit(EntityHealth health, float critDamage)
    // {
    //     InGameUIManager.instance.ShowFloatingText(UIUtils.GetTruncatedDecimalForUIDisplay(critDamage), 35, transform.position + (Vector3.up * 3), Vector3.up * 25, 1.5f, gameObject, "crit");
    // }
}
