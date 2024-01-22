using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Sprites;
using UnityEditor.VersionControl;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor.U2D.Sprites;
using System.Data.Common;
using Editor;
using NUnit.Framework.Constraints;
using Codice.Client.BaseCommands;

public class CharacterSpriteImporterEditor : EditorWindow
{
    private string selectedFilePath = string.Empty;

    public string newAssetPath;

    private string targetPath;

    private string texturePath;

    private string assetPath;

    string characterName = "";

    private const string ExtensionPNG = ".png";
    private const string PixelMapSuffix = ".map";
    private const string PixelWeightsPrefix = "weights.";

    private Texture2D spriteSheet;

    private string targetTileName = "";

    [MenuItem("Window/CharacterSpriteImporter")]
    public static void ShowWindow()
    {
        GetWindow<CharacterSpriteImporterEditor>("Character Sprite Importer");
    }

    private void OnGUI()
    {
        // GUI content for the custom window

        EditorGUILayout.LabelField("Character Sprite Importer", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Sprite File"))
        {
            selectedFilePath = EditorUtility.OpenFilePanel("Select a Sprite File", "", "png,jpg,jpeg");

            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                ImportTextureCopyToFolder(selectedFilePath, "Assets/Animation/Sprite Sheets");
            }
        }




        //for debugging
        EditorGUILayout.TextField("Selected File", selectedFilePath);
        //for debugging



        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Select Character's Sprite Sheet", spriteSheet, typeof(Texture2D), false);



        if (GUILayout.Button("Create Animations"))
        {
            if (spriteSheet != null)
            {
                CreateAnimations(spriteSheet);
            }
            else
            {
                Debug.LogError("Please select a sprite sheet first.");
            }

        }





        if (GUILayout.Button("Create NPC Prefab"))
        {
            targetTileName = characterName + "_Rotation_0";

            if (!string.IsNullOrEmpty(targetTileName))
            {
                CreatePrefabWithSlicedTile();
            }
        }

    }
    static string MakePathRelative(string absolutePath, string basePath)
    {
        if (absolutePath.StartsWith(basePath))
        {
            return "Assets" + absolutePath.Substring(basePath.Length);
        }
        return absolutePath;
    }


