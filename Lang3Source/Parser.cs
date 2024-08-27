using Lang3.Utils;

namespace Lang3;

class Parser(Dictionary<string, string> fileCode, Dictionary<string, List<Lexer.Token>> fileTokens) {
    public class Node(string type, string value, Lexer.Token token) {
        public string type = type;
        public string value = value;
        public List<Node> children = [];
        public Lexer.Token token = token;

        private string ChildrenToString() {
            string s = "[\n";
            foreach (Node child in children) {
                s += $"  {child}\n";
            }
            return $"{s}]";
        }

        public override string ToString() {
            return $"Node({type}, '{value}', {ChildrenToString()}, {token})";
        }
    }

    readonly Errors err = new(fileCode);

    public Node Parse(string fp) {

        List<Lexer.Token> tokens = fileTokens[fp];

        Node root = new("root", "", new("", "", 0, 0, 0, fp));
        int i = 0;
        Parse(tokens, root, ref i);
        return root;
    }

    private Node Parse(List<Lexer.Token> tokens, Node node, ref int i) {
        while (i < tokens.Count) {
            Lexer.Token t = tokens[i];
            if (t.type == "value") {
                node.children.Add(new("value", t.value, t));
                i++;
            } else if (t.type == "parens") {
                throw new NotImplementedException();
            } else if (t.type == "operator") {
                throw new NotImplementedException();
            } else {
                // TODO: add a new error type for this
                err.Raise(Errors.ErrorNames.Error, "Unexpected token", t);
            }
        }
        return node;
    }
}