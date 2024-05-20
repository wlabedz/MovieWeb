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
        if (HttpContext.Session.GetString("Logged") == "Yes")
        {
            var actors = _context.Actors.ToList();
            var reviews = _context.Reviews.ToList();
            var genres = _context.Genres.ToList();
            var films = _context.Films.ToList();
            var directors = _context.Directors.ToList();

            var data = new HomeViewModel{
                Actors = actors,
                Reviews = reviews,
                Genres = genres,   
                Films = films,
                Directors = directors
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

    [HttpPost("/api/IO/review")]
public IActionResult AddReview(string filmId, string rating, string text)
{

    if (HttpContext.Session.GetString("Logged") != "Yes")
    {
        return RedirectToAction("other");
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

        return RedirectToAction("other");
    }
    else
    {
        return RedirectToAction("AddRev");
    }
}
    [HttpGet("api/IO/RevAdd")]
    public IActionResult ReviewAdded()
    {
        return View();
    }

}
