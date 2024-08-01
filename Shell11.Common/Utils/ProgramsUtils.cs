using ManagedShell.Common.Enums;
using ManagedShell.Common.Helpers;
using ManagedShell.Common.Logging;
using ManagedShell.ShellFolders.Interfaces;
using ManagedShell.ShellFolders;
using System.IO;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
using ManagedShell.WindowsTasks;

namespace Shell11.Common.Utils
{
    //public static class ProgramsUtils
    //{
    //    public static ObservableCollection<ApplicationInfo> PinnedPrograms { get; } = 
    //        new ObservableCollection<ApplicationInfo>();

    //    public static void LaunchProgramVerb(ApplicationInfo app, string verb)
    //    {
    //        if (app == null)
    //        {
    //            return;
    //        }

    //        if (!ShellHelper.StartProcess(app.Path, "", verb, getAppParentDirectory(app)))
    //        {
    //            OnFileNotFound();
    //        }
    //    }

    //    public static void LaunchProgramAdmin(ApplicationInfo app)
    //    {
    //        if (app == null)
    //        {
    //            return;
    //        }

    //        if (!app.AllowRunAsAdmin)
    //        {
    //            LaunchProgram(app);
    //            return;
    //        }

    //        LaunchProgramVerb(app, "runas"); 
    //    }

    //    static void OnFileNotFound()
    //    {
    //        MessageBox.Show("Error_FileNotFoundInfo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //    }


    //    private static string getAppParentDirectory(ApplicationInfo app)
    //    {
    //        if (app.IsStoreApp) return null;

    //        string parent = null;

    //        try
    //        {
    //            parent = Directory.GetParent(app.Path).FullName;
    //        }
    //        catch (Exception e)
    //        {
    //            ShellLogger.Warning($"AppGrabberService: Unable to get parent folder for {app.Path}: {e.Message}");
    //        }

    //        return parent;
    //    }

    //    public static void LaunchProgram(ApplicationInfo app)
    //    {
    //        if (app == null)
    //        {
    //            return;
    //        }

    //        else if (EnvironmentHelper.IsAppRunningAsShell && app.Target.ToLower().EndsWith("explorer.exe"))
    //        {
    //            // special case: if we are shell and launching explorer, give it a parameter so that it doesn't do shell things.
    //            if (!ShellHelper.StartProcess(app.Path, ShellFolderPath.ComputerFolder.Value))
    //            {
    //                OnFileNotFound();
    //            }
    //        }
    //        else
    //        {
    //            // Store apps that are FullTrust can be activated, which works even without Explorer
    //            // Non-FullTrust apps will not launch without Explorer and will hang us, so don't use activation for them
    //            if (app.IsStoreApp && app.AllowRunAsAdmin)
    //            {
    //                if (!ShellHelper.ActivateApplication(app.Target))
    //                {
    //                OnFileNotFound();
    //                }
    //            }
    //            else if (!ShellHelper.StartProcess(app.Path, workingDirectory: getAppParentDirectory(app)))
    //            {
    //                OnFileNotFound();
    //            }
    //        }
    //    }

    //    public static readonly string[] ExecutableExtensions = {
    //            ".exe",
    //            ".bat",
    //            ".com",
    //            ".lnk",
    //            ".msc",
    //            ".appref-ms",
    //            ".url"
    //        };
    //    private static readonly string[] excludedNames = { "documentation", "help", "install", "more info", "read me", "read first", "readme", "remove", "setup", "what's new", "support", "on the web", "safe mode" };

    //    public static ApplicationInfo PathToApp(string filePath, string fileDisplayName, bool allowNonApps, bool allowExcludedNames)
    //    {
    //        ApplicationInfo ai = new ApplicationInfo();
    //        ai.AllowRunAsAdmin = true;
    //        string fileExt = Path.GetExtension(filePath);

    //        if (!(allowNonApps || ExecutableExtensions.Contains(fileExt, StringComparer.OrdinalIgnoreCase)))
    //        {
    //            return null;
    //        }

    //        try
    //        {
    //            ai.Name = fileDisplayName;
    //            ai.Path = filePath;
    //            string target;

    //            if (fileExt.Equals(".lnk", StringComparison.OrdinalIgnoreCase))
    //            {
    //                try
    //                {
    //                    // Get the target from the link
    //                    IShellLink link = ShellLinkHelper.Load(IntPtr.Zero, filePath);

    //                    StringBuilder builder = new StringBuilder(260);
    //                    link.GetPath(builder, 260, out ManagedShell.ShellFolders.Structs.WIN32_FIND_DATA pfd, ManagedShell.ShellFolders.Enums.SLGP_FLAGS.SLGP_RAWPATH);

