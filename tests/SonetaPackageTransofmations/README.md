# SonetaPackageTransofmations

Testy sprawdzaja transformacje `PackageReference` w `Sdk.props` bez wykonywania restore dla projektow testowych.
W przypadku wariantu A- testy wymuszaja `IsAddonProject=false` przez parametr MSBuild.

## Uruchomienie testow

```bash
dotnet test tests/SonetaPackageTransofmations/SonetaPackageTransofmations.Tests/SonetaPackageTransofmations.Tests.csproj
```

## Reczna weryfikacja pojedynczego projektu

```bash
MSBuildSDKsPath="$PWD/src" \
  dotnet msbuild tests/SonetaPackageTransofmations/TestProjects/Addon.Test.csproj \
  /pp:/tmp/Addon.Test.pp.csproj /nologo
```

Po wykonaniu polecenia sprawdz zawartosc `/tmp/Addon.Test.pp.csproj` i obecnosc `PackageReference`.
