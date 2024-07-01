using System.Diagnostics;
using Lang3.Utils.Errors;

namespace Lang3;

class Lang3Runner {
    public static void Main(string[] args) {
        string fp;
        if (args.Length == 0) {
            fp = "../Lang3 Test Files/test.l3";
        } else {
            fp = args[0];
        }

        string code = "";
        try {
            code = File.ReadAllText(fp) + "\n";
        } catch (FileNotFoundException) {
            throw new Exception($"File {fp} doesn't exist");
        } catch (DirectoryNotFoundException) {
            throw new Exception($"Directory {fp} doesn't exist");
        } catch (IOException) {
            throw new Exception($"Couldn't read file {fp}");
        }
    }
}