using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GetLocalPlayer))]
public class GamepadUIController : MonoBehaviour
{
    private InputReader inputReader;

    public bool gamepadOn = false;
    private int IGM_index = 0;
    private int D_index = 0;
    private int PU_index = 0;
    private bool chill = false;

    [Header("Meters")]
    public States Cog;

    [Header("Start")]
    public GameObject StartButton;
    public MatchManager matchManager;

    [Header("In-game Menu")]
    public InGameMenu inGameMenu;
    public GameObject[] IGMHover = new GameObject[4];

    [Header("Dilemma Menu")]
    public DilemmaMenu dilemmaMenu;
    public GameObject[] DHover = new GameObject[2];

    [Header("Power Up Menu")]
    public PowerUpMenu powerUpMenu;
    public GameObject[] PUHover = new GameObject[3];

    [Header("Victory Screen")]
    public VictoryScreen victoryScreen;

    [Header("Sound")]
    public SFXManager sfx;
    public AudioSource navigate;

    /// <summary>
    /// Calls an event to register the local player and resizes the panel according to screen size.
    /// </summary>
    public void Awake()
    {
        inputReader = GetComponent<InputReader>();
        GetComponent<GetLocalPlayer>().gotLocalPlayerEvent += Init;
    }

    /// <summary>
    /// Sets up this class, after Player Controller has Started.
    /// </summary>
    public void Init(GameObject localPlayer)
    {
        inputReader.playerControls.Player.Dash.performed += _ => SelectButton();
        inputReader.playerControls.Player.ToggleCog.performed += _ => CogClick();
        inputReader.playerControls.Player.ToggleInGameMenu.performed += _ => ToggleInGameMenu();
        inputReader.playerControls.Player.StartGame.performed += _ => StartGame();
        inputReader.playerControls.Player.Move.performed += input => Navigate(input.ReadValue<Vector2>());
    }

    public void Start()
    {
        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            gamepadOn = true;

            PUHover[0].SetActive(true);
            IGMHover[0].SetActive(true);
            DHover[0].SetActive(true);
        }
    }

    public void Update()
    {
        var gamepad = Gamepad.current;

        if (gamepad != null)
            gamepadOn = true;
    }

    public void Navigate(Vector2 dir)
    {
        if (chill == false)
        {
            int direction = dir.y > 0 ? -1 : +1;
            if (dir.y != 0)
                Vertical_Navigate(direction);
        
            direction = dir.x > 0 ? +1 : -1;
            if (dir.x != 0)
                Horizontal_Navigate(direction);
            chill = true;
            Invoke("Chilled", 0.1f);
        }
    }

    private void Chilled()
    {
        chill = false;
    }

    private void Vertical_Navigate(int direction) // as in +1 is downwards and -1 is upwards
    {
        if (gamepadOn)
        {
            if (inGameMenu.canvas.activeInHierarchy)
            {
                navigate.Play();

                IGMHover[IGM_index].SetActive(false);
                
                IGM_index += direction;
                if (IGM_index >= IGMHover.Length)
                    IGM_index = 0;
                else if (IGM_index < 0)
                    IGM_index = IGMHover.Length - 1;
                
                IGMHover[IGM_index].SetActive(true);
            }
            else if (powerUpMenu.HUD.activeInHierarchy)
            {
                navigate.Play();

                PUHover[PU_index].SetActive(false);

                PU_index += direction;
                if (PU_index >= PUHover.Length)
                    PU_index = 0;
                else if (PU_index < 0)
                    PU_index = PUHover.Length - 1;

                PUHover[PU_index].SetActive(true);
            }
        }
    }

    private void Horizontal_Navigate(int direction) // as in +1 is rightwards and -1 is leftwards
    {
        if (gamepadOn)
        {
            if (dilemmaMenu.HUD.activeInHierarchy)
            {
                navigate.Play();

                DHover[D_index].SetActive(false);

                D_index += direction;
                if (D_index >= DHover.Length)
                    D_index = 0;
                else if (D_index < 0)
                    D_index = DHover.Length - 1;

                DHover[D_index].SetActive(true);
            }
        }
    }

    public void SelectButton()
    {
        if (inGameMenu.canvas.activeInHierarchy)
        {
            if (IGM_index == 0)
                ToggleInGameMenu();
            else if (IGM_index == 1) return/* right now is not interactible*/;
            else if (IGM_index == 2) return/* right now is not interactible*/;
            else
                Disconnect();
            
            IGM_index = 0;
            IGMHover[0].SetActive(true);
            IGMHover[1].SetActive(false);
            IGMHover[2].SetActive(false);
            IGMHover[3].SetActive(false);
        }
        else if (powerUpMenu.HUD.activeInHierarchy)
        {
            sfx.PlayClick();

            powerUpMenu.Choose_PowerUp(PU_index);
            PU_index = 0;
            PUHover[0].SetActive(true);
            PUHover[1].SetActive(false);
            PUHover[2].SetActive(false);
        }
        else if (dilemmaMenu.HUD.activeInHierarchy)
        {
            if (D_index == 0)
            {
                dilemmaMenu.Fight();
                dilemmaMenu.HUD.SetActive(false);
            }
            else
            {
                dilemmaMenu.Share();
                dilemmaMenu.HUD.SetActive(false);
            }
            D_index = 0;
            DHover[0].SetActive(true);
            DHover[1].SetActive(false);
        }
        else if (victoryScreen.HUD.activeInHierarchy)
        {
            Disconnect();
        }
    }

    public void CogClick() // left trigger?
    {
        sfx.PlayCog();
        Cog.Click();
    }

    public void StartGame() // start key
    {
        if (StartButton.activeInHierarchy)
        {
            StartButton.SetActive(false);
            matchManager.BeginMatch(); 
        }
    }

    private void ToggleInGameMenu() // select and resume button
    {
            inGameMenu.ToggleInGameMenu();
    }
    private void Disconnect()
    {
        IGM_index = 0;
        IGMHover[0].SetActive(true);
        IGMHover[1].SetActive(false);
        IGMHover[2].SetActive(false);
        IGMHover[3].SetActive(false);

        inGameMenu.Disconnect();
    }
}
