# pinecone-lang
Pinecone (.pc) is a fast open-sourced configuration language with type annotations and comments
## Marksheet
Line structure: `key`: `type` = `value`<br>
Supported types: `string`, `int`, `float`, `boolean`<br>
Arrays are specified using square-brackets in type definition `type[]`, value should be enclosed in square brackets and elements are split using commas `[element0, element1]`<br>
Whitespace is ignored outside of brackets (`"` or `'`)<br>
You can comment using `//`
## Configuration example
```
// this is a comment and will be ignored

title: string = "Some Title" // you can also place comments here
version: string = "1.0.0"
someInt: int = 0
someFloat: float = 0.15
tags: string[] = ["tag01", "tag02"]
usePassword: boolean == false // you can also write 0 or 1
```
## Usage example
```js
using PineconeSharp;

namespace PineconeDemo
{
	public class Program
	{
		public static void Main()
		{
			try
            {
                var result = Parse("example.pc", new PineconeParseOptions());
                Console.WriteLine(result["title"]);
                Console.WriteLine(result["version"]);
                Console.WriteLine(result["someInt"]);
                Console.WriteLine(result["someFloat"]);

                object[] tags = (object[])result["tags"];
                for (int i = 0; i < tags.Length; i++)
                    Console.WriteLine(tags[i]);

                Console.WriteLine(result["usePassword"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
		}
	}
}
```