using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.CharacterSystem;
using SNEngine.Repositories;
using SNEngine.Services;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SNEngine
{
    public static class TextParser
    {
        
        public static string ParseWithProperties (string text, BaseGraph graph) 
        {
            var Variables = graph.Variables;

           var characters = NovelGame.Instance.GetRepository<CharacterRepository>().Characters;

            var globalVariables = NovelGame.Instance.GetService<VariablesContainerService>().GlobalVariables;

            var dictonaries = new Dictionary<string, object>
             {
            { "[Property=", Variables },
            { "[GlobalProperty=", globalVariables },
            {"[Character=", characters }

             };

            foreach (var pair in dictonaries)
            {
                IDictionary dictionary = pair.Value as IDictionary;

                foreach (DictionaryEntry item in dictionary)
                {
                    if (item.Value is VariableNode)
                    {
                        VariableNode node = item.Value as VariableNode;

                        string attribute = $"{pair.Key}{node.Name}]";
          
                        if (text.Contains(pair.Key, StringComparison.Ordinal) && attribute.Contains(node.Name, StringComparison.Ordinal))
                        {
                            ReplacePart(ref text, attribute, node.GetCurrentValue().ToString());
                        }

                        
                    }
                    else if (item.Value is Character)
                    {
                        Character character = item.Value as Character;

                        string attribute = $"{pair.Key}{character.name}]";

                        if (text.Contains(pair.Key, StringComparison.Ordinal) && attribute.Contains(character.name, StringComparison.Ordinal))
                        {
                            ReplacePart(ref text, attribute, character.GetName());
                        }


                    }
                }
                }
            

            return text;
            
        }

        private static void ReplacePart (ref string text, string attribute, string newValue)
        {
            text = text.Replace(attribute, newValue);
        }
    }
}
