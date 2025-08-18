namespace Device.FingerPrint.Model
{
    public enum FlaggedStatus
    {
        human = 1,
        bot_fake_os = 2,
        bot_fake_browser = 3,
        undefined = 6,
        bot_headless = 9
    }
}
