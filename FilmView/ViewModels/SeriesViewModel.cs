using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using DynamicData;
using FilmView.Models;
using FilmView.Views;
using LibVLCSharp.Shared;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace FilmView.ViewModels;

public class SeriesViewModel : ViewModelBase, IRoutableViewModel
{
    private DateTimeOffset _releaseDate = DateTimeOffset.Now;
    private Season _currentSeason;
    private Episode? _currentEp;

    private List<Episode> _removedEpisodes = [];
    private List<Season> _removedSeasons = [];

    public SeriesViewModel(IScreen hostScreen, TVShow film)
    {
        HostScreen = hostScreen;
        Series = film;
        Seasons.AddRange(film.Seasons.ToList());
        // _ = LoadAsync();
    }

    // private async Task LoadAsync()
    // {
    //     if (Series.VideoUrl == null)
    //         return;
    //
    //     var videoStream = new Uri(Film.VideoUrl);
    //
    //     await Task.Delay(500);
    //     Dispatcher.UIThread.Invoke(() => { Player.Media = new Media(VLCInstance, videoStream); });
    // }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString();
    public IScreen HostScreen { get; }
    public TVShow Series { get; }

    public ICommand Load => ReactiveCommand.CreateFromTask(async () =>
    {
        try
        {
            var files = await TopLevel.GetTopLevel(MainWindow.Instance).StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    AllowMultiple = false,
                    FileTypeFilter =
                    [
                        new FilePickerFileType("Видео")
                            { MimeTypes = ["video/"], Patterns = ["*.mp4", "*.mkv", "*.avi"] }
                    ],
                    Title = "Выбор видео"
                });

            if (files.Count == 0)
            {
                return;
            }

            var video = files.First();
            Dispatcher.UIThread.Invoke(() => { Player.Media = new Media(FilmViewModel.VLCInstance, video.Path); });

            using var formData = new MultipartFormDataContent();
            await using var fileStream = File.OpenRead(video.Path.LocalPath);
            var fileContent = new StreamContent(fileStream);
            formData.Add(fileContent, "file", Path.GetFileName(video.Name));

            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:7790") };
            var response = await client.PostAsync("/api/video/upload", formData);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<VideoUploadResult>();
            CurrentEp.VideoUrl = result!.VideoUrl;
        }
        catch
        {
            _ = MessageBoxManager.GetMessageBoxStandard("Ошибка", "не получается загрузить видео", icon: Icon.Error)
                .ShowAsync();
        }
    }, this.WhenAnyValue(i => i.CurrentEp, (Episode season) => season != null));

    public MediaPlayer Player { get; } = new(FilmViewModel.VLCInstance);

    public ICommand Save => ReactiveCommand.Create(() =>
        {
            if (Series.Id == 0)
            {
                FilmContext.Instance.TVShows.Add(Series);
            }
            else
            {
                FilmContext.Instance.TVShows.Update(Series);
            }

            FilmContext.Instance.SaveChanges();

            foreach (var removedEpisode in _removedEpisodes)
            {
                if (removedEpisode.Id == 0)
                    return;

                FilmContext.Instance.Episodes.Remove(removedEpisode);
            }

            FilmContext.Instance.SaveChanges();

            foreach (var removedSeason in _removedSeasons)
            {
                if (removedSeason.Id == 0)
                    return;

                FilmContext.Instance.Seasons.Remove(removedSeason);
            }

            FilmContext.Instance.SaveChanges();

            foreach (var season in Series.Seasons)
            {
                if (season.Id == 0)
                {
                    FilmContext.Instance.Seasons.Add(season);
                }
                else
                {
                    FilmContext.Instance.Seasons.Update(season);
                }
            }

            FilmContext.Instance.SaveChanges();
            foreach (var episode in Series.Seasons.SelectMany(s => s.Episodes))
            {
                if (episode.Id == 0)
                {
                    FilmContext.Instance.Episodes.Add(episode);
                }
                else
                {
                    FilmContext.Instance.Episodes.Update(episode);
                }
            }

            FilmContext.Instance.SaveChanges();
        },
        this.WhenAnyValue(i => i.Series.Description, i => i.Series.Title,
            (s, s1) => !string.IsNullOrWhiteSpace(s) && !string.IsNullOrWhiteSpace(s1)));

    public DateTimeOffset ReleaseDate
    {
        get => _releaseDate;
        set
        {
            _releaseDate = value;
            Series.ReleaseYear = value.Year;
        }
    }

    public ICommand Back => ReactiveCommand.Create(() =>
    {
        HostScreen.Router.NavigateBack.Execute();
        Player.Stop();
    });

    public ICommand Play => ReactiveCommand.Create(() =>
    {
        if (Player.IsPlaying)
            Player.Pause();

        else
            Player.Play();
    });

    public Season? CurrentSeason
    {
        get => _currentSeason;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentSeason, value);
            CurrentSeries.Clear();
            if (value != null)
            {
                CurrentSeries.AddRange(value.Episodes.ToList());
            }
        }
    }

    public ObservableCollection<Season> Seasons { get; } = [];

    public ICommand AddSeason => ReactiveCommand.Create(() =>
    {
        var season = new Season() { Number = Seasons.Count + 1 };
        Seasons.Add(season);
        Series.Seasons.Add(season);
    });

    public ICommand RemoveSeason => ReactiveCommand.Create(() =>
    {
        _removedSeasons.Add(CurrentSeason);

        Seasons.Remove(CurrentSeason);
        Series.Seasons.Remove(CurrentSeason);
    }, this.WhenAnyValue(i => i.CurrentSeason, (Season season) => season != null));

    public ICommand AddEpisode => ReactiveCommand.Create(() =>
    {
        var episode = new Episode { Number = CurrentSeries.Count + 1 };
        CurrentSeries.Add(episode);
        CurrentSeason.Episodes.Add(episode);
    }, this.WhenAnyValue(i => i.CurrentSeason, (Season season) => season != null));

    public ICommand RemoveEpisode => ReactiveCommand.Create(() =>
        {
            _removedEpisodes.Add(CurrentEp);

            CurrentSeries.Remove(CurrentEp);
            CurrentSeason.Episodes.Remove(CurrentEp);
        },
        this.WhenAnyValue(i => i.CurrentEp, (Episode episode) => episode != null));

    public ObservableCollection<Episode> CurrentSeries { get; } = [];

    public Episode? CurrentEp
    {
        get => _currentEp;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentEp, value);

            if (value == null || value.VideoUrl == null)
                return;

            Player.Media = null;
            Player.Media = new Media(FilmViewModel.VLCInstance, new Uri(value.VideoUrl));
        }
    }
}