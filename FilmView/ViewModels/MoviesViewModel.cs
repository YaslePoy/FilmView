using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using FilmView.Models;
using ReactiveUI;

namespace FilmView.ViewModels;

// CinemaApp/ViewModels/MoviesViewModel.cs
public partial class MoviesViewModel : ViewModelBase, IRoutableViewModel
{
    
    private ObservableCollection<Movie> _movies = new();

    public MoviesViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        LoadMovies();
    }

    private void LoadMovies()
    {
    }
    

    // private async Task<string> UploadVideoAsync(string filePath)
    // {
    //     using var formData = new MultipartFormDataContent();
    //     using var fileStream = File.OpenRead(filePath);
    //     var fileContent = new StreamContent(fileStream);
    //     formData.Add(fileContent, "file", Path.GetFileName(filePath));
    //
    //     var response = await _httpClient.PostAsync("/api/video/upload", formData);
    //     response.EnsureSuccessStatusCode();
    //     
    //     var result = await response.Content.ReadFromJsonAsync<VideoUploadResult>();
    //     return result!.VideoUrl;
    // }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString();
    public IScreen HostScreen { get; }
    public ICommand Create => ReactiveCommand.Create((() =>
    {
        HostScreen.Router.Navigate.Execute(new CreateSelectorViewModel(HostScreen));
    }));
}