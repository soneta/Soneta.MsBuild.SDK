# AGENTS.md

This file is for autonomous coding agents working in this repo.
It summarizes how to build/test and the local code conventions.

## Repo purpose (short)
- MSBuild SDK package for Soneta add-ons.
- Core SDK files live under `src/Soneta.Sdk/Sdk/`.
- Tests for package reference transformations live under `tests/SonetaPackageTransofmations/`.

## Commands

### Build
- Build all projects (CI style):
  - `dotnet build **/*.csproj --configuration Release`
- Pack (after build):
  - `dotnet pack --no-build`

### Tests
- Run all transformation tests:
  - `dotnet test tests/SonetaPackageTransofmations/SonetaPackageTransofmations.Tests/SonetaPackageTransofmations.Tests.csproj`

### Single test
- NUnit name filter (example for the T+ A+ case):
  - `dotnet test tests/SonetaPackageTransofmations/SonetaPackageTransofmations.Tests/SonetaPackageTransofmations.Tests.csproj --filter "Name~T+ A+"`
- You can also filter by fully qualified name if needed:
  - `--filter "FullyQualifiedName~PackageReferenceTransformTests"`

### Manual msbuild preprocess (no restore)
- Manual inspection for one test project:
  - `MSBuildSDKsPath="$PWD/src" dotnet msbuild tests/SonetaPackageTransofmations/TestProjects/Addon.Test.csproj /pp:/tmp/Addon.Test.pp.csproj /nologo`
- Manual dump of PackageReference list (used by tests):
  - `MSBuildSDKsPath="$PWD/src" dotnet msbuild tests/SonetaPackageTransofmations/TestProjects/Addon.Test.csproj /t:DumpPackageReferences /p:PackageDumpPath=/tmp/Addon.Test.packages.txt /nologo`

## Test matrix (Sdk.props)
- T+ A+: includes NUnit, Soneta.Products.Test, NUnit3TestAdapter, Microsoft.NET.Test.Sdk, Soneta.Products.Modules.
- T+ A-: includes NUnit, NUnit3TestAdapter, Microsoft.NET.Test.Sdk, NSubstitute, NSubstitute.Analyzers.CSharp, AwesomeAssertions.
- T- A+: includes Soneta.Products.Modules.
- T- A-: includes none of the above.

Notes:
- A- is forced in tests via `/p:IsAddonProject=false`.
- T+ is detected by project name containing `Test` (see Sdk.props).

## Code style (C#)
- Use file-scoped namespaces (`namespace Foo;`).
- Enable nullable reference types; prefer `string?` for optional values.
- Prefer `var` when the type is obvious; use explicit types for clarity when needed.
- Favor `Array.Empty<T>()` over new empty arrays.
- Use `StringComparer.OrdinalIgnoreCase` or `StringComparison.OrdinalIgnoreCase` for case-insensitive comparisons.
- Exceptions: throw `InvalidOperationException` with actionable messages for unrecoverable states.
- Avoid swallowing exceptions unless explicitly safe; if ignored, do it in a narrowly-scoped helper.
- Avoid non-ASCII unless already present in file (use ASCII in new files).

## Code style (MSBuild XML)
- Keep XML indentation with two spaces.
- Group related properties into `<PropertyGroup>`.
- Group items into `<ItemGroup>` and use metadata to drive transforms.
- Prefer conditions on `PropertyGroup`/`ItemGroup` rather than per-item when possible.
- Do not introduce custom tasks unless needed; prefer built-in MSBuild tasks.

## Naming conventions
- Projects: use descriptive names; test project names include `Test` to trigger IsTestProject.
- C# types: PascalCase; methods/properties: PascalCase; locals/params: camelCase.
- Files: PascalCase for classes; `Sdk.props`, `Sdk.targets` for MSBuild SDK files.

## Error handling and logging
- For MSBuild utilities, prefer failing fast with clear error messages.
- For tests, keep assertions specific and include the missing/unexpected item.

## Repo-specific notes
- SDK content is packaged from `src/Soneta.Sdk/Sdk/**`.
- CI uses Windows (see `azure-pipelines.yml`), but local tests are cross-platform.
- No Cursor rules and no Copilot instructions were found in this repo.

## Adding new tests
- Place test projects under `tests/SonetaPackageTransofmations/TestProjects/`.
- Add assertions in `PackageReferenceTransformTests`.
- Keep tests no-restore; use `MSBuildSDKsPath` overlay logic.

## Suggested agent workflow
- Read `src/Soneta.Sdk/Sdk/Sdk.props` before modifying test expectations.
- Update `tests/SonetaPackageTransofmations/README.md` if commands change.
- Run the single test target when changing one case; run full test suite before PR.
