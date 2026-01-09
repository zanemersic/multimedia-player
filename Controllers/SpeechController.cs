using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vosk;
using MultimedijskiPredvajalnik.Core;

namespace MultimedijskiPredvajalnik.Controllers
{
    public class SpeechController
    {
        private readonly VoskRecognizer recognizer;
        private readonly WaveInEvent waveIn;

        public Action<VoiceCommand>? SpeechRecognized;

        public SpeechController()
        {
            var model = new Model(@"C:\vosk-model-en-us-0.22");
            recognizer = new VoskRecognizer(model, 16000);

            waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(16000, 1),
                BufferMilliseconds = 1000
            };

            waveIn.DataAvailable += OnAudio;
        }

        public void Start() => waveIn.StartRecording();
        public void Stop() => waveIn.StopRecording();

        private void OnAudio(object? sender, WaveInEventArgs e)
        {
            if (!recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                return;

            var json = recognizer.Result();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("text", out var textProp))
                return;

            var rawText = textProp.GetString();
            if (string.IsNullOrEmpty(rawText)) 
                return;

            var command = ResolveCommand(rawText);
            if (command == VoiceCommand.Unknown)
                return;

            SpeechRecognized?.Invoke(command);
        }

        private static VoiceCommand ResolveCommand(string text)
        {
            text = text.ToLowerInvariant().Trim();

            Debug.WriteLine($"[VOICE] {text}");

            if (text.Contains("play"))
                return VoiceCommand.Play;

            if (text.Contains("stop"))
                return VoiceCommand.Stop;

            if (text.Contains("next"))
                return VoiceCommand.Next;

            if (text.Contains("previous"))
                return VoiceCommand.Previous;

            if (text.Contains("select"))
                return VoiceCommand.Select;

            if (text.Contains("remove"))
                return VoiceCommand.Remove;

            if (text.Contains("add"))
                return VoiceCommand.Add;

            if (text.Contains("edit"))
                return VoiceCommand.Edit;

            if (text.Contains("exit"))
                return VoiceCommand.Exit;

            return VoiceCommand.Unknown;
        }
    }
}
