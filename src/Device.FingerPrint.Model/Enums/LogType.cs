namespace Device.FingerPrint.Model.Enums
{
    public enum LogType
    {
        SessionKeyNull = 1,
        SessionKeyEmpty = 2,
        RedisValueEmpty = 3,
        SessionKeyColon = 4,
        SessionKeyParse = 5,
        TaggingLength = 6,
        TaggingEncrypt = 7,
        ScriptVersionOld = 8,
        ScriptVersionParse = 9
    }
}
