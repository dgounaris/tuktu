namespace Hive.Printers;

public class DefaultPrinter
{
    private const string CELL_TEMPLATE = """
                                           _ _
                                          / # \
                                          \_$_/
                                         """;

    public string PrintCell(string value, string color)
    {
        return CELL_TEMPLATE.Replace("#", value)
            .Replace("$", color);
    }
}