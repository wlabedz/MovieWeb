# MovieWeb

MovieWeb to aplikacja internetowa do przeglądania i zarządzania kolekcją filmów. Aplikacja jest zbudowana przy użyciu ASP.NET Core MVC i oferuje funkcje takie jak przeglądanie szczegółów filmów, wyszukiwanie filmów oraz zarządzanie uwierzytelnianiem użytkowników.

# Autorzy
Wojciech Łabędź oraz Urszula Kostuch
## Funkcje

- **Wyszukiwanie filmów**: Przeglądanie dostępnych filmów w aplikacji.
- **Filtrowanie wyników**: Filtrowanie wyświetlanych wyników, np. możliwość wyszukania filmów w których gra wybrany aktor.
- **Uwierzytelnianie użytkowników**: Rejestracja i logowanie użytkownika.
- **Zarządzanie filmami**: Administratorzy mogą dodawać, edytować i usuwać filmy.
- **Zarządzanie urzytkownikami** Administrator może dodawać, edytować i usuwać użytkowników.
- **Dodawanie komentarzy** Użytkownicy mają możliwość dodawania komentarzy.
- **Wystawianie oceny** Użytkownicy przy komentarzu mogą wystawić ocenę, jest ona brana pod uwagę przy liczeniu średniej oceny wszystkich użytkowników, która jest wyświetlana na stronie.

## Struktura projektu
- **Controllers/: Zawiera kontrolery MVC odpowiedzialne za obsługę danych wejściowych użytkownika i zwracanie odpowiedzi.**
- **Models/: Zawiera modele danych używane przez aplikację.**
- **Views/: Zawiera widoki Razor dla interfejsu użytkownika aplikacji.**
- **wwwroot/: Zawiera pliki statyczne, takie jak CSS, JavaScript.**


## Technologie użyte

- **ASP.NET Core**: Framework do budowy aplikacji webowych.
- **SQLite**: Baza danych.
- **HTML/CSS**: Struktura i stylizacja interfejsu użytkownika.
- **JavaScript**: Interaktywność front-endu.

### Wymagania

- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQLite](https://www.sqlite.org/download.html)

### Klonowanie repozytorium
   ```bash
   git clone https://github.com/wlabedz/MovieWeb.git
  
