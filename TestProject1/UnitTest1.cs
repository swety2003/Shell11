using GlazeWm.Messages;
using GlazeWM.Shell11Extensions.Services;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            GlazeWmIpcService IpcClient = new GlazeWmIpcService();
            IpcClient.OnError += IpcClient_OnError;
            IpcClient.OnRspData += IpcClient_OnRspData;
            IpcClient.OnSubData += IpcClient_OnSubData;

            IpcClient.Connect();
            while (true)
            {

            }
        }

        [TestMethod]
        [DataRow(@"C:\Users\swetyPC\Desktop\glazeData\Untitled-1.json")]
        public void TestMethod2(string file)
        {
            var txt = File.ReadAllText(file);
            var data = TopLevel.FromJson(txt);
        }

        private void IpcClient_OnSubData(object sender, GlazeWm.Messages.Data data)
        {

        }

        private void IpcClient_OnRspData(object sender, GlazeWm.Messages.Data data)
        {

        }

        private void IpcClient_OnError(object sender, Exception ex)
        {

        }
    }
}