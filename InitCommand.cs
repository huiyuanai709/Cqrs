using System.CommandLine;

namespace Cqrs;

public class InitCommand : Command
{
    public InitCommand() : base("init", "init cqrs directory")
    {
        this.SetHandler(Handle);
    }

    private static void Handle()
    {
        if (!Directory.Exists("Resources"))
        {
            Directory.CreateDirectory("Resources");
        }

        if (!Directory.Exists("Resources/Commands"))
        {
            Directory.CreateDirectory("Resources/Commands");
        }

        if (!Directory.Exists("Resources/Queries"))
        {
            Directory.CreateDirectory("Resources/Queries");
        }
    }

}