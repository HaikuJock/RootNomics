using Haiku.Audio;
using System;

namespace RootNomicsGame
{
    public class NullAudioPlayer : AudioPlaying
    {
        public string PlayingReplacementMusic => null;
        public void LoadSoundBank(string name) { }
        public void UnloadSoundBank(string name) { }
        public void SetListenerAttributes(
            float x, float y, float z,
            float speedX, float speedY, float speedZ,
            float unitForwardX, float unitForwardY, float unitForwardZ,
            float unitUpX, float unitUpY, float unitUpZ) { }
        public void Update() { }
        public void PlaySound(string name) { }
        public void Play3DSound(
            string name,
            float x, float y, float z,
            float speedX, float speedY, float speedZ,
            float unitForwardX, float unitForwardY, float unitForwardZ,
            float unitUpX, float unitUpY, float unitUpZ,
            float volume = 1f, float pitch = 1f) { }
        public IntPtr PlayLooped3DSound(
            string name,
            float x, float y, float z,
            float speedX, float speedY, float speedZ,
            float unitForwardX, float unitForwardY, float unitForwardZ,
            float unitUpX, float unitUpY, float unitUpZ)
        => IntPtr.Zero;
        public void OnExit() { }
        public IntPtr PlayLoopedSound(string name) => IntPtr.Zero;
        public void StopLoopedSound(IntPtr handle) { }
        public void StopAllSounds() { }
        public void SetBusVolume(string name, float portion) { }
        public void SetBusPitch(string name, float factor) { }
        public void SetInstanceVolume(IntPtr handle, float portion) { }
        public void SetGlobalParameter(string name, float value) { }
        public int GetSoundLength(string name) => 0;
    }
}
