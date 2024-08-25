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
        Dictionary<string, string> files = [];

        Errors err = new(files);

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

            files.Add(fp, code);

            //TestErrors(err, fp);

            // err.Raise(IOError, ["nonexistantFile.txt", "found"], false);
            // Console.WriteLine();

            Lexer lexer = new(files);
            List<Lexer.Token> tokens = lexer.Lex(fp);
            foreach (Lexer.Token token in tokens) {
                Console.WriteLine(token.ToString(false, false));
            }
        } catch (NotImplementedException e) {
            err.Raise(InternalError, e.Message);
        }
    }
}