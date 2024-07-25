# pinecone-lang
Pinecone (.pc) is a fast open-sourced configuration language
## Installation
## Marksheet
Line structure: `key`: `type` = `value`<br>
Supported types: `string`, `int`, `float`<br>
Whitespace is ignored outside of brackets (`"` or `'`)<br>
You can comment using `//`
## Configuration example
```
// this is a comment and will be ignored

title: string = "Some Title" // you can also place comments here
version: string = "1.0.0"
someInt: int = 0
someFloat: float = 0.15
```
## Usage example
```cs
using PineconeSharp;

namespace Demo
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                var example = Pinecone.Parse("example.pc");
                Console.WriteLine(example["title"]);
                Console.WriteLine(example["version"]);
                Console.WriteLine(example["someInt"]);
                Console.WriteLine(example["someFloat"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
```