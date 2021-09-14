using UnityEngine;
using Mirror;

[RequireComponent(typeof(GetLocalPlayer))]
public class InGameMenu : MonoBehaviour
{
    public GameObject canvas;

    private PlayerControls playerControls;

    private NetworkManager networker; // fixme stop player input when this is on

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
        playerControls = localPlayer.GetComponent<PlayerControl>().inputReader.playerControls;
        playerControls.Player.ToggleInGameMenu.performed += _ => ToggleInGameMenu();
    }

    private void Start()
    {
        networker = NetworkManager.singleton;

        MusicManager.Instance.StopMusic("Title Screen");
        MusicManager.Instance.PlayMusic("Lobby");
    }

    public void ToggleInGameMenu()
    {
        Cursor.visible = !canvas.activeInHierarchy;
        canvas.SetActive(!canvas.activeInHierarchy);

        if (canvas.activeInHierarchy)
            playerControls.Disable();
        else
            playerControls.Enable();
    }

    /// <summary>
    /// Returns to main menu; disconnects and activates offline scene.
    /// </summary>
    public void Disconnect()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            networker.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            networker.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            networker.StopServer();
        }
    }
}
