using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Forms;

namespace TimeTracker.Pages;

internal sealed partial class MySecondPage : ContentPage
{
    public MySecondPage()
    {
        Icon = new IconInfo("\uF147");
        Title = "My Second Page";
        Name = "Open";
    }

    private readonly SampleContentForm _sampleContentForm = new();

    public override IContent[] GetContent()
    {
        return [
            _sampleContentForm,
        ];
    }
}