    private void CreateAnimations(Texture2D spriteSheet)
    {
        Debug.Log("Under Construction!");

        return;


        int frameWidth = 32;
        int frameHeight = 32;


        string clipname = "";
        // Adjust frame rate as needed

        string path = AssetDatabase.GetAssetPath(spriteSheet);
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].GetType() == typeof(Sprite) && !sprites[i].name.Contains("Temp"))
            {

                if (i == 0 || clipname != RemoveNumericValuesAndLastCharacter(sprites[i].name))  //if the current clip has ended OR it's the first sprite
                {
                    AnimationClip newclip = new AnimationClip();
                    newclip.frameRate = 12f;

                    clipname = RemoveNumericValuesAndLastCharacter(sprites[i].name);

                    newclip.name = clipname;

                    EditorCurveBinding curveBinding = new EditorCurveBinding();
                    curveBinding.type = typeof(SpriteRenderer);
                    curveBinding.path = "";
                    curveBinding.propertyName = "m_Sprite";

                    //clip = new clip

                }

                //clip = clip + animation

                Debug.Log(sprites[i].name);


            }
        }



    }



    private void CreatePrefabWithSlicedTile()
    {

        Object[] data = AssetDatabase.LoadAllAssetsAtPath(targetPath);

        if (data != null)
        {

            Sprite slicedTile = null;

            Object obj;

            //trying to find the slicedTile with the name "charactername + _Rotation_0"

            for (int i = 0; i < data.Length; i++)
            {

                obj = data[i];

                if (obj.GetType() == typeof(Sprite))
                {

                    if (obj.name == targetTileName)
                    {
                        //Debug.Log("Found: " + obj.name);
                        slicedTile = obj as Sprite;
                        break;
                    }
                }
            }


            if (slicedTile != null)
            {
                // Create a new GameObject with a SpriteRenderer and assign the sliced tile.
                GameObject prefabObject = new GameObject(characterName + "_temp");


                //Adding Children

                string LookDirectionPrefabPath = "Assets/Prefabs/NPC/LookDirection.prefab";

                GameObject LookDirectionPrefab = CreateChildWithPrefabPath(prefabObject.transform, LookDirectionPrefabPath);

                //Adding components

                Rigidbody2D rigidbody2D = prefabObject.AddComponent<Rigidbody2D>();

                SpriteRenderer spriteRenderer = prefabObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = slicedTile;

                NPCRotation npcRotation = prefabObject.AddComponent<NPCRotation>();
                npcRotation.lookPosition = LookDirectionPrefab;

                BoxCollider2D boxCollider2D = prefabObject.AddComponent<BoxCollider2D>();
                boxCollider2D.isTrigger = false;
                boxCollider2D.size = new Vector2(0.09069833f, 0.169854f);
                boxCollider2D.offset = new Vector2(0, -0.02f);

                Animator animator = prefabObject.AddComponent<Animator>();




                return;

                //adding components end

                string savepath = "Assets/Prefabs/Character Prefabs/" + characterName;

                // Check if the desired folder exists, if not: create it
                if (!AssetDatabase.IsValidFolder(savepath))
                {
                    AssetDatabase.CreateFolder("Assets/Prefabs/Character Prefabs", characterName);
                    Debug.Log("Folder created: " + savepath);
                }

                // Create and save the prefab.
                PrefabUtility.SaveAsPrefabAsset(prefabObject, "Assets/Prefabs/Character Prefabs/" + characterName + "/" + characterName + ".prefab");
                DestroyImmediate(prefabObject); // Destroy the temporary GameObject.

                Debug.Log("Prefab with sliced tile '" + characterName + "' created.");
            }
            else
            {
                Debug.LogError("Sliced tile with name '" + targetTileName + "' not found.");
            }
        }
        else
        {
            Debug.LogError("Sprite asset not found at the provided path.");
        }
    }

    private GameObject CreateChildWithPrefabPath(Transform parentObj, string path)
    {

        if (!string.IsNullOrEmpty(path))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                GameObject childObject = Instantiate(prefab);

                childObject.name = ReplaceSubstring(childObject.name, "(Clone)", "_" + parentObj.name);

                childObject.transform.parent = parentObj;

                Debug.Log("Added child: " + childObject.name + " to the parent: " + parentObj.name);

                return childObject;
            }
            else
            {
                Debug.LogError("Can't create child from prefab path, reason: prefab not found.");

                return null;
            }

        }
        else
        {

            Debug.LogError("Can't create child from prefab path, reason: path is empty.");

            return null;
        }


    }


    private void ImportTextureCopyToFolder(string sourcePath, string targetFolderPath)
    {
        // Ensure the source file exists
        if (!File.Exists(sourcePath))
        {
            Debug.LogError("File does not exist: " + sourcePath);
            return;
        }

        // Get the file name without extension
        string sourceFileName = Path.GetFileNameWithoutExtension(sourcePath);

        // Extract the subfolder name until the first underscore
        int underscoreIndex = sourceFileName.IndexOf('_');
        characterName = underscoreIndex >= 0 ? sourceFileName.Substring(0, underscoreIndex) : sourceFileName;

        // Create a copy of the file in the specified folder
        string targetDirectory = Path.Combine(targetFolderPath, characterName);
        Directory.CreateDirectory(targetDirectory);

        // Construct the target file path
        string targetFileName = sourceFileName + Path.GetExtension(sourcePath);
        targetPath = Path.Combine(targetDirectory, targetFileName);

        // Copy the file to the target folder
        File.Copy(sourcePath, targetPath, true);

        AssetDatabase.Refresh();

        ImportTextureWithCustomSettings(targetPath);
    }

    private void ImportTextureWithCustomSettings(string assetPath)
    {
        // Check if the asset exists at the specified path
        if (!File.Exists(assetPath))
        {
            Debug.LogError("File does not exist: " + assetPath);
            return;
        }

        // Get the asset importer for the texture
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;


        if (textureImporter != null)
        {
            // For Pixelart
            textureImporter.maxTextureSize = 4096;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.filterMode = UnityEngine.FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.isReadable = true;
            textureImporter.sRGBTexture = false;

            // Reimport the asset with the new import settings
            AssetDatabase.ImportAsset(assetPath);
        }
        else
        {
            Debug.LogError("Failed to get TextureImporter for: " + assetPath);
            return;
        }

        textureImporter.SaveAndReimport();

        //part for slicing

        SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
        factory.Init();

        Texture2D sourceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        int sliceSize = 32;

        if (sourceTexture != null)
        {

            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(sourceTexture);
            dataProvider.InitSpriteEditorDataProvider();
            //var textureImporter = (dataProvider.targetObject as TextureImporter);


            var textureProvider = dataProvider.GetDataProvider<ITextureDataProvider>();
            if (textureProvider != null)
            {
                int width = 0, height = 0;
                textureProvider.GetTextureActualWidthAndHeight(out width, out height);
                //var rect = InternalSpriteUtility.GenerateGridSpriteRectangles(obj as Texture2D, Vector2.zero, new Vector2(64,64), Vector2.zero, true); 
                var rect = InternalSpriteUtility.GenerateGridSpriteRectangles(sourceTexture, Vector2.zero, new Vector2(sliceSize, sliceSize), Vector2.zero);
                List<SpriteRect> rects = new List<SpriteRect>();
                Debug.Log(width + "," + height);
                Debug.Log(rect.Length);
                for (int i = 0; i < rect.Length; i++)
                {
                    SpriteRect r = new SpriteRect();

                    r.rect = rect[i];
                    r.alignment = SpriteAlignment.Center;
                    // left , bottom, right, top
                    //r.border = new Vector4(width / 3, height / 3, width / 3, height / 3);
                    //r.name = $"{NameTexture(i)}";
                    r.name = $"{NameTexture(i)}";
                    r.pivot = new Vector2(0.5f, 0.5f);
                    r.spriteID = GUID.Generate();
                    rects.Add(r);
                }
                dataProvider.SetSpriteRects(rects.ToArray());
                dataProvider.Apply();
            }

            textureImporter.SaveAndReimport();
        }
        else
        {
            Debug.LogError("Failed to load the texture asset at path: " + assetPath);
        }


    }

    public static string ReplaceSubstring(string input, string substring, string replacement)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(substring))
        {
            // Handle null or empty strings as needed.
            return input;
        }

        if (input.Contains(substring))
        {
            // Use String.Replace to replace occurrences of the substring.
            input = input.Replace(substring, replacement);
        }
        else
        {
            // If the substring is not found, append the replacement to the input.
            input += replacement;
        }

        return input;
    }

    private string NameTexture(int row)
    {

        string name = "Temp" + row;

        string run = "Run";

        string look = "Look";

        string right = "Right";

        string left = "Left";

        string up = "Up";

        string down = "Down";

        string armless = "Armless";

        if (row >= 10 && row <= 17)
        {
            name = characterName + "_" + "Rotation";
            name = name + "_" + (row - 10);
        }
        if (row >= 18 && row <= 27)
        {
            name = characterName + "_" + run + "_" + left;
            name = name + "_" + (row - 18);
        }
        if (row >= 28 && row <= 37)
        {
            name = characterName + "_" + run + "_" + left + "_" + down;
            name = name + "_" + (row - 28);
        }
        if (row >= 38 && row <= 45)
        {
            name = characterName + "_" + run + "_" + down;
            name = name + "_" + (row - 38);
        }
        if (row >= 46 && row <= 55)
        {
            name = characterName + "_" + run + "_" + right + "_" + down;
            name = name + "_" + (row - 46);
        }
        if (row >= 56 && row <= 65)
        {
            name = characterName + "_" + run + "_" + right;
            name = name + "_" + (row - 56);
        }
        if (row >= 66 && row <= 75)
        {
            name = characterName + "_" + run + "_" + right + "_" + up;
            name = name + "_" + (row - 66);
        }
        if (row >= 76 && row <= 83)
        {
            name = characterName + "_" + run + "_" + up;
            name = name + "_" + (row - 76);
        }
        if (row >= 84 && row <= 93)
        {
            name = characterName + "_" + run + "_" + left + "_" + up;
            name = name + "_" + (row - 84);
        }
        if (row >= 94 && row <= 101)
        {
            name = characterName + "_" + "Rotation" + "_" + armless;
            name = name + "_" + (row - 94);
        }
        if (row >= 102 && row <= 111)
        {
            name = characterName + "_" + run + "_" + left + "_" + look + "_" + left + "_" + armless;
            name = name + "_" + (row - 102);
        }
        if (row >= 112 && row <= 121)
        {
            name = characterName + "_" + run + "_" + left + "_" + down + "_" + look + "_" + left + "_" + down + "_" + armless;
            name = name + "_" + (row - 112);
        }
        if (row >= 122 && row <= 129)
        {
            name = characterName + "_" + run + "_" + down + "_" + look + "_" + down + "_" + armless;
            name = name + "_" + (row - 122);
        }
        if (row >= 130 && row <= 139)
        {
            name = characterName + "_" + run + "_" + right + "_" + down + "_" + look + "_" + right + "_" + down + "_" + armless;
            name = name + "_" + (row - 130);
        }
        if (row >= 140 && row <= 149)
        {
            name = characterName + "_" + run + "_" + right + "_" + look + "_" + right + "_" + armless;
            name = name + "_" + (row - 140);
        }
        if (row >= 150 && row <= 159)
        {
            name = characterName + "_" + run + "_" + right + "_" + up + "_" + look + "_" + right + "_" + up + "_" + armless;
            name = name + "_" + (row - 150);
        }
        if (row >= 160 && row <= 167)
        {
            name = characterName + "_" + run + "_" + up + "_" + look + "_" + up + "_" + armless;
            name = name + "_" + (row - 160);
        }
        if (row >= 168 && row <= 177)
        {
            name = characterName + "_" + run + "_" + left + "_" + up + "_" + look + "_" + left + "_" + up + "_" + armless;
            name = name + "_" + (row - 168);
        }
        if (row >= 178 && row <= 183)
        {
            name = characterName + "_" + run + "_" + down + "_" + look + "_" + left + "_" + down + "_" + armless;
            name = name + "_" + (row - 178);
        }
        if (row >= 184 && row <= 189)
        {
            name = characterName + "_" + run + "_" + down + "_" + look + "_" + left + "_" + up + "_" + armless;
            name = name + "_" + (row - 184);
        }
        if (row >= 190 && row <= 195)
        {
            name = characterName + "_" + run + "_" + down + "_" + look + "_" + left + "_" + armless;
            name = name + "_" + (row - 190);
        }
        if (row >= 196 && row <= 201)
        {
            name = characterName + "_" + run + "_" + down + "_" + look + "_" + right + "_" + down + "_" + armless;
            name = name + "_" + (row - 196);
        }
        if (row >= 202 && row <= 207)
        {
            name = characterName + "_" + run + "_" + down + "_" + look + "_" + right + "_" + up + "_" + armless;
            name = name + "_" + (row - 202);
        }
        if (row >= 208 && row <= 213)
        {
            name = characterName + "_" + run + "_" + down + "_" + look + "_" + right + "_" + armless;
            name = name + "_" + (row - 208);
        }
        if (row >= 214 && row <= 221)
        {
            name = characterName + "_" + run + "_" + down + "_" + look + "_" + up + "_" + armless;
            name = name + "_" + (row - 214);
        }
        if (row >= 222 && row <= 231)
        {
            name = characterName + "_" + run + "_" + left + "_" + down + "_" + look + "_" + down + "_" + armless;
            name = name + "_" + (row - 222);
        }
        if (row >= 232 && row <= 239)
        {
            name = characterName + "_" + run + "_" + left + "_" + down + "_" + look + "_" + left + "_" + up + "_" + armless;
            name = name + "_" + (row - 232);
        }
        if (row >= 240 && row <= 247)
        {
            name = characterName + "_" + run + "_" + left + "_" + down + "_" + look + "_" + left + "_" + armless;
            name = name + "_" + (row - 240);
        }
        if (row >= 248 && row <= 257)
        {
            name = characterName + "_" + run + "_" + left + "_" + down + "_" + look + "_" + right + "_" + down + "_" + armless;
            name = name + "_" + (row - 248);
        }
        if (row >= 258 && row <= 265)
        {
            name = characterName + "_" + run + "_" + left + "_" + down + "_" + look + "_" + right + "_" + up + "_" + armless;
            name = name + "_" + (row - 258);
        }
        if (row >= 266 && row <= 273)
        {
            name = characterName + "_" + run + "_" + left + "_" + down + "_" + look + "_" + right + "_" + armless;
            name = name + "_" + (row - 266);
        }
        if (row >= 274 && row <= 281)
        {
            name = characterName + "_" + run + "_" + left + "_" + down + "_" + look + "_" + up + "_" + armless;
            name = name + "_" + (row - 274);
        }
        if (row >= 282 && row <= 289)
        {
            name = characterName + "_" + run + "_" + left + "_" + look + "_" + down + "_" + armless;
            name = name + "_" + (row - 282);
        }
        if (row >= 290 && row <= 297)
        {
            name = characterName + "_" + run + "_" + left + "_" + look + "_" + right + "_" + down + "_" + armless;
            name = name + "_" + (row - 290);
        }
        if (row >= 298 && row <= 305)
        {
            name = characterName + "_" + run + "_" + left + "_" + look + "_" + right + "_" + up + "_" + armless;
            name = name + "_" + (row - 298);
        }
        if (row >= 306 && row <= 315)
        {
            name = characterName + "_" + run + "_" + left + "_" + look + "_" + right + "_" + armless;
            name = name + "_" + (row - 306);
        }
        if (row >= 316 && row <= 323)
        {
            name = characterName + "_" + run + "_" + left + "_" + look + "_" + up + "_" + armless;
            name = name + "_" + (row - 316);
        }
        if (row >= 324 && row <= 333)
        {
            name = characterName + "_" + run + "_" + right + "_" + down + "_" + look + "_" + down + "_" + armless;
            name = name + "_" + (row - 324);
        }
        if (row >= 334 && row <= 343)
        {
            name = characterName + "_" + run + "_" + right + "_" + down + "_" + look + "_" + left + "_" + down + "_" + armless;
            name = name + "_" + (row - 334);
        }
        if (row >= 344 && row <= 351)
        {
            name = characterName + "_" + run + "_" + right + "_" + down + "_" + look + "_" + left + "_" + up + "_" + armless;
            name = name + "_" + (row - 344);
        }
        if (row >= 352 && row <= 359)
        {
            name = characterName + "_" + run + "_" + right + "_" + down + "_" + look + "_" + left + "_" + armless;
            name = name + "_" + (row - 352);
        }
        if (row >= 360 && row <= 367)
        {
            name = characterName + "_" + run + "_" + right + "_" + down + "_" + look + "_" + right + "_" + up + "_" + armless;
            name = name + "_" + (row - 360);
        }
        if (row >= 368 && row <= 375)
        {
            name = characterName + "_" + run + "_" + right + "_" + down + "_" + look + "_" + right + "_" + armless;
            name = name + "_" + (row - 368);
        }
        if (row >= 376 && row <= 383)
        {
            name = characterName + "_" + run + "_" + right + "_" + down + "_" + look + "_" + up + "_" + armless;
            name = name + "_" + (row - 376);
        }
        if (row >= 384 && row <= 391)
        {
            name = characterName + "_" + run + "_" + right + "_" + look + "_" + down + "_" + armless;
            name = name + "_" + (row - 384);
        }
        if (row >= 392 && row <= 399)
        {
            name = characterName + "_" + run + "_" + right + "_" + look + "_" + left + "_" + down + "_" + armless;
            name = name + "_" + (row - 392);
        }
        if (row >= 400 && row <= 407)
        {
            name = characterName + "_" + run + "_" + right + "_" + look + "_" + left + "_" + up + "_" + armless;
            name = name + "_" + (row - 400);
        }
        if (row >= 408 && row <= 417)
        {
            name = characterName + "_" + run + "_" + right + "_" + look + "_" + left + "_" + armless;
            name = name + "_" + (row - 408);
        }
        if (row >= 418 && row <= 425)
        {
            name = characterName + "_" + run + "_" + right + "_" + look + "_" + up + "_" + armless;
            name = name + "_" + (row - 418);
        }
        if (row >= 426 && row <= 433)
        {
            name = characterName + "_" + run + "_" + up + "_" + look + "_" + down + "_" + armless;
            name = name + "_" + (row - 426);
        }
        if (row >= 434 && row <= 439)
        {
            name = characterName + "_" + run + "_" + up + "_" + look + "_" + left + "_" + down + "_" + armless;
            name = name + "_" + (row - 434);
        }
        if (row >= 440 && row <= 445)
        {
            name = characterName + "_" + run + "_" + up + "_" + look + "_" + left + "_" + up + "_" + armless;
            name = name + "_" + (row - 440);
        }
        if (row >= 446 && row <= 451)
        {
            name = characterName + "_" + run + "_" + up + "_" + look + "_" + left + "_" + armless;
            name = name + "_" + (row - 446);
        }
        if (row >= 452 && row <= 457)
        {
            name = characterName + "_" + run + "_" + up + "_" + look + "_" + right + "_" + down + "_" + armless;
            name = name + "_" + (row - 452);
        }
        if (row >= 458 && row <= 463)
        {
            name = characterName + "_" + run + "_" + up + "_" + look + "_" + right + "_" + up + "_" + armless;
            name = name + "_" + (row - 458);
        }
        if (row >= 464 && row <= 469)
        {
            name = characterName + "_" + run + "_" + up + "_" + look + "_" + right + "_" + armless;
            name = name + "_" + (row - 464);
        }
        if (row >= 470 && row <= 479)
        {
            name = characterName + "_" + run + "_" + right + "_" + up + "_" + look + "_" + left + "_" + up + "_" + armless;
            name = name + "_" + (row - 470);
        }
        if (row >= 480 && row <= 487)
        {
            name = characterName + "_" + run + "_" + right + "_" + up + "_" + look + "_" + right + "_" + armless;
            name = name + "_" + (row - 480);
        }
        if (row >= 488 && row <= 497)
        {
            name = characterName + "_" + run + "_" + right + "_" + up + "_" + look + "_" + left + "_" + armless;
            name = name + "_" + (row - 488);
        }
        if (row >= 498 && row <= 505)
        {
            name = characterName + "_" + run + "_" + right + "_" + up + "_" + look + "_" + down + "_" + armless;
            name = name + "_" + (row - 498);
        }
        if (row >= 506 && row <= 513)
        {
            name = characterName + "_" + run + "_" + right + "_" + up + "_" + look + "_" + left + "_" + down + "_" + armless;
            name = name + "_" + (row - 506);
        }
        if (row >= 514 && row <= 523)
        {
            name = characterName + "_" + run + "_" + right + "_" + up + "_" + look + "_" + up + "_" + armless;
            name = name + "_" + (row - 514);
        }
        if (row >= 524 && row <= 533)
        {
            name = characterName + "_" + run + "_" + right + "_" + up + "_" + look + "_" + right + "_" + down + "_" + armless;
            name = name + "_" + (row - 524);
        }
        if (row >= 534 && row <= 543)
        {
            name = characterName + "_" + run + "_" + left + "_" + up + "_" + look + "_" + right + "_" + up + "_" + armless;
            name = name + "_" + (row - 534);
        }
        if (row >= 544 && row <= 551)
        {
            name = characterName + "_" + run + "_" + left + "_" + up + "_" + look + "_" + left + "_" + armless;
            name = name + "_" + (row - 544);
        }
        if (row >= 552 && row <= 561)
        {
            name = characterName + "_" + run + "_" + left + "_" + up + "_" + look + "_" + right + "_" + armless;
            name = name + "_" + (row - 552);
        }
        if (row >= 562 && row <= 569)
        {
            name = characterName + "_" + run + "_" + left + "_" + up + "_" + look + "_" + down + "_" + armless;
            name = name + "_" + (row - 562);
        }
        if (row >= 570 && row <= 579)
        {
            name = characterName + "_" + run + "_" + left + "_" + up + "_" + look + "_" + left + "_" + down + "_" + armless;
            name = name + "_" + (row - 570);
        }
        if (row >= 580 && row <= 589)
        {
            name = characterName + "_" + run + "_" + left + "_" + up + "_" + look + "_" + up + "_" + armless;
            name = name + "_" + (row - 580);
        }
        if (row >= 590 && row <= 597)
        {
            name = characterName + "_" + run + "_" + left + "_" + up + "_" + look + "_" + right + "_" + down + "_" + armless;
            name = name + "_" + (row - 590);
        }






        //add as you draw more animations

        return name;
    }

    static string RemoveNumericValuesAndLastCharacter(string input)
    {
        // Use a regular expression to match numeric values at the end of the string
        string numericPattern = @"\d+$";
        string resultWithoutNumericValues = Regex.Replace(input, numericPattern, string.Empty);

        // Remove the last character from the result
        if (!string.IsNullOrEmpty(resultWithoutNumericValues))
        {
            resultWithoutNumericValues = resultWithoutNumericValues.Substring(0, resultWithoutNumericValues.Length - 1);
        }

        return resultWithoutNumericValues;
    }



}
