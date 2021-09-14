using TMPro;
using UnityEngine;

[RequireComponent(typeof(GetLocalPlayer))]
public class Leaderboard : MonoBehaviour
{
    public GameObject HUD;
    public TMP_Text player_name;
    public PlayerColors playerColors;

    /// <summary>
    /// Calls an event to register the local player and resizes the panel according to screen size.
    /// </summary>
    public void Awake()
    {
        GetComponent<GetLocalPlayer>().playerInfo.gotNewTopPlayerEvent += SetNewLeader;
    }
    private void OnDestroy() {
         GetComponent<GetLocalPlayer>().playerInfo.gotNewTopPlayerEvent -= SetNewLeader;
    }
    /// <summary>
    /// Sets new leader on the UI; to be called any time there is a change in the leader.
    /// </summary>
    public void SetNewLeader(int playerId)
    {
        if(HUD == null)
            HUD = transform.GetChild(0).gameObject;
        HUD.SetActive(true);

        switch (playerId)
        {
            case 0:
                player_name.text = "P1";
                player_name.color = playerColors.colorList[0];
                break;
            case 1:
                player_name.text = "P2";
                player_name.color = playerColors.colorList[1];
                break;
            case 2:
                player_name.text = "P3";
                player_name.color = playerColors.colorList[2];
                break;
            case 3:
                player_name.text = "P4";
                player_name.color = playerColors.colorList[3];
                break;
            default:
                break;
        }
    }
}
