using UnityEngine;

namespace MainMenu
{
    public class ChangeSettingsStateUI : MonoBehaviour
    {
        [SerializeField] private ActiveElement[] otherUI;
        [SerializeField] private ActiveElement settingsUI;

        private void Start()
        {
            var settingsUIpos = settingsUI.transform.position;
            settingsUI.ChangeActive(false).onComplete = () =>
            {
                settingsUI.gameObject.SetActive(true);
                settingsUI.ActivePosition = settingsUIpos;
            };
        }

        public void SetState(bool state)
        {
            foreach (var other in otherUI)
            {
                other.ChangeActive(!state);
            }
            settingsUI.ChangeActiveAndSetNotActivePosition(state);
        }
    }
}