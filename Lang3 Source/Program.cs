using System.Diagnostics;
using Lang3.Utils;
using Lang3.Utils.ErrorTypes;
using static Lang3.Utils.Errors;

namespace Lang3;

class Lang3Runner {
    public static void Main(string[] args) {
        Dictionary<string, string> files = [];
        Errors err = new(files);
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
            err.Raise(2, [fp, "found"]);
            //throw new Exception($"File {fp} doesn't exist");
        } catch (DirectoryNotFoundException) {
            err.Raise(2, [fp, "found"]);
            //throw new Exception($"Directory {fp} doesn't exist");
        } catch (IOException) {
            err.Raise(2, [fp, "accessed"]);
            //throw new Exception($"Couldn't read file {fp}");
        }

        files.Add(fp, code);
        err.Raise(0, null, fp);
    }
}