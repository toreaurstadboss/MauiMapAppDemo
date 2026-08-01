using MauiMapAppDemo.ViewModels;

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
