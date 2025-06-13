# CleaningService

## Ogólny opis projektu

**CleaningService** to aplikacja webowa służąca do zarządzania usługami sprzątającymi. Projekt został stworzony jako aplikacja zaliczeniowa, której głównym celem jest umożliwenie użytkownikom składania zamówień na usługi sprzątające, a także zapewnienie narzędzi do zarządzania nimi przez pracowników i administratorów.

Aplikacja umożliwia:

* Rejestrację i logowanie użytkowników (klientów, pracowników),
* Składanie i przeglądanie zamówień,
* Zarządzanie usługami sprzątającymi,
* Płatności online (Stripe),
* Komunikację w czasie rzeczywistym (SignalR Chat).

---

## Technologie użyte w projekcie

* **ASP.NET Core MVC 8** – główna technologia backendu (język C#).
* **Entity Framework Core (EF Core)** – obsługa bazy danych SQL Server.
* **ASP.NET Core Identity** – system logowania i zarządzania rolami użytkowników.
* **Stripe API** – do obsługi płatności online.
* **SignalR** – obsługa czatu w czasie rzeczywistym.
* **AutoMapper** – mapowanie między encjami a DTO.
* **Bootstrap 5** – stylowanie i responsywny interfejs użytkownika.
* **Wzorzec Repository + DTO** – rozdzielenie logiki warstw i zarządzanie danymi.

---

## Struktura katalogów

```
CleaningService/
│
├── Controllers/           # Kontrolery MVC 
├── DTOs/                  # Obiekty transferowe danych
├── Data/                  # ApplicationDbContext, Seedy, Migrations
├── Models/                # Modele domenowe (Product, Order itp.)
├── Repositories/          # Interfejsy i klasy repozytoriów
├── Views/                 # Widoki Razor (.cshtml)
├── wwwroot/               # Statyczne pliki (JS, CSS, obrazy)
├── MappingProfile.cs      # Konfiguracja AutoMappera
└── Program.cs             # Punkt wejściowy aplikacji
```

---

## Role użytkowników

* **Klient (Client)**

  * Przegląda dostępne usługi sprzątające,
  * Składa nowe zamówienia i przegląda swoje zlecenia,
  * Dokonuje płatności online.

* **Pracownik (Cleaner)**

  * Widzi przydzielone zamówienia sprzątające,
  * Aktualizuje ich status (rozpoczęte, zakończone),
  * Otrzymuje informacje o nowych zleceniach.

* **Administrator (Admin)**

  * Zarządza użytkownikami systemu i rolami,
  * Dodaje i edytuje usługi sprzątające,
  * Ma dostęp do statystyk, analiz i konfiguracji aplikacji.

---


```
```
