namespace Models.AppSpecific;
public record struct MethodResult(bool Status, string? Message = null)
{
    public static MethodResult Success() => new(true);
    public static MethodResult Fail(string message) => new(false, message);
}