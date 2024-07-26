using CommunityToolkit.Mvvm.ComponentModel;
using CZGL.SystemInfo;
using System;
using System.Diagnostics;

namespace Shell11.MenuBarExtensions.ViewModels
{
    public partial class TrafficMonitorViewModel : ViewModelBase
    {
        [ObservableProperty] private int cpuLoad;

        [ObservableProperty] private NetItem download;
        private NetworkInfo network;
        private Rate oldRate;
        [ObservableProperty] private int ramLoad;
        [ObservableProperty] private NetItem upload;
        private CPUTime v1;

        public override void Init()
        {
            base.Init();


            v1 = CPUHelper.GetCPUTime();
            network = NetworkInfo.TryGetRealNetworkInfo() ?? throw new Exception();
            oldRate = network.GetIpv4Speed();
        }

        public void OpenTaskmgr()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.Arguments = "/c taskmgr";
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }

        public override void Update()
        {
            _Timer_Tick();
        }


        private void _Timer_Tick()
        {
            var v2 = CPUHelper.GetCPUTime();
            var value = CPUHelper.CalculateCPULoad(v1, v2);
            v1 = v2;

            var memory = MemoryHelper.GetMemoryValue();
            var newRate = network.GetIpv4Speed();
            var speed = NetworkInfo.GetSpeed(oldRate, newRate);
            oldRate = newRate;

            CpuLoad = (int)(value * 100);
            RamLoad = (int)memory.UsedPercentage;
            Upload = new NetItem(speed.Sent.Size.ToString(), speed.Sent.SizeType.ToString());
            Download = new NetItem(speed.Received.Size.ToString(), speed.Received.SizeType.ToString());
        }


        public record NetItem(string size, string type);
    }
}