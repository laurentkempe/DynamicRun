using System;

var hello = $"{Hello.World} from {args[0]}!";
    
Console.WriteLine(hello);

static class Hello
{
    public static string World => field ??= "Hello World"; // C# 14 field keyword
}