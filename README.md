## Welcome to the prank version of .NET Compiler Platform ("Roslyn")

### The motivation

I created this version as a demo for my presentation What's new in C# 6.0 where the open source Roslyn plays a big role. The reasoning is to show that Roslyn is open source and everybody can contribute/fork/modify/whatever.

### The prank

I've reversed the meaning of true and false literals. The core change is in CSharpCodeAnalysis project's method Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind where I return SyntaxKind.TrueKeyword when "false" and viceversa.
To make this really work one needs to disable a couple of asserts. The two asserts verifies the string length and thus would fail ('false' is longer text than 'true'). The Debug.Assert lines are located in CodeAnalysis project SyntaxTreeExtensions.cs line 76 and CSharpCodeAnalysis project QuickScanner.cs line 254. I've just commented the two.

Perhpas there is a cleaner way to achieve this prank but the one described just works.

### The Test

Try compling and running this piece of code.
```
using System.IO;
using System;

namespace Yolo
{
	
	public class Yolo 
	{
		public static void Main()
		{
			if (true) {
				Console.WriteLine("Yolo");
			}
			else
			{
				Console.WriteLine("Yolo false");
			}
		}
	}
}
```
