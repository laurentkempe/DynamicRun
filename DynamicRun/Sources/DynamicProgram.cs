using System;

var hello = $"{Hello.Word} from {args[0]}!";
    
Console.WriteLine(hello);

static class Hello
{
    public static string Word => field ??= "Hello World"; // C# 14 field keyword
}