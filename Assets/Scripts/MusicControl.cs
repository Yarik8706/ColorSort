using UnityEngine;

public class MusicControl : MonoBehaviour
{
    private static MusicControl _instance;
        
    private void Start()
    {
        if(_instance == this) return;
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        GetComponent<AudioSource>().Play();
        DontDestroyOnLoad(gameObject);
    }
}