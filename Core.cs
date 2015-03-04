using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Strata.DB;
using Strata.Util;
using Strata.Containers;



namespace Strata {
    public class Core {
        private static Core _instance;

        private Database[] _databases;
        internal Core() {
            var core = Core._instance;
            if (core == null)
                return;
            this._databases = core._databases;
        }

        public Database Database {
            get {
                if (this._databases == null) {
                    return null;
                }
                return this._databases[0];
            }
        }

        public Database[] Databases {
            get { return this._databases; }
        }


        private static IDictionary<string, dynamic> ParseSettings(string settingsFilePath = null, string settingsOverrideFilePath = null) {
            if (settingsFilePath != null) {
                var settingsFile = Toolkit.CurrentDirectory.Parent.File(settingsFilePath);
                if (settingsFile.Exists) {
                    var settingsData = settingsFile.ReadText();
                    var settings = (IDictionary<string, dynamic>)Toolkit.UnJson(settingsData.Trim());
                    if (settingsOverrideFilePath == null) {
                        settingsOverrideFilePath = settingsFile.Parent.File("settings.override.json").FullPath;
                    }
                    if (settingsOverrideFilePath != null) {
                        var settingsOverrideFile = Toolkit.CurrentDirectory.Parent.File(settingsOverrideFilePath);
                        if (settingsOverrideFile.Exists) {
                            var settingsOverrideData = settingsOverrideFile.ReadText();
                            var settingsOverride = (IDictionary<string, dynamic>)Toolkit.UnJson(settingsOverrideData.Trim());
                            Toolkit.Override(settings, settingsOverride);
                        }
                    }

                    IDictionary<string, dynamic> node = null;
                    try {
                        node = (IDictionary<string, dynamic>)settings["databases"];
                    } catch (Exception ex) { }

                    if (node != null) {
                        var databases = new List<Database>();
                        var keys = node.Keys.ToArray();
                        foreach (var key in keys) {
                            var dbCfg = node[key];
                            var name = (string)key;
                            var cfg = DatabaseConfig.Parse(name, dbCfg);
                            node[key] = cfg;
                        }
                    }

                    return settings;
                }
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Core Initialize(string settingsFilePath = null, string settingsOverrideFilePath = null) {
            if (Core._instance != null) {
                Console.WriteLine("The core instance has already been initialized!");
                return Core._instance;
            }

            var core = new Core();

            var settings = Core.ParseSettings(settingsFilePath, settingsOverrideFilePath);
            if (settings != null) {
                try { 
                    var databases = new List<Database>();
                    foreach(var dbname in ((IDictionary<string, dynamic>)settings["databases"]).Keys){
                        var cfg = settings["databases"][dbname];
                        var db = Database.Connect(cfg);
                        databases.Add(db);
                    }
                    core._databases = databases.ToArray();
                } catch (Exception ex) {
                    throw ex;
                }
            }

            Core._instance = core;
            return core;
        }
    }
}
