using System.Diagnostics;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record Todo(Guid Id, string Title, bool Done)
{
    private string GetDebuggerDisplay() => ToString();
}