    //                    target = builder.ToString();
    //                }
    //                catch (Exception ex)
    //                {
    //                    ShellLogger.Error($"AppGrabberService: Error resolving link target for {filePath}", ex);
    //                    target = filePath;
    //                }
    //            }
    //            else
    //            {
    //                target = filePath;
    //            }

    //            ai.Target = target;

    //            // remove items that we can't execute.
    //            if (!allowNonApps)
    //            {
    //                if (!string.IsNullOrEmpty(target) && !ExecutableExtensions.Contains(Path.GetExtension(target), StringComparer.OrdinalIgnoreCase))
    //                {
    //                    ShellLogger.Debug($"AppGrabberService: Not an app: {filePath}: {target}");
    //                    return null;
    //                }

    //                // remove things that aren't apps (help, uninstallers, etc)
    //                if (!allowExcludedNames)
    //                {
    //                    foreach (string word in excludedNames)
    //                    {
    //                        if (ai.Name.ToLower().Contains(word))
    //                        {
    //                            ShellLogger.Debug($"AppGrabberService: Excluded item: {filePath}: {target}");
    //                            return null;
    //                        }
    //                    }
    //                }
    //            }

    //            return ai;
    //        }
    //        catch (Exception ex)
    //        {
    //            ShellLogger.Error($"AppGrabberService: Error creating ApplicationInfo object: {ex.Message}");
    //            return null;
    //        }
    //    }

    //    public static ApplicationInfo PathToApp(string filePath, bool allowNonApps, bool allowExcludedNames)
    //    {
    //        string fileDisplayName = ShellHelper.GetDisplayName(filePath);

    //        if (filePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
    //        {
    //            try
    //            {
    //                fileDisplayName = FileVersionInfo.GetVersionInfo(filePath).FileDescription;
    //            }
    //            catch (Exception e)
    //            {
    //                ShellLogger.Warning($"AppGrabberService: Unable to get file description for {filePath}: {e.Message}");
    //            }
    //        }

    //        return PathToApp(filePath, fileDisplayName, allowNonApps, allowExcludedNames);
    //    }

    //    public static IEnumerable<ApplicationInfo> GetByPath(string[] fileNames)
    //    {

    //        foreach (string fileName in fileNames)
    //        {
    //            if (!ShellHelper.Exists(fileName))
    //            {
    //                continue;
    //            }

    //            ApplicationInfo customApp = PathToApp(fileName, false, true);
    //            if (ReferenceEquals(customApp, null))
    //            {
    //                continue;
    //            }
    //            yield return customApp;


    //        }
    //    }

    //    public static void InsertByPath(string[] fileNames, int index)
    //    {
    //        foreach (string fileName in fileNames)
    //        {
    //            if (!ShellHelper.Exists(fileName))
    //            {
    //                continue;
    //            }

    //            ApplicationInfo customApp = PathToApp(fileName, false, true);
    //            if (ReferenceEquals(customApp, null))
    //            {
    //                continue;
    //            }

    //            if (index >= 0) PinnedPrograms.Insert(index, customApp);
    //            else PinnedPrograms.Add(customApp);
    //            //count++;
    //        }

    //    }

    //    public static ApplicationInfo? GetQuickLaunchApplicationInfo(ApplicationWindow window)
    //    {
    //        foreach (ApplicationInfo ai in PinnedPrograms)
    //        {
    //            if (ai.Target.ToLower() == window.WinFileName.ToLower() || (window.IsUWP && ai.Target == window.AppUserModelID))
    //            {
    //                return ai;
    //            }

    //            if (window.Title.ToLower().Contains(ai.Name.ToLower()))
    //            {
    //                return ai;
    //            }
    //        }

    //        return null;

    //    }

    //    public static void AddToQuickLaunch(bool isUWP, string path)
    //    {
    //        if (isUWP)
    //        {
    //            // store app, do special stuff
    //            ProgramsUtils.AddStoreApp(path);
    //        }
    //        else
    //        {
    //            ProgramsUtils.AddByPath(new string[] { path });
    //        }
    //    }

    //    public static void AddByPath(string[] fileNames)
    //    {
    //        InsertByPath(fileNames, -1);
    //    }

    //    public static void AddStoreApp(string appUserModelId)
    //    {
    //        var storeApp = ManagedShell.UWPInterop.StoreAppHelper.AppList.GetAppByAumid(appUserModelId);

    //        if (storeApp == null)
    //        {
    //            return;
    //        }

    //        ApplicationInfo ai = ApplicationInfo.FromStoreApp(storeApp);

    //        PinnedPrograms .Add(ai);    
    //        // add it
    //        //if (!ReferenceEquals(ai, null))
    //        //{
    //        //    CategoryList.GetSpecialCategory(categoryType).Add(ai);
    //        //    Save();
    //        //}
    //    }
    //}
}
