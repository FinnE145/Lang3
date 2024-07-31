﻿using System.Diagnostics;

using Lang3.Utils;

namespace Lang3;

class Lang3Runner {
    public static void Main(string[] args) {
        Dictionary<string, string> files = [];
        dynamic err = new Errors(files);
        ErrorsStatic errS = new(files);

        Stopwatch sw = new();
        sw.Start();
        err.MalformedTokenError("This uses a dynamic object", false);
        sw.Stop();
        Console.WriteLine($"Time: {sw.ElapsedMilliseconds}");
        sw.Restart();
        errS.Raise(4, ["This doesnt"], null, null, null, null, false);
        sw.Stop();
        Console.WriteLine($"Time: {sw.ElapsedMilliseconds}");
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
                err.IOError(fp, "found");
                //err.Raise(2, new List<string>([fp, "found"]));
                //throw new Exception($"File {fp} doesn't exist");
            } catch (DirectoryNotFoundException) {
                err.IOError(fp, "found");
                //err.Raise(2, new List<string>([fp, "found"]));
                //throw new Exception($"Directory {fp} doesn't exist");
            } catch (IOException) {
                err.IOError(fp, "accessed");
                //err.Raise(2, new List<string>([fp, "accessed"]));
                //throw new Exception($"Couldn't read file {fp}");
            }

            files.Add(fp, code);

            Lexer lexer = new(files);
            List<Lexer.Token> tokens = lexer.Lex(fp);
        } catch (Exception e) {
            err.InternalError(e.Message);
        }
    }
}