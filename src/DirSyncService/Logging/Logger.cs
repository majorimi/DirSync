using System;
using log4net;
using log4net.Core;

namespace DirSyncService.Logging
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
