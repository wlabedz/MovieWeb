using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        SeedData();
    }

    public string ComputeHash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        string pass = ComputeHash("admin");
        var adminUser = new Login { Id = 1, Username = "admin", PasswordHash = pass };
        modelBuilder.Entity<Login>().HasData(adminUser);

        modelBuilder.Entity<Review>()
                .HasOne(r => r.Film)
                .WithMany(f => f.Reviews)
                .HasForeignKey(r => r.FilmId);

        modelBuilder.Entity<Film>()
                .HasOne(f => f.Director)
                .WithMany(r => r.Films)
                .HasForeignKey(f => f.DirectorId);

        modelBuilder.Entity<FilmActor>()
                .HasKey(fa => new{ fa.FilmId, fa.ActorId});
        
        modelBuilder.Entity<FilmActor>()
                .HasOne(fa => fa.Film)
                .WithMany(f => f.Actors)
                .HasForeignKey(fa => fa.FilmId);

        modelBuilder.Entity<FilmActor>()
                .HasOne(fa => fa.Actor)
                .WithMany(a => a.Films)
                .HasForeignKey(fa => fa.ActorId);


        modelBuilder.Entity<Film>()
                .HasOne(f => f.Genre)
                .WithMany(g => g.Films)
                .HasForeignKey(f => f.GenreId);

    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=mydatabase.db");

    public DbSet<Login> Logins { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Film> Films { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<FilmActor> FilmActors { get; set; }

    private void SeedData()
    {
        if (!Actors.Any() && !Reviews.Any() && !Genres.Any() && !Films.Any() && !Directors.Any())
        {
            var directors = new List<Director>(){
                new Director{ Name = "Espen", Surname = "Sandberg"},
                new Director{ Name = "Robert", Surname = "Zemeckis"}
            };
            Directors.AddRange(directors);
            SaveChanges();
            var genres = new List<Genre>(){
                new Genre{ Name = "action"},
                new Genre{ Name = "comedy-drama"}
            };
            Genres.AddRange(genres);
            
            SaveChanges();
            var JD = new Actor { Name = "Johnny", Surname = "Depp"};
            var TH = new Actor { Name = "Tom", Surname = "Hanks"};
            var actors = new List<Actor>{
                JD, TH
            };
            Actors.AddRange(actors);
            SaveChanges();
            var films = new List<Film>{
                new Film { Title = "Pirates of the Caribbean", DirectorId = 1, GenreId = 1, Actors = new List<FilmActor>()
                },
                new Film { Title = "Forrest Gump", DirectorId = 2, GenreId = 2 },
            };
            Films.AddRange(films);
            SaveChanges();
            
            var filmactors = new List<FilmActor>{
                new FilmActor{ ActorId = 1, FilmId = 1}
            };
            FilmActors.AddRange(filmactors);
            SaveChanges();

            var reviews = new List<Review>{
                new Review{ Text = "very good film", FilmId = 1, Rating = 5, username = "admin"},
                new Review{ Text = "very nice film", FilmId = 1, Rating = 4, username = "admin"},
                new Review{ Text = "very good film", FilmId = 2, Rating = 3, username = "admin"},
                new Review{ Text = "very nice film", FilmId = 2, Rating = 5, username = "admin"},
                new Review{ Text = "very bad film", FilmId = 2, Rating = 1, username = "admin"}
            };
            Reviews.AddRange(reviews);
            SaveChanges();

        }
    }

}

public class Film
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; }
    public int DirectorId { get; set; }
    public Director Director { get; set; }
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
    public List<FilmActor> Actors { get; set; }
    public List<Review> Reviews { get; set; }
}

public class Actor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public List<FilmActor> Films { get; set; }
}

public class Director
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public List<Film> Films { get; set; }
}

public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public string username { get; set; }
    public string Text { get; set; }
    public int Rating { get; set; }
    public int FilmId { get; set; }
    public Film Film {get; set;}
}

public class Genre
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public string Name { get; set; }
    public List<Film> Films { get; set; }
 }
   
public class FilmActor
{
    public int ActorId {get; set;}
    public Actor Actor{get; set;}
    public int FilmId {get; set;}
    public Film Film{get; set;}
}


public class Login
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    public string Username { get; set; }
    
    public string PasswordHash 
    { 
        get; set;
    }
}
