# Instrukcja wydania ze szczególnym uwzględnieniem wersjonowania.

### Czynności ręczne przed wydaniem i zmierzające do wydania.

1. Funkcjonalność pożądana w nowej wersji powinna być zakończona. W tym celu należy zweryfikować stan _Pull requests_.
2. Zgłoszenia dotyczące zrealizowanej funkcjonalności powinny być zamknięte. Zweryfikować stan _Issues_.
3. Na gałęzi `develop` w pliku [version.json] z numeru wersji **usunąć część _prerelease_** pozostawiając go w formacie `<x.y.z>`.
   * Utworzyć commit z komunikatem stwierdzającym o zmianie wersji.
     ```
     git commit -m "Wersja następna: x.y.z"
     ```
   * Opublikować zmianę wersji.
     ```
     git push origin develop
     ```
4. Otworzyć nowy _pull request_ w celu połączenia gałęzi `develop` i `master`. W tym celu należy zalogować się do [repozytorium].
   * Zweryfikować zmiany zakwalifikowane do wydania.
   * Ostatnią oczekiwaną zmianą jest ta, która określa nową wersję.
5. Zakończyć _pull request_ po uzyskaniu akceptacji. Od tego momentu wszystkie zmiany zakwalifikowane do nowej wersji znajdują się w gałęzi `master`.

### Część zautomatyzowana

Proces publikacji paczki _NuGet_ uruchomiony został automatycznie. Jest realizowany przez [pipeline wydania], którego stan należy zweryfikować. Publikacja może oczekiwać na akceptację osób wskazanych w organizacji. W trakcie oczekiwania na akceptację można podjąć kolejne czynności opisane w następnej sekcji.

### Czynności ręczne po wydaniu

1. Na gałęzi `develop` **przywrócić część _prerelease_** i podnieść numer wersji `<x.y.z+1>-beta.{height}` w pliku [version.json]
   * Utworzyć commit z komunikatem stwierdzającym o zmianie wersji.
     ```
     git commit -m "Wersja następna: x.y.z+1-prerelease"
     ```
   * Opublikować zmianę przyszłej wersji.
     ```
     git push origin develop
     ```

##### `Koniec pracy. Gratulacje!`

[version.json]: src/Soneta.Sdk/version.json
[repozytorium]: https://github.com/soneta/Soneta.MsBuild.SDK
[pipeline wydania]: https://dev.azure.com/soneta/GitHub/_release?_a=releases&view=mine&definitionId=1
