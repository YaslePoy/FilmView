using System.Linq;
using System.Windows.Input;
using Avalonia.Media;
using FilmView.Models;
using ReactiveUI;

namespace FilmView.ViewModels;

public class ContentCardViewModel : ViewModelBase
{
    public ContentCardViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    public IScreen HostScreen { get; }
    public string Name { get; set; }
    public string Format { get; set; }
    public string Type { get; set; }

    public Brush TypeColor => Type switch
    {
        "Фильм" => new SolidColorBrush(Colors.Chocolate),
        "Сериал" => new SolidColorBrush(Colors.DarkCyan)
    };

    public int Id { get; set; }

    public ICommand Goto => ReactiveCommand.Create(() =>
    {
        switch (Type)
        {
            case "Фильм":
                var filmVM = new FilmViewModel(HostScreen, FilmContext.Instance.Movies.First(i => i.Id == Id));
                HostScreen.Router.Navigate.Execute(filmVM);
                break;
            case "Сериал":
                var showVM = new SeriesViewModel(HostScreen, FilmContext.Instance.TVShows.First(i => i.Id == Id));
                HostScreen.Router.Navigate.Execute(showVM);
                break;
        }
    });
}