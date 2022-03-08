using System;

const string s = $"Hello{" "}World"; // C# 10 Constant Interpolated Strings
var hello = $"{s} from {args[0]}!";
    
Console.WriteLine(hello);