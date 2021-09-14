using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(GetLocalPlayer))]
public class DilemmaMenu : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject HUD;
    private SceneInterface sceneInterface;

    /// <summary>
    /// Calls an event to register the local player and resizes the panel according to screen size.
    /// </summary>
    public void Awake()
    {
        // FIXME Do I need to know who the localPlayer is? maybe need to send the server our ID
        GetLocalPlayer getLocalPlayer = GetComponent<GetLocalPlayer>();
        getLocalPlayer.gotLocalPlayerEvent += Init;
        getLocalPlayer.playerInfo.requestDilemmaScreenEvent += SetUpDilemma;
    }
    private void OnDestroy() {
        GetLocalPlayer getLocalPlayer = GetComponent<GetLocalPlayer>();
         getLocalPlayer.playerInfo.requestDilemmaScreenEvent -= SetUpDilemma;
    }
    /// <summary>
    /// Sets up this class, after Player Controller has Started.
    /// </summary>
    public void Init(GameObject localPlayer)
    {
        sceneInterface = localPlayer.GetComponent<SceneInterface>();
    }

    public void SetUpDilemma()
    {
        HUD.SetActive(true);
        Cursor.visible = true;
    }

    /// <summary>
    /// Fight button.
    /// </summary>
    public void Fight()
    {
        sceneInterface.SetDecission(false);
        Cursor.visible = false;
    }

    /// <summary>
    /// Share button.
    /// </summary>
    public void Share()
    {
        sceneInterface.SetDecission(true);
        Cursor.visible = false;
    }
}