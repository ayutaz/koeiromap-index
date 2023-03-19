using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _KoeiromapIndex.Scripts
{
    public class VoiceUtil
    {
        private const string APIEndPoint = "https://api.rinna.co.jp/models/cttse/koeiro";

        public static async UniTask<AudioClip> GetVoiceAudioClip(VoiceParam inputVoiceParam)
        {
            var voiceData = await GetVoiceData(inputVoiceParam);
            var base64Data = VoiceBase64Data(voiceData);
            var fileName =
                $"{inputVoiceParam.style}_{inputVoiceParam.speaker_x}_{inputVoiceParam.speaker_y}_{inputVoiceParam.seed}.wav";
            return await ConvertBase64ToAudioClip(base64Data, fileName);
        }

        private static async UniTask<AudioClip> ConvertBase64ToAudioClip(string base64EncodedMp3String, string fileName)
        {
            var audioBytes = Convert.FromBase64String(base64EncodedMp3String);
            var tempPath = Application.persistentDataPath + fileName;
            await File.WriteAllBytesAsync(tempPath, audioBytes);
            var request = UnityWebRequestMultimedia.GetAudioClip(tempPath, AudioType.WAV);
            var asyncOperation = request.SendWebRequest();
            await asyncOperation;
            if (request.result.Equals(UnityWebRequest.Result.ConnectionError))
            {
                Debug.LogError(request.error);
                return null;
            }

            var content = DownloadHandlerAudioClip.GetContent(request);
            request.Dispose();
            return content;
        }

        private static string VoiceBase64Data(VoiceResponse voiceResponse)
        {
            var audio = voiceResponse.audio;
            return audio[(audio.IndexOf(",", StringComparison.Ordinal) + 1)..];
        }

        private static async UniTask<VoiceResponse> GetVoiceData(VoiceParam inputVoiceParam)
        {
            var voiceParam = new VoiceParam
            {
                text = inputVoiceParam.text,
                speaker_x = inputVoiceParam.speaker_x,
                speaker_y = inputVoiceParam.speaker_y,
                style = inputVoiceParam.style,
                seed = inputVoiceParam.seed
            };
            var json = JsonUtility.ToJson(voiceParam);
            var bodyRaw = Encoding.UTF8.GetBytes(json);
            var request = new UnityWebRequest(APIEndPoint, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();
            return JsonUtility.FromJson<VoiceResponse>(request.downloadHandler.text);
        }
    }
}