using System;
using Device.FingerPrint.Model;

namespace TestApp
{
    class MurmurHashTest
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing MurmurHash implementation in APFingerprintModel...");
            
            var fingerprint = new APFingerprintModel
            {
                Au = "test audio data",
                Br = "test browser data", 
                Hw = "test hardware data",
                Pf = "test preferences data",
                Gp = "test graphics data"
            };
            
            fingerprint.GenerateGroupHashCodes();
            
            Console.WriteLine($"AudioCode: {fingerprint.AudioCode} (Length: {fingerprint.AudioCode?.Length})");
            Console.WriteLine($"BrowserCode: {fingerprint.BrowserCode} (Length: {fingerprint.BrowserCode?.Length})");
            Console.WriteLine($"HardwareCode: {fingerprint.HardwareCode} (Length: {fingerprint.HardwareCode?.Length})");
            Console.WriteLine($"PreferencesCode: {fingerprint.PreferencesCode} (Length: {fingerprint.PreferencesCode?.Length})");
            Console.WriteLine($"GraphicCode: {fingerprint.GraphicCode} (Length: {fingerprint.GraphicCode?.Length})");
            
            // Test with null input
            var fingerprintWithNull = new APFingerprintModel
            {
                Au = null,
                Br = "test browser data only"
            };
            
            fingerprintWithNull.GenerateGroupHashCodes();
            Console.WriteLine($"\nWith null input:");
            Console.WriteLine($"AudioCode (null input): {fingerprintWithNull.AudioCode}");
            Console.WriteLine($"BrowserCode: {fingerprintWithNull.BrowserCode} (Length: {fingerprintWithNull.BrowserCode?.Length})");
            
            // Test same input produces same hash
            var fingerprint2 = new APFingerprintModel
            {
                Au = "test audio data"
            };
            fingerprint2.GenerateGroupHashCodes();
            
            Console.WriteLine($"\nConsistency test:");
            Console.WriteLine($"First hash:  {fingerprint.AudioCode}");
            Console.WriteLine($"Second hash: {fingerprint2.AudioCode}");
            Console.WriteLine($"Are equal: {fingerprint.AudioCode == fingerprint2.AudioCode}");
        }
    }
}
