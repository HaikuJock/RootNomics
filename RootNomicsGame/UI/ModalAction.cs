using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.UI
{
    public class ModalAction
    {
        internal readonly string Title;
        internal readonly Action Action;
        internal readonly Action<string> TextAction;

        public ModalAction(string title)
            : this(title, () => { })
        {
        }

        public ModalAction(string title, Action action)
            : this(title, action, (_) => { })
        {
        }

        public ModalAction(string title, Action<string> textAction)
            : this(title, () => { }, textAction)
        {
        }

        public ModalAction(string title, Action action, Action<string> textAction)
        {
            Title = title;
            Action = action;
            TextAction = textAction;
        }
    }
}
