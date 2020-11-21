using Microsoft.Xna.Framework.Audio;

namespace Stellaris.Music
{
    public abstract class MusicStreaming : IMusic
    {
        protected string path;
        protected DynamicSoundEffectInstance instance;
        public SoundState State => instance.State;
        public MusicStreaming(string path)
        {
            this.path = path;
        }
        public virtual void Load()
        {

        }
        public void Play()
        {
            instance.Play();
        }
        public void Pause()
        {
            instance.Pause();
        }
        public void Stop()
        {
            instance.Stop();
        }
    }
    public interface IMusic
    {
        public SoundState State { get; }
        public void Play()
        {

        }
        public void Pause()
        {

        }
        public void Stop()
        {

        }
    }
}
