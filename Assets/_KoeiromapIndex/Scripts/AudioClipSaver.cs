using System.IO;
using System.Text;
using UnityEngine;

namespace _KoeiromapIndex
{
    public static class AudioClipSaver
    {
        public static void SaveWavFile(AudioClip audioClip, string filePath)
        {
            var wavData = AudioClipToWavBytes(audioClip);
            File.WriteAllBytes(filePath, wavData);
        }

        private static byte[] AudioClipToWavBytes(AudioClip audioClip)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            // WAVヘッダー情報の書き込み
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(44 + (audioClip.samples * audioClip.channels * 2)); // ファイルサイズ
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // fmtチャンクのサイズ
            writer.Write((ushort)1); // フォーマットタグ (1 = PCM)
            writer.Write((ushort)audioClip.channels);
            writer.Write(audioClip.frequency);
            writer.Write(audioClip.frequency * audioClip.channels * 2); // バイトレート
            writer.Write((ushort)(audioClip.channels * 2)); // ブロックアライン
            writer.Write((ushort)16); // ビット深度
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(audioClip.samples * audioClip.channels * 2); // データチャンクのサイズ

            // オーディオデータの書き込み
            var samples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(samples, 0);
            foreach (var t in samples)
            {
                writer.Write((short)(t * short.MaxValue));
            }

            writer.Flush();
            var wavData = stream.ToArray();
            writer.Close();
            stream.Close();
            return wavData;
        }
    }
}