using MauiMapAppDemo.ViewModels;
using MauiMapAppDemo.ViewModels.Messages;

namespace MauiMapAppDemo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            BindingContext = new ShellViewModel();
            InitializeComponent();
        }
    }
}
