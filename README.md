# Wersja SDK 1.1.0 jest do wykorzystania od wersji enova365 2106.0.0
**!!! Korzystanie z tej wersji SDK wymaga usunięcia z plików modul.business.xml i modul.config.xml wpisu (jeżeli występuje):**

`<import>generator<import>`

Pozostawienie wpisu spowoduje konflikt dwóch sposobów pobierania generatora - nowego, automatycznego z SDK oraz starego - kopiowania generatora do folderu projektu, co zaskutkuje wygenerowaniem pustych plików.cs przez generator.

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
  <li><b>AggregateOutput</b> - przyjmuje true/false, domyślnie true. Ustawiony na true, sprawia, iż wszystkie projekty (z wyjątkiem projektu testów) budują się do zbiorczego folderu. Domyślnie będzie on znajdował się o poziom wyżej niż sam folder zawierający projekt: '..\bin' lub w innej lokalizacji jawnie przypisanej do parametru AggregatePath. Parametr ten gromadzi output niemal wszystkich projektów solucji we wspólnej lokalizacji nie tracąc przy tym struktury folderów dla domyślnie wyliczanego parametru OutputPath (np. ..\bin\debug\net46\). Jeśli parametr jest ustawiony na false, zostanie przywrócone domyślne zachowanie. Oznacza to, że folderem decelowym dla każdego projektu będzie ten folder, w którym znajduje się plik projektu.</li>
  <li><b>AggregatePath</b> - domyślnie '..\' (folder nadrzędny względem bieżącego foldera). W przypadku, gdy parametr AggregateOutput ustawiony jest na true, jest to ścieżka względna lub bezwzględna, która zostanie dodana na początku automatycznie wyliczonego atrybutu OutputPath. W przypadku domyślnych ustawień ostateczna wartość OutputPath może przyjąć postać np. '..\bin\debug\net46\'. Jeśli OutputPath zostanie ustawione jawnie w projekcie lub pliku Directory.Build, AggregatePath zostanie zignorowane. W takim wypadku wszystkie pliki generowane przez projekt zostaną wrzucone bezpośrednio do lokalizacji jawnie wskazanej przez parametr OutputPath. Celem wprowadzenia parametru AggregateOutput i AggregatePath było umożliwienie budowania wielu projektów do wspólnej lokalizacji.</li>
  <li><b>EnableDefaultSonetaPackageReferences</b> – przyjmuje true/false, domyślnie true. Jeżeli parametr będzie ustawiony na false to Soneta.MsBuild.SDK nie będzie automatycznie dołączać referencji do bibliotek biznesowych.</li>
  <li><b>UsingSonetaSdk</b> - przyjmuje true/false, domyślnie true. Pozwala zdecydować czy dany projekt korzysta z Soneta.MsBuild.SDK.</li>
  <li><b>SonetaValueTuplePackageVersion, SonetaNUnitPackageVersion, SonetaNUnitTestAdapterPackageVersion</b> - parametry opisujące wersję pakietów.</li>
  <li><b>SonetaAddonStartProgram</b> - ścieżka do programu, wraz z którym powinien być uruchamiany dodatek. Domyślna wartość dla tego parametru jest wyliczana w oparciu o ścieżkę, w której domyślnie instalowane jest oprogramowanie firmy Soneta (<b>C:\Program Files (x86)\Soneta\</b>). Spośród wszystkich zainstalowanych w takiej lokalizacji wersji programu, wybierana jest ostatnia. Jeśli program, z którym powinien być uruchamiany dodatek znajduje się w innej lokalizacji, należy ścieżkę do niego przypisać do tego parametru (np: 
<b>C:\Program Files (x86)\Soneta\enova365 2002.0.0.17856\SonetaExplorer.exe</b>). Działanie parametru <b>SonetaAddonStartProgram</b> opiera się na automatycznym ustawieniu programu startowego i przekazaniu mu argumentu (parametr uruchomieniowy): 
<b>/extpath=<ścieżka do binariów projektu></b>. W celu wyłączenia tego mechanizmu wystarczy jawnie ustawić <b>SonetaAddonStartProgram</b> na pustą wartość. Mechanizm ten nie nadpisze jawnie ustawionego parametru <b>StartProgram</b>, ani ustawień zapisanych w pliku <b>launchSettings.json</b>, w którym także można jawnie wskazać program startowy. Jeśli mechanizm oparty o <b>SonetaAddonStartProgram</b> nie działa poprawnie, należy w pierwszej kolejności sprawdzić, czy nie jest on nadpisywany przez wyżej wymienione ustawienia.</li>
</ul>


