using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

//Manages the Start Screen
//Need to create an options menu
//Eventually there will be an option to load a save or start a new game
public class StartManager : MonoBehaviour
{
    [SerializeField] private Animator animator = null;

    [SerializeField] private GameObject volumeButton = null;

    [SerializeField] private Button startButton = null;
    [SerializeField] private GameObject optionsMenu = null;
    [SerializeField] private GameObject allButtons = null;

    [SerializeField] private GameObject normalMode = null;
    [SerializeField] private GameObject hardMode = null;

    [SerializeField] private Slider musicSlider = null;
    [SerializeField] private Slider sfxSlider = null;

    [SerializeField] private AudioSource musicVolume = null;
    [SerializeField] private AudioSource sfxVolume = null;

    private void Start()
    {
        StartCoroutine(HighlightButton(startButton.gameObject));
    }

    private void Update()
    {        
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }

        if(optionsMenu.activeSelf)
        {
            musicVolume.volume = musicSlider.value;            
            sfxVolume.volume = sfxSlider.value;
        }
    }

    //BUTTONS
    public void Play()
    {
        normalMode.SetActive(true);
        hardMode.SetActive(true);
        StartCoroutine(Highlight(normalMode));
    }
    public void Options()
    {
        if(!optionsMenu.activeSelf){
            optionsMenu.SetActive(true);
            SetupOptions();
            StartCoroutine(HighlightButton(volumeButton));
        }
        else{
            optionsMenu.SetActive(false);
        }
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void NormalMode()
    {
        PlayerPrefs.SetString("Difficulty", "Normal");
        SceneManager.LoadScene("Cutscene 1");
    }

    public void HardMode()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        SceneManager.LoadScene("Cutscene 1");
    }

    private void SetupOptions()
    {
        musicSlider.value = musicVolume.volume;
        sfxSlider.value = sfxVolume.volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private IEnumerator HighlightButton(GameObject button)
    {
        animator.SetTrigger("Enter");

        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForSeconds(0.05f);
        EventSystem.current.SetSelectedGameObject(button);
    }
    private IEnumerator Highlight(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForSeconds(0.05f);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
