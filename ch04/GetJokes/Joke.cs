namespace GetJokes;

public record Joke(
  int Id,
  string Type,
  string Setup,
  string Punchline
) {}
