using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Analytics;
using UnityEngine.Events;

// FIXME do i need to block the animator?

[RequireComponent(typeof(GetLocalPlayer))]
public class PowerUpMenu : MonoBehaviour
{
    // rarity probabilities
    private const float NORMAL = 0.5f;
    private const float SPECIAL = 0.9f;

    // starting levels of the tiers
    private const int STARTER_LEVEL_TIER2 = 2;
    private const int STARTER_LEVEL_TIER3 = 5;
    private const int STARTER_LEVEL_TIER4 = 9;

    // types of power ups
    private const int HP = 0;
    private const int MAXHP = 1;
    private const int DAMAGE = 2;
    private const int SPEED = 3;
    private const int SIZE = 4;
    private const int MANA = 5;
    private const int STAMINA = 6;

    // Time
    private const int TIME = 16;
    private int time = 15;
    private bool chosen = false; // bool for the coroutines not execute after player has chosen

    [Header("UI Objects")]
    public GameObject HUD;
    public Image[] images;
    public TMP_Text[] titles;
    public TMP_Text[] descriptions;
    public TMP_Text[] bonus_names;
    public TMP_Text[] bonus_attributes;
    public TMP_Text timer;
    public MetersHUD meters;
    public Image[] PowerUpsRarityBackground;

    // Player's Info
    private Stats stats;
    private Attributes attributes;

    // Events
    [HideInInspector] public UnityAction powerUpChoosedEvent;

    #region Tier class
    [System.Serializable]
    public class Tier
    {
        public List<PowerUp> HP = new List<PowerUp>();
        public List<PowerUp> maxHP = new List<PowerUp>();
        public List<PowerUp> damage = new List<PowerUp>();
        public List<PowerUp> size = new List<PowerUp>();
        public List<PowerUp> speed = new List<PowerUp>();
        public List<PowerUp> mana = new List<PowerUp>();
        public List<PowerUp> stamina = new List<PowerUp>();
    }
    #endregion

    [Header("Tiers")]
    public Tier[] TierList = new Tier[4];

    private List<PowerUp> powerUps_selection = new List<PowerUp>();

    // Coroutine
    Coroutine coroutineTimer = null;
    Coroutine coroutineUpdateTimer = null;
    
    // Flags
    private bool triggeredByItem = false;
    private int rarity;

    //Lines
    public AudioSource[] Lines;

    /// <summary>
    /// Calls an event to register the local player and resizes the panel according to screen size.
    /// </summary>
    public void Awake()
    {
        GetComponent<GetLocalPlayer>().gotLocalPlayerEvent += Init;
    }

    /// <summary>
    /// Sets up this class, after Player Controller has Started.
    /// </summary>
    public void Init(GameObject localPlayer)
    {
        Debug.Log("Init powerupHUD");
        stats = localPlayer.GetComponent<Stats>();
        attributes = localPlayer.GetComponent<Attributes>();
        SceneInterface sceneInterface = localPlayer.GetComponent<SceneInterface>();
        sceneInterface.requestPowerUps += SetPowerUps;
        sceneInterface.requestExtraPowerUps += SetExtraPowerUps;
        sceneInterface.requestBossPowerUps += SetBossPowerUps;
        powerUpChoosedEvent += sceneInterface.ReadyForNewRoom;
        localPlayer.GetComponent<Health>().deathEvent += Restart;
    }

    /// <summary>
    /// Updates a Power Up slot on the menu.
    /// </summary>
    /// <param name="index">Index of the slot.</param>
    /// <param name="powerUp">Power Up for the slot.</param>
    private void Update_UI(int index, PowerUp powerUp)
    {
        images[index].sprite = powerUp.image;
        titles[index].text = powerUp.title;
        descriptions[index].text = powerUp.description;
        bonus_names[index].text = powerUp.bonus_name;
        bonus_attributes[index].text = powerUp.bonus_attribute;

        switch (rarity)
        {
            case 0:
                PowerUpsRarityBackground[index].color = new Color32(255, 255, 255, 255); //Cinzento
                break;
            case 1:
                PowerUpsRarityBackground[index].color = new Color32(0, 255, 44, 255);    //Verde
                break;
            case 2:
                PowerUpsRarityBackground[index].color = new Color32(0, 147, 225, 255);   //Azul
                break;
            case 3:
                PowerUpsRarityBackground[index].color = new Color32(212, 0, 255, 255);   //Violeta
                break;
            case 4:
                PowerUpsRarityBackground[index].color = new Color32(255, 199, 0, 255);   //Dourado
                break;
            case 5:
                PowerUpsRarityBackground[index].color = new Color32(255, 0, 2, 255);     //Vermelho
                break;
        }
    }

