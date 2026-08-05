# Changelog

Wszystkie istotne zmiany w projekcie **Soneta.MsBuild.SDK** są dokumentowane w tym pliku.

Format oparty jest na [Keep a Changelog](https://keepachangelog.com/pl/1.1.0/),
a projekt stosuje [wersjonowanie semantyczne](https://semver.org/lang/pl/).

Daty wydań odpowiadają dacie publikacji paczki na [nuget.org](https://www.nuget.org/packages/Soneta.Sdk/),
a nie dacie commita podnoszącego numer wersji — te potrafiły różnić się o tygodnie.

> **Uwaga o kompletności.** Wpisy dla wersji 1.0.0–1.2.0 zostały odtworzone wstecz z historii
> gita, historii `version.json` i not wersji, które wcześniej znajdowały się na początku
> `README.md`. Zakres zmian przypisano na podstawie commitów leżących na gałęzi `master`
> pomiędzy kolejnymi commitami wydaniowymi. Szczegóły metody i znane niepewności opisano
> w sekcji [Uwagi do rekonstrukcji](#uwagi-do-rekonstrukcji) na końcu pliku.

## [Nieopublikowane]

### Zmienione

- **Generator: wykrywanie zmian po sumie kontrolnej treści zamiast po czasie modyfikacji pliku.**
  Wejścia generatora (własne `*.business.xml` / `*.config.xml` oraz `*.business.xml` modułów
  nadrzędnych) są haszowane (SHA256), a wynik zapisywany do pliku-stempla w `obj`. Regeneracja
  następuje po zmianie treści, a nie po zmianie znacznika czasu. Rozwiązuje to dwa problemy:
  `git checkout` ustawiał czas modyfikacji na „teraz" i wymuszał zbędne przegenerowania, a zmiana
  wyłącznie schematu nadrzędnego nie zmieniała własnego XML, przez co `.cs` pozostawał
  nieaktualny i wymagał ręcznego `clean`. Wspólny stempel projektu powoduje też, że zmiana
  `business.xml` regeneruje również `config.cs`. ([#99])

## [1.2.0] – 2026-07-01

> **Wymagana od enova365 2606.0.0 (NET 10).** Zmiana zależności i platformy testowej na zgodne
> z .NET 10.

### Dodane

- Obsługa **Microsoft.Testing.Platform (MTP)** dla projektów testowych: pakiety
  `Microsoft.Testing.Extensions.CrashDump`, `HangDump`, `Retry` i `TrxReport` wraz z parametrami
  wersji (`SonetaMicrosoftTestingExtensions*PackageVersion`). Runner NUnit dla MTP włączany
  właściwością `EnableNUnitRunner` (domyślnie `true` dla projektów testowych).
- Właściwość `SonetaPackageReferenceNeedsVersion`, przygotowana pod centralne zarządzanie
  wersjami pakietów (`ManagePackageVersionsCentrally`).
- Parametr `SonetaNSubstituteAnalyzersCSharpPackageVersion`.

### Zmienione

- Referencje do pakietów przebudowane z listy sztywnych wpisów `PackageReference` na **macierz
  pozycji `SonetaPackage`** opisującą, który pakiet trafia do jakiego typu projektu
  (testowy/dodatek). Ułatwia to nadpisywanie i rozszerzanie zestawu zależności.
- Podniesione wersje narzędzi testowych: NUnit `4.3.2` → `4.4.0`, NUnit3TestAdapter `5.0.0` → `6.1.0`,
  Microsoft.NET.Test.Sdk `17.13.0` → `18.0.1`, AwesomeAssertions `9.1.0` → `9.3.0`.

### Usunięte

- Referencja do `System.ValueTuple` oraz parametr `SonetaValueTuplePackageVersion` — zbędne na .NET 10.

## [1.1.8] – 2026-06-05

### Zmienione

- Dostosowanie SDK do **.NET 10**.
- Docelowe platformy samej paczki SDK: `netstandard1.4` → `netstandard2.0`.
- `README.md` dołączany do paczki jako opis pakietu na nuget.org (`PackageReadmeFile`).
- Pula agentów builda w Azure Pipelines ustawiona na `windows-2022`.

## [1.1.7] – 2025-10-27

> **Wymagana od enova365 2510.0.0** w przypadku korzystania z warstwy testów integracyjnych —
> zmieniła się biblioteka do wykonywania asercji.

### Zmienione

- **Zmiana biblioteki asercji: `FluentAssertions` → `AwesomeAssertions`** (wersja `9.1.0`).
  Parametr `SonetaFluentAssertionsPackageVersion` zastąpiony przez
  `SonetaAwesomeAssertionsPackageVersion`.

## [1.1.6] – 2025-06-20

### Dodane

- Rozpoznawanie plików `*.form.console.xml` oraz `*.form.browsernew.xml` jako `EmbeddedResource`.

### Zmienione

- Aktualizacja zależności narzędzi testowych: NUnit `3.14.0` → `4.3.2`,
  NUnit3TestAdapter `4.5.0` → `5.0.0`, Microsoft.NET.Test.Sdk `17.8.0` → `17.13.0`,
  NSubstitute `5.1.0` → `5.3.0`, FluentAssertions `6.12.0` → `7.2.0`.

## [1.1.5] – 2024-01-02

### Dodane

- Obsługa wzorców **DOTX** (`*.dotx`) jako zasobów osadzonych.
- Pliki `*.repx` (wydruki DevExpress) traktowane jako `EmbeddedResource`.

### Zmienione

- Dostosowanie przekazywania `extPath` do nowej składni z separatorem `--`; usunięcie cudzysłowów.
- Podniesienie wersji narzędzi zewnętrznych: NUnit `3.14.0`, NUnit3TestAdapter `4.5`.

### Usunięte

- Referencja do `Moq` wraz z parametrem wersji.

## [1.1.4] – 2022-11-25

> **Wymagana od enova365 2306.0.0-net (.NET 6).**

### Zmienione

- Podniesienie wersji `FluentAssertions`.

## [1.1.3] – 2022-09-26

### Dodane

- **Integracja `Soneta.Generator` jako zadania MSBuild** zamiast wywołania zewnętrznego procesu.
- Narzędzia testowe dla projektów pod **.NET 6**.

### Zmienione

- `CopyLocalLockFileAssemblies` ustawiane wyłącznie dla projektów dodatków.

## [1.1.2] – 2022-06-22

> Kompatybilna z poprzednimi wersjami bibliotek FrameworkSoneta; nadaje się do kompilacji
> w `netstandard2.0` od wersji enova365 2204.3.6.

### Naprawione

- Wyeliminowanie problemu **zduplikowanych referencji do `*.business.xml`**.

### Zmienione

- `CopyLocalLockFileAssemblies` ustawione na `true`.

## [1.1.1] – 2021-12-09

### Naprawione

- Poprawka błędu zgłoszonego w [#49] dotyczącego `Sdk.targets`. ([#52])

## [1.1.0] – 2021-03-24

> **Zmiana łamiąca kompatybilność.** Korzystanie z tej wersji wymaga **usunięcia** z plików
> `modul.business.xml` i `modul.config.xml` wpisu `<import>generator</import>`, jeżeli występuje.
> Pozostawienie go powoduje konflikt dwóch sposobów pobierania generatora — nowego,
> automatycznego z SDK, oraz starego, opartego na kopiowaniu generatora do folderu projektu —
> czego skutkiem są **puste pliki `.cs`** wyprodukowane przez generator.

### Zmienione

- **Automatyczne pobieranie generatora przez SDK** w miejsce ręcznego kopiowania do projektu.
- Ujednolicenie działania `Soneta.Generator` — obsługa `import` w trybie `--schema`. ([#43])
- Ścieżka przekazywana przez `extPath` ujęta w cudzysłów. ([#45])
- Aktualizacja `Nerdbank.GitVersioning` do `3.3.37`.

## [1.0.4] – 2020-04-06

### Dodane

- Obsługa plików `form.xml` w wariantach **premium** i **standard**. ([#36])
- Pliki XML związane z prawami oznaczane jako `EmbeddedResource`. ([#39])
- Ostrzeżenie informujące o korzystaniu z nieaktualnej wersji SDK. ([#35])

### Zmienione

- Sparametryzowanie wersji `NSubstitute` i `FluentAssertions`. ([#34])

## [1.0.3] – 2019-10-28

### Zmienione

- Uniezależnienie uruchamiania `SonetaAddonStartProgram` od właściwości `Configuration`. ([#31])
- Uniezależnienie działania `AggregateOutput` od `StartProgram`, `SonetaAddonStartProgram`
  i `Configuration`. ([#24])

### Naprawione

- Uzupełnienie brakującego `Condition` przy elemencie `Import`. ([#30])

### Dokumentacja

- Dodanie `RELEASING.md` opisującego proces wydania. ([#23])

## [1.0.2] – 2019-09-20

### Dodane

- Obsługa **wielu plików `business.xml`** w jednym projekcie. ([#15])

### Naprawione

- Niewrażliwość na wielkość liter w nazwach `[Cc]onfig.xml` i `[Bb]usiness.xml`. ([#18])
- Generator nie tworzył plików `config.cs` na podstawie `config.xml`. ([#16])
- Wygenerowane pliki `*.business.cs` nie wchodzą już w skład paczki NuGet. ([#12])

### Dokumentacja

- Instrukcja wersjonowania podczas wydania (`VERSIONING.md`).

## [1.0.1] – 2019-06-26

### Naprawione

- Uwzględnienie spacji w ścieżkach do plików `business.xml` projektu.
- `SonetaGeneratorExe`, `SavedFile` oraz elementy `SonetaSchemaReference` ujęte w cudzysłowy.
- Tymczasowa poprawka dla Visual Studio 2019: wywołanie `Compile` nie kompiluje ponownie
  całego rozwiązania.

### Zmienione

- Zniesienie grupowania w repozytorium plików `*.business.xml` / `*.config.xml`
  z odpowiadającymi im `*.cs`.

## [1.0.0] – 2019-05-21

Pierwsze publiczne wydanie SDK.

### Dodane

- Pliki konfiguracyjne SDK: `Sdk.props`, `Sdk.targets`, `common.items.props`.
- Automatyczne referencje do pakietów `Soneta.Products.Modules` i `Soneta.Products.Test`
  zależnie od typu projektu.
- Automatyczne uruchamianie generatora (`ExecuteSonetaGenerator`) podczas budowania dodatku,
  wraz z obsługą zapisu pojedynczego pliku.
- Ekspozycja `business.xml` w paczkach NuGet.
- Domyślne ustawienie `StartProgram` na `SonetaExplorer` oraz `StartArguments`
  na `/extpath=<OutputPath>` dla projektów dodatków.
- Referencja do `NUnitTestAdapter` dla projektów testowych oraz parametr
  `SonetaNUnitPackageVersion`.
- Flaga `UsingSonetaSdk` pozwalająca wyłączyć SDK dla wybranego projektu.
- Licencja MIT.

## Uwagi do rekonstrukcji

Historia wersji do 1.2.0 włącznie została odtworzona po fakcie. Warto znać jej ograniczenia:

- **Metoda.** Commit wydaniowy = commit ustawiający w `src/Soneta.Sdk/version.json` numer wersji
  bez części *prerelease*. Zakres zmian danej wersji = commity na `master` pomiędzy poprzednim
  a bieżącym commitem wydaniowym. Tam, gdzie kolejność commitów rozmijała się z ich datami
  (skutek merge'y i cherry-picków), rozstrzygała pozycja w historii `master`, a nie data.
- **Wersja 1.0.5 nie została nigdy opublikowana.** Numer został przygotowany w repozytorium
  (commit `5bf25ae`, 2021-03-24), ale paczka nie trafiła na nuget.org — wydano od razu 1.1.0
  tego samego dnia. W changelogu nie ma dla niej wpisu.
- **Wersja 1.1.5 była wydawana dwukrotnie.** Pierwsze podejście z 2022-11-25 (`1db02fa`) zostało
  wycofane commitem „Porządki w wydaniu wersji 1.1.4" (`bcfc11c`) i numer wrócił do 1.1.4.
  Faktyczna publikacja 1.1.5 nastąpiła dopiero 2024-01-02.
- **Przypisanie zmian w narzędziach testowych do 1.1.5 jest przybliżone.** Commity podnoszące
  NUnit, test adapter i usuwające Moq powstały po commicie wydaniowym 1.1.5, ale przed datą
  publikacji paczki — przypisano je do 1.1.5.
- **Brakujące tagi.** Repozytorium zawiera tagi wyłącznie dla wersji 1.0.2, 1.0.3, 1.0.4, 1.1.0
  i 1.1.1. Pozostałe dziesięć wydań nie ma tagów, ponieważ dotychczasowy proces przewidywał
  tagowanie jako ostatni, ręczny krok po publikacji.
- **Numery `#NN`** odnoszą się do zgłoszeń i Pull Requestów w tym repozytorium, o ile dało się
  je jednoznacznie odczytać z komunikatów commitów. Odwołania do wewnętrznych identyfikatorów
  Azure DevOps (np. `Task #146366`) zostały pominięte jako niedostępne publicznie.

[Nieopublikowane]: https://github.com/soneta/Soneta.MsBuild.SDK/compare/master...develop
[1.2.0]: https://www.nuget.org/packages/Soneta.Sdk/1.2.0
[1.1.8]: https://www.nuget.org/packages/Soneta.Sdk/1.1.8
[1.1.7]: https://www.nuget.org/packages/Soneta.Sdk/1.1.7
[1.1.6]: https://www.nuget.org/packages/Soneta.Sdk/1.1.6
[1.1.5]: https://www.nuget.org/packages/Soneta.Sdk/1.1.5
[1.1.4]: https://www.nuget.org/packages/Soneta.Sdk/1.1.4
[1.1.3]: https://www.nuget.org/packages/Soneta.Sdk/1.1.3
[1.1.2]: https://www.nuget.org/packages/Soneta.Sdk/1.1.2
[1.1.1]: https://www.nuget.org/packages/Soneta.Sdk/1.1.1
[1.1.0]: https://www.nuget.org/packages/Soneta.Sdk/1.1.0
[1.0.4]: https://www.nuget.org/packages/Soneta.Sdk/1.0.4
[1.0.3]: https://www.nuget.org/packages/Soneta.Sdk/1.0.3
[1.0.2]: https://www.nuget.org/packages/Soneta.Sdk/1.0.2
[1.0.1]: https://www.nuget.org/packages/Soneta.Sdk/1.0.1
[1.0.0]: https://www.nuget.org/packages/Soneta.Sdk/1.0.0
[#12]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/12
[#15]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/15
[#16]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/16
[#18]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/18
[#23]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/23
[#24]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/24
[#30]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/30
[#31]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/31
[#34]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/34
[#35]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/35
[#36]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/36
[#39]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/39
[#43]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/43
[#45]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/45
[#49]: https://github.com/soneta/Soneta.MsBuild.SDK/issues/49
[#52]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/52
[#99]: https://github.com/soneta/Soneta.MsBuild.SDK/pull/99
