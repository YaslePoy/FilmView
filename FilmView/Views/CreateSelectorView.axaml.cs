using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FilmView.ViewModels;
using ReactiveUI.Avalonia;

namespace FilmView.Views;

public partial class CreateSelectorView : ReactiveUserControl<CreateSelectorViewModel>
{
    public CreateSelectorView()
    {
        InitializeComponent();
    }
}