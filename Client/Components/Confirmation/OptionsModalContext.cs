using System;

namespace Quibble.Client.Components.Confirmation
{
    public class OptionsModalContext<TValue>
    {
        private Action<TValue?> ChooseAction { get; }

        public OptionsModalContext(Action<TValue?> chooseAction)
        {
            ChooseAction = chooseAction;
        }

        public void Choose(TValue? value) => ChooseAction(value);
    }
}
