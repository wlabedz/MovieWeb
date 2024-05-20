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
public IActionResult AddReview(string filmId, string rating, string text)
{

    if (HttpContext.Session.GetString("Logged") != "Yes")
    {
        return RedirectToAction("Welcome");
    }

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
    public IActionResult AddAct(string name, string surname)
    {

        if (HttpContext.Session.GetString("Logged") != "Yes" || HttpContext.Session.GetString("User") != "admin")
        {
            return RedirectToAction("Welcome");
        }

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
    public IActionResult Films()
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

    [HttpGet("/api/IO/Directors")]
    public IActionResult Directors()
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
    public IActionResult Actors()
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
    public IActionResult DeleteAct(int id)
    {

        if (HttpContext.Session.GetString("Logged") != "Yes" || HttpContext.Session.GetString("User") != "admin")
        {
            return RedirectToAction("Welcome");
        }

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
    public IActionResult AssAct(int FilmId, int ActorId)
    {

        if (HttpContext.Session.GetString("Logged") != "Yes" || HttpContext.Session.GetString("User") != "admin")
        {
            return RedirectToAction("Welcome");
        }
        
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

}
