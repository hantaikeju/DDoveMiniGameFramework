using DDoveFramework.Extension.DDoveAudio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Mono
{
    public sealed class ClickSound : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string soundName = "click";

        public void SetSoundName(string value)
        {
            soundName = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (string.IsNullOrEmpty(soundName))
            {
                return;
            }

            DDoveAudioKit.PlaySound(soundName);
        }
    }
}
