using System.Diagnostics;
using Lang3.Utils.Errors;

namespace Lang3;

class Lang3Runner {
    public static void Main(string[] args) {
        string fp;
        if (args.Length == 0) {
            //fp = "../Lang3 Test Files/test.l3";
            fp = "THIS DOES NOT EXIST";
        } else {
            fp = args[0];
        }

        string code = "";
        try {
            code = File.ReadAllText(fp) + "\n";
        } catch (FileNotFoundException) {
            IOError.i.Raise($"File {fp} doesn't exist");
            //throw new Exception($"File {fp} doesn't exist");
        } catch (DirectoryNotFoundException) {
            IOError.i.Raise($"Directory {fp} doesn't exist");
            //throw new Exception($"Directory {fp} doesn't exist");
        } catch (IOException) {
            IOError.i.Raise($"Couldn't read file {fp}");
            //throw new Exception($"Couldn't read file {fp}");
        }
    }
}