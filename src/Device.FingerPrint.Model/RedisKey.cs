using System;

namespace Device.FingerPrint.Model
{
    public enum RedisKey
    {
        Cookie,
        Headless,
        FingerPrint,
        Header,
        FingerPrintEncode
    }

    public static class RedisString
    {
        public static string GetRedisKeyName(RedisKey redisKey)
        {
            return Enum.GetName(typeof(RedisKey), redisKey);
        }

        public static string[] GetRedisKeyNames()
        {
            return Enum.GetNames(typeof(RedisKey));
        }
    }
}
