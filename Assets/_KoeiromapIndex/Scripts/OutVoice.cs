using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _KoeiromapIndex
{
    public class OutVoice : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI seedText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Button playVoiceButton;
        [SerializeField] private Button saveVoiceButton;
        public AudioClip AudioClip { get; set; }
        public string SaveName { get; private set; }

        private void Awake()
        {
#if UNITY_WEBGL || UNITY_EDITOR
            saveVoiceButton.gameObject.SetActive(false);
#endif
        }

        public IObservable<Unit> OnClickAsObservable()
        {
            return playVoiceButton.OnClickAsObservable();
        }

        public IObservable<Unit> OnSaveAsObservable()

        {
            return saveVoiceButton.OnClickAsObservable();
        }

        public void SetText(string seed, float x, float y)
        {
            seedText.text = "seed:" + seed;
            valueText.text = $"x:{x:F2},y:{y:F2}";
            SaveName = $"{seed}_{x}_{y}.wav";
        }
    }
}