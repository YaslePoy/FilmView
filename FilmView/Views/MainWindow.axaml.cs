using Avalonia.Controls;
using FilmView.ViewModels;
using ReactiveUI.Avalonia;

namespace FilmView.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public static MainWindow? Instance { get; set; }
    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
    }
}