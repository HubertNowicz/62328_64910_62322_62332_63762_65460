Projekt z przedmiotu:   
Aplikacyjny projekt zespołowy  
Tytuł projektu:   
“Aplikacja webowa: Organizer przepisów kulinarnych” 
Skład zespołu:   
Arkadiusz Naróg - 64910 , Daniel Witczak – 62322, Hubert Nowicz - 63762 , Jakub Łukjaniuk -62332 , Łukasz Kamiński - 62328 , Patryk Zbożyna -  65460 
Opis projektu: 

Aplikacja "Organizer przepisów kulinarnych" została zaprojektowana jako system umożliwiający użytkownikom tworzenie, przechowywanie i przeglądanie przepisów kulinarnych, pogrupowanych w kategorie. Projekt kładzie nacisk na prostotę użytkowania, przejrzystość interfejsu oraz funkcjonalność dostosowaną do różnych ról – 
użytkownika końcowego oraz administratora. 
System został stworzony w architekturze trójwarstwowej (warstwa danych, logiki biznesowej oraz prezentacji), z użyciem technologii ASP.NET Core MVC i Entity Framework Core. Aplikacja zawiera pełny system rejestracji, logowania, autoryzacji oraz zarządzania treścią. 
Użytkownicy systemu i uprawnienia: 

Użytkownik aplikacji: 

Może zarejestrować konto i zalogować się do systemu, 
Ma możliwość tworzenia własnych przepisów kulinarnych, 
Może edytować oraz usuwać tylko swoje własne przepisy, 
Może przeglądać wszystkie dostępne przepisy innych użytkowników, 
Ma dostęp do systemu filtrowania przepisów według kategorii i czasu przygotowania, 
Może oznaczać przepisy jako ulubione i wyświetlać je w dedykowanej zakładce. 

Administrator: 
Posiada wszystkie uprawnienia użytkownika, 
Może przeglądać i usuwać wszystkie przepisy niezależnie od ich autora, 
Ma dostęp do panelu administracyjnego, gdzie zarządza użytkownikami (np. usuwa konta, przegląda przepisy danego użytkownika). 

Główne funkcjonalności systemu: 
Rejestracja i logowanie: 
Aplikacja umożliwia zakładanie kont użytkownika oraz logowanie się za pomocą nazwy użytkownika i hasła. Proces uwzględnia walidację danych, a po zalogowaniu użytkownik otrzymuje uprawnienia zgodne z przypisaną rolą. 

Przeglądanie przepisów: 
Użytkownicy mogą przeglądać wszystkie dostępne przepisy. Przepisy wyświetlane są w formie przejrzystych kart zawierających nazwę, czas przygotowania, miniaturkę oraz przycisk przekierowujący do szczegółów. 

Szczegóły przepisu: 
Po kliknięciu w przepis użytkownik widzi jego szczegóły: autora, opis, listę składników z jednostkami i ilościami oraz instrukcję krok po kroku. Dodatkowo może dodać przepis do ulubionych. 

Zarządzanie własnymi przepisami: 
Użytkownik może dodawać nowe przepisy, edytować istniejące oraz usuwać swoje wpisy. Formularz tworzenia/edycji zawiera dynamiczne dodawanie składników i instrukcji. 

System kategorii: 
Każdy przepis przypisany jest do jednej z predefiniowanych kategorii (np. śniadanie, obiad, deser). Kategorie te są wykorzystywane w filtrach i nawigacji. 

Filtrowanie i sortowanie: 
Użytkownik może zawęzić widok przepisów do wybranych kategorii i zakresów czasu przygotowania. Dostępne jest także sortowanie według trafności lub nazw alfabetycznie. 

Ulubione przepisy: 
Każdy użytkownik może dodać przepis do listy ulubionych. Widok ulubionych umożliwia szybki dostęp do oznaczonych wcześniej przepisów. 

Panel administratora: 
Administrator ma dostęp do dedykowanego panelu, z poziomu którego może zarządzać użytkownikami systemu: przeglądać ich dane, przypisane przepisy, a także usuwać konta lub wpisy. 

Instrukcja uruchomienia aplikacji: 

Wymagania: 

.NET 7.0 SDK 
Visual Studio 2022 lub VS Code 

Klonowanie projektu z githuba  
git clone https://github.com/HubertNowicz/62328_64910_62322_62332_63762_65460 

Konfiguracja appsettings.json z poprawnym connection stringiem do bazy danych. 

Wykonanie migracji bazy danych  

Uruchomienie aplikacji  
dotnet run 

Otworzenie przeglądarki i przejście do http://localhost:5107/ 

Opis przebiegu procesu – dodawanie nowego przepisu: 

Opis ten przedstawia szczegółowy przebieg operacji dodawania nowego przepisu kulinarnego przez zalogowanego użytkownika. Proces ten składa się z logicznych kroków wykonywanych przez użytkownika oraz system, uwzględniając interakcje z formularzem, walidację danych oraz zapis do bazy danych. 

