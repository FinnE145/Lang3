class Operator {
    public static string Add(string a, string b) {
        return a + b;
    }

    public static string Subtract(string a, string b) {
        return a.Replace(b, "");
    }
}