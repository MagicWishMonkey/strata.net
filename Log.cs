using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Text;


namespace Strata {
    public static class Log {
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
            Trace("");
        }

        public static void Debug(Exception ex) {
            string msg = (ex == null) ? "" : ex.Message;
            Write(msg);
        }

        public static void Debug(params object[] args) {
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
