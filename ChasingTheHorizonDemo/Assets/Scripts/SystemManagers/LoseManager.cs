using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoseManager : MonoBehaviour
{
    public static LoseManager instance { get; private set; }

    [SerializeField] private GameObject systemManager = null;
    [SerializeField] private GameObject combatReadout = null;
    [SerializeField] private GameObject restartButton = null;
    [SerializeField] private GameObject fadeOut = null;
    [Header("Check this box if you want the player to lose when all ally units die")]
    public bool allAllies = false;
    [Header("Check this box if you want the player to lose when any ally unit dies")]
    public bool anyAlly = false;
    [Header("Check this box if you want the player to lose when a specific ally unit dies")]
    public bool specificAlly = false;
    [Header("Drop the specific allies who's deaths will trigger a loss")]
    public List<UnitLoader> specificAllies = new List<UnitLoader>();
    private bool gameOver = false;
    private CursorController cursor = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cursor = FindObjectOfType<CursorController>();
    }
    private void Update()
    {
        if(!gameOver){
            if(allAllies){
                if(TurnManager.instance.allyUnits.Count <= 0 && !combatReadout.activeSelf){
                    StartCoroutine(GameOver());
                    gameOver = true;
                }
            }            
        }
    }

    public void RestartMap()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        MusicPlayer.instance.RestartTrack();
    }
    public void TitleScreen()
    {
        MusicPlayer.instance.PauseTrack();
        SceneManager.LoadScene("StartScreen");
    }
    public void GiveUp()
    {
        Application.Quit();
    }


    public void StartGameOver()
    {
        StartCoroutine(GameOver());
    }
    private IEnumerator GameOver()
    {
        cursor.SetState(new MenuState(cursor));
        cursor.cursorControls.SwitchCurrentActionMap("UI");
        systemManager.SetActive(false);
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
        yield return null;
    }
}
