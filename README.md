# Soneta.MsBuild.SDK

[![NuGet](https://img.shields.io/nuget/v/Soneta.Sdk.svg)](https://www.nuget.org/packages/Soneta.Sdk)
  [![NuGet](https://img.shields.io/nuget/dt/Soneta.Sdk.svg)](https://www.nuget.org/packages/Soneta.Sdk)
  [![Build Status](https://soneta.visualstudio.com/GitHub/_apis/build/status/Soneta.MsBuild.SDK?branchName=master)](https://soneta.visualstudio.com/GitHub/_build/latest?definitionId=2&branchName=master)
  
# Wstęp 
SDK (Software Development Kit) jest to zestaw narzędzi dla programistów niezbędnych w tworzeniu aplikacji z danej biblioteki. Soneta.MsBuild.SDK jest zestawem narzędzi niezbędnym do tworzenie dodatków dla sytemu enova365. Pozwala automatycznie skonfigurować projekt oraz uzupełniać projekty dodatku o niezbędne elementy potrzebne do wsprółpracy z oprogramowaniem enova365. Do elementów konfiguracyjnych Soneta SDK zaliczamy następujące pliki wraz z ich przeznaczeniem:<br>
<ul>
  <li><b>Common.item.props</b> -plik zapewnia automatyczną obsługę dołączania nowych plików do projektu. Między innymi pliki *.pageform.xml, *dbinit.xml zostaną automatycznie skonfigurowane jako EmbeddResource.</li>

  <li><b>Sdk.props</b> -plik zapewnia poprawne utworzenie referencji dla różnych typów projektów np. Dla projektu testowego zostaną załadowane biblioteki testowe.</li>
  <li><b>Skd.targets</b> -plik przechowuje informacje dotyczące procesu budowanie m.in. automatycznego uruchamiania generatora tworzenia definicji baz danych (*.business.cs). Podsumowując prostymi słowami, od teraz nie będzie trzeba się zastanawiać, którą wersję generatora powinniśmy podpiąć, ponieważ zostanie to wykonane automatycznie.</li>
</ul>

# Pierwsze kroki
## Zdefiniowanie wersji Soneta.sdk.<br>

W celu zaimportowania **Soneta.MsBuild.SDK** do projektu dodatku w pliku "**nazwaprojektu.csproj**" edytujemy linię dotyczącą projektu: 
```xml
   <Project Sdk="Soneta.Sdk/numerWersji">  
```
Po zapisie zmian zostanie wykonana automatyczna konfiguracja projektu.


W przypadku gdy w solucji znajduje się wiele projektów, by uniknąć konieczności zmian w każdym projekcie numeru wersji, istnieje możliwość stworzenia pliku nadrzędnego **"global.json"** (poza projektami) o zawartości: 
```json
   { 
     "msbuild-sdks": { 
        "Soneta.Sdk": "numerWersji" 
      }
   } 
```
 Dzięki czemu w plikach "**.csproj**" wymagany będzie tylko wpis bez konieczności umieszczania numeru wersji. **Najnowszą** opublikowaną **wersję** dodatku **Soneta.MsBuild.SDK** znajdziemy [tutaj](https://www.nuget.org/packages/Soneta.Sdk/)
 ## Zdefiniowanie wersji bibliotek 
 Powinniśmy także utworzyć plik **"Directory.Build.props"** o zawartości:
 ```json
 <?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="14.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <SonetaPackageVersion>1905.0.1</SonetaPackageVersion>
    <SonetaTargetFramework>net46</SonetaTargetFramework>
  </PropertyGroup>
</Project>
```
Plik „**Directory.Build.props**” zawiera informację o wersji bibliotek pobieranych przez Soneta.SDK. Dzięki temu nie musimy już manualnie dodawać referencji do bibliotek Sonety tylko globalnie definiujemy wersję bibliotek z której ma korzystać dodatek. Należy tutaj wspomnieć, że dzięki takiemu rozwiązaniu możemy łatwo zmienić wersję bibliotek, którą chcemy wykorzystać w naszym dodatku. Wersję bibliotek Sonety definiujemy za pomocą parametru "**SonetaPackageVersion**", który wskazuje na wersję paczki [Soneta.Product.Modules](https://www.nuget.org/packages/Soneta.Products.Modules/) . W pliku „Directory.Build.props” mamy także **możliwość zdefiniowania własnych zmiennych**, które mogą być później używane w naszych projektach.<br>
Kolejnym ważnym elementem znajdującym się w pliku „Directory.Build.props jest wersja .NET, którą używamy w naszej solucji. Dzięki temu każdy plik "**.csproj**" może odwołać się do parametru **SonetaTargetFramework** zdefiniowanego w jednym miejscu. Poniżej przedstawiono zawartość pliku "***.csproj**" wykorzystujący **Soneta.MsBuild.SDK**.
```json
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Soneta.Sdk">
  <PropertyGroup>
    <TargetFramework>$(SonetaTargetFramework)</TargetFramework>
  </PropertyGroup>
</Project>
```

# Informacje Ogólne
Soneta.sdk obsługuje **3 typy projektów**, które można stworzyć. W zależności od rodzaju projektu pobierane są inne biblioteki. Możemy stworzyć takie projekty jak:<br>
<ul>
   <li>Projekt testowy, który w swojej nazwie będzie zawierał słowo "**Tests**"</li>
   <li>Projekt interfejsu użytkownika kończacy się na wyrażeniu "***.UI**"</li>
   <li>Projekt dodatku zawierający elementy logiki biznesowej</li>
</ul>

Istnieje możliwość stworzenia **projektu testowego**, który nie podąża za wyżej opisaną konwencją. Można to zrobić poprzez ustawienie w pliku **.csproj** flagi **\<IsTestProject>true\</IsTestProject>**.<br>
Możemy także wykorzystać takie parametry jak:<br>
<ul>
  <li>AggregateOutput - przyjmuje true/false, domyślnie true. Kiedy jest ustawiony na true, to wszystkie projekty budują się do folderu bin, który będzie o poziom wyżej niż sam projekt. Dzięki temu prawie wszystkie projekty (z wyjątkiem testów) w solucji mogą budować się do zbiorczego folderu bin. Jeśli parametr jest ustawiony na false to folder bin będzie znajdował się FolderProjektu/bin/, a jeśli na false to FolderSolucji/bin/.</li>
  <li>EnableDefaultSonetaPackageReferences – przyjmuje true/false, domyślnie true. Jeżeli parametr będzie ustawiony na false to Soneta.MsBuild.SDK nie będzie automatycznie dołączać referencji do bibliotek biznesowych.</li>
  <li>UsingSonetaSdk- przyjmuje true/false, domyślnie true. Pozwala zdecydować czy dany projekt korzysta z Soneta.MsBuild.SDK</li>
  <li>SonetaValueTuplePackageVersion, SonetaNUnitPackageVersion, SonetaNUnitTestAdapterPackageVersion- parametry opisujące wersję   pakietów</li>
</ul>


Wraz z bibliotekami jest pobierana odpowiednia wersja generatora. Zadaniem generatora jest przekonwertowanie plików „.xml” na pliki „.cs”. Konwersja wykonywana jest podczas budowania dodatku.   

# Współpraca
1. Po poprawnym zbudowaniu Sdk ustawiamy się w konsoli na ścieżce \bin\Release
2. Wykonujemy push do naszego lokalnego folderu z paczkami: 
```powershell
dotnet nuget push nazwaPaczki.nupkg -s C:\Users....nuget\packages
``` 
3. Dzięki powyższym czynnościom nasze SDK będzie widoczne dla naszych nowo utworzonych projektów. 


