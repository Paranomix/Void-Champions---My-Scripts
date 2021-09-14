using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Health))]
public class Stats : MonoBehaviour
{
    public int level = 1;
    public int number_of_deaths = 0;
    public int number_of_rooms_completed = 0;
    [HideInInspector] public UnityAction UpdatedStatsEvent;

    /// <summary>
    /// Sets up this class, after Player Controller has Started.
    /// </summary>
    public void Start()
    {
        // FIXME should we be subscribing a death event? maybe a respawn event!
        GetComponent<Health>().deathEvent += ResetStats;
    }

    /// <summary>
    /// Run when player dies.
    /// </summary>
    public void ResetStats()
    {
        level = 1;
        number_of_deaths += 1;
        if (UpdatedStatsEvent != null)
            UpdatedStatsEvent.Invoke();
        else
            Debug.Log("This is bulshit if statement! If it's printing, then it's wrong. Might have later consequences, because the code wasn't thought to have an empty event (it makes no sense to be empty). Cause is most likely turning off the MetersHUD.");
    }

    /// <summary>
    /// Run when player enters a new room.
    /// </summary>
    public void NextRoom()
    {
        level += 1;
        number_of_rooms_completed += 1;

        UpdatedStatsEvent.Invoke();
    }

    /// <summary>
    /// Update when Boss power up is chosen.
    /// </summary>
    public void BossUpdate()
    {
        level += 1;

        UpdatedStatsEvent.Invoke();
    }
}
