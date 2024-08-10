using Shell11.Common.Application.Contracts;
using System.Windows;
using System.Windows.Controls;

namespace Shell11.Common.Selectors
{
    public class MenuItemContainerTemplateSelector : ItemContainerTemplateSelector
    {
        public DataTemplate MenuItemTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, ItemsControl container)
        {
            if (item is IMenuItem menuItem)
            {
                return menuItem.Separator ? SeparatorTemplate : MenuItemTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }



}
