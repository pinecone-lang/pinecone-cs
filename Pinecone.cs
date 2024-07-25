namespace PineconeSharp
{
    public class Pinecone
    {
        public static Dictionary<string, object> Parse(string filePath)
        {
            try
            {
                Dictionary<string, object> obj = new Dictionary<string, object>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string? line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length < 1)
                            continue;

                        IterationStates iterationState = IterationStates.Key;

                        bool isString = false;
                        bool isComment = false;
                        string key = "";
                        string type = "";
                        string value = "";

                        for (int i = 0; i < line.Length; i++)
                        {
                            char c = line[i];

                            if (c == '/' && line[i + 1] == '/')
                            {
                                isComment = true;
                                break;
                            }

                            if (c == '"' || c == '\'')
                                isString = !isString;

                            if (c == ' ' && !isString)
                                continue;

                            if (c == ':')
                            {
                                iterationState = IterationStates.Type;
                                continue;
                            }

                            if (c == '=')
                            {
                                iterationState = IterationStates.Value;
                                continue;
                            }

                            switch (iterationState)
                            {
                                case IterationStates.Key:
                                    key += c;
                                    break;
                                case IterationStates.Type:
                                    type += c;
                                    break;
                                case IterationStates.Value:
                                    value += c;
                                    break;
                            }
                        }

                        if (isComment && type.Length < 1)
                            continue;

                        object parsedValue;

                        switch (type)
                        {
                            case "string":
                                char startsWith = value[0];
                                char endsWith = value[value.Length - 1];

                                if ((startsWith != '"' && startsWith != '\'') || endsWith != startsWith)
                                    throw new Exception("Value at key \"" + key + "\" on line " + (0 + 1) + " does not match it's type (" + type + ")"); // change 0 to index

                                parsedValue = value.Substring(0, value.Length - 1).Substring(1);
                                break;
                            case "int":
                                int v = 0;

                                if (!Int32.TryParse(value, out v))
                                    throw new Exception("Value at key \"" + key + "\" on line " + (0 + 1) + " does not match it's type (" + type + ")");

                                parsedValue = v;
                                break;
                            case "float":
                                float f = 0;

                                if (!float.TryParse(value.Replace(".", ","), out f))
                                    throw new Exception("Value at key \"" + key + "\" on line " + (0 + 1) + " does not match it's type (" + type + ")");

                                parsedValue = f;
                                break;
                            default:
                                throw new Exception("Unknown type \"" + type + "\" on line " + (0 + 1));
                        }

                        obj[key] = parsedValue;
                    }

                    reader.Close();
                }

                return obj;
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception("Pinecone file not found: " + filePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandled exception: " + ex);
            }
        }

        private enum IterationStates
        {
            Key,
            Type,
            Value
        }
    }
}