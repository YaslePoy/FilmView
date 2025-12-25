using System;
using System.Windows.Input;
using FilmView.Models;
using ReactiveUI;

namespace FilmView.ViewModels;

public class CreateSelectorViewModel : ViewModelBase, IRoutableViewModel
{
    public CreateSelectorViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString();
    public IScreen HostScreen { get; }

    public ICommand GotoFilm => ReactiveCommand.Create((() =>
    {
        HostScreen.Router.Navigate.Execute(new FilmViewModel(HostScreen, new Movie()));
    }));

    public ICommand GotoShow => ReactiveCommand.Create((() =>
    {
        HostScreen.Router.Navigate.Execute(new SeriesViewModel(HostScreen, new TVShow()));
    }));

    public ICommand Back => ReactiveCommand.Create((() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            }
        ));
}