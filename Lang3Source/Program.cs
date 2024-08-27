// dotnet run --project Lang3Source/Lang3Source.csproj --os win
// dotnet publish Lang3Source/Lang3Source.csproj

using Lang3.Utils;
using static Lang3.Utils.Errors.ErrorNames;

namespace Lang3;

class Lang3Runner {
    private static void TestErrors(Errors err, string fp) {
        Lexer.Token t = new("test", "test", 1, 0, 1, fp);

        err.Raise(Success, ["Ex loc"], fp, 1, 0, 1, false);
        err.Raise(Success, "Ex loc", fp, 1, 0, 1, false);
        err.Raise(Success, fp, 1, 0, 1, false);

        err.Raise(Success, ["Line"], fp, 0, false);
        err.Raise(Success, "Line", fp, 0, false);
        err.Raise(Success, fp, 0, false);

        err.Raise(Success, ["File"], fp, false);
        err.Raise(Success, "File", fp, false);
        //err.Raise(Success, fp, false);

        err.Raise(Success, ["Token"], t, false);
        err.Raise(Success, "Token", t, false);
        err.Raise(Success, t, false);

        err.Raise(Success, ["Arg"], false);
        err.Raise(Success, "Arg", false);
        err.Raise(Success, false);
    }

    public static void Main(string[] args) {

        bool debug = args.Contains("--debug") || args.Contains("-d");
        bool verbose = args.Contains("--verbose") || args.Contains("-v");
        bool showTokens = args.Contains("--tokens") || args.Contains("-t");
        bool showAST = args.Contains("--ast") || args.Contains("-a");

        Dictionary<string, string> fileCode = [];

        Errors err = new(fileCode);

        try {
            string fp;
            if (args.Length == 0) {
                fp = "C:/Users/finne/OneDrive/Documents/0coding/Lang3_Ghub/Lang3 Test Files/test.l3";
                //fp = "nonexistantFile.l3";
            } else {
                fp = args[0];
            }

            string code = "";
            try {
                code = File.ReadAllText(fp) + "\n";
            } catch (FileNotFoundException) {
                err.Raise(IOError, [fp, "found"]);
                //err.Raise(2, new List<string>([fp, "found"]));
                //throw new Exception($"File {fp} doesn't exist");
            } catch (DirectoryNotFoundException) {
                err.Raise(IOError, [fp, "found"]);
                //err.Raise(2, new List<string>([fp, "found"]));
                //throw new Exception($"Directory {fp} doesn't exist");
            } catch (IOException) {
                err.Raise(IOError, [fp, "accessed"]);
                //err.Raise(2, new List<string>([fp, "accessed"]));
                //throw new Exception($"Couldn't read file {fp}");
            }

            fileCode.Add(fp, code);

            //TestErrors(err, fp);

            // err.Raise(IOError, ["nonexistantFile.txt", "found"], false);
            // Console.WriteLine();

            Lexer lexer = new(fileCode);
            List<Lexer.Token> tokens = lexer.Lex(fp);

            if (showTokens || debug) {
                foreach (Lexer.Token token in tokens) {
                    Console.WriteLine(token.ToString(verbose, verbose));
                }
                Console.WriteLine();
            }

            Dictionary<string, List<Lexer.Token>> fileTokens = new() {
                [fp] = tokens
            };

            Parser parser = new(fileCode, fileTokens);
            Parser.Node ast = parser.Parse(fp);

            if (showAST || debug) {
                Console.WriteLine(ast.ToString(verbose));
            }

        } catch (NotImplementedException e) {
            err.Raise(InternalError, e.Message);
        }
    }
}