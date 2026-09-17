using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public AudioClip charSound;
    public void ChangeCharSound(string charSoundName)
    {
        
    }
    public void PlayCharSound()
    {
        gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f); 
        gameObject.GetComponent<AudioSource>().PlayOneShot(charSound, 0.3f); 
    }
}
