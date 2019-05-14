## Build 
[![Build Status](https://soneta.visualstudio.com/GitHub/_apis/build/status/Soneta.MsBuild.SDK?branchName=master)](https://soneta.visualstudio.com/GitHub/_build/latest?definitionId=2&branchName=master)

# Introduction 
Sdk stworzone przez firmę Soneta pozwalające automatycznie skonfigurować oraz uzupełnić projekty dodatków o niezbędne elementy potrzebne do współpracy z oprogramowaniem enova.

# Getting Started
W celu zaimportowania Soneta.Sdk w projekcie importującym w pliku nazwaprojektu.csproj w linii dotyczącej projektu ustawiamy: 
```xml
   <Project Sdk="Soneta.Sdk/numerWersji">  
```
Po zapisie zmian zostanie wykonana automatyczna konfiguracja projektu.

W przypadku gdy w solucji znajduję się wiele projektów, by uniknąć konieczności zmian w każdym projekcie numeru wersji, istnieje możliwość stworzenia pliku nadrzędnego global.json (poza projektami) o zawartości: 
```json
   { 
     "msbuild-sdks": { 
        "Soneta.Sdk": "numerWersji" 
      }
   } 
```
 Dzięki czemu w plikach .csproj wymagany będzie tylko wpis bez konieczności umieszczania numeru wersji.

# Contribute
1. Po poprawnym zbudowaniu Sdk ustawiamy się w konsoli na ścieżce \bin\Release
2. Wykonujemy push do naszego lokalnego folderu z paczkami: 
```powershell
dotnet nuget push nazwaPaczki.nupkg -s C:\Users....nuget\packages
``` 
3. Dzięki powyższym czynnościom nasze SDK będzie widoczne dla naszych nowo utworzonych projektów. 


