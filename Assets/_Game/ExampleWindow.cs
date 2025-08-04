using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class ExampleWindow :EditorWindow
{
   
    private int rows = 12;
    private int cols = 12;
    private int[,] grid;
    private int currentTileID = 1; // 
    private Vector2 scrollPos;
    //
    private string fileName = "level1.json";
    private TextAsset jsonFile;
    [MenuItem("Window/LevelEditor")]
    public static void ShowWindow()
    {
        GetWindow<ExampleWindow>("Level Editor");
    }

    private void OnEnable()
    {
        InitGrid();
    }

    void InitGrid()
    {
        grid = new int[rows, cols];
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label("Cài đặt lưới", EditorStyles.boldLabel);

        rows = EditorGUILayout.IntField("Rows", rows);
        cols = EditorGUILayout.IntField("Cols", cols);

        if (GUILayout.Button("Tạo Lại Lưới"))
        {
            InitGrid();
        }

        currentTileID = EditorGUILayout.IntField("Tile ID hiện tại", currentTileID);

        GUILayout.Space(10);
        DrawGrid();

        GUILayout.Space(10);
        GUILayout.Label("Kéo thả file JSON để load:", EditorStyles.boldLabel);
        jsonFile = EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false) as TextAsset;

        if (jsonFile != null && GUILayout.Button("Load từ JSON"))
        {
            LoadGridFromJson(jsonFile);
        }
        fileName = EditorGUILayout.TextField("Tên file JSON", fileName);

        if (GUILayout.Button("Lưu Level ra JSON"))
        {
            SaveLevelToJson();
        }
        GUILayout.EndScrollView();
    }
    private void LoadGridFromJson(TextAsset jsonFile)
    {
        try
        {
            string json = jsonFile.text;
            int[] arr = JsonHelper.FromJson<int>(json);

            // 1D->2D
            int[,] matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = arr[i * cols + j];
                }
            }
            
            grid = new int[rows, cols];

            // GAN GRID
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    grid[i, j] = matrix[i,j];
                }
            }

            Debug.Log("Đã tải lưới từ JSON thành công!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi tải file JSON: " + e.Message);
        }
    }
    void DrawGrid()
    {
        if (grid == null) return;

        for (int r = 0; r < rows; r++)
        {
            GUILayout.BeginHorizontal();
            for (int c = 0; c < cols; c++)
            {
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = (grid[r, c] == 0) ? Color.gray : Color.green;

                if (GUILayout.Button(grid[r, c].ToString(), GUILayout.Width(40), GUILayout.Height(40)))
                {
                    grid[r, c] = currentTileID;
                }

                GUI.backgroundColor = oldColor;
            }
            GUILayout.EndHorizontal();
        }
    }

    void SaveLevelToJson()
    {
        //2D->1D
        int[] matrix1D = new int[rows*cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                matrix1D[r*cols+c] = grid[r, c];
            }
        }

        string json = JsonHelper.ToJson(matrix1D,cols,rows,true);
        // duong dan thu muc JsonData
        string folderPath = Application.dataPath + "/JsonData";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Tạo thư mục JsonData tại: " + folderPath);
        }

        // tao thanh file
        string filePath = Path.Combine(folderPath, fileName);

        // Ghi file
        File.WriteAllText(filePath, json);

        // Refresh AssetDatabase de Unity thay file moi
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Thông báo", "Đã lưu level ra JSON:\nAssets/JsonData/" + fileName, "OK");
    }

}

