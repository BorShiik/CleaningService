# CleanDeal – system do zarządzania usługami sprzątającymi

## Cel i koncepcja

CleanDeal to webowa aplikacja, która łączy klientów potrzebujących usług sprzątania z firmą oraz zatrudnianymi sprzątaczami. System:

* automatyzuje składanie i obsługę zamówień (usługi sprzątania oraz różnego rodzaju produkty),
* udostępnia płatności online (Stripe),
* umożliwia komunikację w czasie rzeczywistym (SignalR),
* zapewnia panel administracyjny do zarządzania użytkownikami, usługami i danymi finansowymi.

Rozwiązanie zmniejsza nakład pracy operacyjnej, zwiększa przejrzystość procesów i podnosi satysfakcję użytkowników.

---

## Role i możliwości użytkowników

| Rola              | Najważniejsze możliwości                                                                                                                                                                                                                                   |
| ----------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Klient**        | \* Rejestracja / logowanie\* Przegląd katalogu usług sprzątania\* Tworzenie nowego zamówienia (data, adres, wybrane usługi)\* Płatność online (Stripe)\* Podgląd historii zamówień i statusów\* Czat z pracownikiem / obsługą                              |
| **Sprzątacz**     | \* Lista przydzielonych zleceń\* Podgląd szczegółów (adres, termin, zakres)\* Zmiana statusu („rozpoczęte”, „ukończone”)                                                                                                                                   |
| **Administrator** | \* Zarządzanie wszystkimi użytkownikami i rolami (ASP.NET Identity)\* Tworzenie / edycja typów usług, cen i produktów\* Panel statystyk (przychody, popularność usług)\* Podgląd i modyfikacja wszystkich zamówień\* Dostęp do czatu i systemu powiadomień |

---

## Kluczowe moduły

### 1. Zarządzanie zamówieniami

Obiekty `CleaningOrder` przechowują: klienta, adres, termin, listę usług, status i powiązaną płatność. Kontrolery umożliwiają tworzenie, edycję, przegląd i usuwanie zamówień.

### 2. Katalog usług

Model `ServiceType` opisuje nazwę, opis i cennik każdej usługi. Panel admina pozwala dodawać/aktualizować rekordy, a klienci wybierają je na formularzu zamówienia.

### 3. Płatności (Stripe)

Po złożeniu zamówienia tworzona jest sesja Checkout. Po udanej płatności Stripe webhook zapisuje encję `Payment` i ustawia zamówienie jako *opłacone*.

### 4. Komunikacja (SignalR)

Hub SignalR obsługuje czat między stronami (klient ↔ sprzątacz ↔ admin). Wiadomości zapisuje model `ChatMessage`.

### 5. Panel administracyjny

Widoki Razor + Bootstrap pokazują karty i tabele (Dashboard) z metrykami: liczba zleceń, przychody, best-sellery itp.

---

## Stos technologiczny

* **ASP.NET Core 8 MVC** + Razor Pages
* **Entity Framework Core** (SQL Server, migracje)
* **ASP.NET Identity** (uwierzytelnianie / autoryzacja)
* **AutoMapper** (mapowanie encje ⇄ DTO)
* **Stripe API** (Checkout / Webhooks)
* **SignalR** (czat w czasie rzeczywistym)
* **Bootstrap 5** + AOS / Bootstrap Icons (front-end, responsywność)

Architektura opiera się na wzorcu MVC, warstwie repozytoriów oraz separacji DTO/encje – ułatwia testy i rozbudowę.

---

## Bezpieczeństwo

* Role i uprawnienia kontrolowane przez `[Authorize(Roles = …)]`.
* Hasła haszowane przez Identity.
* CSRF chronione automatycznie w formularzach Razor.
* Stripe przechowuje wszystkie dane kart; w aplikacji zapisujemy jedynie tokeny i statusy płatności.

---

## UX i interfejs

* Responsywny szablon Bootstrap 5 (Mobile-First).
* Karty, tabele i formularze spójne kolorystycznie (neutralne kolory, ikony Bootstrap Icons).
* AOS dodaje subtelne animacje pojawiania się elementów.
* Dashboard admina zawiera szybkie skróty i wykresy (docelowo można dodać Recharts).

---

## Aktualnie w procesie rozwoju

1. **Kalendarz dostępności sprzątaczy** i automatyczny algorytm przydziału.
2. **Push / Email Notifications** (status zamówienia, przypomnienia).
3. **System lojalnościowy** (punkty, rabaty) i pakiety abonamentowe.
4. **Eksport faktur / raportów** (PDF/CSV) z panelu admina.
5. **Oceny i recenzje** – ranking sprzątaczy, statystyki jakości.

---

## Struktura katalogów

'''
CleanDeal/
├── Controllers/ # kontrolery MVC (Orders, Payments, Chat, Admin…)
├── Data/
│ ├── ApplicationDbContext.cs
│ └── Migrations/
├── DTOs/ # obiekty transferu danych
├── Models/ # encje EF (CleaningOrder, ServiceType, Payment…)
├── Repositories/ # repozytoria + interfejsy
├── Views/ # Razor Views (Bootstrap 5)
├── wwwroot/ # statyczne zasoby (css, js, images)
└── Program.cs # konfiguracja DI, Identity, Stripe, SignalR
'''

---