    /// <summary>
    /// Picks 3 Power Ups to be inserted in the menu.
    /// Also enables Power Up Menu and disables Player's movement.
    /// </summary>
    public void SetPowerUps()
    {
        Cursor.visible = true;
        chosen = false;

        time = 15;
        timer.text = "00 : 15";
        coroutineTimer = StartCoroutine(StartTimer(TIME));
        coroutineUpdateTimer = StartCoroutine(UpdateTimer());

        PlayLinePowerUp();

        HUD.SetActive(true);

        int playerLevel = stats.level;
        int playerTier = ConvertLevelToTier(playerLevel);
        if (!triggeredByItem)
        {
            stats.NextRoom();
        }

        bool hp_selected = false;
        bool maxhp_selected = false;
        bool damage_selected = false;
        bool speed_selected = false;
        bool size_selected = false;
        bool mana_selected = false;
        bool stamina_selected = false;
        while (powerUps_selection.Count < 3)
        {
            // fixme leaving stamina PUs out until they are ready to ship
            int type = Random.Range(0, 6); //maxExclusive

            PowerUp selected = null;
            if (type == HP && !hp_selected)
            {
                selected = PickPowerUp(HP, playerTier);
                hp_selected = true;
            }
            else if (type == MAXHP && !maxhp_selected)
            {
                selected = PickPowerUp(MAXHP, playerTier);
                maxhp_selected = true;
            }
            else if (type == DAMAGE && !damage_selected)
            {
                selected = PickPowerUp(DAMAGE, playerTier);
                damage_selected = true;
            }
            else if (type == SPEED && !speed_selected)
            {
                selected = PickPowerUp(SPEED, playerTier);
                speed_selected = true;
            }
            else if (type == SIZE && !size_selected)
            {
                selected = PickPowerUp(SIZE, playerTier);
                size_selected = true;
            }
            else if (type == MANA && !mana_selected)
            {
                selected = PickPowerUp(MANA, playerTier);
                mana_selected = true;
            }
            else if (type == STAMINA && !stamina_selected && attributes.getMaxMana() < 5)
            {
                selected = PickPowerUp(STAMINA, playerTier);
                stamina_selected = true;
            }
            
            if (selected != null)
            {
                powerUps_selection.Add(selected);

                Update_UI(powerUps_selection.Count - 1, selected);
            }
        }
    }

    /// <summary>
    /// Applies Power Ups to the Player.
    /// Also disables Power Up Menu and enables Player's movement.
    /// </summary>
    /// <param name="choice">Index of the choice.</param>
    public void Choose_PowerUp(int choice)
    {
        Cursor.visible = false;
        chosen = true;

        switch (powerUps_selection[choice].bonus_name)
        {
            case "Health:":
                attributes.BuffHealth(powerUps_selection[choice].bonus);
                break;
            case "Max Health:":
                attributes.BuffMaxHealth(powerUps_selection[choice].bonus);
                break;
            case "Damage:":
                attributes.BuffDamage(powerUps_selection[choice].bonus);
                break;
            case "Speed:":
                attributes.BuffSpeed(powerUps_selection[choice].bonus);
                break;
            case "Size:":
                attributes.BuffSize(powerUps_selection[choice].bonus);
                break;
            case "Mana:":
                attributes.BuffMaxMana(powerUps_selection[choice].bonus);
                break;
            case "Stamina:":
                attributes.BuffStamina(powerUps_selection[choice].bonus);
                break;
            case "Duration:":
                //attributes.BuffKickKnockback(powerUps_selection[choice].bonus);
                break;
        }

        meters.AddPowerUp(powerUps_selection[choice].title, powerUps_selection[choice].image,
            powerUps_selection[choice].bonus_attribute);

        HUD.SetActive(false);
        powerUps_selection.Clear();

        if (!triggeredByItem)
        {
            powerUpChoosedEvent.Invoke(); // start a new room
        }

        triggeredByItem = false;
        StopCoroutine(coroutineTimer);
        StopCoroutine(coroutineUpdateTimer);
    }

