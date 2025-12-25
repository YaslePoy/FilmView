using Microsoft.EntityFrameworkCore;

namespace FilmView.Models;

public class FilmContext : DbContext
{
    public static FilmContext Instance { get; } = new();
    
    public DbSet<Movie> Movies { get; set; }
    public DbSet<TVShow> TVShows { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Episode> Episodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options
            .UseLazyLoadingProxies() // Включаем поддержку Lazy Loading
            .UseNpgsql("Host=micialware.ru;Database=film_db;Username=store_api_db;Password=gamestore");
    }
}