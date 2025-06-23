using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource audioSource;

    public AudioClip gameStartSound;
    public AudioClip moveSound;
    public AudioClip captureSound;
    public AudioClip checkSound;
    public AudioClip castleSound;
    public AudioClip promoteSound;
    public AudioClip gameOverSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayGameStartSound()
    {
        audioSource.PlayOneShot(gameStartSound);
    }

    public void PlayMoveSound()
    {
        audioSource.PlayOneShot(moveSound);
    }

    public void PlayCaptureSound()
    {
        audioSource.PlayOneShot(captureSound);
    }

    public void PlayCheckSound()
    {
        audioSource.PlayOneShot(checkSound);
    }
    public void PlayCastleSound()
    {
        audioSource.PlayOneShot(castleSound);
    }
    public void PlayPromoteSound()
    {
        audioSource.PlayOneShot(promoteSound);
    }

    public void PlayGameOverSound()
    {
        audioSource.PlayOneShot(gameOverSound);
    }
}
