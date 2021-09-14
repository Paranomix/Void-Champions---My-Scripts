using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GetLocalPlayer))]
public class MetersHUD : MonoBehaviour
{
    // Constants
    private const float empty_meters_height = 223f;

    private float max_height_panel;

    [Header("UI Objects")]
    // HP
    public Image hp_bar;
    public Text hp_ratio_text;

    // Mana
    public Image mana_bar;
    public Text mana_ratio_text;
    
    // Stats
    public TMP_Text level_text;
    public TMP_Text deaths_text;
    public TMP_Text rooms_text;

    // Power Up's List
    public GameObject powerUpsList;
    public GameObject panel;
    public GameObject mask;
    public GameObject sheet;
    public States cog;

    [Header("Prefabs")]
    public GameObject PUs_prefab;

    [Header("Player's Info")]
    private Health health;
    private Stats stats;
    private ManaMeter mana;

    /// <summary>
    /// Calls an event to register the local player and resizes the panel according to screen size.
    /// </summary>
    public void Awake()
    {
        GetComponent<GetLocalPlayer>().gotLocalPlayerEvent += Init;

        #region Setting heights according to Aspect Ratio
        float canvas_height = ((RectTransform)gameObject.transform).rect.height;
        max_height_panel = canvas_height - empty_meters_height;

        RectTransform rt = mask.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(110, max_height_panel + 2);
        #endregion
    }

    /// <summary>
    /// Sets up this class, after Player Controller has Started.
    /// </summary>
    public void Init(GameObject localPlayer)
    {
        health = localPlayer.GetComponent<Health>();
        health.damagedEvent += UpdateHealthBar;
        health.deathEvent += UpdateHealthBar;
        health.buffedHPEvent += UpdateHealthBar;
        stats = localPlayer.GetComponent<Stats>();
        stats.UpdatedStatsEvent += UpdateCharacterSheet;
        mana = localPlayer.GetComponent<ManaMeter>();
        mana.updateStaminaEvent += UpdateManaBar;
        UpdateManaBar();
        StaminaMeterHud staminaMeterHud = GetComponentInChildren<StaminaMeterHud>();
        if(staminaMeterHud != null)
            staminaMeterHud.SetLocalPlayer(localPlayer);
        
        health.deathEvent += ResetList;
        cog.Click();

    }

    /// <summary>
    /// Updates the MetersHUD's hp bar.
    /// </summary>
    public void UpdateHealthBar()
    {
        float healthRatio = health.GetRatio_HP_HPMax();
        PostProcessorInterface.SetHpVignette(healthRatio);
        hp_bar.fillAmount = healthRatio;

        float hp = health.hp;
        if (hp < 0)
        {
            hp = 0f;
        }

        hp_ratio_text.text = hp + " / " + health.maxHp;
    }
    /// <summary>
    /// Updates the MetersHUD's mana bar.
    /// </summary>
    private void UpdateManaBar()
    {
        mana_bar.fillAmount = mana.GetRatio_Mana_ManaMax();

        float m = mana.currentMana;
        if (m < 0)
        {
            m = 0f;
        }

        mana_ratio_text.text = m + " / " + mana.currentMaxMana;
    }
    /// <summary>
    /// Updates the MetersHUD's Character Sheet.
    /// </summary>
    public void UpdateCharacterSheet()
    {
        level_text.text = stats.level.ToString();
        deaths_text.text = stats.number_of_deaths.ToString();
        rooms_text.text = stats.number_of_rooms_completed.ToString();
    }
    /// <summary>
    /// Updates the MetersHUD's Power Ups List.
    /// </summary>
    public void UpdatePowerUpsList()
    {
        RectTransform rt = powerUpsList.GetComponent<RectTransform>();
        RectTransform rt2 = (RectTransform)panel.transform;
        float height = rt2.rect.height;

        if (height > max_height_panel)
        {
            height = max_height_panel;
        }

        rt.sizeDelta = new Vector2(110, height);
    }

    /// <summary>
    /// Cleans the Power Up list. Used after a death.
    /// </summary>
    public void ResetList()
    {
        foreach (Transform child in panel.transform)
        {
             GameObject.Destroy(child.gameObject);
        }

        Invoke("UpdatePowerUpsList", 0.02f);
    }

    /// <summary>
    /// Adds an icon of the chosen Power Up to the Power Up list of MetersHUD. Called by Power Up Menu, after an item being chosen. 
    /// </summary>
    /// <param name="title">Of the chosen Power Up.</param>
    /// <param name="image">Icon of the chosen Power Up.</param>
    /// <param name="bonus">Of the chosen Power Up.</param>
    public void AddPowerUp(string title, Sprite image, string bonus)
    {
        GameObject powerUp = GameObject.Instantiate(PUs_prefab, panel.transform);

        powerUp.transform.GetChild(0).GetComponent<TMP_Text>().text = title;
        powerUp.transform.GetChild(1).GetComponent<Image>().sprite = image;
        powerUp.transform.GetChild(2).GetComponent<TMP_Text>().text = bonus;

        // it has to update later to calculate the height properly
        Invoke("UpdatePowerUpsList", 0.02f);
    }
}
