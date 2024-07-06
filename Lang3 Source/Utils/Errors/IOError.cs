namespace Lang3.Utils.Errors;

class IOError : Error {
    readonly string defaultMessage = "The file couldn't be read or found.";
    readonly int errorCode = 2;
}