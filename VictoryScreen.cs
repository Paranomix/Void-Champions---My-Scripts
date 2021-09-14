using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(GetLocalPlayer))]
/// <summary>
/// Add the players to the manager first, then reorder them and then update the UI.
/// </summary>
public class VictoryScreen : MonoBehaviour
{ // FIXME colour may have changed
    [Header("UI Objects")]
    public GameObject HUD;
    public GameObject[] medals;
    public TMP_Text[] colours;
    public TMP_Text[] levels;
    public TMP_Text[] deaths;
    public TMP_Text[] rooms;
    public PlayerColors playerColors;

    #region Player class
    public class Player
    {
        public int playerID;
        public int level;
        public int deaths;
        public int rooms;
    }
    #endregion

    private List<Player> players = new List<Player>();
    private int local_id;

    [Header("Player's Info")]
    private Stats stats;
    private VictoryNetwork network;


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
        local_id = localPlayer.GetComponent<PlayerControl>().playerID;
        stats = localPlayer.GetComponent<Stats>();
        network = localPlayer.GetComponent<VictoryNetwork>();
        network.vs = this;
    }

    /// <summary>
    /// Sent by game manager to all clients (not related to player scripts).
    /// </summary>
    public void GimmeYourInfo()
    {
        network.AddSelfPlayer(local_id, stats.level, stats.number_of_deaths, stats.number_of_rooms_completed);
    }

    /// <summary>
    /// To add players to the Victory Screen Manager. Server sends this to all.
    /// </summary>
    /// <param name="level">Level of the player.</param>
    /// <param name="deaths">Number of deaths of the player.</param>
    /// <param name="rooms">Number of rooms completed by the player.</param>
    public void AddPlayer(int playerID, int level, int deaths, int rooms)
    {
        Player new_player = new Player()
        {
            playerID = playerID,
            level = level,
            deaths = deaths,
            rooms = rooms
        };
        players.Add(new_player);
    }

    /// <summary>
    /// Reorders the players according to their stats and then updates the UI.
    /// Also turns on the HUD.
    /// </summary>
    public void SetVictoryScreen()
    {
        HUD.SetActive(true);
        Cursor.visible = true;

        players.Sort(delegate (Player x, Player y)
        {
            // FIXME in case winner is in the same level as the 2nd, order depends on stats...
            if (x.level > y.level) return -1;
            else if (x.level < y.level) return 1;
            else if (x.deaths < y.deaths) return -1;
            else if (x.deaths > y.deaths) return 1;
            else if (x.rooms > y.rooms) return -1;
            else if (x.rooms < y.rooms) return 1;
            else return 0;
        });
        Update_UI();
    }

    /// <summary>
    /// Updates a Power Up slot on the menu.
    /// </summary>
    private void Update_UI()
    {
        int number_of_players = players.Count;
        for (int i = 0; i < number_of_players; i++)
        {
            colours[i].text = "P"+(players[i].playerID+1);
            colours[i].color = SetColour(players[i].playerID);
            levels[i].text = players[i].level.ToString();
            deaths[i].text = players[i].deaths.ToString();
            rooms[i].text = players[i].rooms.ToString();
            if (local_id == i)
            {
                colours[i].fontStyle = FontStyles.Bold;
            }
        }
        for (int i = 0; i < 4 - number_of_players; i++)
        {
            if (number_of_players + i < 3)
                medals[number_of_players + i].SetActive(false);
            colours[number_of_players + i].text = null;
            levels[number_of_players + i].text = null;
            deaths[number_of_players + i].text = null;
            rooms[number_of_players + i].text = null;
        }
    }

    private Color SetColour(int id)
    {
        return playerColors.colorList[id];
    }

    #region Force
    public void Force()
    {
        network.Force();
    }
    #endregion
}