    /// <summary>
    /// Resets settings to default. Used when the player dies. Just in case he dies while choosing a power up.
    /// </summary>
    public void Restart()
    {
        triggeredByItem = false;
        powerUps_selection.Clear();
        if (coroutineTimer != null)
            StopCoroutine(coroutineTimer);
        if (coroutineUpdateTimer != null)
            StopCoroutine(coroutineUpdateTimer);
    }

    /// <summary>
    /// Forces the UI to appear without performing a room switching
    /// </summary>
    public void SetExtraPowerUps()
    {
        triggeredByItem = true;
        SetPowerUps();
    }

    #region Helper Functions
    /// <summary>
    /// Converts the player's current level into a tier of Power Ups.
    /// </summary>
    /// <param name="playerLevel">Current level of the player.</param>
    private int ConvertLevelToTier(int playerLevel)
    {
        if (playerLevel < STARTER_LEVEL_TIER2)
            return 0;
        if (playerLevel < STARTER_LEVEL_TIER3)
            return 1;
        if (playerLevel < STARTER_LEVEL_TIER4)
            return 2;
        return 3;
    }

    /// <summary>
    /// Picks a Power Up of from the current tier.
    /// </summary>
    /// <param name="type">Type of the Power Up (HP, Max HP, Damage...).</param>
    /// <param name="playerLevel">Current tier of the player.</param>
    private PowerUp PickPowerUp(int type, int playerTier)
    {
        int index;
        if (type == HP)
        {
            float rarity = Random.Range(0.0f, 1.0f); // inclusive,inclusive
            if (rarity < NORMAL || TierList[playerTier].HP.Count == 1)
            {
                index = 0;
            }
            else if (rarity < SPECIAL)
                index = 1;
            else // RARE
                index = 2;

            this.rarity = index + playerTier;
            return TierList[playerTier].HP[index];
        }
        if (type == MAXHP)
        {
            index = GetRarityIndex();
            rarity = index + playerTier;
            return TierList[playerTier].maxHP[index];
        }
        if (type == DAMAGE)
        {
            index = GetRarityIndex();
            rarity = index + playerTier;
            return TierList[playerTier].damage[index];
        }
        if (type == SPEED)
        {
            index = GetRarityIndex();
            rarity = index + playerTier;
            return TierList[playerTier].speed[index];
        }
        if (type == SIZE)
        {
            index = GetRarityIndex();
            rarity = index + playerTier;
            return TierList[playerTier].size[index];
        }
        if (type == MANA)
        {
            index = GetRarityIndex();
            rarity = index + playerTier;
            return TierList[playerTier].mana[index];
        }
        if (type == STAMINA)
        {
            float rarity = Random.Range(0.0f, 0.9f); // inclusive,inclusive
            if (rarity < NORMAL || attributes.getMaxMana() == 4)
            {
                index = 0;
            }
            else // Special
                index = 1;

            this.rarity = index;
            return TierList[0].stamina[index];
        }

        return null;
    }
    /// <summary>
    /// Randomly selects one of them based on rarity, assuming the list has only 3 and at least 3 options.
    /// </summary>
    private int GetRarityIndex()
    {
        int index;
        float rarity = Random.Range(0.0f, 1.0f); // inclusive,inclusive
        if (rarity < NORMAL)
        {
            index = 0;
        }
        else if (rarity < SPECIAL)
            index = 1;
        else // RARE
            index = 2;

        return index;
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Updates the timer every second.
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateTimer()
    {
        yield return new WaitForSeconds(1);
        if (!chosen)
        {
            time--;
            if (time > 9)
                timer.text = "00 : " + time;
            else
                timer.text = "00 : 0" + time;

            if (time > 0)
            {
                coroutineUpdateTimer = StartCoroutine(UpdateTimer());
            }
        }
    }
    /// <summary>
    /// Starts timer, so players doesn't take forever to choose.
    /// </summary>
    /// <param name="seconds"> How long, in seconds, to wait.</param>
    /// <returns></returns>
    IEnumerator StartTimer(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (!chosen)
        {
            Choose_PowerUp(0);
        }
    }
    #endregion

    private void PlayLinePowerUp()
    {
        Lines[UnityEngine.Random.Range(0, Lines.Length)].GetComponent<AudioSource>().Play();
    }

#region BossPowerUps
    /// <summary>
    /// Picks 3 level 6 Power Ups to be inserted in the menu.
    /// Also enables Power Up Menu and disables Player's movement.
    /// </summary>
    public void SetBossPowerUps()
    {
        chosen = false;
         Cursor.visible = false;
        //triggeredByItem = true;
        // this flag needs to be off to change room, so it will need to be on if you dont wanna change room

        time = 15;
        timer.text = "00 : 15";
        coroutineTimer = StartCoroutine(StartTimer(TIME));
        coroutineUpdateTimer = StartCoroutine(UpdateTimer());

        // FIXME Maybe play a different song, just for the boss

        HUD.SetActive(true);

        int playerLevel = stats.level;
        int playerTier = ConvertLevelToTier(playerLevel);
        stats.BossUpdate();

        bool hp_selected = false;
        bool maxhp_selected = false;
        bool damage_selected = false;
        bool speed_selected = false;
        bool size_selected = false;
        bool mana_selected = false;
        bool stamina_selected = false;
        while (powerUps_selection.Count < 3)
        {
            // fixme leaving stamina PUs out until they are ready to ship
            int type = Random.Range(0, 6); //maxExclusive

            PowerUp selected = null;
            if (type == HP && !hp_selected)
            {
                selected = PickBossPowerUp(HP, playerTier);
                hp_selected = true;
            }
            else if (type == MAXHP && !maxhp_selected)
            {
                selected = PickBossPowerUp(MAXHP, playerTier);
                maxhp_selected = true;
            }
            else if (type == DAMAGE && !damage_selected)
            {
                selected = PickBossPowerUp(DAMAGE, playerTier);
                damage_selected = true;
            }
            else if (type == SPEED && !speed_selected)
            {
                selected = PickBossPowerUp(SPEED, playerTier);
                speed_selected = true;
            }
            else if (type == SIZE && !size_selected)
            {
                selected = PickBossPowerUp(SIZE, playerTier);
                size_selected = true;
            }
            else if (type == MANA && !mana_selected)
            {
                selected = PickBossPowerUp(MANA, playerTier);
                mana_selected = true;
            }
            else if (type == STAMINA && !stamina_selected && attributes.getMaxMana() < 5)
            {
                selected = PickBossPowerUp(STAMINA, playerTier);
                stamina_selected = true;
            }

            if (selected != null)
            {
                powerUps_selection.Add(selected);

                Update_UI(powerUps_selection.Count - 1, selected);
            }
        }
    }

    /// <summary>
    /// Picks a level 6 Power Up of from the current tier.
    /// </summary>
    /// <param name="type">Type of the Power Up (HP, Max HP, Damage...).</param>
    /// <param name="playerLevel">Current tier of the player.</param>
    private PowerUp PickBossPowerUp(int type, int playerTier)
    {
        int index = 2;
        if (type == HP)
        {
            rarity = index + playerTier;
            return TierList[playerTier].HP[index];
        }
        if (type == MAXHP)
        {
            rarity = index + playerTier;
            return TierList[playerTier].maxHP[index];
        }
        if (type == DAMAGE)
        {
            rarity = index + playerTier;
            return TierList[playerTier].damage[index];
        }
        if (type == SPEED)
        {
            rarity = index + playerTier;
            return TierList[playerTier].speed[index];
        }
        if (type == SIZE)
        {
            rarity = index + playerTier;
            return TierList[playerTier].size[index];
        }
        if (type == MANA)
        {
            rarity = index + playerTier;
            return TierList[playerTier].mana[index];
        }
        if (type == STAMINA)
        {
            if (attributes.getMaxMana() == 4)
                index = 0;
            else
                index = 1;

            this.rarity = index;
            return TierList[0].stamina[index];
        }

        return null;
    }
    #endregion
}