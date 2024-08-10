using GlazeWm.Messages;
using GlazeWM.Shell11Extensions.Utils;
using static GlazeWM.Shell11Extensions.Utils.WebSocketClientHelper;

namespace GlazeWM.Shell11Extensions.Services
{
    public class GlazeWmIpcService
    {
        WebSocketClientHelper clientWebSocket { get; set; }


        public delegate void MessageDataEventHandler(object sender, Data data);

        public event MessageDataEventHandler? OnRspData;
        public event MessageDataEventHandler? OnSubData;
        public event ErrorEventHandler? OnError;
        public GlazeWmIpcService()
        {
            var webSocketUrl = $@"ws://localhost:{6123}";
            clientWebSocket = new WebSocketClientHelper(webSocketUrl);
            clientWebSocket.OnError += ClientWebSocket_OnError;
            clientWebSocket.OnMessage += ClientWebSocket_OnMessage;
            clientWebSocket.OnOpen += ClientWebSocket_OnOpen;
        }

        private void ClientWebSocket_OnOpen(object? sender, EventArgs e)
        {
            QueryMonitors();
            SubscribeEvents();
        }

        private void ClientWebSocket_OnMessage(object sender, string data)
        {
            try
            {

                TopLevel msg = TopLevel.FromJson(data) ?? throw new Exception();
                if (msg.MessageType == "client_response")
                    OnRspData?.Invoke(this, msg.Data);
                else
                {
                    OnSubData?.Invoke(this, msg.Data);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void QueryFocused()
        {
            clientWebSocket.Send("query focused");
        }
        public void FocuseWorkspace(int index)
        {
            clientWebSocket.Send($"command focus --workspace {index}");
        }
        public void QueryMonitors()
        {
            clientWebSocket.Send("query monitors");
        }
        public void QueryBindingModes()
        {
            clientWebSocket.Send("query binding-modes");
        }
        public void SubscribeEvents()
        {
            clientWebSocket.Send("sub --events binding_modes_changed focus_changed focused_container_moved tiling_direction_changed workspace_activated workspace_deactivated workspace_updated");
        }

        private void ClientWebSocket_OnError(object sender, Exception ex)
        {
            this.OnError?.Invoke(this, ex);
            //throw new NotImplementedException();
        }

        public void Connect()
        {
            clientWebSocket.Open();
        }

        public void Close()
        {
            clientWebSocket.Close();
        }
    }
}
