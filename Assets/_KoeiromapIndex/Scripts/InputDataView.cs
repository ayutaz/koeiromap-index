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
        [SerializeField] [BoxGroup("xValue")] private Button isLockXValueButton;
        [SerializeField] [BoxGroup("yValue")] private Slider yVoiceValue;
        [SerializeField] [BoxGroup("yValue")] private TMP_InputField yVoiceValueText;
        [SerializeField] [BoxGroup("yValue")] private Button isLockYValueButton;
        [SerializeField] [BoxGroup("seed")] private TMP_InputField seedInput;
        [SerializeField] [BoxGroup("seed")] private Button isLockSeedButton;

        [SerializeField] [BoxGroup("lockIconImage")]
        private Sprite lockIconImage;

        [SerializeField] [BoxGroup("lockIconImage")]
        private Sprite unlockIconImage;

        private inputDataSouce.InputData _inputData;

        private void Start()
        {
            SetInitData();
            OnObserveXValue();
            OnObserveYValue();
            OnObserveSeed();
        }

        private void SetInitData()
        {
            _inputData = new inputDataSouce.InputData
            {
                inputTextData = inputText.text,
                xValue = 0f,
                yValue = 0f,
                seed = int.Parse(seedInput.text),
                isLockSeed = false,
                isLockXValue = false,
                isLockYValue = false
            };
            (_inputData.isLockXValue, isLockXValueButton.image.sprite) =
                ChangeLockState(_inputData.isLockXValue, isLockXValueButton);
            (_inputData.isLockYValue, isLockYValueButton.image.sprite) =
                ChangeLockState(_inputData.isLockYValue, isLockYValueButton);

            (_inputData.isLockSeed, isLockSeedButton.image.sprite) =
                ChangeLockState(_inputData.isLockSeed, isLockSeedButton);
        }

        private void OnObserveXValue()
        {
            xVoiceValue.OnValueChangedAsObservable().Subscribe(x =>
            {
                xVoiceValueText.text = x.ToString("F2");
                _inputData.xValue = x;
            });


            xVoiceValueText.onValueChanged.AddListener(x =>
            {
                if (string.IsNullOrEmpty(x)) return;
                xVoiceValue.value = float.Parse(x);
                _inputData.xValue = float.Parse(x);
            });

            isLockXValueButton.OnClickAsObservable().Subscribe(_ =>
            {
                (_inputData.isLockXValue, isLockXValueButton.image.sprite) =
                    ChangeLockState(_inputData.isLockXValue, isLockXValueButton);
            }).AddTo(this);
        }

        private void OnObserveYValue()
        {
            yVoiceValue.OnValueChangedAsObservable().Subscribe(y =>
            {
                yVoiceValueText.text = y.ToString("F2");
                _inputData.yValue = y;
            });

            yVoiceValueText.onValueChanged.AddListener(y =>
            {
                if (string.IsNullOrEmpty(y)) return;
                yVoiceValue.value = float.Parse(y);
                _inputData.yValue = float.Parse(y);
            });

            isLockYValueButton.OnClickAsObservable().Subscribe(_ =>
            {
                (_inputData.isLockYValue, isLockYValueButton.image.sprite) =
                    ChangeLockState(_inputData.isLockYValue, isLockYValueButton);
            }).AddTo(this);
        }

        private void OnObserveSeed()
        {
            seedInput.onValueChanged.AddListener(seed =>
            {
                if (string.IsNullOrEmpty(seed)) return;
                _inputData.seed = int.Parse(seed);
            });
            isLockSeedButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    (_inputData.isLockSeed, isLockSeedButton.image.sprite) =
                        ChangeLockState(_inputData.isLockSeed, isLockSeedButton);
                }).AddTo(this);
        }

        public IObservable<inputDataSouce.InputData> OnClickCreateVoiceAsObservable()
        {
            return createVoiceButton.OnClickAsObservable().Select(_ => _inputData);
        }

        public IObservable<Unit> OnClickOpenSaveFolderAsObservable()
        {
            return openSaveFolderButton.OnClickAsObservable();
        }

        private (bool isLockState, Sprite lockImage) ChangeLockState(bool isLockState, Selectable button)
        {
            var lockImage = !isLockState ? lockIconImage : unlockIconImage;
            return (!isLockState, lockImage);
        }
    }
}