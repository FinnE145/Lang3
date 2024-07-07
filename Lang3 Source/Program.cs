using System.Diagnostics;
using Lang3.Utils;
using static Lang3.Utils.Errors;

namespace Lang3;

class Lang3Runner {
    public static void Main(string[] args) {
        string fp;
        if (args.Length == 0) {
            //fp = "../Lang3 Test Files/test.l3";
            fp = "nonexistantFile.l3";
        } else {
            fp = args[0];
        }

        string code = "";
        try {
            code = File.ReadAllText(fp) + "\n";
        } catch (FileNotFoundException) {
            Raise(1, [fp, "found"]);
            //throw new Exception($"File {fp} doesn't exist");
        } catch (DirectoryNotFoundException) {
            Raise(1, [fp, "found"]);
            //throw new Exception($"Directory {fp} doesn't exist");
        } catch (IOException) {
            Raise(1, [fp, "accessed"]);
            //throw new Exception($"Couldn't read file {fp}");
        }
    }
}