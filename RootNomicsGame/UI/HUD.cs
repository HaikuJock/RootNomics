using Haiku.Audio;
using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.UI
{
    internal class HUD : Window
    {
        AgentCountSliders agentCountSliders;

        public HUD(Rectangle frame, AudioPlaying audio) 
            : base(frame, audio)
        {
            var slidersFrame = new Rectangle(0, 0, frame.Width, Math.Min(500, (int)(frame.Height * 0.26667f)));
            agentCountSliders = new AgentCountSliders(slidersFrame, Configuration.AgentTypeNames, 9);

            AddChild(agentCountSliders);
        }
    }
}
