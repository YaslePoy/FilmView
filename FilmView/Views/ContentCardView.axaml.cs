using Avalonia.Controls;
using Avalonia.Input;
using FilmView.ViewModels;

namespace FilmView.Views;

public partial class ContentCardView : UserControl
{
    public ContentCardView()
    {
        InitializeComponent();
    }

    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        (DataContext as ContentCardViewModel).Goto.Execute(null);
    }
}