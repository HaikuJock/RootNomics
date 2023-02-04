using Haiku.Audio;
using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using RootNomicsGame.Simulation;
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
        readonly Simulator simulator;

        public HUD(Rectangle frame, AudioPlaying audio, Simulator simulator) 
            : base(frame, audio)
        {
            this.simulator = simulator;
            var slidersFrame = new Rectangle(0, 0, frame.Width, Math.Min(500, (int)(frame.Height * 0.26667f)));
            agentCountSliders = new AgentCountSliders(slidersFrame, Configuration.AgentTypeNames, 9);

            AddChild(agentCountSliders);
            agentCountSliders.SetValues(Configuration.InitialAgentTypeCount);

            simulator.Initialize(Configuration.InitialAgentTypeCount);
        }
    }
}
