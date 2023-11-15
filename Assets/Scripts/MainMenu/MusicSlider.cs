using Michsky.MUIP;
using UnityEngine;
using UnityEngine.Audio;

namespace MainMenu
{
    public class MusicSlider : MonoBehaviour
    {
        [SerializeField] private SliderManager sliderManager;
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private string musicValueName;

        private void Start()
        {
            var musicValue = PlayerPrefs.GetInt(musicValueName, 0);
            musicGroup.audioMixer.SetFloat(musicValueName, musicValue);
            sliderManager.mainSlider.SetValueWithoutNotify(musicValue+80);
            sliderManager.UpdateUI();
        }
    
        public void SetValue()
        {
            musicGroup.audioMixer.SetFloat(musicValueName, sliderManager.mainSlider.value-80);
            PlayerPrefs.SetInt(musicValueName, (int)sliderManager.mainSlider.value-80);
        }
    }
}