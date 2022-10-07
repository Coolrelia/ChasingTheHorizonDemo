using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance { get; private set; }

    [SerializeField] private AudioClip[] tracks = null;
    public AudioSource source = null;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(tracks[SceneManager.GetActiveScene().buildIndex]) {
            source.clip = tracks[SceneManager.GetActiveScene().buildIndex];
            FadeMusic(true);
        }
        else if(tracks[SceneManager.GetActiveScene().buildIndex] == null && SceneManager.GetActiveScene().buildIndex > 0)
        {
            GetComponent<AudioSource>().clip = null;
        }
    }

    public void PlayTrack(AudioClip track)
    {
        StartCoroutine(FadeVolume(track));        
    }
    public void PauseTrack()
    {
        source.Pause();
    }
    public void RestartTrack()
    {
        source.Stop();
        source.Play();
    }

    private IEnumerator FadeVolume(AudioClip track)
    {
        var currentVolume = source.volume;
        source.volume = 0.05f;

        source.clip = track;
        source.Play();

        source.volume += 0.05f;

        while(source.volume != currentVolume)
        {
            source.volume += 0.1f;
            yield return new WaitForSeconds(0.2f);
        }
        StopCoroutine(FadeVolume(null));
        yield break;
    }

    public void FadeMusic(bool fadeIn)
    {
        StartCoroutine(MusicFade(fadeIn));
    }
    private IEnumerator MusicFade(bool fadeIn)
    {
        if(fadeIn)
        {
            var targetVolume = source.volume;
            source.volume = 0.05f;

            source.Play();

            source.volume += 0.05f;

            while(source.volume != targetVolume)
            {
                source.volume += 0.1f;
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }
        else
        {
            while (source.volume != 0)
            {
                source.volume -= 0.1f;
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
