namespace JS
{
    class TokenLocation
    {
        private readonly string file_path;
        private readonly int cursor;
        private readonly int row;
        private readonly int col;

        public TokenLocation(int cursor, int row, int col)
        {
            file_path = "";
            this.cursor = cursor;
            this.row = row;
            this.col = col;
        }

        public TokenLocation(string file_path, int cursor, int row, int col)
        {
            this.file_path = file_path;
            this.cursor = cursor;
            this.row = row;
            this.col = col;
        }

        public string FilePath { get { return file_path; } }
        public int Cursor { get { return cursor; } }
        public int Row { get { return row; } }
        public int Col { get { return col; } }

        public override string ToString()
        {
            return "TokenLocation(file_path=\"" + file_path + "\", cursor=" + cursor + ", row=" + row + ", col=" + col + ")";
        }
    }

    enum TokenType
    {
        Identifier,
        Keyword,
        String,
        Number,

        Plus,
        Dash,
        Slash,
        Asterisk,
        Pipe,
        Carot,
        Ampersand,
        Percent,
        Exclamation,
        Equal,

        Colon,
        Semicolon,
        Period,
        Comma,
        Hashtag,

        OpenParen,
        CloseParen,
        OpenBracket,
        CloseBracket,
        OpenSquareBracket,
        CloseSquareBracket,
        OpenAngleBracket,
        CloseAngleBracket,

        EOF
    }


    class Token
    {
        private readonly TokenType token_type;
        private readonly string token_as_string;
        private readonly int token_as_int;
        private readonly TokenLocation token_location;

        public Token(TokenType type, TokenLocation token_location)
        {
            this.token_type = type;
            this.token_as_string = "";
            this.token_location = token_location;
        }

        public Token(TokenType type, string raw, TokenLocation token_location)
        {
            this.token_type = type;
            this.token_as_string = raw;
            this.token_location = token_location;
        }

        public Token(TokenType type, string raw, int value, TokenLocation token_location)
        {
            this.token_type = type;
            this.token_as_string = raw;
            this.token_as_int = value;
            this.token_location = token_location;
        }

        public TokenType Type { get { return token_type; } }
        public string AsString { get { return token_as_string; } }
        public int AsInt { get { return token_as_int; } }
        public TokenLocation Location { get { return token_location; } }

        public override string ToString()
        {
            // TODO: as_*
            return "Token(type=" + token_type + ", raw=\"" + token_as_string + "\", location=" + token_location.ToString() + ")";
        }
    }
}