namespace Cinema_System.Areas.Types
{
    public record Response(
        int error,
        string message,
        object? data
    );
}
