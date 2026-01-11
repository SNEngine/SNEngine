using System;
using System.Text;

namespace SNEngine.Editor.SNILSystem.NodeCreation
{
    public class NodeFormatter
    {
        public static string FormatNodeDisplayName(string nodeTypeName)
        {
            // Преобразуем "ShowCharacter" в "Show Character"
            // Добавляем пробел перед заглавной буквой, если перед ней есть строчная буква
            if (string.IsNullOrEmpty(nodeTypeName)) return nodeTypeName;

            var result = new StringBuilder();
            result.Append(nodeTypeName[0]); // Первую букву добавляем как есть

            for (int i = 1; i < nodeTypeName.Length; i++)
            {
                char currentChar = nodeTypeName[i];
                char prevChar = nodeTypeName[i - 1];

                // Если текущий символ - заглавная буква, а предыдущий - строчная, вставляем пробел
                if (char.IsUpper(currentChar) && char.IsLower(prevChar))
                {
                    result.Append(' ');
                }
                result.Append(currentChar);
            }

            string displayName = result.ToString();

            // Убираем "Node" из конца, если оно есть
            if (displayName.EndsWith("Node"))
                displayName = displayName.Substring(0, displayName.Length - 4);

            // Нормализуем регистр: приводим к Title Case (Каждое слово с заглавной буквы)
            displayName = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(displayName.ToLowerInvariant());

            return displayName;
        }

        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input.ToLowerInvariant());
        }
    }
}