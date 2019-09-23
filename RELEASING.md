# Instrukcja wydania ze szczególnym uwzględnieniem wersjonowania.

### Czynności ręczne przed wydaniem i zmierzające do wydania.

1. Zweryfikować stan synchronizacji gałęzi `develop` i `master`. W razie potrzeby wykonać odpowiednią synchronizację.
2. Funkcjonalność pożądana w nowej wersji powinna być zakończona. W tym celu należy zweryfikować stan _Pull requests_.
3. Zgłoszenia dotyczące zrealizowanej funkcjonalności powinny być zamknięte. Zweryfikować stan _Issues_.
4. Na gałęzi `develop` w pliku [version.json] z numeru wersji **usunąć część _prerelease_** pozostawiając go w formacie `<x.y.z>`.
5. Utworzyć i opublikować commit z komunikatem stwierdzającym o zmianie wersji.
   ```
   git commit -m "Wersja następna: x.y.z"
   git push origin develop
   ```
6. Zintegrować `develop` z `master` i opublikować wydanie z `master`
   ```
   git checkout master
   git merge develop
   git push origin master
   ```

### Część zautomatyzowana

Proces publikacji paczki _NuGet_ uruchomiony został automatycznie. Jest realizowany przez [pipeline wydania], którego stan należy zweryfikować. Publikacja może oczekiwać na akceptację osób wskazanych w organizacji. W trakcie oczekiwania na akceptację można podjąć kolejne czynności opisane w następnej sekcji.

### Czynności ręczne po wydaniu

1. Na gałęzi `develop` **przywrócić część _prerelease_** i podnieść numer wersji `<x.y.z+1>-beta.{height}` w pliku [version.json]
2. Utworzyć i opublikować commit z komunikatem stwierdzającym o zmianie wersji.
   ```
   git commit -m "Wersja następna: x.y.z+1-prerelease"
   git push origin develop
   ```

##### `Koniec pracy. Gratulacje!`

[version.json]: src/Soneta.Sdk/version.json
[repozytorium]: https://github.com/soneta/Soneta.MsBuild.SDK
[pipeline wydania]: https://dev.azure.com/soneta/GitHub/_release?_a=releases&view=mine&definitionId=1
