using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ProjektLogowanie.Models;
using SQLitePCL;

namespace ProjektLogowanie.Controllers;

[Route("api/[controller]")]
public class IO : Controller
{
    private readonly AppDbContext _context;

    public IO(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Welcome()
        {
            return View();
        }

    [HttpPost("/api/IO/Login")]
    public IActionResult Login(string login, string password)
    {
        string ComputeHash(string input)
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
        if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
        {
            string hashed_password = ComputeHash(password);
            var user = _context.Logins.FirstOrDefault(u => u.Username == login && u.PasswordHash == hashed_password);
            if (user != null)
            {
                HttpContext.Session.SetString("Logged", "Yes");
                HttpContext.Session.SetString("User",login);
                return RedirectToAction("Logged");
            }
            else
            {
                ViewBag.WrongLogin = "Wrong data";
                return View();
            }
        }
        return View();
    }

    [HttpGet("/api/IO/Logout")]
    public IActionResult LogOut()
    {
        HttpContext.Session.Remove("Logged");
        HttpContext.Session.Remove("User");
        return RedirectToAction("Welcome");
    }

    [HttpGet("/api/IO/Logged")]
    public IActionResult Logged()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes")
        {
            return View();
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpGet("/api/IO/Other")]
    public IActionResult other()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/Register")]
    public IActionResult Register(string username, string password)
    {
        string ComputeHash(string input)
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
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
        var existingUser = _context.Logins.FirstOrDefault(u => u.Username == username);
        if (existingUser != null)
        {
            Console.WriteLine("exists");
            ViewBag.ErrorMessage = "Username already exists. Please choose a different one.";
            return View();
        }

        string hashed_password = ComputeHash(password);
        var newUser = new Login{Username =  username, PasswordHash = hashed_password};
        _context.Logins.Add(newUser);
        _context.SaveChanges();
        HttpContext.Session.SetString("Logged","Yes");
        HttpContext.Session.SetString("User",username);
        return RedirectToAction("Welcome");
        }
        else
        {
         Console.WriteLine("not  exists but");
        ViewBag.ErrorMessage = "Please provide both username and password.";
        return View();
        }
    }

    [HttpGet("/api/IO/admin")]
    public IActionResult adm()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var users = _context.Logins.ToList();
            return View(users);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/AddUser")]
     [ValidateAntiForgeryToken]
    public IActionResult AddUser(string username, string password)
    {
        string ComputeHash(string input)
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
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            var existingUser = _context.Logins.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Username already exists. Please choose a different one.";
                return View("adm", _context.Logins.ToList());
            }

            string hashed_password = ComputeHash(password);
            var newUser = new Login { Username = username, PasswordHash = hashed_password };
            _context.Logins.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("adm");
        }
        else
        {
            ViewBag.ErrorMessage = "Please provide both username and password.";
            return View("adm", _context.Logins.ToList());
        }
    }

    [HttpGet("api/IO/addreview")]
    public IActionResult AddRev()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes")
        {
            var viewModel = _context.Films.ToList();
            return View(viewModel);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpGet("api/IO/addactor")]
    public IActionResult AddActor()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/review")]
    [ValidateAntiForgeryToken]
