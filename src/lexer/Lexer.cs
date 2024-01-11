namespace JS
{
    class Lexer
    {
        private readonly string file_path;
        private readonly string contents;
        private int cursor;
        private int row;
        private int bol;

        public Lexer(string file_path, string contents)
        {
            this.file_path = file_path;
            this.contents = contents;
            this.cursor = 0;
            this.row = 0;
            this.bol = 0;
        }

        public Lexer(string contents)
        {
            this.file_path = "null";
            this.contents = contents;
            this.cursor = 0;
            this.row = 0;
            this.bol = 0;
        }

        private bool IsEof() { return cursor >= contents.Length; }

        // current

        private char Peek(int offset = 0)
        {
            return IsEof() ? '\0' : contents[cursor + offset];
        }

        // location
        private TokenLocation location()
        {
            return new(this.file_path, this.cursor, this.row, this.cursor - this.bol);
        }

        private char Consume()
        {
            char ch = Peek();
            cursor++;
            if (ch == '\n')
            {
                row++;
                bol = cursor;
            }
            return ch;
        }

        private bool ConsumeExpectString(string it)
        {
            if (IsEof()) return false;
            bool _is = true;
            foreach (char ch in it)
            {
                char p = Peek();
                if (ch == p)
                    Consume();
                else
                {
                    _is = false;
                }
            }
            return _is;
        }

        private bool ConsumeExpect(char ch)
        {
            if (IsEof()) return false;
            char p = Peek();
            if (ch == p)
            {
                Consume();
                return true;
            }
            return false;
        }

        private string ConsumeWhile(Func<char, bool> condition)
        {
            int start = cursor;
            while (!IsEof() && condition(Peek()))
                Consume();
            return contents.Substring(start, cursor - start);
        }

        private void TrimLeft()
        {
            ConsumeWhile(char.IsWhiteSpace);
        }

        private (char, TokenType)[] RawChars = {
        ('+', TokenType.Plus),
        ('-', TokenType.Dash),
        ('/', TokenType.Slash),
        ('*', TokenType.Asterisk),
        ('|', TokenType.Pipe),
        ('^', TokenType.Carot),
        ('&', TokenType.Ampersand),
        ('%', TokenType.Percent),
        ('!', TokenType.Exclamation),
        ('=', TokenType.Equal),
        (':', TokenType.Colon),
        (';', TokenType.Semicolon),
        ('.', TokenType.Period),
        (',', TokenType.Comma),
        ('#', TokenType.Hashtag),
        ('(', TokenType.OpenParen),
        (')', TokenType.CloseParen),
        ('{', TokenType.OpenBracket),
        ('}', TokenType.CloseBracket),
        ('[', TokenType.OpenSquareBracket),
        (']', TokenType.CloseSquareBracket),
        ('<', TokenType.OpenAngleBracket),
        ('>', TokenType.CloseAngleBracket)
    };

        private string[] Keywords = {
        "this", "new",
        "async", "function",
        "return", "yield", "continue", "break",
        "let", "const", "var",
        "private", "public", "protected", "override",
        "interface", "class", "enum",
        "if", "while", "do", "else", "catch",
        "debugger"
    };

        private Token eof()
        {
            return new Token(TokenType.EOF, "", location());
        }

        public Token NextToken()
        {
            TrimLeft();
            if (IsEof())
                return eof();

            char ch = Peek();

            if (ch == '/' && Peek(1) == '/')
            {
                if (!ConsumeExpectString("//"))
                    throw new Exception("[Lexer] NextToken: Failed to lex comment");
                ConsumeWhile((char ch) => ch != '\n');
                return NextToken();
            }

            if (ch == '\'' || ch == '"' || ch == '`')
            {
                TokenLocation location = this.location();
                int start = this.cursor;
                char quote = Consume();

                while (!IsEof() && Peek() != quote)
                {
                    char ch_ = Consume();
                    if (ch_ == '\\')
                        Consume();
                }

                // expecting closing quote
                if (!ConsumeExpect(quote))
                    throw new Exception("[Lexer] NextToken: expected closing quote on string");

                string raw = contents.Substring(start + 1, cursor - (start + 2));
                return new(TokenType.String, raw, location);
            }

            if (char.IsAsciiLetter(ch))
            {
                TokenLocation location = this.location();
                int start = cursor;
                string raw = ConsumeWhile((char ch) => !IsEof() && char.IsAsciiLetterOrDigit(ch));
                Token token = new(Keywords.Contains(raw) ? TokenType.Keyword : TokenType.Identifier, raw, location);
                return token;
            }

            else if (char.IsNumber(ch))
            {
                TokenLocation location = this.location();
                int start = cursor;
                int value = 0;
                while (!IsEof() && char.IsDigit(Peek()))
                {
                    value *= 10;
                    value += Consume() - 48;
                }
                Token token = new(TokenType.Number, contents.Substring(start, cursor - start), value, location);
                return token;
            }

            // todo: single chars
            foreach ((char, TokenType) raw in RawChars)
            {
                if (raw.Item1 != ch) continue;
                TokenLocation location = this.location();
                Consume(); // eat ch
                Token token = new(raw.Item2, contents.Substring(cursor - 1, 1), location);
                return token;
            }

            Console.Error.WriteLine("[Lexer] NextToken: Unknown char '" + ch + "'");
            return eof();
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new();
            Token next = NextToken();
            while (next.Type != TokenType.EOF)
            {
                tokens.Add(next);
                next = NextToken();
            }
            return tokens;
        }
    }
}