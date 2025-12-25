using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace FilmView.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public MainWindowViewModel()
    {
        Router = new RoutingState();
        Router.Navigate.Execute(new MoviesViewModel(this));
    }

    public RoutingState Router { get; }
}