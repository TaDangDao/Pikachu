using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using TMPro;

public class TileManager : MonoBehaviour
{
    [SerializeField] int tileCols = 0;
    [SerializeField] int tileRows = 0;
    [SerializeField] Tile prefab;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private LayerMask clickableLayer;
    [SerializeField] private Tile[,] gridArray;
    [SerializeField] private float time;
    [SerializeField] private List<TileData> tileData= new List<TileData>();
    [SerializeField] private int YOffset = 20;// khoang cach giua tile va vi tri cua no trong mang
    [SerializeField] private int XOffset = 20;// khoang cach giua tile va vi tri cua no trong mang
    [SerializeField] private EffectManager effectManager;
    private int randomIndex;
    Dictionary<Type, int> typeDictionary= new Dictionary<Type, int>();
    List<Vector2Int> nonEmptyPositions = new List<Vector2Int>();
    private Tile tile_1;
    private Tile tile_2;
    List<Vector2> points= new List<Vector2>();
    string folderPath = Application.dataPath + "/JsonData";
    string fileName ;
    List<Tile> listRemove = new List<Tile>();
    List<HintPair> currentHints = new List<HintPair>();
    private Vector2 gridPos;
    private Vector2 instantiatePos= Vector2.zero;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&GameManager_.Instance.State==GameState.Playing)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, clickableLayer);
            if (hit.collider != null)
            {
                SelectTile(hit.collider.GetComponent<Tile>());
            }
            else
            {
            }
        }
     
    }
    // khoi tao mang
    public void OnInit(int level)
    {
        fileName = "level" + level.ToString() + ".json";
        string filePath = Path.Combine(folderPath, fileName);
        int[,] arrayType = LoadMatrixFromJson(filePath);
        float timeDelay;
        gridArray = new Tile[tileCols, tileRows];
        gridPos.x=-(float)(tileCols - 1) / 2;
        gridPos.y=-(float)(tileRows - 1) / 2;
        if(tileCols>16||tileRows>13){
            gridPos.y -= 1;
        }
        transform.position = gridPos;
        randomIndex = UnityEngine.Random.Range(0, 2);
        timeDelay = 0.5f;
        for (int i = 0; i < tileCols; i++)
        {
            for (int j = 0; j < tileRows; j++)
            {
                if (gridArray[i, j] == null)
                {
                    gridArray[i, j] = Instantiate(prefab, this.transform);
                }   
                gridArray[i, j].OnInit();
                gridArray[i, j].SetPosition(i, j);// set vi tri cua tile
                // neu la ria ngoai cung cua mang thhi khong lam animation + set empty
                if (i == 0 || j == 0 || j == tileRows - 1 || i == tileCols - 1)
                {
                    instantiatePos.x = i;
                    instantiatePos.y = j;
                    gridArray[i, j].transform.localPosition = instantiatePos;
                    gridArray[i, j].SetEmpty(true);
                }
                else
                {
                    // khoi tao vi tri bat dau cho tile
                    instantiatePos.x = i+ YOffset*randomIndex;
                    instantiatePos.y = j + YOffset;
                    gridArray[i, j].transform.localPosition = instantiatePos;
                    // thuc hien animation roi xuong
                    gridArray[i, j].StartCoroutine(gridArray[i, j].MoveDown(timeDelay));
                    timeDelay += 0.01f;
                    if (arrayType!= null)
                    {
                        // neu la obstacle thi khong set tile data
                        if (arrayType[tileRows - 1 - j, i] != (int)Type.obstacle)
                        {
                            // set tile data cho tile
                            gridArray[i, j].SetTileData(tileData[arrayType[tileRows - 1 - j, i] / 10]);
                            // them vao dictionary so luong tile theo type
                            if (!typeDictionary.ContainsKey((Type)arrayType[tileRows - 1 - j, i]))
                            {
                                List<Vector2> list = new List<Vector2>();
                                list.Add(new Vector2(i,j));
                                typeDictionary.Add((Type)arrayType[tileRows - 1 - j, i], 1);
                            }
                            else
                            {
                                typeDictionary[(Type)arrayType[tileRows - 1 - j, i]]++;
                                    
                            }
                            
                        }
                    }
                }
            }
        }
        // tim tat ca tile co the match duoc, cho vao 1 lisst
        currentHints = FindAllHints();
        Debug.Log("Count"+currentHints.Count);
        // neu khong co tile nao match duoc thi shuffle lai
        if (currentHints.Count==0)
        {
            Shuffle();
        }
    }
    // clear mang va dictionary
    public void OnDespawn()
    {
        if (gridArray!=null)
        {
            for (int i = 0; i < tileCols; i++)
            {
                for (int j = 0; j < tileRows; j++)
                {
                    if (gridArray[i, j] != null)
                    {
                        Destroy(gridArray[i, j].gameObject);
                    }
                }
            }
        }
       typeDictionary.Clear();
    }
    // chon tile    
    public void SelectTile(Tile tile)
    {
        // play sound effect click
        SoundManager.PlaySoundEffect(SoundType.CLICK);
        Debug.Log(SoundManager.Instance.GetBGVolume()+" "+SoundManager.Instance.GetSFXVolume());
        // neu tile_1 la null thi gan tile_1 bang tile
        if (tile_1 == null)
        {
            tile_1 = tile;
            tile_1.ShowSelected();
        }
        // con khong thi gan tile_2 bang tile
        else if (tile_2 == null)
        {
            // neu tile_1 khac tile hien tai thi gan tile_2 bang tile, con khong thi huy chon tile_1
            if (tile != tile_1)
            {
                tile_2 = tile;
                tile_2.ShowSelected();
                Mid_Line line = Check2Point(tile_1, tile_2);// tim duong di co the
                // neu co duong di 
                if (line != null)
                {
                    effectManager.SpawnEffect((Type)tile_1.GetTileType(), tile_1.GetPos());// phat hieu ung 
                    effectManager.SpawnEffect((Type)tile_2.GetTileType(), tile_2.GetPos());// phat hieu ung 
                    Debug.Log("Chon 2 tile");
                    DrawPath(tile_1.GetPos(), line.GetPoint1(), line.GetPoint2(), tile_2.GetPos());// ve duong di
                    // xoa cac goi y chua 2 tile
                    currentHints.RemoveAll(hint => {
                        if (hint.Contains(tile_1) || hint.Contains(tile_2)) { 
                            //hint.Test.gameObject.SetActive(false);
                            return true; }
                        else return false;
                    } 
                    //hint.Contains(tile_1) || hint.Contains(tile_2)
                    );
                    // tinh toan diem
                    GameManager_.Instance.CalculateScore();
                    // tinh toan streak
                    GameManager_.Instance.PlusStreak();
                    // tru so luong tile theo type
                    typeDictionary[(Type)tile_1.GetTileType()] -= 2;
                    // tat hien thi 2 tile da chon sau 1 khoang time
                    Invoke(nameof(Clear2Tile), time);
                }
                else
                {
                    // khong co duong di thi thuc hien shake 2 tile da chon, phat sound va reset streak, gan tile_1 bang tile_2
                    GameManager_.Instance.ResetStreak();
                    SoundManager.PlaySoundEffect(SoundType.MISS);
                    tile_1.Shake();
                    tile_2.Shake();
                    DeSelected(tile_1);
                }
            }
            else
            {
                DeSelectedAll();
            }
        }
    }
    // huy cho tile_1, gan tile_2 bang null, gan tile_1 bang tile_2
    public void DeSelected(Tile tile)
    {
        tile.HideSelected();
        tile_1 = tile_2;
        tile_2 = null;
    }
    // huy chon ca 2 tile, gan tile_1 va tile_2 bang null
    public void DeSelectedAll()
    {
        tile_1.HideSelected();
        tile_1 = null;
        tile_2 = null;
    }
    // check xem co the win chua
    public bool CheckEndGame()
    {
        Debug.Log("Check end game");
        for (int i = 0; i < tileCols; i++)
        {
            for (int j = 0; j < tileRows; j++)
            {
                if (!gridArray[i, j].IsEmpty() && gridArray[i,j].GetTileType()!=(int)Type.obstacle)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // ham kiem tra 2 tile co the ket noi duoc khong
    public Mid_Line Check2Point(Tile tile_1, Tile tile_2)
    {
        if(tile_1 != null && tile_2 != null)
        {
            if (tile_1.GetTileType()==tile_2.GetTileType()&&tile_1.GetTileType()!=(int)Type.obstacle) {
                if (CheckLineX(tile_1, tile_2) != null)
                {
                    Debug.Log("Line X");
                    return CheckLineX(tile_1, tile_2);
                }
                if (CheckLineY(tile_1, tile_2) != null)
                {
                    Debug.Log("Line Y");
                    return CheckLineY(tile_1, tile_2);
                }
                
                if (CheckRectY(tile_1, tile_2) != null)
                {
                    return CheckRectY(tile_1, tile_2);
                }
                if (CheckRectX(tile_1, tile_2) != null)
                {
                    return CheckRectX(tile_1, tile_2);
                }
                if (Check_U_ShapeX(tile_1, tile_2, 1) != null)
                {
                    return Check_U_ShapeX(tile_1, tile_2, 1);
                }
                if (Check_U_ShapeX(tile_1, tile_2, -1) != null)
                {
                    return Check_U_ShapeX(tile_1, tile_2, -1);
                }
                if (Check_U_ShapeY(tile_1, tile_2, 1) != null)
                {
                    return Check_U_ShapeY(tile_1, tile_2, 1);
                }
                if (Check_U_ShapeY(tile_1, tile_2, -1) != null)
                {
                    return Check_U_ShapeY(tile_1, tile_2, -1);
                }
            }
        }
        return null;
    }
    // kiem tra 2 tile co the noi theo truc X
    public Mid_Line CheckLineX(Tile tile_1, Tile tile_2)
    {
        if (Mathf.Abs(tile_1.GetLocalPos().y - tile_2.GetLocalPos().y)<0.001f)
        {
            int min;
            int max;
            if (tile_1.GetLocalPos().x < tile_2.GetLocalPos().x)
            {
                min = (int)tile_1.GetLocalPos().x;
                max = (int)tile_2.GetLocalPos().x;
            }
            else
            {
                min = (int)tile_2.GetLocalPos().x;
                max = (int)tile_1.GetLocalPos().x;
            }
            for (int i = min; i <= max; i++)
            {
                if (!gridArray[i, (int)tile_1.GetLocalPos().y].IsEmpty())
                {
                    if (gridArray[i, (int)tile_1.GetLocalPos().y].IsSelected())
                    {
                        continue;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return new Mid_Line(tile_1.GetPos(), tile_2.GetPos());
        }
        return null;

    }
    // kiem tra 2 tile co the noi theo truc Y
    public Mid_Line CheckLineY(Tile tile_1, Tile tile_2)
    {
        if (Mathf.Abs(tile_1.GetLocalPos().x - tile_2.GetLocalPos().x)<0.001f)
        {
            int min;
            int max;
            if (tile_1.GetLocalPos().y < tile_2.GetLocalPos().y)
            {
                min = (int)tile_1.GetLocalPos().y;
                max = (int)tile_2.GetLocalPos().y;
            }
            else
            {
                min = (int)tile_2.GetLocalPos().y;
                max = (int)tile_1.GetLocalPos().y;
            }
            for (int i = min; i <= max; i++)
            {
                if (!gridArray[(int)tile_1.GetLocalPos().x,i].IsEmpty())
                {
                    if (gridArray[(int)tile_1.GetLocalPos().x,i].IsSelected())
                    {
                        continue;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return new Mid_Line(tile_1.GetPos(), tile_2.GetPos());
        }
        return null;
    }
    
    // Kiem tra xem co the noi theo hinh Z theo truc X 
    public Mid_Line CheckRectX(Tile tile_1, Tile tile_2)
    {
        int minX = (int)tile_1.GetLocalPos().x;
        int maxX = (int)tile_2.GetLocalPos().x;
        if (tile_1.GetLocalPos().x > tile_2.GetLocalPos().x)
        {
            minX= (int)tile_2.GetLocalPos().x;
            maxX= (int)tile_1.GetLocalPos().x;
        }
        for (int i = minX; i <= maxX; i++) {
            if (CheckLineX(tile_1,gridArray[i, (int)tile_1.GetLocalPos().y])!=null
                && CheckLineX(tile_2, gridArray[i,(int)tile_2.GetLocalPos().y])!=null
                && CheckLineY(gridArray[i, (int)tile_1.GetLocalPos().y], gridArray[i, (int)tile_2.GetLocalPos().y])!=null 
                )
            {
                return new Mid_Line(gridArray[i, (int)tile_1.GetLocalPos().y].GetPos(), gridArray[i, (int)tile_2.GetLocalPos().y].GetPos());
            }
        }
        return null;
    }
    
    // Kiem tra xem co the noi theo hinh Z theo truc Y
    public Mid_Line CheckRectY(Tile tile_1, Tile tile_2)
    {

        int minY = (int)tile_1.GetLocalPos().y;
        int maxY = (int)tile_2.GetLocalPos().y;
        if (tile_1.GetLocalPos().y > tile_2.GetLocalPos().y)
        {
            minY= (int)tile_2.GetLocalPos().y;
            maxY= (int)tile_1.GetLocalPos().y;
        }
        for (int i = minY; i <= maxY; i++) {
            if (CheckLineY(tile_1,gridArray[(int)tile_1.GetLocalPos().x,i])!=null
                && CheckLineY(tile_2, gridArray[(int)tile_2.GetLocalPos().x,i])!=null
                && CheckLineX(gridArray[(int)tile_1.GetLocalPos().x,i], gridArray[(int)tile_2.GetLocalPos().x,i])!=null 
                )
            {
                return new Mid_Line(gridArray[(int)tile_1.GetLocalPos().x, i].GetPos(), gridArray[(int)tile_2.GetLocalPos().x, i].GetPos());
            }
        }
        return null;
    }

    // Kiem tra xem co the noi theo hinh U theo truc X
    public Mid_Line Check_U_ShapeX(Tile tile_1, Tile tile_2, int j)
    {
        int minX = (int)tile_1.GetLocalPos().x;
        int maxX = (int)tile_2.GetLocalPos().x;
        if (tile_1.GetLocalPos().x > tile_2.GetLocalPos().x)
        {
            minX = (int)tile_2.GetLocalPos().x;
            maxX = (int)tile_1.GetLocalPos().x;
        }
        if (j == -1)
        {
            for (int i = minX; i >=0; i--) {
                if (CheckLineX(tile_1, gridArray[i, (int)tile_1.GetLocalPos().y]) != null
              && CheckLineX(tile_2, gridArray[i, (int)tile_2.GetLocalPos().y]) != null
              && CheckLineY(gridArray[i, (int)tile_2.GetLocalPos().y], gridArray[i, (int)tile_1.GetLocalPos().y]) != null
              )
                {
                    return new Mid_Line(gridArray[i, (int)tile_1.GetLocalPos().y].GetPos(), gridArray[i, (int)tile_2.GetLocalPos().y].GetPos());
                }
            }
        }
        else
        {
            for (int i = maxX; i < tileCols; i++) {

                if (CheckLineX(tile_1, gridArray[i, (int)tile_1.GetLocalPos().y]) != null
                 && CheckLineX(tile_2, gridArray[i, (int)tile_2.GetLocalPos().y]) != null
                 && CheckLineY(gridArray[i, (int)tile_2.GetLocalPos().y], gridArray[i, (int)tile_1.GetLocalPos().y]) != null
                   )
                {
                    return new Mid_Line(gridArray[i, (int)tile_1.GetLocalPos().y].GetPos(), gridArray[i, (int)tile_2.GetLocalPos().y].GetPos());
                }
            }
        }

        return null;
    }

    // Kiem tra xem co the noi theo hinh U theo truc Y
    public Mid_Line Check_U_ShapeY(Tile tile_1, Tile tile_2, int j)
    {
        int minY = (int)tile_1.GetLocalPos().y;
        int maxY = (int)tile_2.GetLocalPos().y;
        if (tile_1.GetLocalPos().x > tile_2.GetLocalPos().x)
        {
            minY = (int)tile_2.GetLocalPos().y;
            maxY = (int)tile_1.GetLocalPos().y;
        }
        if (j == -1)
        {
            for (int i = minY; i >=0; i--) {
                if (CheckLineY(tile_1, gridArray[(int)tile_1.GetLocalPos().x, i]) != null
                && CheckLineY(tile_2, gridArray[(int)tile_2.GetLocalPos().x, i]) != null
                && CheckLineX(gridArray[(int)tile_2.GetLocalPos().x, i], gridArray[(int)tile_1.GetLocalPos().x, i]) != null
                )
                {
                    Debug.Log("Check U ShapeY down");
                    return new Mid_Line(gridArray[(int)tile_1.GetLocalPos().x, i].GetPos(), gridArray[(int)tile_2.GetLocalPos().x, i].GetPos());
                }
            }
        }
        else
        {
            for (int i = maxY; i < tileRows; i++) {

                if (CheckLineY(tile_1, gridArray[(int)tile_1.GetLocalPos().x, i]) != null
               && CheckLineY(tile_2, gridArray[(int)tile_2.GetLocalPos().x, i]) != null
               && CheckLineX(gridArray[(int)tile_2.GetLocalPos().x, i], gridArray[(int)tile_1.GetLocalPos().x, i]) != null
               )
                {
                    Debug.Log("Check U ShapeY up");
                    return new Mid_Line(gridArray[(int)tile_1.GetLocalPos().x, i].GetPos(), gridArray[(int)tile_2.GetLocalPos().x, i].GetPos());
                }
            }
        }

        return null;
    }

    // Ve duong di giua 2 tile
    public void DrawPath(
        Vector2 p1,
        Vector2 lineP1,
        Vector2 lineP2,
        Vector2 p2
        )
    {
        Vector2 connectWithP1;
        Vector2 connectWithP2;
        if (p1.x == lineP1.x || p1.y == lineP1.y)
        {
            connectWithP1 = lineP1;
            connectWithP2 = lineP2;
        }
        else{
            connectWithP1 = lineP2;
            connectWithP2 = lineP1;
        }
        points.Add(p1);
        points.Add(connectWithP1);
        points.Add(connectWithP2);
        points.Add(p2);
        
        lineRenderer.positionCount = 4;
        for(int i = 0; i < 4; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }

    }
    public void Clear2Tile()
    {
        SoundManager.PlaySoundEffect(SoundType.MATCH);
        tile_1?.SetEmpty(true);
        tile_2?.SetEmpty(true);
        tile_1 = null;
        tile_2 = null;
        lineRenderer.positionCount=0;
        points.Clear();
        //Debug.Log("hint pair"+currentHints.Count);
        if (currentHints.Count == 0)
        {
            RefreshHintList();
        }
        if (CheckEndGame())
        {
            Debug.Log("Endgame");
            GameManager_.Instance.ShowEndGameCanvas(1);
        }
    }

    // tai ma tran tu file json
    public int[,] LoadMatrixFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File không tồn tại: " + filePath);
            filePath = Path.Combine(folderPath, "level1.json");
        }
        string json = File.ReadAllText(filePath);
        int[]arr = JsonHelper.FromJson<int>(json);
        Tuple<int, int> colsAndRows = JsonHelper.FromJsonColsAndRows<int>(json);
        tileCols=colsAndRows.Item1;
        tileRows=colsAndRows.Item2;

        int[,] matrix = new int[tileRows, tileCols];
        for (int i = 0; i < tileRows; i++)
        {
            for (int j = 0; j < tileCols; j++)
            {
                matrix[i, j] = arr[i * tileCols + j];
            }
        }

        return matrix;
    }

    // Clear booster, xoa tat ca cac con co the noi duoc voi 1 tile duoc chon 
    public void Booster()
    {
        if (tile_1!=null)
        {
                RefreshHintList();
                ClearMatchableTile(tile_1);
        }
       
    }

    // Clear tile tile co the an duoc voi tile duoc chon 
    public void ClearMatchableTile(Tile t1)
    {
        // neu dictionary chua type cua tile duoc chon thi tru so luong booster, tru so luong tile theo type, tinh diem, xoa tile, xoa goi y
        if (typeDictionary.ContainsKey((Type)t1.GetTileType()))
        {
            GameManager_.Instance.ChangeBoosterCount(-1);
            typeDictionary[(Type)t1.GetTileType()]--;
            GameManager_.Instance.CalculateScore(90);
            // tim trong list hint tat ca goi y chua tile duoc chon
            int count = currentHints.FindAll(hints => hints.Contains(t1)).Count;
            listRemove.Clear();
            Tile t2;
            // neu khong co goi y nao chua tile duoc chon thi xoa tile va tinh diem
            if (count <= 0)
            {
                Debug.Log("chon");
                tile_1?.SetSelected(false);
                t1.BlowUp();
                //t1.SetEmpty(true);
                GameManager_.Instance.CalculateScore(90);
                tile_1 = null;
            }
            else
            {// nguoc lai , neu co goi y chua tile duoc chon thi xoa tat ca cac tile trong goi y do
                currentHints.RemoveAll(hints =>
                {
                    if (hints.Contains(t1))
                    {
                        t2 = ((Vector2.Distance(t1.GetLocalPos(), hints.tile1.GetLocalPos()) < 0.01f) ? hints.tile2 : hints.tile1);
                        // them t2 vao listRemove de xoa sau
                        listRemove.Add(t2);
                        hints.tile1.BlowUp();
                        hints.tile2.BlowUp();
                        effectManager.SpawnEffect((Type)t1.GetTileType(), t1.GetPos());
                        effectManager.SpawnEffect((Type)t2.GetTileType(), t2.GetPos());
                        Debug.Log("vi tri" + t2.GetLocalPos());
                        Debug.Log("vi tri" + t1.GetLocalPos());
                        tile_1 = null;
                        tile_2 = null;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                });
                int j = 1;
                // xoa tat ca cac goi y chua tile trong currentHints neu tile nam trong listRemove
                for (int i = 0; i < listRemove.Count; i++)
                {
                    currentHints.RemoveAll(hints => hints.Contains(listRemove[i]));
                    GameManager_.Instance.CalculateScore(90);
                    typeDictionary[(Type)t1.GetTileType()]--;
                    j++;
                }
            }
            Invoke(nameof(PlayBoosterSound), 0.5f);
            // neu currentHints khong con goi y nao thi refresh list hint
            if (currentHints.Count <= 0)
            {
                RefreshHintList();
            }
            // check win game 
            if (CheckEndGame())
            {
                Debug.Log("Endgame");
                GameManager_.Instance.ShowEndGameCanvas(1);
            }
        }
        else// neu khong co type cua tile duoc chon trong dictionary thi phat am thanh, reset streak, shake tile va huy chon tile
        {
            GameManager_.Instance.ResetStreak();
            SoundManager.PlaySoundEffect(SoundType.MISS);
            tile_1.Shake();
            DeSelected(tile_1);
        }
    }
    public void PlayBoosterSound()
    {
        SoundManager.PlaySoundEffect(SoundType.BOOSTER_1);
    }
    // hien thi cap goi y
    public void ShowHints()
    {
        Debug.Log("Show hints");
        Debug.Log(currentHints.Count);
        if (currentHints.Count > 0)
        {
            Debug.Log("Co hints");
            HighlightHint(currentHints[0].tile1 , currentHints[0].tile2);
            SoundManager.PlaySoundEffect(SoundType.SHOW2TILE);
            GameManager_.Instance.ResetStreak();
            Debug.Log(currentHints[0].tile1.GetLocalPos() +" "+currentHints[0].tile2.GetLocalPos());
        }
    }
    
    // goi animation goi y cua 2 tile
    public void HighlightHint(Tile t1, Tile t2) 
    {
      t1.PopUpAndDownAnimation();
      t2.PopUpAndDownAnimation();
    }
    // Tim tat ca goi y trong mang
    public List<HintPair> FindAllHints()
    {
        List<HintPair> hints = new List<HintPair>();
        Tile t1;
        Tile t2;
        for (int r1 = 1; r1 < tileCols-1; r1++)
        {
            for (int c1 = 1; c1 < tileRows-1; c1++)
            {
                t1 = gridArray[r1, c1];
                if (t1 == null || t1.IsEmpty()) continue;

                for (int r2 = r1; r2 < tileCols-1; r2++)
                {
                    for (int c2 = 1; c2 < tileRows-1; c2++)
                    {

                        if (r1 == r2 && c1 == c2) continue;

                        t2 = gridArray[r2, c2];
                        if (t2 == null || t2.IsEmpty()) continue;


                        if (t1.GetTileType() != t2.GetTileType()|| t1.GetTileType() == (int)Type.obstacle|| t2.GetTileType() == (int)Type.obstacle) continue;

                        t1.SetSelected(true);
                        t2.SetSelected(true);
                        

                        Mid_Line path = null;
                        if ((path = CheckLineX(t1, t2)) != null ||
                            (path = CheckLineY(t1, t2)) != null ||
                            (path = CheckRectX(t1, t2)) != null ||
                            (path = CheckRectY(t1, t2)) != null ||
                            (path = Check_U_ShapeX(t1, t2, -1)) != null ||
                            (path = Check_U_ShapeX(t1, t2, 1)) != null ||
                            (path = Check_U_ShapeY(t1, t2, -1)) != null ||
                            (path = Check_U_ShapeY(t1, t2, 1)) != null)
                        {
                            if (!hints.Contains(new HintPair(t2, t1)))
                            {

                                hints.Add(new HintPair(t1, t2
                                    //, path
                                    //, DrawPathTest(t1.GetPos(), path.GetPoint1(), path.GetPoint2(), t2.GetPos())
                                    ));
                                // DrawPathTest(t1.GetPos(), path.GetPoint1(), path.GetPoint2(), t2.GetPos());
                               
                            }
                        }
                        t1.SetSelected(false);
                        t2.SetSelected(false);
                        Debug.Log((path == null) + " " + t1.GetLocalPos() + " " + t2.GetLocalPos());
                    }
                }
            }
        }
        return hints;
    }
    public void RefreshHintList()
    {
       Debug.Log("Refresh hints");
        currentHints = FindAllHints();// Tim lai tat ca goi y trong mang
        // neu van = 0 -> khong con cap nao an duoc thi shuffle lai mang
        if (currentHints.Count == 0) {
            Shuffle();
        }
    }
    public void Shuffle()
    {
        Debug.Log("Suffle");
        // tao 1 mang cac vi tri tile khong rong
         nonEmptyPositions.Clear();
        Vector2Int temp;
        Vector2Int pos1;
        Vector2Int pos2;
        //Vector2Int pos;
        for (int i = 0; i < tileCols; i++)
        {
            for (int j = 0; j < tileRows; j++)
            {
                if (!gridArray[i, j].IsEmpty()&& gridArray[i, j].GetTileType() != (int)Type.obstacle)
                {
                    // them vao mang 
                    nonEmptyPositions.Add(new Vector2Int(i, j));
                }
            }
        }
        // random cac vi tri trong mang khong rong
        for (int i = 0; i < nonEmptyPositions.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, nonEmptyPositions.Count);
            temp = nonEmptyPositions[i];
            nonEmptyPositions[i] = nonEmptyPositions[randomIndex];
            nonEmptyPositions[randomIndex] = temp;
        }

        // swap cac tile trong mang theo thu tu trong mang khong rong
        for (int i = 0; i < nonEmptyPositions.Count - 1; i++)
        {
             pos1 = nonEmptyPositions[i];
             pos2 = nonEmptyPositions[i + 1];

            SwapTile(gridArray[pos1.x, pos1.y], gridArray[pos2.x, pos2.y]);
        }
        CheckOddTile();
    }
    /* kiem tra xem co le? cac tile nao khong, neu so luong tile theo type la cha~n thi return void, con le so luong tile theo type thi 
        * check xem so luong booster co lon hon hoac bang khong so tile theo type le khong. Neu khong thi hien thi Fail Canvas
        */
    public void CheckOddTile()
    {
        Vector2Int pos;
        if (nonEmptyPositions.Count <= tileData.Count)
        {
            int count = 0;
            int countEven = 0;
            for (int i = 0; i < nonEmptyPositions.Count; i++)
            {
                pos = nonEmptyPositions[i];
                if (typeDictionary[(Type)gridArray[pos.x, pos.y].GetTileType()] > 0)
                {
                    if (typeDictionary[(Type)gridArray[pos.x, pos.y].GetTileType()]
                        //.Count 
                        % 2 != 0)
                    {
                        count++;
                    }
                    else
                    {
                        countEven++;
                    }
                }
            }
            if (countEven > 0)
            {
                return;
            }
            if (count > GameManager_.Instance.BoosterCount)
            {
                GameManager_.Instance.ShowEndGameCanvas(0);
                return;
            }
        }
    }
    public void ShowDic()
    {
        foreach(var key in typeDictionary.Keys)
        {
            Debug.Log(key.ToString()+" " + typeDictionary[key]);
        }
    }
    // doi tile dua tren tileData   
    public void SwapTile(Tile t1, Tile t2)
    {
        TileData temp = t1.Data;
        t1.SwapTile(t2.Data);
        t2.SwapTile(temp);
    }
}
