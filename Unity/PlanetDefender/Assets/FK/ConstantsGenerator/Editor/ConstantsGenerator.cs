﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;


namespace FK.ConstantsGenerator
{
    /// <summary>
    /// <para>A Editor Tool that generates constants classes for Tags, Layers and Scenes</para>
    /// 
    /// v1.2 08/2018
    /// Written by Fabian Kober
    /// fabian-kober@gmx.net
    /// </summary>
    public class ConstantsGenerator : EditorWindow
    {
        // ######################## PRIVATE VARS ######################## //
        /// <summary>
        /// Config Data
        /// </summary>
        private static ConstantsGeneratorConfig _config;

        /// <summary>
        /// The Base Path (Assets Folder)
        /// </summary>
        private static readonly string BASE_PATH = "Assets/";
        /// <summary>
        /// Path to the Config Data Object
        /// </summary>
        private static readonly string CONFIG_PATH = $"{BASE_PATH}Scripts/Editor/Tools/";
        /// <summary>
        /// Name of Config Data Object
        /// </summary>
        private static readonly string CONFIG_FILE_NAME = "ConstantsGeneratorConfig.asset";

        /// <summary>
        /// Template for the generated classes
        /// </summary>
        private static readonly string CLASS_TEMPLATE = "INCLUDES\r\nnamespace NAMESPACE\r\n{\r\n\tDESCRIPTION\r\n\tpublic static class CLASS_NAME\r\n\t{\r\n\tCONTENT}\r\n}";



        // ######################## UNITY START & UPDATE ######################## //
        private void OnGUI()
        {
            // if we have no reference to the config, load it
            if (_config == null)
            {
                GetConfig();
            }

            // Fields for editing config
            _config.Namespace = EditorGUILayout.TextField(new GUIContent("Namespace", "Namespace used by generated classes"), _config.Namespace);
            _config.GeneratedClassesPath = EditorGUILayout.TextField(new GUIContent("Generated Scripts Path", "The Path where the auto generated scripts are created"), _config.GeneratedClassesPath);

            // Button for generating classes
            if (GUILayout.Button("Generate Constants Classes"))
            {
                GenerateConstantsClasses();
            }
        }



        // ######################## INITS ######################## //
        /// <summary>
        /// Opens a window to edit the config data
        /// </summary>
        [MenuItem("Tools/Constants Generator Config", false, 50)]
        public static void Init()
        {
            GetConfig();
            ConstantsGenerator window = (ConstantsGenerator)GetWindow(typeof(ConstantsGenerator));
            window.titleContent = new GUIContent("Constants Generator Config");
            window.Show();
        }

        /// <summary>
        /// Generates the Constant classes
        /// </summary>
        [MenuItem("Tools/Generate Constants Classes %&g", false, 51)]
        public static void GenerateConstantsClasses()
        {
            GetConfig();
            GenerateTagsClass();
            GenerateLayersClass();
            GenerateScenesClass();

            Debug.Log("Done Generating Constants Classes!");
        }


