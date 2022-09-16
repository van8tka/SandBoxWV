using System;

namespace SandBoxWV
{
    public static class SbLog
    {
        public static void I(string tag, string msg)
        {
            I(tag.ToUpper()+": "+msg);
        }

        public static void I(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }

        public static void E(string tag, Exception e)
        {
            string startMsg = string.Empty;
            if (!string.IsNullOrEmpty(tag))
                startMsg = tag + ": ";
            I(startMsg + e.Message);
            I(tag+": "+e.StackTrace);
            System.Diagnostics.Debugger.Break();
        }

        public static void E(Exception e)
        {
            E(string.Empty, e);
        }
    }
}
