using UnityEngine;
using UnityEngine.UI;

public class HoverHpBar : MonoBehaviour
{
    private const float SIZE_FACTOR = 0.5f;

    [Header("Image Objects")]
    public Image healthBar;
    public Image border;

    [Header("Owner Objects")]
    public GameObject gameEntity;
    private Health health;

    void Start()
    {
        health = gameEntity.GetComponent<Health>();
        UpdateSize();

        health.damagedEvent += UpdateDamage;
        health.deathEvent += UpdateDamageToZero;
        health.buffedHPEvent += UpdateDamage;
        health.buffedHPEvent += UpdateSize;
    }

    private void OnEnable() {
        UpdateDamage();
    }

    private void Update()
    {
        transform.LookAt(gameEntity.transform.position + new Vector3(0,0,1));
    }

    public void UpdateSize()
    {
        healthBar.transform.localScale = new Vector3(GetXScale(), GetYScale(), 1);
        border.transform.localScale = new Vector3(GetXScale(), GetYScale(), 1);
    }

    public void UpdateDamage()
    {
        healthBar.fillAmount = GetRatio_HP_HPMax();
    }
    public void UpdateDamageToZero()
    {
        healthBar.fillAmount = 0;
    }

    #region Getters
    public float GetRatio_HP_HPMax()
    {
        if(health == null)return 1;
        return health.hp / health.maxHp;
    }

    public float GetXScale()
    {
        // scales only by 50% of the change
        float x_scale = 1 + SIZE_FACTOR * (health.maxHp - 100) / 100;
        // scales according to scaled gameEntity
        x_scale = x_scale / gameEntity.transform.localScale.x;
        return x_scale;
    }
    public float GetYScale()
    {
        return 1 / gameEntity.transform.localScale.y;
    }
    #endregion
}
