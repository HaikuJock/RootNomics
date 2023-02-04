using System;

namespace Haiku.Audio
{
    public interface AudioPlaying
    {
        void LoadSoundBank(string name);
        void UnloadSoundBank(string name);
        void SetListenerAttributes(
            float x, float y, float z,
            float speedX, float speedY, float speedZ,
            float unitForwardX, float unitForwardY, float unitForwardZ,
            float unitUpX, float unitUpY, float unitUpZ);
        void Update();
        void PlaySound(string name);
        void Play3DSound(
            string name, 
            float x, float y, float z,
            float speedX, float speedY, float speedZ,
            float unitForwardX, float unitForwardY, float unitForwardZ,
            float unitUpX, float unitUpY, float unitUpZ,
            float volume = 1f, float pitch = 1f);
        IntPtr PlayLooped3DSound(
            string name,
            float x, float y, float z,
            float speedX, float speedY, float speedZ,
            float unitForwardX, float unitForwardY, float unitForwardZ,
            float unitUpX, float unitUpY, float unitUpZ);

        void OnExit();
        IntPtr PlayLoopedSound(string name);
        void StopLoopedSound(IntPtr handle);
        void StopAllSounds();
        void SetBusVolume(string name, float portion);
        void SetBusPitch(string name, float factor);
        void SetInstanceVolume(IntPtr handle, float portion);
        void SetGlobalParameter(string name, float value);
        int GetSoundLength(string name);
    }
}
