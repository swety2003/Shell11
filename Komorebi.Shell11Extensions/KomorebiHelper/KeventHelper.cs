using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO.Pipes;

using Komorebi.Notifications;
using System.IO;
using TestLib.ViewModels;
using System.Windows;

namespace Komorebi.Shell11Extensions.KomorebiHelper
{
    public class KEventHelper
    {
        private ILogger<KEventHelper>? _logger = null;
        private NamedPipeServerStream _serverStream;

        ViewKHViewModel? vm;

        public KEventHelper(NamedPipeServerStream s)
        {
            _serverStream = s;
        }

        public void Init(ViewKHViewModel vm)
        {
            this.vm = vm;
        }

        public static TypeEnum[] WhiteList = new TypeEnum[]
        {
            TypeEnum.Uncloak,
            TypeEnum.AddSubscriberPipe,
        };

        public async Task Watch(CancellationToken token)
        {
            KEventHandlerAttribute.InitEventHandlers();
            if (!_serverStream.IsConnected)
            {
                await _serverStream.WaitForConnectionAsync();
                _logger?.LogDebug("Connected");
            }

            if (_serverStream == null) throw new Exception();

            var sr = new StreamReader(_serverStream);
            while (!sr.EndOfStream || !token.IsCancellationRequested)
            {
                await Task.Yield();
                var line = await sr.ReadLineAsync();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                try
                {
                    var data = Coordinate.FromJson(line);
                    vm.KState = data.State;
                    //Console.WriteLine(data.Event.Type);
                    if (KEventHandlerAttribute.EventHandlers.TryGetValue(data.Event.Type, out var handler))
                    {

                        _logger?.LogDebug($"Event:{data.Event.Type}");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (WhiteList.Contains(data.Event.Type))
                            {
                                handler.Handle(vm, data.State.Monitors);
                            }
                            else
                            {
                                handler.Handle(vm, data.Event.Content);
                            }
                        });
                    }
                    else
                    {
                        _logger?.LogWarning($"Unhandled event:{data.Event.Type}");
                    }

                    //Debug.Assert(data.Event.Type != TypeEnum.FocusWorkspaceNumber);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex.Message, ex);
                    //Debugger.Break();
                }
            }
        }
    }
}