Wraz z bibliotekami jest pobierana odpowiednia wersja generatora. Zadaniem generatora jest przekonwertowanie plików „.xml” na pliki „.cs”. Konwersja wykonywana jest podczas budowania dodatku.   

# Studium przypadku
## Korzystanie z bibliotek Soneta, niedostarczanych automatycznie przez **Soneta.MsBuild.SDK** (na przykładzie dodatków opartych o biblioteki WinForms)

W wielu przypadkach do utworzenia lub zaktualizowania dodatku w oparciu o Soneta Sdk, wszystkie niezbędne biblioteki Soneta zostaną dostarczone automatycznie.
Czasem jednak istnieje potrzeba skorzystania z bibliotek nie ujętych w zakresie **SonetaPackage**. Przykładem może być aktualizacja dodatku, którego interfejs użytkownika został zbudowany przed wprowadzeniem mechanizmu form.xml, za pomocą bibliotek opartych o technologię WinForms. **W większości przypadków najlepszym rozwiązaniem takiego problemu jest zaktualizowanie dodatku do formatu form.xml, który jest w pełni wspierany i kompatybilny z Sdk**.

Z przyczyn biznesowych lub technicznych takie rozwiązanie nie zawsze jest możliwe. Aby mimo wszystko umożliwić takim dodatkom korzystanie z **Soneta.MsBuild.SDK** można jawnie zareferować do brakujących bibliotek, np. znajdujących się w folderze instalacyjnym programu enova365. Z poziomu **Visual Studio** referencję można dodać bezpośrednio z interfejsu użytkownika. W tym celu w oknie eksploratora solucji należy kliknąć odpowiedni projekt dodatku prawym przyciskiem myszy. Następnie, w menu kontekstowym, wybrać Add/Reference i wskazać odpowiedni plik DLL. Referencję można także dodać bezpośrednio w pliku projektu \*.csproj, dopisując do niego poniższy fragment kodu. 

```
<ItemGroup>
  <Reference Include="Soneta.Forms">
    <HintPath>C:\Program Files (x86)\Soneta\enova365 1908.0.1.17324\Soneta.Forms.dll</HintPath>
    <SpecificVersion>false</SpecificVersion>
    <Private>false</Private>
  </Reference>
</ItemGroup>
```

Jeśli istnieje potrzeba referowania do większej ilości bibliotek, można podać lokalizację, pod którą będą one wyszukiwane. W takim wypadku nie ma potrzeby dłużej korzystać z elementu HintPath. W tym celu w pliku projektu należy dodać poniższy fragment, gdzie w elemencie **ReferencePath** należy umieścić ścieżkę do folderu zawierającego biblioteki, do których odnosi się referencja.

```
<PropertyGroup>
  <ReferencePath>C:\Program Files (x86)\Soneta\enova365 1908.0.1.17324\</ReferencePath>
  <AssemblySearchPaths>$(AssemblySearchPaths);$(ReferencePath);</AssemblySearchPaths>
</PropertyGroup>
```

Warto zwrócić uwagę, że jeśli zareferowane biblioteki, same w sobie referują do bibliotek Soneta dostarczanych przez Sdk, wersja bibliotek z obu źródeł powinna być zgodna. W przypadku podniesienia wersji **SonetaPackageVersion** w pliku **Directory.Build.props** pomimo różnicy wersji bibliotek DLL, nadal mogą one zachować zgodność. Może się jednak okazać, że w nowej paczce **SonetaPackage** zaszły znaczące zmiany uniemożliwiające dalsze współdziałanie bibliotek. W takim wypadku konieczne będzie zaktualizowanie referencji w dodatku do nowszej wersji bibliotek Soneta.

# Współpraca
W celu zaproponowania zmian należy stworzyć Pull Request do gałęzi develop. Po podjęciu decyzji o wydaniu nowej wersji branch develop zostanie zmergowany do mastera i dodatek zostanie automatycznie wydany. 
1. Po poprawnym zbudowaniu Sdk ustawiamy się w konsoli na ścieżce \bin\Release
2. Wykonujemy push do naszego lokalnego folderu z paczkami: 
```powershell
dotnet nuget push nazwaPaczki.nupkg -s C:\Users....nuget\packages
``` 
3. Dzięki powyższym czynnościom nasze SDK będzie widoczne dla naszych nowo utworzonych projektów. 

# Proces wydawania nowych wersji
Dokument [instrukcja wydania] szczegółowo opisuje potrzebne czynności zmierzające do wydania nowej wersji.

[instrukcja wydania]: RELEASING.md
