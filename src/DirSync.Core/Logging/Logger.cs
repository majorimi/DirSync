using log4net;

namespace DirSync.Core.Logging
{
    public static class Logger
    {
        public static ILog Current { get; set; }

        static Logger()
        {
            Current = new NullLog();
        }
    }    
}
