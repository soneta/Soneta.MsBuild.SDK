# Soneta.MsBuild.SDK

[![NuGet](https://img.shields.io/nuget/v/Soneta.Sdk.svg)](https://www.nuget.org/packages/Soneta.Sdk)
[![NuGet](https://img.shields.io/nuget/dt/Soneta.Sdk.svg)](https://www.nuget.org/packages/Soneta.Sdk)
[![Build Status](https://soneta.visualstudio.com/GitHub/_apis/build/status/Soneta.MsBuild.SDK?branchName=master)](https://soneta.visualstudio.com/GitHub/_build/latest?definitionId=2&branchName=master)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

MSBuild SDK do budowania dodatków dla enova365 — automatyczne referencje, generator kodu i zasoby platformy.

Zdejmuje z projektu dodatku ręczną konfigurację: sam dobiera referencje do bibliotek Soneta,
sam uruchamia generator kodu i sam rozpoznaje pliki zasobów charakterystyczne dla platformy.

> 📖 **Historia zmian:** [CHANGELOG.md](CHANGELOG.md) · **Proces wydania:** [RELEASING.md](RELEASING.md)

## Spis treści

- [Co robi SDK](#co-robi-sdk)
- [Kompatybilność](#kompatybilność)
- [Pierwsze kroki](#pierwsze-kroki)
- [Typy projektów](#typy-projektów)
- [Pliki rozpoznawane automatycznie](#pliki-rozpoznawane-automatycznie)
- [Parametry konfiguracyjne](#parametry-konfiguracyjne)
- [Studium przypadku: biblioteki spoza SonetaPackage](#studium-przypadku-biblioteki-spoza-sonetapackage)
- [Współpraca](#współpraca)

## Co robi SDK

SDK dostarcza trzy pliki, które MSBuild wciąga do projektu dodatku automatycznie:

| Plik | Odpowiada za |
|---|---|
| `Sdk.props` | Referencje do pakietów NuGet dobierane pod typ projektu (dodatek / UI / testy) oraz wartości domyślne parametrów. |
| `Sdk.targets` | Przebieg budowania — przede wszystkim uruchomienie generatora zamieniającego pliki `*.business.xml` i `*.config.xml` na kod `*.cs`. Wersja generatora dobierana jest automatycznie do wersji bibliotek. |
| `common.items.props` | Rozpoznawanie plików specyficznych dla platformy (`*.*form.xml`, `*.dbinit.xml`, `*.repx` i inne) i oznaczanie ich jako `EmbeddedResource`. |

W praktyce znika pytanie „którą wersję generatora podpiąć" — wynika ona z wersji bibliotek
zadeklarowanej w jednym miejscu.

## Kompatybilność

Wersję SDK dobiera się do wersji enova365, z którą budowany jest dodatek.

| Wersja SDK | Wymagana od enova365 | Platforma | Uwagi |
|---|---|---|---|
| **1.2.0** | 2606.0.0 | .NET 10 | Przejście na Microsoft.Testing.Platform. |
| 1.1.8 | — | .NET 10 | Pierwsze dostosowanie do .NET 10. |
| 1.1.7 | 2510.0.0 | .NET 6+ | Wymagana przy korzystaniu z testów integracyjnych — zmiana biblioteki asercji na `AwesomeAssertions`. |
| 1.1.6 | — | .NET 6+ | Aktualizacja zależności testowych (NUnit 4). |
| 1.1.5 | — | .NET 6+ | Obsługa wzorców DOTX i wydruków `*.repx`. |
| 1.1.4 | 2306.0.0-net | .NET 6 | |
| 1.1.2 | 2204.3.6 | `netstandard2.0` | Zachowuje zgodność z wcześniejszymi wersjami bibliotek FrameworkSoneta. |
| 1.1.0 | 2106.0.0 | .NET Framework | ⚠️ Zmiana łamiąca — patrz niżej. |

> ⚠️ **Migracja na 1.1.0 i nowsze.** Z plików `modul.business.xml` i `modul.config.xml` należy
> **usunąć** wpis `<import>generator</import>`, jeżeli występuje. Pozostawienie go powoduje
> konflikt dwóch sposobów pobierania generatora — nowego, automatycznego z SDK, oraz starego,
> opartego na kopiowaniu generatora do folderu projektu. Skutkiem są **puste pliki `.cs`**
> wyprodukowane przez generator.

Pełna lista zmian: [CHANGELOG.md](CHANGELOG.md).

## Pierwsze kroki

### 1. Wskazanie SDK w projekcie

W pliku `.csproj` dodatku wskaż SDK zamiast domyślnego:

```xml
<Project Sdk="Soneta.Sdk/1.2.0">
  <PropertyGroup>
    <TargetFramework>$(SonetaTargetFramework)</TargetFramework>
  </PropertyGroup>
</Project>
```

Po zapisaniu pliku konfiguracja projektu wykona się automatycznie.

### 2. Wersja SDK w jednym miejscu (zalecane)

Przy wielu projektach w rozwiązaniu wygodniej jest podać wersję SDK raz — w pliku `global.json`
położonym obok pliku rozwiązania:

```json
{
  "msbuild-sdks": {
    "Soneta.Sdk": "1.2.0"
  }
}
```

Wtedy w plikach `.csproj` wystarczy `<Project Sdk="Soneta.Sdk">`, bez numeru wersji.

Najnowszą opublikowaną wersję znajdziesz na
[nuget.org/packages/Soneta.Sdk](https://www.nuget.org/packages/Soneta.Sdk/).

### 3. Wersja bibliotek Soneta

Obok rozwiązania utwórz plik `Directory.Build.props`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <SonetaPackageVersion>2606.0.0</SonetaPackageVersion>
    <SonetaTargetFramework>net10.0</SonetaTargetFramework>
  </PropertyGroup>
</Project>
```

`SonetaPackageVersion` wskazuje wersję paczki
[Soneta.Products.Modules](https://www.nuget.org/packages/Soneta.Products.Modules/), z której
korzysta dodatek. Dzięki temu nie trzeba dodawać referencji ręcznie, a zmiana wersji bibliotek
sprowadza się do zmiany jednej wartości.

`SonetaTargetFramework` pozwala trzymać wersję .NET w jednym miejscu — każdy `.csproj` odwołuje
się do niej zamiast powtarzać wartość.

W `Directory.Build.props` można też definiować własne właściwości do użycia w projektach.

## Typy projektów

Zestaw referencji dobierany jest na podstawie **dwóch niezależnych flag**:

| Flaga | Ustalana przez | Znaczenie |
|---|---|---|
| `IsTestProject` | nazwa projektu zawiera `Test`, albo wartość ustawiona jawnie | Projekt testowy. |
| `IsAddonProject` | ustawiana automatycznie na `true`, gdy projekt deklaruje `<Project Sdk="Soneta.Sdk">` jako SDK zewnętrzne (`UsingMicrosoftNETSdk != true`) | Projekt dodatku budowany „pod" Soneta.Sdk. Gdy Soneta.Sdk jest importowane wewnątrz projektu korzystającego już z `Microsoft.NET.Sdk`, flaga pozostaje pusta. |

Jeżeli projekt testowy nie trzyma się konwencji nazewniczej, ustaw flagę jawnie:

```xml
<PropertyGroup>
  <IsTestProject>true</IsTestProject>
</PropertyGroup>
```

Kombinacja obu flag decyduje o zestawie pakietów:

| Pakiet | Test + dodatek | Test | Dodatek | Pozostałe |
|---|:--:|:--:|:--:|:--:|
| NUnit | ✅ | ✅ | — | — |
| NUnit3TestAdapter | ✅ | ✅ | — | — |
| Microsoft.NET.Test.Sdk | ✅ | ✅ | — | — |
| Microsoft.Testing.Extensions.\* | ✅ | ✅ | — | — |
| Soneta.Products.Test | ✅ | — | — | — |
| NSubstitute | — | ✅ | — | — |
| NSubstitute.Analyzers.CSharp | — | ✅ | — | — |
| AwesomeAssertions | — | ✅ | — | — |
| Soneta.Products.Modules | — | — | ✅ | — |

Kolumny odpowiadają kombinacjom `IsTestProject` / `IsAddonProject`. Źródłem tej tabeli jest
macierz `SonetaPackage` w `Sdk.props`.

### Konwencja nazewnicza `.UI`

Projekty warstwy interfejsu użytkownika nazywamy z sufiksem `.UI` — na przykład `Soneta.CRM.UI`
obok biznesowego `Soneta.CRM`.

Konwencja ta **nie wpływa na dobór referencji przez SDK** — z punktu widzenia budowania projekt
`.UI` jest zwykłym projektem dodatku. Ma natomiast znaczenie **w czasie działania programu**,
po stronie mechanizmów enova365:

- **Rozwiązywanie typów w definicjach formularzy.** Gdy assembly nazywa się `X.UI`, platforma
  odszukuje wśród jego referencji assembly `X` i dołącza je jako drugie źródło typów. Dzięki temu
  formularze w `X.UI` odwołują się do typów z `X` bez pełnej kwalifikacji.
- **Wydruki.** Szablony raportów szukane są w `X.Reports` na podstawie nazwy `X.UI`.
- **Zasoby graficzne.** Ścieżki obrazów mapowane są między `X.Forms` a `X.UI`.

Dlatego nazwę projektu UI warto ustalać zgodnie z konwencją, nawet jeśli SDK jej nie wymaga.

## Pliki rozpoznawane automatycznie

Poniższe pliki są bez dodatkowej konfiguracji oznaczane jako `EmbeddedResource`:

| Kategoria | Wzorce |
|---|---|
| Formularze | `*.*form.xml` oraz warianty `*.*form.premium.xml`, `*.*form.standard.xml`, `*.*form.mobile.xml`, `*.*form.console.xml`, `*.*form.browsernew.xml` |
| Konfiguracja i dane | `*.config.xml`, `*.dbinit.xml`, `*.publish.xml`, `*.convert.sql` |
| Prawa i role | `*.role.xml`, `*.rightstree.xml` |
| Wydruki | `*.repx`, `*.repx.cs`, `Repx\*.png`, `Repx\*.jpg`, `Repx\*.gif`, `Repx\*.bmp` |
| Wzorce dokumentów | `*.dotx` |
| Fragmenty kodu | `*.snippet.cs` |
| Licencje | `Properties\licenses.licx*` |

> **Uwaga na wzorzec `*.*form.xml`.** Gwiazdka przed `form` sprawia, że wzorzec obejmuje wszystkie
> rodzaje formularzy — `*.pageform.xml`, `*.viewform.xml`, `*.lookupform.xml`, `*.gridform.xml`
> i `*.form.xml`. Nie trzeba dopisywać ich osobno.

Pliki traktowane inaczej niż jako zwykły zasób osadzony:

| Wzorzec | Traktowanie |
|---|---|
| `*.business.xml` | **Nie** jest zasobem osadzonym. Trafia do paczki NuGet dodatku pod ścieżkę `schema\`, dzięki czemu moduły zależne widzą schemat. |
| `*.business.cs`, `*.config.cs` | Kompilowane, oznaczone jako wygenerowane i podpięte w drzewie projektu pod odpowiadający im plik XML. |
| `*.repx.cs`, `*.snippet.cs` | Jednocześnie kompilowane i osadzane jako zasób. |
| `*.dll`, `*.exe`, `*.traineddata` | Kopiowane do katalogu wynikowego w trybie `PreserveNewest`. |

Pliki `*.business.xml` i `*.config.xml` są wejściem dla generatora, który produkuje z nich
odpowiadające im pliki `*.business.cs` i `*.config.cs`. Wygenerowany kod nie wchodzi w skład
paczki NuGet dodatku.

## Parametry konfiguracyjne

### Sterowanie budowaniem

| Parametr | Domyślnie | Opis |
|---|---|---|
| `EnableDefaultSonetaPackageReferences` | `true` | Gdy `false`, SDK nie dołącza automatycznie referencji do bibliotek biznesowych. |
| `RunSonetaGenerator` | — (włączony) | Generator działa, dopóki parametr nie zostanie ustawiony na `false`. |
| `IsTestProject` | wykrywane z nazwy projektu | Wymusza traktowanie projektu jako testowego. |
| `UsingSonetaSdk` | `true` | Pozwala zdecydować, czy dany projekt korzysta z Soneta.MsBuild.SDK — patrz uwaga niżej. |

> **Jak działa `UsingSonetaSdk`.** SDK ustawia tę flagę na `true` w momencie, gdy zostanie
> załadowane. Korzystają z niej pliki `Directory.Build.props` po stronie rozwiązania, żeby nie
> zaimportować SDK po raz drugi w projekcie, który deklaruje je już przez
> `<Project Sdk="Soneta.Sdk">`. Typowy warunek wygląda tak:
>
> ```xml
> <PropertyGroup>
>   <ImportSonetaSdk Condition="'$(UsingSonetaSdk)' != 'true' AND '$(UsingMicrosoftNETSdk)' == 'true'">true</ImportSonetaSdk>
> </PropertyGroup>
> <Import Project="Sdk.props" Sdk="Soneta.Sdk" Condition="'$(ImportSonetaSdk)' == 'true'" />
> ```
>
> Dzięki temu ustawienie `UsingSonetaSdk` na `true` w konkretnym projekcie wyłącza dla niego
> automatyczny import SDK z `Directory.Build.props`.

### Lokalizacja wyników budowania

| Parametr | Domyślnie | Opis |
|---|---|---|
| `AggregateOutput` | `true` | Gdy `true`, wszystkie projekty poza testowymi budują się do wspólnego folderu zbiorczego, zachowując strukturę podfolderów (np. `..\bin\debug\net10.0\`). Gdy `false`, obowiązuje domyślne zachowanie MSBuild — każdy projekt buduje się do własnego folderu. |
| `AggregatePath` | `..\` | Ścieżka (względna lub bezwzględna) doklejana na początku wyliczonego `OutputPath`. Ignorowana, jeżeli `OutputPath` ustawiono jawnie w projekcie lub w `Directory.Build.props`. |

### Uruchamianie dodatku

| Parametr | Domyślnie | Opis |
|---|---|---|
| `SonetaAddonStartProgram` | najnowsza wersja wykryta w `C:\Program Files (x86)\Soneta\` | Ścieżka do programu, z którym uruchamiany jest dodatek. |

Mechanizm ustawia program startowy i przekazuje mu argument
`/extpath=<ścieżka do binariów projektu>`. Aby go wyłączyć, ustaw parametr na pustą wartość.

Mechanizm **nie nadpisze** jawnie ustawionego `StartProgram` ani ustawień z pliku
`launchSettings.json`. Jeżeli nie działa zgodnie z oczekiwaniem, sprawdź w pierwszej kolejności
te dwa miejsca.

Przykład jawnego wskazania:

```xml
<PropertyGroup>
  <SonetaAddonStartProgram>C:\Program Files (x86)\Soneta\enova365 2606.0.0.17856\SonetaExplorer.exe</SonetaAddonStartProgram>
</PropertyGroup>
```

### Wersje pakietów zewnętrznych

Każdy z parametrów nadpisuje wersję odpowiadającego mu pakietu NuGet. Wartości domyślne
odpowiadają SDK 1.2.0.

| Parametr | Domyślnie | Pakiet |
|---|---|---|
| `SonetaNUnitPackageVersion` | `4.4.0` | NUnit |
| `SonetaNUnitTestAdapterPackageVersion` | `6.1.0` | NUnit3TestAdapter |
| `SonetaMicrosoftNETTestSdkPackageVersion` | `18.0.1` | Microsoft.NET.Test.Sdk |
| `SonetaNSubstitutePackageVersion` | `5.3.0` | NSubstitute |
| `SonetaNSubstituteAnalyzersCSharpPackageVersion` | `1.0.17` | NSubstitute.Analyzers.CSharp |
| `SonetaAwesomeAssertionsPackageVersion` | `9.3.0` | AwesomeAssertions |
| `SonetaMicrosoftTestingExtensionsCrashDumpPackageVersion` | `2.0.2` | Microsoft.Testing.Extensions.CrashDump |
| `SonetaMicrosoftTestingExtensionsHangDumpPackageVersion` | `2.0.2` | Microsoft.Testing.Extensions.HangDump |
| `SonetaMicrosoftTestingExtensionsRetryPackageVersion` | `2.0.2` | Microsoft.Testing.Extensions.Retry |
| `SonetaMicrosoftTestingExtensionsTrxReportPackageVersion` | `2.0.2` | Microsoft.Testing.Extensions.TrxReport |

> Do wersji 1.1.6 włącznie biblioteką asercji było `FluentAssertions`, sterowane parametrem
> `SonetaFluentAssertionsPackageVersion`. Od 1.1.7 zastąpiła je `AwesomeAssertions`.

### Microsoft.Testing.Platform

| Parametr | Domyślnie | Opis |
|---|---|---|
| `EnableNUnitRunner` | `true` dla projektów testowych | Włącza runner NUnit dla Microsoft.Testing.Platform. |


### Powiadomienie o nieaktualnym SDK

SDK potrafi sprawdzić, czy używana wersja jest aktualna, i ostrzec podczas budowania.

| Parametr | Domyślnie | Opis |
|---|---|---|
| `SonetaSdkUpdateIntervalTime` | `None` | Jednostka odstępu między sprawdzeniami (`None` = sprawdzanie wyłączone). |
| `SonetaSdkUpdateIntervalValue` | `1` | Liczba jednostek odstępu. |

## Studium przypadku: biblioteki spoza SonetaPackage

Zwykle wszystkie potrzebne biblioteki Soneta dostarcza SDK automatycznie. Czasem jednak
dodatek potrzebuje biblioteki spoza zakresu **SonetaPackage** — typowo przy aktualizacji
starszego dodatku, którego interfejs zbudowano na WinForms, przed wprowadzeniem mechanizmu
`form.xml`.

> **W większości przypadków najlepszym rozwiązaniem jest przeniesienie dodatku na format
> `form.xml`**, który jest w pełni wspierany i zgodny z SDK.

Gdy nie jest to możliwe, brakujące biblioteki można zareferować jawnie — na przykład z folderu
instalacyjnego enova365.

W Visual Studio: prawy przycisk myszy na projekcie → *Add* → *Reference* → wskazanie pliku DLL.

Bezpośrednio w `.csproj`:

```xml
<ItemGroup>
  <Reference Include="Soneta.Forms">
    <HintPath>C:\Program Files (x86)\Soneta\enova365 1908.0.1.17324\Soneta.Forms.dll</HintPath>
    <SpecificVersion>false</SpecificVersion>
    <Private>false</Private>
  </Reference>
</ItemGroup>
```

Przy większej liczbie bibliotek wygodniej wskazać folder wyszukiwania zamiast powtarzać
`HintPath`:

```xml
<PropertyGroup>
  <ReferencePath>C:\Program Files (x86)\Soneta\enova365 1908.0.1.17324\</ReferencePath>
  <AssemblySearchPaths>$(AssemblySearchPaths);$(ReferencePath);</AssemblySearchPaths>
</PropertyGroup>
```

⚠️ Jeżeli zareferowane biblioteki same odwołują się do bibliotek dostarczanych przez SDK,
wersje z obu źródeł powinny być zgodne. Po podniesieniu `SonetaPackageVersion` biblioteki
mogą nadal współdziałać mimo różnicy wersji, ale przy większych zmianach w paczce
**SonetaPackage** konieczne będzie zaktualizowanie jawnych referencji.

## Współpraca

Zmiany zgłaszamy przez Pull Request do gałęzi `develop`. Po decyzji o wydaniu `develop`
jest scalany do `master`, co uruchamia publikację nowej wersji.

Każdy PR zmieniający zachowanie SDK powinien dopisać wpis do sekcji **Niewydane**
w [CHANGELOG.md](CHANGELOG.md).

### Testowanie zmian lokalnie

Po zbudowaniu SDK opublikuj paczkę do lokalnego źródła, aby stała się widoczna dla nowo
tworzonych projektów:

```powershell
cd bin\Release
dotnet nuget push nazwaPaczki.nupkg -s C:\Users\<uzytkownik>\.nuget\packages
```

### Wydawanie nowych wersji

Proces wydania opisuje [RELEASING.md](RELEASING.md).

## Licencja

[MIT](LICENSE)
