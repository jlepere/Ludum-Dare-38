using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip playerDash;

    private AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.name = "Sound Manager";
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayPlayerDash()
    {
        audioSource.clip = playerDash;
        audioSource.Play();
    }
}
