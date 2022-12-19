using System.Reflection;

namespace HelloDotnet;

public static class AsciiArt
{
  public static void Write(Options o)
  {
    FiggleFont? font = null;
    if (!string.IsNullOrWhiteSpace(o.Font))
    {
      font = typeof(FiggleFonts)
        .GetProperty(o.Font,
          BindingFlags.Static | BindingFlags.Public)
        ?.GetValue(null)
        as FiggleFont;
      if (font == null)
      {
        WriteLine($"Could not find font '{o.Font}'");
      }
    }

    font ??= FiggleFonts.Standard;

    if (o?.Text != null)
    {
      WriteLine(font.Render(o.Text));
      WriteLine($"Brought to you by {typeof(AsciiArt).FullName}");
    }

    // If any other words specified on command line, write them 
    // each separately.
    if (o?.OtherWords != null)
    {
      foreach (var word in o.OtherWords)
      {
        WriteLine(font.Render(word));
      }
    }
  }

  /// <remarks>
  /// Used for exercise that lists all the fonts in the help text.
  /// </remarks>
  public static void WriteAllFontNames()
  {
    foreach (var font in typeof(FiggleFonts)
      .GetProperties(BindingFlags.Static | BindingFlags.Public))
    {
      Console.Write(font.Name + "  ");
    }
  }
}
