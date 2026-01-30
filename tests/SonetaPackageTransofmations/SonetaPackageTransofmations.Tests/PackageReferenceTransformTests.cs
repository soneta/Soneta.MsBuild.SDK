using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace SonetaPackageTransofmations.Tests;

public class PackageReferenceTransformTests
{
    private static readonly string[] AllPackages =
    [
        "NUnit",
        "Soneta.Products.Test",
        "NUnit3TestAdapter",
        "Microsoft.NET.Test.Sdk",
        "NSubstitute",
        "NSubstitute.Analyzers.CSharp",
        "AwesomeAssertions",
        "Soneta.Products.Modules"
    ];

    private static IEnumerable<TestCaseData> Cases()
    {
        yield return new TestCaseData(
                "tests/SonetaPackageTransofmations/TestProjects/Addon.Test.csproj",
                new[]
                {
                    "NUnit",
                    "Soneta.Products.Test",
                    "NUnit3TestAdapter",
                    "Microsoft.NET.Test.Sdk",
                    "Soneta.Products.Modules"
                },
                Array.Empty<string>())
            .SetName("T+ A+ (Addon.Test)");

        yield return new TestCaseData(
                "tests/SonetaPackageTransofmations/TestProjects/Core.Test.csproj",
                new[]
                {
                    "NUnit",
                    "NUnit3TestAdapter",
                    "Microsoft.NET.Test.Sdk",
                    "NSubstitute",
                    "NSubstitute.Analyzers.CSharp",
                    "AwesomeAssertions"
                },
                new[] { "/p:IsAddonProject=false" })
            .SetName("T+ A- (Core.Test)");

        yield return new TestCaseData(
                "tests/SonetaPackageTransofmations/TestProjects/Addon.csproj",
                new[]
                {
                    "Soneta.Products.Modules"
                },
                Array.Empty<string>())
            .SetName("T- A+ (Addon)");

        yield return new TestCaseData(
                "tests/SonetaPackageTransofmations/TestProjects/Core.csproj",
                Array.Empty<string>(),
                new[] { "/p:IsAddonProject=false" })
            .SetName("T- A- (Core)");
    }

    [TestCaseSource(nameof(Cases))]
    public void PreprocessAddsExpectedPackageReferences(string relativeProjectPath, string[] expectedPackages, string[] msbuildArguments)
    {
        var packageReferences = GetPackageReferences(relativeProjectPath, msbuildArguments);
        var expected = expectedPackages.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var unexpected = AllPackages.Except(expected, StringComparer.OrdinalIgnoreCase).ToArray();

        Assert.Multiple(() =>
        {
            foreach (var package in expected)
            {
                Assert.That(packageReferences, Does.Contain(package), $"Missing package reference: {package}");
            }

            foreach (var package in unexpected)
            {
                Assert.That(packageReferences, Does.Not.Contain(package), $"Unexpected package reference: {package}");
            }
        });
    }

    private static HashSet<string> GetPackageReferences(string relativeProjectPath, string[] msbuildArguments)
    {
        var repoRoot = FindRepoRoot();
        var projectPath = Path.Combine(repoRoot, relativeProjectPath);
        var dumpPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.packageref.txt");

        try
        {
            RunMsbuildDump(repoRoot, projectPath, dumpPath, msbuildArguments);

            var lines = File.Exists(dumpPath)
                ? File.ReadAllLines(dumpPath)
                : Array.Empty<string>();

            return lines
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            if (File.Exists(dumpPath))
            {
                File.Delete(dumpPath);
            }
        }
    }

    private static void RunMsbuildDump(string repoRoot, string projectPath, string dumpPath, string[] msbuildArguments)
    {
        var sdkOverlayPath = CreateSdkOverlay(repoRoot);
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = repoRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            startInfo.ArgumentList.Add("msbuild");
            startInfo.ArgumentList.Add(projectPath);
            startInfo.ArgumentList.Add("/nologo");
            startInfo.ArgumentList.Add("/v:m");
            startInfo.ArgumentList.Add($"/t:DumpPackageReferences");
            startInfo.ArgumentList.Add($"/p:PackageDumpPath={dumpPath}");
            startInfo.Environment["MSBuildSDKsPath"] = sdkOverlayPath;

            foreach (var argument in msbuildArguments)
            {
                if (!string.IsNullOrWhiteSpace(argument))
                {
                    startInfo.ArgumentList.Add(argument);
                }
            }

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start dotnet msbuild.");
            }

            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"dotnet msbuild failed with exit code {process.ExitCode}.{Environment.NewLine}{stdout}{Environment.NewLine}{stderr}");
            }
        }
        finally
        {
            TryDeleteDirectory(sdkOverlayPath);
        }
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory != null)
        {
            var marker = Path.Combine(directory.FullName, "src", "Soneta.Sdk", "Sdk", "Sdk.props");
            if (File.Exists(marker))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root not found (missing src/Soneta.Sdk/Sdk/Sdk.props).");
    }

    private static string CreateSdkOverlay(string repoRoot)
    {
        var overlayRoot = Path.Combine(Path.GetTempPath(), $"soneta-sdk-overlay-{Guid.NewGuid():N}");
        Directory.CreateDirectory(overlayRoot);

        var sonetaSdkSource = Path.Combine(repoRoot, "src", "Soneta.Sdk", "Sdk");
        var sonetaSdkTarget = Path.Combine(overlayRoot, "Soneta.Sdk", "Sdk");
        CopySdkFiles(sonetaSdkSource, sonetaSdkTarget);

        var microsoftSdkTarget = Path.Combine(overlayRoot, "Microsoft.NET.Sdk", "Sdk");
        Directory.CreateDirectory(microsoftSdkTarget);
        WriteMinimalSdkFile(Path.Combine(microsoftSdkTarget, "Sdk.props"));
        WriteMinimalSdkFile(Path.Combine(microsoftSdkTarget, "Sdk.targets"));

        return overlayRoot;
    }

    private static void CopySdkFiles(string sourceDirectory, string targetDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
        {
            throw new InvalidOperationException($"SDK source directory not found: {sourceDirectory}");
        }

        Directory.CreateDirectory(targetDirectory);
        foreach (var file in Directory.GetFiles(sourceDirectory))
        {
            var fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(targetDirectory, fileName), true);
        }
    }

    private static void WriteMinimalSdkFile(string path)
    {
        const string contents = "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\" />";
        File.WriteAllText(path, contents);
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
        catch
        {
        }
    }
}
