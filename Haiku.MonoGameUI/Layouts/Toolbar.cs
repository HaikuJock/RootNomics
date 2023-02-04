using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.Layouts
{
    public class Toolbar : ButtonGroup
    {
        public Toolbar(Rectangle frame, LayoutStrategy layoutStrategy)
            : base(frame, layoutStrategy)
        {
        }

        public void Add(GroupButton newButton)
        {
            AddChild(newButton);
        }

        public ToolbarButton SelectSubToolbarButton(int tool)
        {
            foreach (var button in ToolbarButtons())
            {
                if (button.SelectSubToolbarButton(tool))
                {
                    return button;
                }
            }
            return null;
        }

        public List<ToolbarButton> ToolbarButtons()
        {
            return Children.Select(child => child).OfType<ToolbarButton>().ToList();
        }

        internal int ButtonCount()
        {
            return Children.Count((child) => child is GroupButton);
        }

        protected override bool OnCursorMove(Point point, Rectangle container)
        {
            base.OnCursorMove(point, container);
            return true;
        }
    }
}
