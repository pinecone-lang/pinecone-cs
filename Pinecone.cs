namespace PineconeSharp
{
    public struct PineconeParseOptions
    {
        public PineconeParseOptions()
        {
            readFile = true;
        }

        public PineconeParseOptions(bool readFile)
        {
            this.readFile = true;
        }

        public bool readFile;
    }

    public class Pinecone
    {
        public static Dictionary<string, object> Parse(string x, PineconeParseOptions options)
        {
            string content = x;

            if (options.readFile)
            {
                try
                {
                    StreamReader reader = new StreamReader(x);
                    content = reader.ReadToEnd();
                    reader.Close();
                }
                catch (FileNotFoundException ex)
                {
                    throw new Exception("Pinecone file not found: " + x);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unhandled exception: " + ex);
                }
            }

            string[] lines = content.Split("\n");

            Dictionary<string, object> parsed = new Dictionary<string, object>();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Replace("\r", "");

                if (line.Length < 1)
                    continue;

                IterationStates iterationState = IterationStates.Key;

                bool isString = false;
                bool isComment = false;
                bool isArray = false;
                string key = "";
                string type = "";
                string value = "";

                for (int i2 = 0; i2 < line.Length; i2++)
                {
                    char currentChar = line[i2];
                    char nextChar = i2 + 1 > line.Length - 1 ? ' ' : line[i2 + 1];

                    if (currentChar == '/' && nextChar == '/')
                    {
                        isComment = true;
                        break;
                    }

                    if (currentChar == '"' || currentChar == '\'')
                        isString = !isString;

                    if (currentChar == ' ' && !isString)
                        continue;

                    if (currentChar == ':')
                    {
                        iterationState = IterationStates.Type;
                        continue;
                    }

                    if (currentChar == '=')
                    {
                        iterationState = IterationStates.Value;
                        continue;
                    }

                    switch (iterationState)
                    {
                        case IterationStates.Key:
                            key += currentChar;
                            break;
                        case IterationStates.Type:
                            if (currentChar == '[' && nextChar == ']')
                            {
                                isArray = true;
                                i2 += 1; // skip over next iteration
                                break;
                            }

                            type += currentChar;
                            break;
                        case IterationStates.Value:
                            value += currentChar;
                            break;
                    }
                }

                if (isComment && type.Length < 1)
                    continue;

                var parseResult = parseValue(isArray, type, value);

                if (parseResult.Item2)
                    throw new Exception("Value at key \"" + key + "\" on line " + (i + 1) + " does not match it's type (" + type + ")");

                parsed[key] = parseResult.Item1;
            }

            return parsed;
        }

        private static Tuple<object, bool> parseValue(bool isArray, string type, string value)
        {
            object parsedValue = 0;

            if (isArray)
            {
                char startsWith = value[0];
                char endsWith = value[value.Length - 1];

                if (startsWith != '[' || endsWith != ']')
                    return new Tuple<object, bool>(0, true);

                var arr = new List<object>();

                string[] elements = value.Substring(0, value.Length - 1).Substring(1).Split(",");

                for (int i = 0; i < elements.Length; i++)
                {
                    var result = parseValue(false, type, elements[i]);

                    if (result.Item2)
                        return result; // instead of creating a new instance of a tuple, we are going to return the already existing result if item1 (typeError) is true

                    arr.Add(result.Item1);
                }

                parsedValue = arr.ToArray();
            }
            else
            {
                switch (type)
                {
                    case "string":
                        char startsWith = value[0];
                        char endsWith = value[value.Length - 1];

                        if ((startsWith != '"' && startsWith != '\'') || endsWith != startsWith)
                            return new Tuple<object, bool>(0, true);

                        parsedValue = value.Substring(0, value.Length - 1).Substring(1);
                        break;
                    case "int":
                        int i;

                        if (!Int32.TryParse(value, out i))
                            return new Tuple<object, bool>(0, true);

                        parsedValue = i;
                        break;
                    case "float":
                        float fl;

                        if (!float.TryParse(value.Replace(".", ","), out fl))
                            return new Tuple<object, bool>(0, true);

                        parsedValue = fl;
                        break;
                    case "boolean":
                        if (value == "true" || value == "1")
                            parsedValue = true;
                        else if (value == "false" || value == "0")
                            parsedValue = false;
                        else
                            return new Tuple<object, bool>(0, true);

                        break;
                    default:
                        return new Tuple<object, bool>(0, true);
                }
            }

            return new Tuple<object, bool>(parsedValue, false);
        }

        private enum IterationStates
        {
            Key,
            Type,
            Value
        }
    }
}