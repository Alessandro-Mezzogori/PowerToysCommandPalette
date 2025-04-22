using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Properties;
using TimeTracker.Services;

namespace TimeTracker.Helpers;

public class SettingsManager : JsonSettingsManager
{
    // ##### Private fields #####
    private static readonly string _namespace = "timetracker";
    private static string Namespaced(string propertyName) => $"{_namespace}.{propertyName}";

    private readonly TextSetting _storageFolder = new TextSetting(
        Namespaced(nameof(StorageFolder)),
        Resources.conf_storagefolder_label,
        Resources.conf_storagefolder_description,
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TimeTracker")
    );

    private readonly TextSetting _filenameTemplate = new TextSetting(
        Namespaced(nameof(FilenameTemplate)),
        Resources.conf_filename_template_label,
        Resources.conf_filename_template_description,
        "{0:yyyyMMdd}_timetracker.txt"
    );

    // #### Public properties #####

    public string? StorageFolder => _storageFolder.Value;
    public string? StateJsonPath => string.IsNullOrWhiteSpace(StorageFolder) ? null : Path.Combine(StorageFolder, ".state.json");
    public string? FilenameTemplate => _filenameTemplate.Value;


    // ##### Impl #####
    internal static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("Microsoft.CmdPal");
        Directory.CreateDirectory(directory);

        return Path.Combine(directory, "settings.json");
    }

    public SettingsManager()
    {
        // JsonSettingsManager 
        FilePath = SettingsJsonPath();

        Settings.Add(_storageFolder);
        Settings.Add(_filenameTemplate);

        LoadSettings();

        Settings.SettingsChanged += (s, a) => this.SaveSettings();
    }
}
