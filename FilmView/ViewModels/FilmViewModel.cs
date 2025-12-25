using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using FilmView.Models;
using FilmView.Views;
using LibVLCSharp.Shared;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace FilmView.ViewModels;

public class FilmViewModel : ViewModelBase, IRoutableViewModel
{
    private DateTimeOffset _releaseDate = DateTimeOffset.Now;
    public static LibVLC VLCInstance { get; set; } = new LibVLC(enableDebugLogs: true);

    public FilmViewModel(IScreen hostScreen, Movie film)
    {
        HostScreen = hostScreen;
        Film = film;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        if (Film.VideoUrl == null)
            return;

        var videoStream = new Uri(Film.VideoUrl);

        await Task.Delay(500);
        Dispatcher.UIThread.Invoke(() => { Player.Media = new Media(VLCInstance, videoStream); });
    }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString();
    public IScreen HostScreen { get; }
    public Movie Film { get; }

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
                        new FilePickerFileType("Видео") { MimeTypes = ["video/"], Patterns = ["*.mp4", "*.mkv", "*.avi"] }
                    ],
                    Title = "Выбор видео"
                });

            if (files.Count == 0)
            {
                return;
            }

            var video = files.First();
            Dispatcher.UIThread.Invoke(() => { Player.Media = new Media(VLCInstance, video.Path); });

            using var formData = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(video.Path.LocalPath);
            var fileContent = new StreamContent(fileStream);
            formData.Add(fileContent, "file", Path.GetFileName(video.Name));

            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:7790") };
            var response = await client.PostAsync("/api/video/upload", formData);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<VideoUploadResult>();
            Film.VideoUrl = result!.VideoUrl;
        }
        catch
        {
            _ = MessageBoxManager.GetMessageBoxStandard("Ошибка", "не получается загрузить видео", icon: Icon.Error).ShowAsync();
        }
       
    });

    public MediaPlayer Player { get; } = new(VLCInstance);

    public ICommand Save => ReactiveCommand.Create(() =>
        {
            if (Film.Id == 0)
            {
                FilmContext.Instance.Movies.Add(Film);
            }
            else
            {
                FilmContext.Instance.Movies.Update(Film);
            }

            FilmContext.Instance.SaveChanges();
        },
        this.WhenAnyValue(i => i.Film.Description, i => i.Film.Title, i => i.Film.VideoUrl,
            (s, s1, s2) => !string.IsNullOrWhiteSpace(s) && !string.IsNullOrWhiteSpace(s1) &&
                           !string.IsNullOrWhiteSpace(s2)));

    public DateTimeOffset ReleaseDate
    {
        get => _releaseDate;
        set
        {
            _releaseDate = value;
            Film.ReleaseDate = value.UtcDateTime.Date;
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

    
    
}

    public record VideoUploadResult(string VideoUrl);
