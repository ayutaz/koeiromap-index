using System;
using NaughtyAttributes;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _KoeiromapIndex
{
    public class InputDataView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputText;
        [SerializeField] private Button createVoiceButton;
        [SerializeField] private Button openSaveFolderButton;
        [SerializeField] [BoxGroup("xValue")] private Slider xVoiceValue;
        [SerializeField] [BoxGroup("xValue")] private TMP_InputField xVoiceValueText;
        [SerializeField] [BoxGroup("yValue")] private Slider yVoiceValue;
        [SerializeField] [BoxGroup("yValue")] private TMP_InputField yVoiceValueText;
        private inputDataSouce.InputData _inputData;

        private void Start()
        {
            xVoiceValue.OnValueChangedAsObservable().Subscribe(x => xVoiceValueText.text = x.ToString("F2"));
            yVoiceValue.OnValueChangedAsObservable().Subscribe(y => yVoiceValueText.text = y.ToString("F2"));

            xVoiceValueText.onValueChanged.AddListener(x =>
            {
                if (string.IsNullOrEmpty(x)) return;
                xVoiceValue.value = float.Parse(x);
            });

            yVoiceValueText.onValueChanged.AddListener(y =>
            {
                if (string.IsNullOrEmpty(y)) return;
                yVoiceValue.value = float.Parse(y);
            });
        }

        public IObservable<inputDataSouce.InputData> OnClickCreateVoiceAsObservable()
        {
            _inputData = new inputDataSouce.InputData
            {
                inputTextData = inputText.text,
                xValue = xVoiceValue.value,
                yValue = yVoiceValue.value
            };
            return createVoiceButton.OnClickAsObservable().Select(_ => _inputData);
        }

        public IObservable<Unit> OnClickOpenSaveFolderAsObservable()
        {
            return openSaveFolderButton.OnClickAsObservable();
        }
    }
}