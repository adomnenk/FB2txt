using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace fb2ToText {
    class configLoader : ConfigurationSection {
        private bool _isSaveInFolder;
        private int _maxLineLen;

        /// //////////////////

        public configLoader() {
            string path = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            Console.Out.WriteLine( "loading config file: " + path );
            //Configuration conf = ConfigurationManager.OpenExeConfiguration( "app.config" );
            //Console.Out.WriteLine( "has file: " + conf.HasFile + " in path: " + conf.FilePath );

            string v = ConfigurationManager.AppSettings["is-save-in-folder"];
            if (v != null && v.Trim().Length > 0) {
                _isSaveInFolder = Boolean.Parse(v.Trim());
            } else {
                _isSaveInFolder = false;
                Console.Out.WriteLine("didn't find config parameter: 'is-save-in-folder'");
            } // if

            v = ConfigurationManager.AppSettings["maxLineLen"];
            if (v != null) {
                _maxLineLen = int.Parse(v.Trim());
            } else {
                _maxLineLen = 0;
                Console.Out.WriteLine("didn't find config parameter: 'maxLineLen'");
            } // if

            string [] lst = ConfigurationManager.GetSection( "kuku" ) as string [];
            if (lst != null) {
                Console.WriteLine( "sz=" + lst.Length );
            } else {
                Console.WriteLine( "empty list" );
            } // if
        } // fun

        public bool isSaveInFolder() { return _isSaveInFolder;; }
        public int maxLineLen() { return _maxLineLen; }
    } // class
}
