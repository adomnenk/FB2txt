using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace fb2ToText {
    class Program {
        private static Encoding _encoding = Encoding.GetEncoding(1251);
        private static configLoader _config;
        private static readonly HashSet<string> _ignoreElementsMap = new HashSet<string>();
        private static int _maxLineLen;
        private static string _bookName, _sequence, _date;
        private static bool _hadAnotation;

        static void Main( string [] args ) {
            //args = new string [] { "c:/0/fb2in/0middleName-Streljcov_Ivan_Afganskij_osvedomitelj.fb2" };
            //if ( args != null && args.Length > 0 )
                //Console.Out.WriteLine( "first args: " + args[0] );

            try {
                _config = new configLoader();
            } catch(Exception ex ) {
                Console.Out.WriteLine(ex.ToString() );
                return;
            } // end try

            _maxLineLen = _config.maxLineLen();

            // full the maps
            _ignoreElementsMap.Add( "binary" );
            _ignoreElementsMap.Add( "city" );
            _ignoreElementsMap.Add( "coverpage" );
            _ignoreElementsMap.Add( "custom-info" );
            _ignoreElementsMap.Add( "email" );
            _ignoreElementsMap.Add( "empty-line" );
            _ignoreElementsMap.Add( "id" );
            _ignoreElementsMap.Add( "isbn" );
            _ignoreElementsMap.Add( "history" );
            _ignoreElementsMap.Add( "home-page" );
            _ignoreElementsMap.Add( "nickname" );
            _ignoreElementsMap.Add( "lang" );
            _ignoreElementsMap.Add( "program-used" );
            _ignoreElementsMap.Add( "publisher" );
            _ignoreElementsMap.Add( "publish-info" );
            _ignoreElementsMap.Add( "stylesheet" );
            _ignoreElementsMap.Add( "src-lang" );
            _ignoreElementsMap.Add( "src-ocr" );
            _ignoreElementsMap.Add( "src-url" );
            _ignoreElementsMap.Add( "version" );
            //Console.Out.WriteLine( "map size=" + _ignoreElementsMap.Count );
            readFiles(args);
        } // fun

        private static void readFiles(string[] args)
        {
            string name = "empty";
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    name = args[i];
                    if (Directory.Exists(name))
                    {
                        string[] items = Directory.GetDirectories(name);
                        readFiles(items);

                        items = Directory.GetFiles(name);
                        readFiles(items);
                    } else { // it's file
                        if (name.ToLower().EndsWith(".fb2") == false)
                            continue;

                        convertXml2text(name);
                    } // if
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("error: '" + name + "', cause: " + ex.ToString());
                } //  end try
            } // loop
        } // fun

        public static void convertXml2text( string fileName ) {
            //string xml = File.ReadAllText("c:/books/Martjyanov_Andrej_Chuzhie_Operaciya_Ryugen_.fb2.txt", System.Text.Encoding.Unicode);
            //BinaryReader br = new BinaryReader( File.Open( fileName, FileMode.Open ) );
            //byte [] arr = br.ReadBytes( 2 );

            _date = _bookName = _sequence = null;
            _hadAnotation = false;
            AuthorData aData = new AuthorData();
            determineEncoding( fileName, aData );
            XmlDocument doc = new XmlDocument();

            using (StreamReader sr = new StreamReader( 
                fileName,
                _encoding, true) ) {
                doc.Load(sr);
            }  // end block

            string outFileName = getOutputFileName(fileName);
            using (FileStream stream = new FileStream( outFileName,
                FileMode.Create)) {
                    using (BinaryWriter writer = new BinaryWriter( stream )) {
                        parseChild( null, doc.ChildNodes, 0, writer, aData );
                        writer.Flush();
                        writer.Close();
                    } // end block
            } // end block
        } // fun

        private static string getOutputFileName(string inFileName) {
            StringBuilder sb = new StringBuilder();

            if (_config.isSaveInFolder()) {
                int pos = inFileName.LastIndexOf("\\");
                if ( pos < 1 )
                    pos = inFileName.LastIndexOf("/");

                if (pos < 1)
                    throw new Exception("expected folder separator signed in fileName: '" + inFileName + "'");

                sb.Append(inFileName.Substring(0, pos)).Append("\\text");
                if ( File.Exists( sb.ToString()) == false )
                    Directory.CreateDirectory( sb.ToString());
                int pos2 = inFileName.LastIndexOf(".");
                if (pos2 < 1)
                    throw new Exception("expected file extention in path: '" + inFileName + "'");

                sb.Append("\\").Append(inFileName.Substring(pos + 1, pos2 -pos-1)).Append(".txt");
            } else { // save in same folder 
                int pos = inFileName.LastIndexOf(".");
                if ( pos < 1 )
                    throw new Exception("expected file extention in path: '" + inFileName + "'");

                sb.Append(inFileName.Substring(0, pos)).Append(".txt");
            } // if

            return sb.ToString();
        } // fun

        private static string divideLine(string inText) {
            if (_maxLineLen  < 1 || inText.Length <= _maxLineLen )  // not separate line
                return inText;

            //  need divide the line
            int startPos = _maxLineLen -_maxLineLen/5;
            StringBuilder sb = new StringBuilder();
            while (inText.Length > _maxLineLen ) {
                int p = inText.IndexOf('.', startPos );
                if (p < 0 )
                    p = inText.IndexOf(',', startPos );
                if (p < 0 )
                    p = inText.IndexOf('?', startPos );
                if (p < 0)
                    p = inText.IndexOf( '!', startPos );
                if ( p < 0 )
                    p = inText.IndexOf( '—', startPos );
                if (p < 0)
                    p = inText.IndexOf( ':', startPos );
                if (p < 0)
                    p = inText.IndexOf( '-', startPos );
                if (p < 0)
                    p = inText.IndexOf( ' ', startPos );

                if ( p > 0 ) {
                    sb.Append(inText.Substring(0, p+1 )).Append("\r\n");
                    inText = inText.Substring(p+1, inText.Length - p -1);
                } else { // didn't find the signs
                        sb.Append(inText.Substring(0, _maxLineLen)).Append("-\r\n");
                        inText = inText.Substring(_maxLineLen+1, inText.Length - _maxLineLen -1);
                } // if
            } // loop

            if (inText.Length > 0) // may rmaind taill
                sb.Append( inText );

            return sb.ToString();
        } // fun

        private static void parseChild(string parentName, XmlNodeList child, int level,
            BinaryWriter outBuf, AuthorData aData ) {
            if (child == null)
                return;

            for (int i = 0; i < child.Count; i++) {
                // check if we in test mode for detection encoding
                if (outBuf == null
                    && aData.hasData() )
                    return;

                XmlNode n = child.Item(i);
                string k = n.Name;
                if ( _ignoreElementsMap.Contains( k ) )
                    continue;

                //if ( aData.getName() != null && aData.getLastName() != null)
                   //continue;

                if (n.HasChildNodes == false)
                    continue;

                if (n.ChildNodes.Count == 1
                        && n.ChildNodes.Item( 0 ).HasChildNodes == false) {
                    XmlNodeType t = n.NodeType;
                    switch (t) {
                        case XmlNodeType.Text:
                            //case XmlNodeType.Entity:
                            //Console.Out.WriteLine(n.Value);
                            break;
                        case XmlNodeType.Element:
                            processLeafElement( parentName, n, level, outBuf, aData );
                            break;
                        default:
                            break;
                    } // end switch
                } else { // mor from one child of have sub child
                    parseChild( k, n.ChildNodes, level + 1, outBuf, aData );
                } // if
            } // loop
        } // gfun

        private static bool isCorrectEncoding( string value ) {
            // in Utf8: А=1040  а=1072
            //byte [] utf8bytes = Encoding.UTF8.GetBytes( value );
            //byte [] arr = enc.GetBytes( value );
            char [] chArr = value.ToCharArray();
            int errCount = 0;
            int maxValue = 1072 + 32; // smol ya in russion
            for(int i = 0; i < chArr.Length; i++ ) {
                int ch = (int) chArr [ i ];
                if (ch > maxValue)
                    errCount++;

                //Console.Write( " " + ch );
            } // loop

            // if more char is not related to cirilik so result is false
            if (errCount < value.Length / 2)
                return true;

            return false;
        } // fun

        private static void determineEncoding( string fileName, AuthorData aData ) {
            using (StreamReader sr = new StreamReader( fileName )) {
                Encoding enc = sr.CurrentEncoding;
                //Console.Out.WriteLine( "got encoding from file: " + enc );
                //if ( enc != Encoding.UTF8 ) // utf8 is invalid encoding
                _encoding = enc;
                XmlDocument doc = new XmlDocument();
                doc.Load(sr);
                parseChild( null, doc.ChildNodes, 0, null, aData );
            } // end block

            // validate if name and last name are correct
            if ( _encoding == Encoding.UTF8 ) {
                if ( ( aData.getName() != null && isCorrectEncoding( aData.getName() ) == false )
                || ( aData.getLastName() != null && isCorrectEncoding( aData.getLastName() ) == false )) {
                    _encoding = Encoding.GetEncoding( 1251 );
                    aData.clear();
                } // if
            } // if
        } // fun

        private static void processLeafElement(string parentName,
            XmlNode node, int level,
            BinaryWriter outBuf, AuthorData aData ) {
            string k = node.Name;
            string v = node.ChildNodes.Item( 0 ).Value;
            if (v == null
                || v.Trim().Length < 1)
                return;

            // validate if author is already apeared
            switch (k) {
                case "genre":
                    if ( aData.getGenre() == null)
                        aData.setGenre( v );

                    return;
                case "first-name":
                    if ( aData.getName() == null) // does not appear
                        aData.setName( v );

                    return;
                case "last-name":
                    if ( aData.getLastName() == null) // does not  appear
                        aData.setLastName( v );

                    return;
                case "middle-name":
                    if ( aData.getMiddleName() == null )
                        aData.setMiddleName( v );
                            
                    return;
                case "book-title": //if ( _bookName ) // already appears
                    if (aData.getBookTitle() == null)
                        aData.setBookTitle( v );

                    return;
                case "date":
                    if (_date != null)
                        return;

                    _date = k;
                    break;
                case "year":
                    if (_date != null)
                        return;
                    break;
                case "sequence":
                    if (_sequence != null)
                        return;

                    _sequence = k;
                    break;
            } // end switch

            if (outBuf == null)
                return; // we can't write

            // write author data a head
            if ( aData.isWroteData() == false 
                && aData.hasData() ) {
                outBuf.Write( _encoding.GetBytes( aData.ToString() ) );
                aData.isWroteData( true );
            } // if

            if (parentName == "annotation"
                && _hadAnotation == false ) 
                outBuf.Write( parentName + "\r\n" );
            
            if (k == "sequence"
                 || k == "date")
                    outBuf.Write( k + " - " );

            v = divideLine( v.Trim() );
            if (v.StartsWith( "http://" ) || v.StartsWith( "www." ))
                    return;

            outBuf.Write( _encoding.GetBytes( v ) );
            outBuf.Write( "\r\n" );

            if (parentName == "annotation"
                && _hadAnotation == false ) {
                outBuf.Write( "\r\n" );
                _hadAnotation = true;
            } // if
        } // fun
    } // class
}
