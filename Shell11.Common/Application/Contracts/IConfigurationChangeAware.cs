namespace Shell11.Common.Application.Contracts
{
    public interface IConfigurationChangeAware
    {
        void HandleSettingChange(string setting);
    }
}