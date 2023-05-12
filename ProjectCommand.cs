using System.CommandLine;
using System.Diagnostics;

namespace Cqrs;

public class ProjectCommand : Command
{
    public ProjectCommand() : base("project", "create CQRS pattern project")
    {
        var nameOption = new Option<string>(new[] { "-n", "--name" }, "Create CQRS pattern project with given name like: Application, Domain, Infrastructure");
        var targetFrameworkOption = new Option<string>(new[] { "-t", "-tf", "--TargetFramework" },
            "Create CQRS pattern project with given <TargetFramework> like net6.0");
        targetFrameworkOption.SetDefaultValue("net6.0");
        var slnOption = new Option<string[]>(new[] { "-s", "--sln" }, "Add CQRS pattern project to specified sln path");
        AddOption(nameOption);
        AddOption(targetFrameworkOption);
        AddOption(slnOption);
        this.SetHandler(Handle, nameOption, targetFrameworkOption, slnOption);
    }

    private static void Handle(string name, string targetFramework, string[]? slnPaths)
    {
        var applicationName = $"{name}.Application";
        var domainName = $"{name}.Domain";
        var infrastructure = $"{name}.Infrastructure";
        Directory.CreateDirectory(applicationName);
        Directory.CreateDirectory(domainName);
        Directory.CreateDirectory(infrastructure);

        File.WriteAllText($"{applicationName}/{applicationName}.csproj", $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>{targetFramework}</TargetFramework>
  </PropertyGroup>

  <PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Debug|AnyCPU'"">
    <DocumentationFile>bin\Debug\{applicationName}.xml</DocumentationFile>
    <NoWarn>1701;1702;1591</NoWarn>
  </PropertyGroup>

  <PropertyGroup Condition="" '$(Configuration)' == 'Release' "">
    <DocumentationFile>bin\Release\{applicationName}.xml</DocumentationFile>
    <NoWarn>1701;1702;1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""MediatR"" Version=""12.0.1"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\{infrastructure}\{infrastructure}.csproj"" />
  </ItemGroup>

</Project>
");
        File.WriteAllText($"{domainName}/{domainName}.csproj", $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>{targetFramework}</TargetFramework>
  </PropertyGroup>

</Project>
");
        File.WriteAllText($"{infrastructure}/{infrastructure}.csproj", $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>{targetFramework}</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include=""..\{domainName}\{domainName}.csproj"" />
  </ItemGroup>

</Project>
");
        if (slnPaths == null || slnPaths.Length == 0)
        {
            var slnPath = GetSlnPath();
            if (string.IsNullOrEmpty(slnPath))
            {
                return;
            }
            AddProjectsToSln(applicationName, domainName, infrastructure, slnPath);
            return;
        }

        AddProjectsToSln(applicationName, domainName, infrastructure, slnPaths);
    }

    private static void AddProjectsToSln(string applicationName, string domainName, string infrastructure, params string[] slnPaths)
    {
        foreach (var slnPath in slnPaths)
        {
            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments =
                $"sln {slnPath} add {applicationName}/{applicationName}.csproj {domainName}/{domainName}.csproj {infrastructure}/{infrastructure}.csproj";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (_, e) => Console.WriteLine(e.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
        }
    }

    private static string GetSlnPath()
    {
        var i = 0;
        while (i <= 10)
        {
            var path = "../".Repeat(i);
            if (string.IsNullOrEmpty(path))
            {
                path = "./";
            }

            try
            {
                var file = Directory.GetFiles(path, "*.sln").FirstOrDefault();
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                var fileInfo = new FileInfo(file);
                return $"{path}{fileInfo.Name}";
            }
            catch
            {
                // ignored
            }
            finally
            {
                i++;
            }
        }

        return string.Empty;
    }
}