public IActionResult AddReview(string filmId, string rating, string text)
{

    if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(filmId) && !string.IsNullOrEmpty(rating))
    {
        var review = new Review
        {
            FilmId = int.Parse(filmId),
            Rating = int.Parse(rating),
            Text = text,
            username = HttpContext.Session.GetString("User")
        };

        _context.Reviews.Add(review);
        _context.SaveChanges();

        return RedirectToAction("ReviewAdded");
    }
    else
    {
        return RedirectToAction("AddRev");
    }
}

    [HttpPost("/api/IO/act")]
     [ValidateAntiForgeryToken]
    public IActionResult AddAct(string name, string surname)
    {

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname))
        {
            bool exists = _context.Actors.Any(a => a.Name == name && a.Surname == surname);

            if(!exists){
                var actor = new Actor
                {
                    Name = name,
                    Surname = surname,
                    Films = new List<FilmActor>()
                };

                _context.Actors.Add(actor);
                _context.SaveChanges();

                return RedirectToAction("ActorsSuccess");
            }
            else{
                return RedirectToAction("ActorsFailed");
            }
        }
        else
        {
            return RedirectToAction("ActorsFailed");
        }
    }

    [HttpGet("api/IO/ActorsSuccess")]
    public IActionResult ActorsSuccess()
    {
        return View();
    }

    [HttpGet("api/IO/ActorsFailed")]
    public IActionResult ActorsFailed()
    {
        return View();
    }

    [HttpGet("api/IO/RevAdd")]
    public IActionResult ReviewAdded()
    {
        return View();
    }
    
    [HttpGet("api/IO/film/{id}")]
    public IActionResult FilmDetails(int id)
    {
        if (HttpContext.Session.GetString("Logged") != "Yes")
        {
            return RedirectToAction("Welcome");
        }

        var film = _context.Films
            .FirstOrDefault(f => f.Id == id);

        if (film == null)
        {
            return NotFound();
        }

        var reviews = _context.Reviews.Where(r => r.FilmId == id).ToList();
        var filmactors = _context.FilmActors.ToList();
        var films = new List<Film>();
        var genres = _context.Genres.ToList();
        var directors = _context.Directors.ToList();
        films.Add(film);
        var actors = _context.Actors.ToList();
        var viewModel = new HomeViewModel
        {
            Actors = actors,   
            Films = films,
            Reviews = reviews,
            FilmActors = filmactors,
            Directors = directors,
            Genres = genres
        };

        return View(viewModel);
    }

    [HttpGet("/api/IO/Films")]
