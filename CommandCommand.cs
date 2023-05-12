using System.CommandLine;
using System.Xml;

namespace Cqrs;

public class CommandCommand : Command
{
    public CommandCommand() : base("command", "create CQRS Command, CommandHandler")
    {
        var nameOption = new Option<string>(new []{"--name", "-n"}, "create CQRS Command, CommandHandler with given name");
        var typeOption = new Option<string>(new []{"--type", "-t"}, "create CQRS command, CommandHandler with given return type");
        var nsOption = new Option<string>(new []{"--namespace", "-ns"}, "create CQRS command, CommandHandler with given namespace");
        Add(nameOption);
        Add(typeOption);
        Add(nsOption);
        this.SetHandler(Handle, nameOption, typeOption, nsOption);
    }

    private static void Handle(string name, string type, string ns)
    {
        name = name.EndsWith("Command") ? name : $"{name}Command";
        ns = TryGetNamespace(ns);

        var hasReturnType = !string.IsNullOrWhiteSpace(type);
        File.WriteAllText($"{name}.cs", $@"using MediatR;
namespace {ns}
{{
    public class {name} : {(hasReturnType ? $"IRequest<{type}>" : "IRequest")}
    {{
        
    }}
}}");

        File.WriteAllText($"{name}Handler.cs", $@"using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace {ns}
{{
    public class {name}Handler : {(hasReturnType ? $"IRequestHandler<{name}, {type}>" : $"IRequestHandler<{name}>")}
    {{
        public async Task{(hasReturnType ? $"<{type}>" : "<Unit>")} Handle({name} request, CancellationToken cancellationToken)
        {{
            {(hasReturnType ? string.Empty : "return Unit.Value;")}
        }}
    }}
}}");
    }

    private static string TryGetNamespace(string ns)
    {
        if (!string.IsNullOrWhiteSpace(ns))
        {
            return ns;
        }

        int i = 0;

        var directories = new List<string>(6);
        
        while (i < 5)
        {
            try
            {
                var path = "../".Repeat(i);
                if (string.IsNullOrEmpty(path))
                {
                    path = "./";
                }
                
                var file = Directory.GetFiles(path, "*.csproj").FirstOrDefault();
                if (string.IsNullOrEmpty(file))
                {
                    directories.Add(new DirectoryInfo(path).Name);
                    continue;
                }

                var xd = new XmlDocument();
                xd.Load(file);
                var node = xd.SelectSingleNode("descendant::PropertyGroup/RootNamespace")?.FirstChild;
                if (node != null && !string.IsNullOrWhiteSpace(node.Value))
                {
                    directories.Add(node.Value);
                }
                else
                {
                    directories.Add(Path.GetFileNameWithoutExtension(file));
                }

                directories.Reverse();
                return string.Join(".", directories);
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

        return "Commands";
    }
}