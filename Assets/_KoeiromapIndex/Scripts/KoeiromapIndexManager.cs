using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using KoeiromapUnity.Core;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace _KoeiromapIndex
{
    public class KoeiromapIndexManager : MonoBehaviour
    {
        [SerializeField] private InputDataView inputDataView;
        [SerializeField] [ReadOnly] private List<OutVoice> outVoices;

        [SerializeField] private Transform content;

        [SerializeField] [Min(0)] [Range(1, 30)]
        private int voiceOutCount = 10;

        [SerializeField] private OutVoice outVoicePrefab;
        private AudioSource _audioSource;
        private string _savePath;
        private CancellationToken _token;

        private void Awake()
        {
            _savePath = Application.persistentDataPath + "/Koeiromap";
            _token = this.GetCancellationTokenOnDestroy();
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            //画面の作成
            for (var i = 0; i < voiceOutCount; i++)
            {
                var outVoice = Instantiate(outVoicePrefab, content);
                outVoices.Add(outVoice);
                outVoice.gameObject.SetActive(false);
            }

            if (!Directory.Exists(_savePath)) Directory.CreateDirectory(_savePath);
            inputDataView.OnClickCreateVoiceAsObservable().Subscribe(async inputData =>
            {
                foreach (var (outVoice, index) in outVoices.Select((info, index) => (info, index)))
                {
                    var voiceParam = new VoiceParam
                    {
                        text = inputData.inputTextData,
                        speaker_x = inputData.isLockXValue ? inputData.xValue : Random.Range(-3f, 3f),
                        speaker_y = inputData.isLockYValue ? inputData.yValue : Random.Range(-3f, 3f),
                        style = "talk",
                        seed = inputData.isLockSeed
                            ? inputData.seed.ToString()
                            : Random.Range(-10000, 100000).ToString()
                    };
                    var audioClip = await Koeiromap.GetVoice(voiceParam, _token);
                    outVoice.SetText(voiceParam.seed, voiceParam.speaker_x, voiceParam.speaker_y);
                    outVoice.AudioClip = audioClip.audioClip;
                    outVoices[index].gameObject.SetActive(true);
                    await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: _token); //APIの連続アクセスを防ぐ

                    outVoice.OnClickAsObservable().Subscribe(_ =>
                    {
                        _audioSource.clip = audioClip.audioClip;
                        _audioSource.Play();
                    }).AddTo(this);

                    outVoice.OnSaveAsObservable().Subscribe(_ =>
                    {
                        var savePath = Path.Combine(_savePath, outVoice.SaveName);
                        Debug.Log($"save path:{savePath}");
                        AudioClipSaver.SaveWavFile(audioClip.audioClip, savePath);
                    }).AddTo(this);
                }
            }).AddTo(this);

            inputDataView.OnClickOpenSaveFolderAsObservable().Subscribe(_ => { OpenFolder(_savePath); }).AddTo(this);
        }

        private static void OpenFolder(string path)
        {
            Process.Start(path);
        }
    }
}