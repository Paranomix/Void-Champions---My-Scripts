using Mirror;
using UnityEngine;

public class VictoryNetwork : NetworkBehaviour
{
    public VictoryScreen vs;

    [ClientRpc]
    public void GimmeYourInfoCall()
    {
        if (isLocalPlayer)
           GimmeYourInfo();
    }

    [Client]
    private void GimmeYourInfo()
    {
        vs.GimmeYourInfo();
    }

    [Command]
    public void AddSelfPlayer(int id, int level, int deaths, int rooms)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<VictoryNetwork>().AddPlayerCall(id, level, deaths, rooms);
        }
    }

    [ClientRpc]
    private void AddPlayerCall(int playerID, int level, int deaths, int rooms)
    {
        if (isLocalPlayer)
            AddPlayer(playerID, level, deaths, rooms);
    }


    [Client]
    private void AddPlayer(int playerID, int level, int deaths, int rooms)
    {
        vs.AddPlayer(playerID, level, deaths, rooms);

        vs.SetVictoryScreen(); // calling it all the time, maybe shouldn't
    }

    #region Force
    [Command]
    public void Force()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<VictoryNetwork>().GimmeYourInfoCall();
        }
    }

    #endregion
}
