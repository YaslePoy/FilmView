using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FilmView.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace FilmView.Views;

public partial class FilmView : ReactiveUserControl<FilmViewModel>
{

    public ReactiveCommand<Unit, Unit> Full { get; set; }

    public FilmView()
    {
        Full = ReactiveCommand.Create(() =>
        {
            var player = VideoPlayer;

            if (PlayerHost.Children.Count == 1)
            {
                PlayerHost.Children.Remove(player);
                FullPlayerHost.Children.Add(player);
                MainWindow.Instance.Window.Background = new SolidColorBrush(Colors.Black);
                MainWindow.Instance.FVLogo.IsVisible = false;
            }
            else
            {
                FullPlayerHost.Children.Remove(player);
                PlayerHost.Children.Add(player);
                MainWindow.Instance.Window.Background = new SolidColorBrush(Colors.White);
                MainWindow.Instance.FVLogo.IsVisible = true;
                
            }
        });
        
        InitializeComponent();
        

    }
}