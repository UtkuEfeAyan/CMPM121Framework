using UnityEngine;
using System.Collections;


// Music Manager that plays different music based on selected difficulty.
// Attach this to a GameObject with an AudioSource.
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;

    [Header("Music Tracks")]
    public AudioClip easyMusic;    // Music for Easy difficulty
    public AudioClip mediumMusic;  // Music for Medium difficulty
    public AudioClip endlessMusic; // Music for Endless difficulty

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("MusicManager: No AudioSource found!");
        }
    }

    // Plays music based on the selected difficulty name.

    //The difficulty level ("Easy", "Medium", "Endless").
    public void PlayMusicForDifficulty(string difficulty)
    {
        if (audioSource == null)
            return;

        AudioClip selectedClip = null;

        switch (difficulty.ToLower())
        {
            case "easy":
                selectedClip = easyMusic;
                break;
            case "medium":
                selectedClip = mediumMusic;
                break;
            case "endless":
                selectedClip = endlessMusic;
                break;
            default:
                Debug.LogWarning($"MusicManager: Unknown difficulty '{difficulty}'.");
                break;
        }

        if (selectedClip != null)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeInMusic(selectedClip, 7f)); // 7 seconds fade-in
        }
    }
    //create a smoother beginig for the music
    private IEnumerator FadeInMusic(AudioClip clip, float duration)
    {
        audioSource.Stop();          // Stop any previous music safely
        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();

        float elapsed = 0f;
        float targetVolume = 0.7f; //Set maximum volume here (0.7 = 70% volume)
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Clamp01(elapsed / duration) * targetVolume;
            yield return null;
        }
        audioSource.volume = targetVolume; // Ensure full volume at end
    }

    // Stops the currently playing music.
    // posibly redunded after implementing inside teh fade in but could be usefull so still here
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
