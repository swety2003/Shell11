using AppGrabber;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Shell11.ViewModels
{
    public partial class AppGrabberSettingsViewModel : ObservableObject
    {
        private readonly IAppGrabber appGrabber;

        public IList<ApplicationInfo> ProgramList => appGrabber.ProgramList;
        public CategoryList CategoryList => appGrabber.CategoryList;

        [ObservableProperty]
        private Category selectedCategory;

        public AppGrabberSettingsViewModel(IAppGrabber appGrabber)
        {
            this.appGrabber = appGrabber;

            selectedCategory = CategoryList.FirstOrDefault();
        }
    }
}