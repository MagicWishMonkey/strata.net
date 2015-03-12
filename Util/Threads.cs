using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Strata.Util {
    public class Worker {
        private FN _fn;
        private Callback _callback;
        private Thread _thread;

        public Worker(FN fn) {
            this._fn = fn;
            this._callback = null;
            this._thread = null;
        }

        public Worker(FN fn, Callback callback) {
            this._fn = fn;
            this._callback = callback;
            this._thread = null;
        }

        public Worker Launch() {
            this._thread = new Thread(this.Run);
            this._thread.IsBackground = true;
            this._thread.Start();
            return this;
        }

        private void Run() {
            try {
                this._fn();
                
                //if (this._callback != null) {
                //    try {
                //        this._callback();
                //    } catch { }
                //}
            } catch (Exception ex) {
                Strata.Log.Error(new Exception("Error running worker function: " + ex.Message, ex));
            }

            this._thread = null;
            this._fn = null;
            this._callback = null;
        }

    }
}
