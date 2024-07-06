namespace Lang3.Utils.Errors;

class IOError : Error {
    string ErrorName {get {return "IOError";}}
    readonly string defaultMessage = "The file couldn't be read or found.";
    readonly int errorCode = 2;

    public static IOError i = new();
}