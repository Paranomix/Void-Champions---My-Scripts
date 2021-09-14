using System.Collections;
using UnityEngine;
using Mirror;
// FIXME connect attributes to real stuff: damage and speed 
// FIXME attributes should be registered on the server

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NetworkedMovement))]
[RequireComponent(typeof(ManaMeter))]
[RequireComponent(typeof(StaminaMeter))]
public class Attributes : NetworkBehaviour
{
    // all player "physical" attributes (except Health and Speed)

    // consts
    private const float INITIAL_SPEED = 6f;
    private const float SIZE_UP_DURATION = 0.5f;

    [Header("Attributes")]
    public float size = 1;
   
   [SyncVar] public float damage = 1;
    // FIXME will we have knockback? don't think so

    private NetworkedMovement speed;
    private Health health;
    private ManaMeter mana;
    private StaminaMeter stamina;

    [Header("UI Objects")]
    public HoverHpBar hp_bar;

    // Size vars
    private Vector3 current_scale;
    private float StartGrowEffectTime;
    

    /// <summary>
    /// Sets up this class, after Player Controller has Started.
    /// </summary>
    void Start()
    {
        health = GetComponent<Health>();
        speed = GetComponent<NetworkedMovement>();
        mana = GetComponent<ManaMeter>();
        stamina = GetComponent<StaminaMeter>();

        // FIXME should we be subscribing a death event? maybe a respawn event!
        health.deathEvent += ResetAttributes;
    }

    /// <summary>
    /// Run when player dies.
    /// </summary>
    [Client]
    public void ResetAttributes()
    {
        // hp part should be on the hp script
        speed.speed = INITIAL_SPEED;
        size = 1;
        transform.localScale = Vector3.one;
        hp_bar.UpdateSize();
        SettDamage(1);
        mana.ResetMana();
    }
    [Client]
    public void BuffSpeed(float value)
    {
        speed.speed += INITIAL_SPEED * value;
    }
    [Command]
    private void SettDamage(float val)
    {
        damage = val;
    }
    [Command]
    public void BuffDamage(float value)
    {
        // damage is a multiplier of the base damage
        damage += value;
    }

    [Client]
    public void BuffHealth(float value)
    {
        // Still think it should be a percentage...
        health.hp += value;
        if (health.hp > health.maxHp)
            health.hp = health.maxHp;
        health.buffedHPEvent.Invoke();
    }
    [Client]
    public void BuffMaxHealth(float value)
    {
        health.maxHp += value;
        health.hp += value;
        health.buffedHPEvent.Invoke();
    }
    [Client]
    public void BuffSize(float value)
    {
        current_scale = transform.localScale;
        StartGrowEffectTime = Time.time;
        this.StartCoroutine(this.SizeUpEffect(transform.localScale.x + value));
    }
    private IEnumerator SizeUpEffect(float goal_scale)
    {
        while (StartGrowEffectTime + SIZE_UP_DURATION > Time.time)
        {
            float smoothing_ratio = Mathf.SmoothStep(goal_scale, current_scale.x, (StartGrowEffectTime + SIZE_UP_DURATION - Time.time) / SIZE_UP_DURATION);
            transform.localScale = Vector3.one * smoothing_ratio;
            hp_bar.UpdateSize();
            yield return null;
        }
        current_scale = Vector3.one * goal_scale;
    }
    [Client]
    public void BuffMaxMana(float value)
    {
        mana.currentMaxMana += value;
        mana.currentMana += value;
        mana.UpdateMana();
    }
    [Client]
    public void BuffStamina(float value)
    {
        stamina.maxStamina += (int) value;
        stamina.currentStamina += (int) value;
        // FIXME should need an event to update the HUD
    }

    #region Getters
    public int getMaxMana()
    {
        return (int) mana.currentMaxMana;
    }
    #endregion
}
