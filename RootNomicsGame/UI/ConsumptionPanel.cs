using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.UI
{
    internal class ConsumptionPanel : Panel
    {
        //Label potionAvailable;
        //Label potionsForPlants;
        //Label maxPotionsForPlants;
        //OrdinalSlider potionsForPlantsSlider;
        //Label potionsForPlants;
        //Label maxPotionsForPlants;
        //OrdinalSlider potionsForPlantsSlider;

        public ConsumptionPanel(Rectangle frame)
            : base(frame, new LinearLayoutStrategy(Orientation.Vertical, 8, 16))
        {

        }
    }
}
