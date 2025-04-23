using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Commands;

internal sealed partial class ExtensionInfoCommand : InvokableCommand
{
    public override string Name => "Extension info";
    public override IconInfo Icon => new("\uE8A7");

    public override ICommandResult Invoke()
    {
        var info = $"""
        Extension info:
            Logging folder: {ApplicationData.GetDefault().TemporaryFolder.Path}
        """;

        _ = MessageBox(0, info, "Extension info", 0x00001000);
        return CommandResult.KeepOpen();
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int MessageBox(
        IntPtr hWnd,
        string lpText,
        string lpCaption,
        uint uType
    );
}
