using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fb2ToText {
    class AuthorData {
        private string _genre, _name, _lastName, _middleName, _bookTitle;
        private bool _isWroteData = false;

        //////////////////////////

        public string getGenre() { return _genre; }
        public void setGenre( string value) { _genre = value; }
        public string getName() { return _name; }
        public void setName( string value) { _name = value; }
        public string getMiddleName() { return _middleName; }
        public void setMiddleName( string value ) { _middleName = value; }
        public string getLastName() { return _lastName; }
        public void setLastName( string value ) { _lastName = value; }
        public string getBookTitle() { return _bookTitle; }
        public void setBookTitle( string value ) { _bookTitle = value; }
        public bool isWroteData() { return _isWroteData; }
        public void isWroteData( Boolean value ) { _isWroteData = value; }
        public bool hasData() {
            return _name != null || _lastName != null;
        } // fun
        public void clear() {
            _genre = _name = _lastName = _middleName = _bookTitle = null;
        } // fun

        public override string ToString() {
            StringBuilder sb = new StringBuilder( 200 );
            if (_genre != null)
                sb.Append( "genre - " ).Append( _genre ).Append( "\r\n" );
            if (_name != null)
                sb.Append( _name ).Append( " " );
            if (_middleName != null)
                sb.Append( _middleName ).Append( " " );
            if (_lastName != null)
                sb.Append( _lastName );
            if (sb.Length > 0)
                sb.Append( "\r\n" );

            if (_bookTitle != null)
                sb.Append( _bookTitle ).Append( "\r\n" );

            return sb.ToString();
        } // fun
          } // class
}
