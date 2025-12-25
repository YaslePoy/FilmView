using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FilmView.Models;

public class Movie : INotifyPropertyChanged
{
    private int _id;
    private string _title = null!;
    private string _description = null!;
    private DateTime _releaseDate;
    private string _videoUrl = null!;

    [Key]
    public int Id
    {
        get => _id;
        set
        {
            if (value == _id) return;
            _id = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (value == _description) return;
            _description = value;
            OnPropertyChanged();
        }
    }

    public DateTime ReleaseDate
    {
        get => _releaseDate;
        set
        {
            if (value.Equals(_releaseDate)) return;
            _releaseDate = value;
            OnPropertyChanged();
        }
    }

    public string VideoUrl
    {
        get => _videoUrl;
        set
        {
            if (value == _videoUrl) return;
            _videoUrl = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Cinema.Core/Models/TVShow.cs
public class TVShow : INotifyPropertyChanged
{
    private int _id;
    private string _title = null!;
    private string _description = null!;
    private int _releaseYear;
    private ICollection<Season> _seasons = new List<Season>();

    [Key]
    public int Id
    {
        get => _id;
        set
        {
            if (value == _id) return;
            _id = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (value == _description) return;
            _description = value;
            OnPropertyChanged();
        }
    }

    public int ReleaseYear
    {
        get => _releaseYear;
        set
        {
            if (value == _releaseYear) return;
            _releaseYear = value;
            OnPropertyChanged();
        }
    }

    public virtual ICollection<Season> Seasons
    {
        get => _seasons;
        set
        {
            if (value.SequenceEqual(_seasons)) return;
            _seasons = value;
            OnPropertyChanged();
        }
    }

    public void UpdateSeasons()
    {
        OnPropertyChanged(nameof(Seasons));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

// Cinema.Core/Models/Season.cs
public class Season : INotifyPropertyChanged
{
    private int _id;
    private int _number;
    private ICollection<Episode> _episodes = new List<Episode>();

    [Key]
    public int Id
    {
        get => _id;
        set
        {
            if (value == _id) return;
            _id = value;
            OnPropertyChanged();
        }
    }

    [ForeignKey("TVShow")] public int TVShowId { get; set; }

    public int Number
    {
        get => _number;
        set
        {
            if (value == _number) return;
            _number = value;
            OnPropertyChanged();
        }
    }

    public virtual TVShow TVShow { get; set; }

    public virtual ICollection<Episode> Episodes
    {
        get => _episodes;
        set
        {
            if (Equals(value, _episodes)) return;
            _episodes = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

// Cinema.Core/Models/Episode.cs
public class Episode : INotifyPropertyChanged
{
    private int _id;
    private int _seasonId;
    private string _title = "";
    private int _number;
    private string _videoUrl = null!;
    private Season _season = null!;

    [Key]
    public int Id
    {
        get => _id;
        set
        {
            if (value == _id) return;
            _id = value;
            OnPropertyChanged();
        }
    }

    [ForeignKey("Season")]
    public int SeasonId
    {
        get => _seasonId;
        set
        {
            if (value == _seasonId) return;
            _seasonId = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public int Number
    {
        get => _number;
        set
        {
            if (value == _number) return;
            _number = value;
            OnPropertyChanged();
        }
    }

    public string VideoUrl
    {
        get => _videoUrl;
        set
        {
            if (value == _videoUrl) return;
            _videoUrl = value;
            OnPropertyChanged();
        }
    }

    public virtual Season Season
    {
        get => _season;
        set
        {
            if (Equals(value, _season)) return;
            _season = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}