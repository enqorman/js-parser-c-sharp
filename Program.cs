using JS;

class Program
{
    static void Main(string[] args)
    {
        string filePath = args.Length > 0 ? args[0] : ".\\test.txt";
        string contents = File.ReadAllText(filePath);
        JS.Lexer lexer = new(filePath, contents);
        List<Token> tokens = lexer.Tokenize();
        foreach (Token token in tokens)
        {
            Console.WriteLine(token.ToString());
        }
    }
}