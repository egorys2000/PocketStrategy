using UnityEngine;
using UnityEngine.UI;

//Is responsible for screens switch, UI and camera managing
public class UIManagerScript : MonoBehaviour
{
    [SerializeField]
    private bool _party_running;

    [SerializeField]
    private CameraScript CameraControllScript;
    [SerializeField]
    private GameObject MainCamera;
    [SerializeField]
    private GameScreen _current_screen = GameScreen.MainMenu;

    public GameScreen CurrentScreen
    {
        get { return _current_screen; } 
        set
        {
            HowToPlayScreenUI.SetActive(false);
            PauseScreenUI.SetActive(false);
            WelcomeScreenUI.SetActive(false);
            PlayContentUI.SetActive(false);
            MapCreatorUI.SetActive(false);
            MainPlayContentUI.SetActive(false);

            if (value == GameScreen.Play)
            {
                PartyRunning = true;
                PlayContentUI.SetActive(true);
                MainPlayContentUI.SetActive(true);
            }

            if (value == GameScreen.MainMenu)
            {
                PartyRunning = false;
                WelcomeScreenUI.SetActive(true);
            }

            if (value == GameScreen.Pause)
            {
                PartyRunning = false;
                PlayContentUI.SetActive(true);
                MainPlayContentUI.SetActive(true);
                PauseScreenUI.SetActive(true);
            }

            if (value == GameScreen.MapCreator) 
            {
                MapCreatorUI.SetActive(true);
            }

            _current_screen = value;
        }
    }

    public enum GameScreen
    {
        MainMenu,
        Play,
        Pause,
        MapCreator
    }

    [SerializeField]
    private GameCoreManager GCManager;

    public bool PartyRunning
    {
        get => _party_running;
        private set
        {
            _party_running = value;
            CameraControllScript.SwitchCamera(value);
            GCManager.SwitchGameRoutine(value);
        }
    }

    public void PauseUnPauseGame() //reacts both pause and unpause
    {
        if (CurrentScreen == GameScreen.Play) CurrentScreen = GameScreen.Pause;
        else { if (CurrentScreen == GameScreen.Pause) CurrentScreen = GameScreen.Play; }
    }


    [SerializeField]
    private Map Map;

    [SerializeField]
    private GameObject WelcomeScreenUI, PauseScreenUI, HowToPlayScreenUI, PlayContentUI, 
        MapCreatorUI, MainPlayContentUI, TurnColorUI, UnitBuilderUI;

    public void BeginNewGame()
    {
        if (CurrentScreen == GameScreen.MapCreator)
        {            
            CurrentScreen = GameScreen.Play;

            var AnyVertex = Map.GenerateAndDrawMap();
            MainCamera.transform.position = new Vector3(AnyVertex.x, AnyVertex.y, MainCamera.transform.position.z);

            GCManager.StartTurnCounter();
        }

    }

    public void CreateMap() 
    {
        if (CurrentScreen == GameScreen.MainMenu)
        {
            CurrentScreen = GameScreen.MapCreator;
        }
    }

    public void ToMainMenu()
    {
        CurrentScreen = GameScreen.MainMenu;
    }

    [SerializeField]
    private Slider PlayersAmountSlider, MapSizeSlider, CellSidesSlider;

    private enum MapSize 
    {
        Small, Medium, Large
    }

    MapSize NewMapSize;

    void Awake() 
    {
        ChangeMapPreset();
    }

    public void ChangeTurnColor(Color color) 
    {
        TurnColorUI.GetComponent<Image>().color = color;
    }

    public void AppearUnitBuilder(bool on) 
    {
        UnitBuilderUI.SetActive(on);
    }

    public void ChangeMapPreset() 
    {
        NewMapSize = (MapSize)MapSizeSlider.value;

        int min_r = 0, max_r = 0, ult_rad = 0;
        if (NewMapSize == MapSize.Small) { min_r = 4; ult_rad = 8; }
        if (NewMapSize == MapSize.Medium) { min_r = 8; ult_rad = 16; }
        if (NewMapSize == MapSize.Large) { min_r = 12; ult_rad = 35; }

        max_r = 2 * min_r;

        Map.MinMapRadius = min_r;
        Map.MaxMapRadius = max_r;
        Map.UltimateMapRadius = ult_rad;

        Map.PlayersAmount = (int)PlayersAmountSlider.value;
        if (CellSidesSlider.value == 4) MapConstants.CellSides = 4;
        else MapConstants.CellSides = 6;
    }
}
