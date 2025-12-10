using Lang3.Utils;
using static Lang3.Utils.Errors.ErrorNames;

namespace Lang3;

class Interpreter(Dictionary<string, string> fileCode, Dictionary<string, List<Lexer.Token>> fileTokens, Dictionary<string, Parser.Node> AST) {
    public class Value(string type, string value) {
        public string type = type;
        public string value = value;
    }

    Errors err = new(fileCode);

    public void Link(Parser.Node node) {
        if (node.type == "operation") {
            
    }

    private void Eval(Parser.Node node) {
        switch (node.type) {
            case "operation": 
                Eval(node.);
                Eval(node.right);
                break;
        }
    }

    public void Interpret(string fp) {
        Parser.Node top = AST[fp];

        
    }
}