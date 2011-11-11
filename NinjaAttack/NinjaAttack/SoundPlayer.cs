using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace NinjaAttack
{
    public class SoundPlayer
    {
        private WaveBank waveBank;
        private AudioEngine audioEngine;
        private SoundBank soundBank;

        public SoundPlayer()
        {
            audioEngine = new AudioEngine("Content/FruitNinja.xgs");
            waveBank = new WaveBank(audioEngine, "Content/effects.xwb");
            soundBank = new SoundBank(audioEngine, "Content/effects.xsb");
        }

        public void Shutdown()
        {
            waveBank.Dispose();
            soundBank.Dispose();
            audioEngine.Dispose();
        }

        public void PlaySound(string sound)
        {
            soundBank.PlayCue(sound);            
        }
    }
}
