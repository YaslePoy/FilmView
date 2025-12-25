using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FilmView.Models;
using FilmView.ViewModels;
using Microsoft.EntityFrameworkCore;
using ReactiveUI.Avalonia;

namespace FilmView.Views;

public partial class MoviesView : ReactiveUserControl<MoviesViewModel>
{
    public MoviesView()
    {
        InitializeComponent();
        _ = LoadMoviesAsync();
    }

    private async Task LoadMoviesAsync()
    {
        List<ContentCardViewModel> contents = [];

        var films = await FilmContext.Instance.Movies.ToListAsync();
        var shows = await FilmContext.Instance.TVShows.ToListAsync();
        
        contents.AddRange(films.Select(i => new ContentCardViewModel(this.ViewModel!.HostScreen){ Name = i.Title, Type = "Фильм", Id = i.Id}));
        contents.AddRange(shows.Select(i => new ContentCardViewModel(this.ViewModel!.HostScreen){ Name = i.Title, Type = "Сериал", Id = i.Id}));
        
        Dispatcher.UIThread.Invoke(() =>
        {
            var built = contents.Select(i =>
            {
                var view = new ContentCardView
                {
                    DataContext = i
                };
                return view;
            }).ToArray();
            ContentPanel.Children.AddRange(built);
        });
    }
}