Przebieg procesu: 

Start procesu 
 Proces rozpoczyna się, gdy zalogowany użytkownik wybiera z menu opcję „Dodaj przepis”. 

Wyświetlenie formularza tworzenia przepisu 
 System prezentuje formularz zawierający pola do wprowadzenia informacji, takich jak: nazwa przepisu, opis, czas przygotowania, kategoria, lista składników oraz instrukcja krok po kroku. 

Wprowadzenie danych przez użytkownika 
 Użytkownik wypełnia wszystkie wymagane pola. Formularz umożliwia dynamiczne dodawanie składników i kolejnych kroków przygotowania. 

Zatwierdzenie formularza 
 Po wypełnieniu formularza użytkownik zatwierdza dane, klikając przycisk „Stwórz przepis”, co inicjuje walidację danych. 

Walidacja danych po stronie serwera 
System sprawdza poprawność wprowadzonych informacji: 

czy wszystkie wymagane pola zostały uzupełnione, 
czy wartości liczbowe (np. ilość składników, czas przygotowania) są dodatnie, 
czy pola tekstowe nie przekraczają dopuszczalnych limitów znaków. 
W przypadku błędów formularz zostaje ponownie wyświetlony z odpowiednimi komunikatami o błędach. 

Zapis danych do bazy danych 
 Po pozytywnej walidacji system zapisuje nowy przepis w bazie danych, wraz ze wszystkimi składnikami i krokami przygotowania. 

Potwierdzenie i przekierowanie 
 Użytkownik otrzymuje informację o pomyślnym dodaniu przepisu i zostaje przekierowany do widoku szczegółowego nowo utworzonego przepisu. 

Zakończenie procesu 
 Proces zostaje zakończony – użytkownik może teraz zarządzać swoim przepisem lub dodać kolejny. 

 Opis przepływu komunikacji między komponentami: 
Prezentuje kolejność i sposób komunikacji między różnymi komponentami aplikacji podczas wykonywania konkretnej funkcjonalności. W dokumentacji został wykorzystany do przedstawienia przebiegu jednej z kluczowych operacji: dodawania przepisu przez użytkownika. 
Scenariusz: Dodawanie przepisu 
Przedstawiony został sposób interakcji między warstwą prezentacji (kontroler), warstwą logiki biznesowej (serwis), warstwą dostępu do danych (repozytorium) oraz bazą danych. 

Przebieg: 

Użytkownik korzystając z interfejsu aplikacji wypełnia formularz nowego przepisu i przesyła go do serwera. 
RecipeController odbiera dane z formularza w postaci obiektu widoku (RecipeCreateViewModel) i wywołuje odpowiednią metodę serwisu – AddRecipe(). 
RecipeService przetwarza dane oraz wykonuje logikę biznesową – przypisuje autora, sprawdza poprawność i przygotowuje dane do zapisu. 
RecipeRepository odpowiada za fizyczne zapisanie danych do bazy, wywołując metody Entity Framework. 
Baza danych zapisuje przepis i powiązane z nim składniki i kroki instrukcji. 

Opis sposobów i metod testowania 

W projekcie Organizer_Przepisów_Kulinarnych zastosowano unit testy do weryfikacji poprawności działania logiki biznesowej zawartej w warstwie serwisów. 
Metody i narzędzia testowania 
Testy jednostkowe (Unit Tests): 
Testy jednostkowe skupiają się na pojedynczych metodach klas (np.RecipeService). Ich celem jest sprawdzenie, czy poszczególne funkcje serwisów zachowują się zgodnie z oczekiwaniami dla różnych scenariuszy. 
Mockowanie zależności: 
Do izolacji testowanych klas użyto biblioteki Moq do tworzenia atrap (mocków) repozytoriów oraz loggerów. Dzięki temu testy są niezależne od rzeczywistej implementacji repozytorium i mogą symulować różne scenariusze, takie jak zwracanie danych lub generowanie wyjątków. 
Asynchroniczne wywołania: 
Testowane metody są asynchroniczne (async/await), co pozwala na poprawne testowanie operacji asynchronicznych, np. dostępu do bazy danych. 
Assercje i weryfikacja: 
Do oceny wyników testów wykorzystano klasyczne asercje (Assert.True, Assert.False, Assert.Equal), które sprawdzają: 
Poprawność wyników (np. zwracane wartości i komunikaty), 
Wywołanie metod mockowanych zależności (np. czy logger zapisał odpowiedni komunikat błędu). 
Pokrycie testami: 
Testy obejmują zarówno pozytywne scenariusze działania serwisu, jak i sytuacje błędne lub wyjątkowe, co zapewnia wysoką jakość i stabilność kodu. 

Technologie i biblioteki: 

ASP.NET Core MVC 7.0 
Entity Framework Core 
Identity Framework (autoryzacja i logowanie) 
Bootstrap 5 (interfejs użytkownika) 
Tom Select (do dynamicznych selectów składników) 
HTML, CSS, JavaScript (custom styling, formularze) 
