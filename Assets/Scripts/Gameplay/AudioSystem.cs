using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSystem : MonoBehaviour
{
    public AudioClip charSound;
    public bool IsPitchable = false;

    private AudioSource source;
    private AudioSource Source
    {
        get
        {
            if (source == null) source = GetComponent<AudioSource>();
            return source;
        }
    }

    public void ChangeCharSound(string charSoundName)
    {
        // sin cambios por ahora
    }

    public void PlayCharSound()
    {
        if (charSound == null) return;

        Source.Stop();
        Source.clip = charSound;
        if (!IsPitchable) Source.pitch = Random.Range(0.9f, 1.1f);
        Source.Play();
    }

    public void StopCharSound()
    {
        Source.Stop();
    }
}