using UnityEngine;

namespace MainMenu
{
    public class ChangeSettingsStateUI : MonoBehaviour
    {
        [SerializeField] private ActiveElement[] otherUI;
        [SerializeField] private ActiveElement settingsUI;

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