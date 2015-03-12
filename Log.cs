using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Text;


namespace Strata {
    public enum LogLevels {
        DEBUG = 0,
        INFO = 1,
        WARN = 2,
        ERROR = 3
    }

    public class LogEntry {
        public LogLevels level;
        public string message;
        public string username;
        public DateTime timestamp;

        public LogEntry(LogLevels level, string message) {
            this.timestamp = DateTime.UtcNow;
            this.level = level;
            this.message = message;
            try {
                this.username = Strata.Context.User.Username;
            } catch {
                this.username = "guest";
            }
        }
    }

    public static class Log {
        public static LogLevels LEVEL = LogLevels.DEBUG;


        #region -------- STATIC - WRITE OBJECTS --------
        [Conditional("TRACE")]
        public static void Trace() {
            Trace("");
        }

        [Conditional("TRACE")]
        public static void Write(Exception ex) {
            string msg = (ex == null) ? "" : ex.Message;
            Write(msg);
        }

        [Conditional("TRACE")]
        public static void Trace(params object[] args) {
            if (args == null) {
                Write("");
                return;
            }
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < args.Length; i++) {
                buffer.AppendLine(Convert.ToString(args[i]));
                if (i < args.Length - 1)
                    buffer.AppendLine(" ");
            }
            Write(buffer.ToString());
        }

        [Conditional("TRACE")]
        public static void Trace(object msg) {
            Write(Convert.ToString(msg));
        }
        #endregion


        #region -------- STATIC - DEBUG --------
        public static void Debug() {
            if (Log.LEVEL != LogLevels.DEBUG)
                return;
            Write("");
        }

        public static void Debug(Exception ex) {
            if (Log.LEVEL != LogLevels.DEBUG)
                return;

            string msg = (ex == null) ? "" : ex.Message;
            Write(msg);
        }

        public static void Debug(params object[] args) {
            if (Log.LEVEL != LogLevels.DEBUG)
                return;

            if (args == null) {
                Write("");
                return;
            }
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < args.Length; i++) {
                buffer.AppendLine(Convert.ToString(args[i]));
                if (i < args.Length - 1)
                    buffer.AppendLine(" ");
            }
            Write(buffer.ToString());
        }

        public static void Debug(object msg) {
            if (Log.LEVEL != LogLevels.DEBUG)
                return;

            Write(Convert.ToString(msg));
        }
        #endregion


        #region -------- STATIC - INFO --------
        public static void Info() {
            if ((int)Log.LEVEL > (int)LogLevels.INFO)
                return;
            Write("");
        }

        public static void Info(Exception ex) {
            if ((int)Log.LEVEL > (int)LogLevels.INFO)
                return;
            string msg = (ex == null) ? "" : ex.Message;
            Write(msg);
        }

        public static void Info(params object[] args) {
            if ((int)Log.LEVEL > (int)LogLevels.INFO)
                return;
            if (args == null) {
                Write("");
                return;
            }
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < args.Length; i++) {
                buffer.AppendLine(Convert.ToString(args[i]));
                if (i < args.Length - 1)
                    buffer.AppendLine(" ");
            }
            Write(buffer.ToString());
        }

        public static void Info(object msg) {
            if ((int)Log.LEVEL > (int)LogLevels.INFO)
                return;
            Write(Convert.ToString(msg));
        }
        #endregion


        #region -------- STATIC - Warn --------
        public static void Warn() {
            if ((int)Log.LEVEL > (int)LogLevels.WARN)
                return;
            Write("");
        }

        public static void Warn(Exception ex) {
            if ((int)Log.LEVEL > (int)LogLevels.WARN)
                return;
            string msg = (ex == null) ? "" : ex.Message;
            Write(msg);
        }

        public static void Warn(params object[] args) {
            if ((int)Log.LEVEL > (int)LogLevels.WARN)
                return;
            if (args == null) {
                Write("");
                return;
            }
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < args.Length; i++) {
                buffer.AppendLine(Convert.ToString(args[i]));
                if (i < args.Length - 1)
                    buffer.AppendLine(" ");
            }
            Write(buffer.ToString());
        }

        public static void Warn(object msg) {
            if ((int)Log.LEVEL > (int)LogLevels.WARN)
                return;
            Write(Convert.ToString(msg));
        }
        #endregion

        #region -------- STATIC - Error --------
        public static void Error() {
            if ((int)Log.LEVEL > (int)LogLevels.ERROR)
                return;
            Write("");
        }

        public static void Error(Exception ex) {
            if ((int)Log.LEVEL > (int)LogLevels.ERROR)
                return;
            string msg = (ex == null) ? "" : ex.Message;
            Write(msg);
        }

        public static void Error(params object[] args) {
            if ((int)Log.LEVEL > (int)LogLevels.ERROR)
                return;
            if (args == null) {
                Write("");
                return;
            }
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < args.Length; i++) {
                buffer.AppendLine(Convert.ToString(args[i]));
                if (i < args.Length - 1)
                    buffer.AppendLine(" ");
            }
            Write(buffer.ToString());
        }

        public static void Error(object msg) {
            if ((int)Log.LEVEL > (int)LogLevels.ERROR)
                return;
            Write(Convert.ToString(msg));
        }
        #endregion


        #region -------- STATIC - WRITE --------
        public static void Write(params string[] args) {
            if (args == null) {
                Write("");
                return;
            }
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            //StringBuffer buffer = new StringBuffer();
            for (int i = 0; i < args.Length; i++) {
                buffer.Append(args[i]);
                if (i < args.Length - 1)
                    buffer.Append(" ");
            }
            Write(buffer.ToString());
            buffer = null;
        }

        public static void Write(string msg) {
            Console.WriteLine(msg);
            // DashboardEvents.Trace(msg);
        }
        #endregion
    }
}
