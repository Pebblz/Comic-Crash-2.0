using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FootStepToMaterialEditor : EditorWindow
{
    public  Dictionary<string, string> map = new Dictionary<string, string>();
    public  string[] footStepTypes;
    public  int[] selectedSounds;
    


    public HashSet<string> findMaterials()
    {
        HashSet<string> assets = new HashSet<string>();
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Levels" });
        foreach (string guid in guids)
        {
            string name = AssetDatabase.GUIDToAssetPath(guid);
            int idxBeforeFName = name.LastIndexOf("/");
            string fileName = name.Substring(idxBeforeFName + 1, name.Length - idxBeforeFName - 1);
            string[] parts = name.Split('.');
            if (parts[1] == "mat")
                assets.Add(fileName);
        }
        return assets;
    }

    public HashSet<string> findFootSteps()
    {
        HashSet<string> assets = new HashSet<string>();
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Resources/Sounds/FootSteps" });
        foreach (string guid in guids)
        {
            string name = AssetDatabase.GUIDToAssetPath(guid);
            int idxBeforeFName = name.LastIndexOf("/");
            string fileName = name.Substring(idxBeforeFName + 1, name.Length - idxBeforeFName - 1);
            string[] parts = name.Split('.');
            if (parts[1] == "wav")
                assets.Add(fileName);
        }
        return assets;
    }

    [MenuItem("Tools/Footstep Material Map Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FootStepToMaterialEditor));
    }


    private void OnEnable()
    {
        TryLoad();
        setup();
    }


    private void save()
    {

        DictionarySerializer s = new DictionarySerializer();
        s.parse(this.map);
        string data = JsonUtility.ToJson(s, true);
        File.WriteAllText("FootStepConfig.json", data);

    }


    private void TryLoad()
    {
        if (System.IO.File.Exists("FootStepConfig.json"))
        {
            string data = File.ReadAllText("FootStepConfig.json");
            DictionarySerializer s =  JsonUtility.FromJson<DictionarySerializer>(data);
            map = s.toDictionary();
        }
    }

    private void CallBack(string mat, VisualElement element)
    {
        string name = element.name;
        map[mat] = name;
        var p = element.parent;
        foreach(VisualElement child in p.Children())
        {
            if( child is UnityEngine.UIElements.Button)
            {
                child.style.backgroundColor = new StyleColor(Color.gray);
            }
        }
        element.style.backgroundColor = new StyleColor(Color.white);
    }


    private void setup()
    {
        foreach (string mat in findMaterials())
        {
            if (map.ContainsKey(mat)) continue;
            map.Add(mat, null);
        }
        footStepTypes = findFootSteps().ToArray();
        selectedSounds = new int[map.Count];
        
        string[] keys = map.Keys.ToArray();
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/FootStepMap.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        rootVisualElement.name = "root";

        var saveBtn = new UnityEngine.UIElements.Button();
        saveBtn.text = "Save";
        saveBtn.clicked += () => save();
        rootVisualElement.Add(saveBtn);

        ScrollView scroller = new ScrollView(ScrollViewMode.Vertical);
        rootVisualElement.Add(scroller);

        for (int i = 0; i < keys.Length; i++)
        {

           
            string mat = keys[i];
            VisualElement group = new VisualElement();
            Label test = new Label(mat);
            group.Add(test);
            group.name = $"group";

            var t = new UnityEngine.UIElements.Button();
            t.text = "No Footstep";
            t.clicked += () => CallBack(mat, t);

            if (!map.ContainsKey(mat) || map.ContainsKey(mat) && map[mat] == "")
            {
                t.style.backgroundColor = new StyleColor(Color.white);
            }
            group.Add(t);
            for (int j = 0; j < footStepTypes.Length; j++)
            {
                
                string footstep = footStepTypes[j];
                var b = new UnityEngine.UIElements.Button();
                b.text = footstep;
                b.name = footstep;
                b.clicked += () => CallBack(mat, b);

                if(map.ContainsKey(mat) && map[mat] == footstep)
                {
                    b.style.backgroundColor = new StyleColor(Color.white);
                }

                group.Add(b);
                
                    

            }

            scroller.Add(group);
    
           


        }
    }

}


    

