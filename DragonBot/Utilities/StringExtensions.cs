namespace DragonBot.Utilities
{
    public static class StringExtensions
    {
        public static string ToCodeBlock(this string value) => $@"```
{value}
```";

        public static string ToCodeBlock(this string value, string type) => $@"```{type}
{value}
```";
    }
}
