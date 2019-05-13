using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Game_Kinect_Project
{
    public class AudioManager
    {
        AudioEngine ae;
        WaveBank wb;
        SoundBank sb;
        Cue dead, overworld, victory, open, GM2;
        public AudioManager()
        {
            ae = new AudioEngine("Content/kinect_game.xgs");
            wb = new WaveBank(ae, "Content/Wave Bank.xwb");
            sb = new SoundBank(ae, "Content/Sound Bank.xsb");        
        }

        public void PlayAudio(string name)
        {
            sb.PlayCue(name);           
        }

        public void PlayDeadBGM(string nameBGM) 
        {
            dead = sb.GetCue(nameBGM);
            if (!dead.IsPlaying)
                dead.Play();
        }

        public void StopDeadBGM(string nameBGM) 
        {
            dead = sb.GetCue(nameBGM);
            if (dead.IsPlaying)
                dead.Pause();
        }

        public void PlayOverworldBGM()
        {
            //if (overworld == null)
                overworld = sb.GetCue("Overworld_Super_Mario_Brothers_Tropical_Mix");
            overworld.Play();
        }

        public void StopOverworldBGM() 
        {
            overworld.Stop(AudioStopOptions.Immediate);
            //overworld = null;
        }

        public void PlayVictoryBGM()
        {
            //if (victory == null)
                victory = sb.GetCue("Super_Mario_Bros._-_Flag");
            if (!victory.IsPlaying)
            {
                victory.Play();
            }
        }

         public void StopVictoryBGM()
         {
             victory.Stop(AudioStopOptions.Immediate);
            // victory = null;
         }

         public void PlayOpenBGM()
         {
             if (open == null)
                 open = sb.GetCue("Kart_Circuit");
             if (!open.IsPlaying)
             {
                 open.Play();
             }
         }

         public void StopOpenBGM()
         {
             open.Stop(AudioStopOptions.Immediate);
             open = null;
         }

         public void PlayGM2BGM()
         {

             GM2 = sb.GetCue("Kalimba");
             if (!GM2.IsPlaying)
             {
                 GM2.Play();
             }
         }

         public void StopGM2BGM()
         {          
             GM2.Stop(AudioStopOptions.Immediate);            
         }
    }
}
