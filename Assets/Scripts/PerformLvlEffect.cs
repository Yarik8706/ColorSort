using UnityEngine;

public class PerformLvlEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem lightEffect;
    [SerializeField] private ParticleSystem starsEffect;

    public void StartEffect()
    {
        lightEffect.Clear();
        starsEffect.Clear();
        lightEffect.Play();
        starsEffect.Play();
    }
}