public IActionResult Films(string sortOrder, string searchString, string selectedGenre)
{
    if (HttpContext.Session.GetString("Logged") == "Yes")
    {
        var films = from f in _context.Films
                    select f;

        if (!String.IsNullOrEmpty(searchString))
        {
            films = films.Where(f => f.Title.Contains(searchString));
        }

        if (!String.IsNullOrEmpty(selectedGenre))
        {
            films = films.Where(f => f.Genre.Name == selectedGenre);
        }
        
        var filmWithReviews = films.Select(f => new
        {
            Film = f,
            AverageRating = f.Reviews.Any() ? f.Reviews.Average(r => r.Rating) : 0
        });

        switch (sortOrder)
        {
            case "title":
                filmWithReviews = filmWithReviews.OrderBy(f => f.Film.Title);
                break;
            case "title_desc":
                filmWithReviews = filmWithReviews.OrderByDescending(f => f.Film.Title);
                break;
            case "director":
                filmWithReviews = filmWithReviews.OrderBy(f => f.Film.Director.Name);
                break;
            case "director_desc":
                filmWithReviews = filmWithReviews.OrderByDescending(f => f.Film.Director.Name);
                break;
            case "review":
                filmWithReviews = filmWithReviews.OrderBy(f => f.AverageRating);
                break;
            case "review_desc":
                filmWithReviews = filmWithReviews.OrderByDescending(f => f.AverageRating);
                break;
            default:
                filmWithReviews = filmWithReviews.OrderBy(f => f.Film.Title);
                break;
        }

        var orderedFilms = filmWithReviews.Select(f => f.Film).ToList();

        var data = new HomeViewModel
        {
            Films = orderedFilms,
            Directors = _context.Directors.ToList(),
            Reviews = _context.Reviews.ToList(),
            Actors = _context.Actors.ToList(),
            Genres = _context.Genres.ToList(),
            FilmActors = _context.FilmActors.ToList(),
            SearchString = searchString,
            SelectedGenre = selectedGenre,
            SortOrder = sortOrder
        };

        return View(data);
    }
    else
    {
        return RedirectToAction("Welcome");
    }
    }

    [HttpGet("/api/IO/Genres")]
    public IActionResult Genres()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };
            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpGet("/api/IO/Actors")]
    public IActionResult Actors(string searchString)
    {
        if (HttpContext.Session.GetString("Logged") == "Yes")
        {

            var actors = _context.Actors.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                string[] names = searchString.Split(' ');

                actors = actors.Where(f => f.Name.Contains(names[0]) || f.Surname.Contains(names[0]));

                if (names.Length > 1){
                    actors = actors.Where(f => f.Name.Contains(names[names.Length-1]) || f.Surname.Contains(names[names.Length-1]));
                }
            }
            var directors = _context.Directors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors.ToList(),
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };
            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpGet("/api/IO/Directors")]
    public IActionResult Directors(string searchString)
    {
        if (HttpContext.Session.GetString("Logged") == "Yes")
        {

            var directors = _context.Directors.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                string[] names = searchString.Split(' ');

                directors = directors.Where(f => f.Name.Contains(names[0]) || f.Surname.Contains(names[0]));

                if (names.Length > 1){
                    directors = directors.Where(f => f.Name.Contains(names[names.Length-1]) || f.Surname.Contains(names[names.Length-1]));
                }
            }
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors.ToList(),
                FilmActors = filmactors
            };
            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpGet("api/IO/delactor")]
    public IActionResult DeleteActor()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/delAct")]
     [ValidateAntiForgeryToken]
    public IActionResult DeleteAct(int id)
    {

        var actor = _context.Actors.FirstOrDefault(a => a.Id == id);
        if (actor == null){
            return RedirectToAction("DelFailed");
        }

        _context.Actors.Remove(actor);
        _context.SaveChanges();
        return RedirectToAction("DelSucc");
    }

    [HttpGet("api/IO/delSuc")]
    public IActionResult DelSucc()
    {
        return View();
    }

    [HttpGet("api/IO/delFail")]
    public IActionResult DelFailed()
    {
        return View();
    }

    [HttpGet("api/IO/assActor")]
    public IActionResult AssignActor()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/assAct")]
     [ValidateAntiForgeryToken]
    public IActionResult AssAct(int FilmId, int ActorId)
    {

        bool exists = _context.FilmActors.Any(a => a.FilmId == FilmId && a.ActorId == ActorId);

        if(!exists){
                var filmact = new FilmActor
                {
                    FilmId = FilmId,
                    ActorId = ActorId
                };

                _context.FilmActors.Add(filmact);
                _context.SaveChanges();

                return RedirectToAction("AssignSucc");
        }

        return RedirectToAction("AssignFailed");
    }


    [HttpGet("api/IO/AssignSucc")]
    public IActionResult AssignSucc()
    {
        return View();
    }

    [HttpGet("api/IO/AssignFailed")]
    public IActionResult AssignFailed()
    {
        return View();
    }

    [HttpGet("api/IO/addfilm")]
    public IActionResult AddFilm()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/fi")]
     [ValidateAntiForgeryToken]
    public IActionResult AddFi(string title, int dirId, int genId)
    {

        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(dirId.ToString()) && !string.IsNullOrEmpty(genId.ToString()))
        {
            bool exists = _context.Films.Any(a => a.Title == title && a.DirectorId == dirId);

            if(!exists){
                var film = new Film
                {
                    Title = title,
                    GenreId = genId,
                    DirectorId = dirId
                };

                _context.Films.Add(film);
                _context.SaveChanges();

                return RedirectToAction("FilmsSuccess");
            }
            else{
                return RedirectToAction("FilmsFailed");
            }
        }
        else
        {
            return RedirectToAction("FilmsFailed");
        }
    }

    [HttpGet("api/IO/FilmsSuccess")]
    public IActionResult FilmsSuccess()
    {
        return View();
    }

    [HttpGet("api/IO/FilmsFailed")]
    public IActionResult FilmsFailed()
    {
        return View();
    }

    [HttpGet("api/IO/delfilm")]
    public IActionResult DeleteFilm()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/delfi")]
     [ValidateAntiForgeryToken]
    public IActionResult DeleteFi(int id)
    {

        var film = _context.Films.FirstOrDefault(a => a.Id == id);
        if (film == null){
            return RedirectToAction("DelFilmFailed");
        }

        _context.Films.Remove(film);
        _context.SaveChanges();
        return RedirectToAction("DelFilmSucc");
    }

    [HttpGet("api/IO/DelFilmSucc")]
    public IActionResult DelFilmSucc()
    {
        return View();
    }

    [HttpGet("api/IO/DelFilmFailed")]
    public IActionResult DelFilmFailed()
    {
        return View();
    }


    [HttpGet("api/IO/deldir")]
    public IActionResult DeleteDirector()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/deldi")]
     [ValidateAntiForgeryToken]
    public IActionResult DeleteDi(int id)
    {

        var director = _context.Directors.FirstOrDefault(a => a.Id == id);
        if (director == null){
            return RedirectToAction("DelDirectorFailed");
        }

        _context.Directors.Remove(director);
        _context.SaveChanges();
        return RedirectToAction("DelDirSucc");
    }

    [HttpGet("api/IO/DelDirSucc")]
    public IActionResult DelDirSucc()
    {
        return View();
    }

    [HttpGet("api/IO/DelDirectorFailed")]
    public IActionResult DelDirectorFailed()
    {
        return View();
    }


    [HttpGet("api/IO/adddirector")]
    public IActionResult AddDirector()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/adddi")]
     [ValidateAntiForgeryToken]
    public IActionResult AddDi(string name, string surname)
    {

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname))
        {
            bool exists = _context.Directors.Any(a => a.Name == name && a.Surname == surname);

            if(!exists){
                var director = new Director
                {
                    Name = name,
                    Surname = surname
                };

                _context.Directors.Add(director);
                _context.SaveChanges();

                return RedirectToAction("DirectorsSuccess");
            }
            else{
                return RedirectToAction("DirectorsFailed");
            }
        }
        else
        {
            return RedirectToAction("DirectorsFailed");
        }
    }

    [HttpGet("api/IO/dirsucc")]
    public IActionResult DirectorsSuccess()
    {
        return View();
    }

    [HttpGet("api/IO/dirfailed")]
    public IActionResult DirectorsFailed()
    {
        return View();
    }


    [HttpGet("api/IO/delrev")]
    public IActionResult DelRev()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/delre")]
     [ValidateAntiForgeryToken]
    public IActionResult DelRe(int id)
    {

        var review = _context.Reviews.FirstOrDefault(a => a.Id == id);
        if (review == null){
            return RedirectToAction("other");
        }

        _context.Reviews.Remove(review);
        _context.SaveChanges();
        return RedirectToAction("other");
    }

    [HttpGet("api/IO/addgenre")]
    public IActionResult AddGenre()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/addgen")]
    [ValidateAntiForgeryToken]
    public IActionResult AddGe(string name)
    {

        if (!string.IsNullOrEmpty(name))
        {
            bool exists = _context.Genres.Any(a => a.Name == name);

            if(!exists){
                var genre = new Genre
                {
                    Name = name
                };

                _context.Genres.Add(genre);
                _context.SaveChanges();

                return RedirectToAction("other");
            }
            else{
                return RedirectToAction("other");
            }
        }
        else
        {
            return RedirectToAction("other");
        }
    }
    [HttpGet("api/IO/delgenr")]
    public IActionResult DelGenre()
    {
        if (HttpContext.Session.GetString("Logged") == "Yes" && HttpContext.Session.GetString("User") == "admin")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();
            var filmactors = _context.FilmActors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors,
                FilmActors = filmactors
            };

            return View(data);
        }
        else
        {
            return RedirectToAction("Welcome");
        }
    }

    [HttpPost("/api/IO/delg")]
    [ValidateAntiForgeryToken]
    public IActionResult DelGen(int id)
    {

        var genre = _context.Genres.FirstOrDefault(a => a.Id == id);
        if (genre == null){
            return RedirectToAction("other");
        }

        _context.Genres.Remove(genre);
        _context.SaveChanges();
        return RedirectToAction("other");
    }
}
