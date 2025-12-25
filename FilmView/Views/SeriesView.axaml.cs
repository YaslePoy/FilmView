using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FilmView.Models;
using FilmView.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace FilmView.Views;

public partial class SeriesView : ReactiveUserControl<SeriesViewModel>
{
    public SeriesView()
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

    public ICommand Full { get; }
}