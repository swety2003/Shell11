using System.Windows.Input;

namespace Shell11.Common.Application.Contracts
{

    public interface IMenuItem
    {
        string Header { get; }

        ICommand Command { get; }

        bool Separator { get; }

        IList<IMenuItem> Items { get; }
    }

    public class MenuItemData : IMenuItem
    {
        public MenuItemData(string header, ICommand command = null)
        {
            Header = header;
            Command = command;
        }

        public string Header { get; private set; } = "header";

        public ICommand Command { get; private set; }

        public bool Separator => false;

        public IList<IMenuItem> Items { get; } = new List<IMenuItem>();
    }
    public class SeparatorData : IMenuItem
    {
        public bool Separator => true;

        public string Header => throw new NotImplementedException();

        public ICommand Command => throw new NotImplementedException();

        public IList<IMenuItem> Items => throw new NotImplementedException();
    }
}
