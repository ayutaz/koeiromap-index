using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _KoeiromapIndex.Scripts
{
    public class Presenter : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputText;
        [SerializeField] private Button createVoiceButton;
        [SerializeField] private Button openSaveFolderButton;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            createVoiceButton.OnClickAsObservable().Subscribe(async _ =>
            {
                var voiceParam = new VoiceParam()
                {
                    text = inputText.text,
                    speaker_x = 0.5f,
                    speaker_y = 0.5f,
                    style = "talk",
                    seed = Random.Range(-999999, 999999).ToString()
                };
                var audioClip = await VoiceUtil.GetVoiceAudioClip(voiceParam);
                _audioSource.clip = audioClip;
                _audioSource.Play();
            }).AddTo(this);
        }
    }
}