        // ######################## FUNCTIONALITY ######################## //
        /// <summary>
        /// Tries to load the config data and creates it if it does not find it
        /// </summary>
        private static void GetConfig()
        {
            // try to load
            _config = AssetDatabase.LoadAssetAtPath<ConstantsGeneratorConfig>(CONFIG_PATH);

            // if we got no data, create it
            if (_config == null)
            {
                _config = ScriptableObject.CreateInstance<ConstantsGeneratorConfig>();
                if(!Directory.Exists(CONFIG_PATH))
                   Directory.CreateDirectory(CONFIG_PATH);
                AssetDatabase.CreateAsset(_config, CONFIG_PATH+CONFIG_FILE_NAME);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Generates a C# class with the given name and content using the CLASS_TEMPLATE
        /// </summary>
        /// <param name="className"></param>
        /// <param name="content"></param>
        /// <param name="includes"></param>
        /// <param name="summary">An optional summary for the class that is added as a Visual Studio summary comment</param>
        private static void GenerateClass(string className, string content, string[] includes, string summary = null)
        {
            // if there is no namespace specified, use default
            if (string.IsNullOrEmpty(_config.Namespace))
            {
                _config.Namespace = "C";
                Debug.LogWarning("Cannot create Constants Class without namespace, using default C");
            }

            // create includes
            string includesString = "";
            if (includes != null)
            {
                foreach (string include in includes)
                {
                    includesString += $"using {include};\r\n";
                }
            }
            
            // replace marked parts in template with the data for this class
            string generatedClassContent = CLASS_TEMPLATE.Replace("INCLUDES", includesString).Replace("NAMESPACE", _config.Namespace).Replace("CLASS_NAME", className).Replace("CONTENT", content);

            // if we have a summary, add it
            if(!string.IsNullOrEmpty(summary))
            {
                string descriptionComment = $"/// <summary>\r\n\t/// {summary}\r\n\t/// </summary>";
                generatedClassContent = generatedClassContent.Replace("DESCRIPTION", descriptionComment);
            } else
            {
                generatedClassContent = generatedClassContent.Replace("DESCRIPTION", string.Empty);
            }

            // get the path to where the class should be generated
            string path = $"{BASE_PATH}{_config.GeneratedClassesPath}";

            // if the path does not exist, create it
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // write the file
            using (StreamWriter sw = new StreamWriter($"{path}/{className}.cs"))
            {
                sw.Write(generatedClassContent);
            }

            // refresh database
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Generates the Constants class for tags
        /// </summary>
        private static void GenerateTagsClass()
        {
            // create empty content
            string content = "";

            //define some parts that we need multiple times
            string lineStart = "\tpublic const string ";
            string lineEnding = ";\r\n\t";

            // get tags and add one line of formatted code for each tag that exists
            foreach (string tag in UnityEditorInternal.InternalEditorUtility.tags)
            {
                content += $"{lineStart}{ConverteCamelCase(tag)} = \"{tag}\"{lineEnding}";
            }

            // generate the class
            GenerateClass("Tags", content, null, "Constants for easy access to Tags. If you add tags, you have to regenerate this class by pressing CTRL+ALT+C or clicking on Tools/Generate Constats Classes");
        }

        /// <summary>
        /// Generates the Constants class for layers
        /// </summary>
        private static void GenerateLayersClass()
        {
            // create includes
            string[] includes = {"UnityEngine"};
            
            // create empty content
            string content = "";

            //define some parts that we need multiple times
            string lineStart = "\tpublic const int ";
            string lineEnding = ";\r\n\t";

            // Layers are stored as a Bitmask in an int, which has 32 bits, so there can be a maximum of 32 layers. Go through each of them and add them, if they are named
            for(int i = 0; i < 32; ++i)
            {
                // get the layer name
                string layer = LayerMask.LayerToName(i);

                // Remove all spaces
                layer = layer.Replace(" ", string.Empty);

                // if the layer is named, add it to the content
                if(!string.IsNullOrEmpty(layer))
                {
                    content += $"{lineStart}{ConverteCamelCase(layer)} = {i}{lineEnding}";
                }
            }

            // layer mask functions
            string onlyInclundingFunction = "\t\t/// <summary>\r\n\t\t/// Returns a layermask only including the provided Layers\r\n\t\t/// </summary>\r\n\t\tpublic static int OnlyIncluding(params int[] layers)\r\n\t\t{\r\n\t\t\tint mask = 0;\r\n\t\t\tforeach(int layer in layers)\r\n\t\t\t\tmask |= (1 << layer);\r\n\t\t\treturn mask;\r\n\t\t}";
            string everythingButFunction  = "\t\t/// <summary>\r\n\t\t/// Returns a layermask including all Layers but the provided ones\r\n\t\t/// </summary>\r\n\t\tpublic static int EverythingBut(params int[] layers)\r\n\t\t{\r\n\t\t\treturn ~OnlyIncluding(layers);\r\n\t\t}";
            string isInLayerMaskFunction  = "\t\t/// <summary>\r\n\t\t/// Returns true if the provided layer is in the layermask and false if it is not\r\n\t\t/// </summary>\r\n\t\tpublic static bool IsInLayerMask(this int layer, LayerMask mask)\r\n\t\t{\r\n\t\t\treturn mask == (mask | (1 << layer));\r\n\t\t}";
            content += $"\r\n{onlyInclundingFunction}\r\n\r\n{everythingButFunction}\r\n\r\n{isInLayerMaskFunction}\r\n\t";

            // generate the class
            GenerateClass("Layers", content, includes, "Constants for easy access to layers and easy creation of Layer Masks. If you add layers, you have to regenerate this class by pressing CTRL+ALT+C or clicking on Tools/Generate Constats Classes");
        }

        /// <summary>
        /// Generates the Constants class for scenes
        /// </summary>
        private static void GenerateScenesClass()
        {
            // create empty content
            string content = "";

            //define some parts that we need multiple times
            string lineStart = "\tpublic const string ";
            string lineEnding = ";\r\n\t";

            // get all scenes in the build settings
            string[] scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);

            // go through all scenes and get the name if the scene to add it to the content
            for(int i = 0; i < scenes.Length; ++i)
            {
                string scene = scenes[i].Substring(scenes[i].LastIndexOf("/") + 1).Replace(".unity", string.Empty);
                content += $"{lineStart}{ConverteCamelCase(scene)} = \"{scene}\"{lineEnding}";
            }

            // add a constant for the total number of scenes
            content += $"\r\n\t\tpublic const int TOTAL_SCENES = {scenes.Length}{lineEnding}";

            // generate the class
            GenerateClass("Scenes", content, null, "Constants for easy access to Scenes that are referenced in the Build settings. If you add scenes, you have to regenerate this class by pressing CTRL+ALT+C or clicking on Tools/Generate Constats Classes");
        }


        // ######################## UTILITIES ######################## //
        /// <summary>
        /// Converts from camelCase to ALL_UPPER_WITH_UNDERSCORE using a regular expression
        /// </summary>
        /// <param name="camelCase"></param>
        /// <returns></returns>
        private static string ConverteCamelCase(string camelCase)
        {
            // find all matches with a regular expression
            MatchCollection matches = Regex.Matches(camelCase, "(.(?:[^a-z_]+(?=[A-Z_]|$)|[^A-Z_]+))");

            // after each match exept the last one add an underscore.
            string converted = "";
            for (int i = 0; i < matches.Count - 1; ++i)
            {
                converted += matches[i].Value + '_';
            }

            // add the last part
            converted += matches[matches.Count - 1];

            // if the string starts with a number, add an underscore in front
            if(Regex.IsMatch(converted, "^[0-9].*$"))
            {
                converted = '_' + converted;
            }

            // convert to all uppercase and return
            return converted.ToUpper();
        }

    }
}