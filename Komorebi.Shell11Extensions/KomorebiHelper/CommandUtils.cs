using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text.RegularExpressions;

namespace Komorebi.Shell11Extensions.KomorebiHelper
{
    public class CommandHelper
    {
        //private static ILogger? _logger = Logging.CreateLogger<CommandHelper>();
        private static ILogger? _logger = null;

        public static void StartProcess()
        {
            //CallKomorebic($"quickstart");
            CallKomorebic($"start --whkd");
        }
        static string CamelCaseToKebabCase(string camelCaseString)
        {
            if (camelCaseString == "BSP")
            {
                return camelCaseString.ToLower();
            }
            return Regex.Replace(camelCaseString, "(?<!^)([A-Z])", "-$1").ToLower();
        }

        public static bool Running()
        {
            var p = Process.GetProcessesByName("komorebi");
            return p.Length > 0;
        }
        public static void SetWorkspaceLayout(int workspace, string layout, int monitor = 0)
        {
            CallKomorebic($"workspace-layout {monitor} {workspace} {CamelCaseToKebabCase(layout)}");
        }

        public static void SetWinHideBehavior()
        {
            CallKomorebic("window-hiding-behaviour hide");
        }
        public static void ToggleMonocle()
        {
            CallKomorebic("toggle-monocle");
        }
        public static void StopProcess()
        {
            CallKomorebic("stop");
        }

        private const string K_ROOT = @"";
        private static void CallKomorebic(string args)
        {
            _logger?.LogDebug(args);
            var process = new Process();
            process.StartInfo.WorkingDirectory = Path.Combine(K_ROOT);
            process.StartInfo.FileName = Path.Combine(K_ROOT, "komorebic-no-console.exe");
            process.StartInfo.Arguments = args;
            //process.StartInfo.EnvironmentVariables["USERPROFILE"] = ".";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                _logger?.LogError(e.Message);
            }
        }

        public static void Subscribe(string pipeName)
        {
            CallKomorebic($"subscribe {pipeName}");
        }

        public static void UnSubscribe(string pipeName = PipeServer.pipeName)
        {
            CallKomorebic($"unsubscribe {pipeName}");
        }

        public static void ChangeWorkSpace(int index = 0)
        {
            CallKomorebic($"focus-workspace {index}");
        }

        public static void SendFocusedToWorkSpace(string name)
        {
            CallKomorebic($"send-to-named-workspace {name}");
        }

        public static void RestoreWindows()
        {
            CallKomorebic($"restore-windows");
        }
    }

    public class PipeServer
    {
        private static ILogger? _logger = null;

        public const string pipeName = "/swety/pipe/komorebi-cs";

        public NamedPipeServerStream? PipeServerStream;

        public NamedPipeServerStream Create()
        {
            PipeServerStream =
                new NamedPipeServerStream(pipeName, PipeDirection.InOut);

            _logger?.LogDebug($"PipeServer {pipeName} started, waiting for connection.");

            return PipeServerStream;
        }
    }
}
