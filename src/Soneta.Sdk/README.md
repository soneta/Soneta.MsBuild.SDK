Soneta.Sdk
Sdk stworzone przez firmê Soneta pozwalaj¹ce automatycznie skonfigurowaæ oraz uzupe³niæ projekty dodatków o niezbêdne elementy potrzebne do wspó³pracy z oprogramowaniem enova oraz triva.

Sposób zaimplementowania/Example:
W celu zaimportowania Soneta.Sdk w projekcie importuj¹cym w pliku nazwaprojektu.csproj w linii dotycz¹cej projektu ustawiamy: <Project Sdk="Soneta.Sdk/numerWersji">.
Po zapisie zmian zostanie wykonana automatyczna konfiguracja projektu.

W przypadku gdy w solucji znajdujê siê wiele projektów, by unikn¹æ koniecznoœci zmian w ka¿dym projekcie numeru wersji, istnieje mo¿liwoœæ stworzenia pliku nadrzêdnego 
global.json (poza projektami) o zawartoœci:
 {
  "msbuild-sdks": {
    "Soneta.Sdk": "numerWersji"
  }
}
Dziêki temu w plikach .csproj wymagany bêdzie tylko wpis <Project Sdk="Soneta.Sdk"> bez koniecznoœci umieszczania numeru wersji.


For Contributors: 
1) Po poprawnym zbudowaniu Sdk ustawiamy siê w cmd na œcie¿ce **\bin\Release
2) Wpisujemy komendê -> dotnet nuget push nazwaPaczki.nupkg -s C:\Users\...\.nuget\packages  (pushujemy do naszego lokalnego folderu z paczkami)
3) Dziêki powy¿szym czynnoœciom nasze SDK bêdzie widoczne dla naszych nowo utworzonych projektów.