using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sokoban_stage : MonoBehaviour
{
    [SerializeField]
    private TextAsset stageFile; // ステージ構造が記述されたテキストファイル

    private int rows; // 行数
    private int columns; // 列数
    private TileType[,] tileList; // タイル情報を管理する二次元配列
    private int max_loop_count;//ループ回数の制限;
    private int loop_count;//現在のループ回数
    private int player_move_cnt;//歩いた歩数をカウントする変数
                                // 移動方向格納用
    int move_vec = -1;     // 移動方向
    int pre_move_vec = -1; // 一つ前の移動方向
                           // 位置格納用
    int[] box_pos = { 0, 0 };      // ボックス位置
    int[] walk_pos = { 0, 0 };     // 歩行ポイント位置
    int[] pre_box_pos = { 0, 0 };  // 一つ前のボックス位置
    int[] pre_walk_pos = { 0, 0 }; // 一つ前の歩行ポイント位置
    int[] walk2_pos = { 0, 0 };    // 歩行ポイント位置２（経路用）
    int[] walk3_pos = { 0, 0 };    // 歩行ポイント位置３（経路用）
    int[] random_pos = { 0, 0 };
    int[] random_last_pos = { 0, 0 };
    [SerializeField]
    private float tileSize; // タイルのサイズ
    [SerializeField]
    private Sprite groundSprite; // 地面のスプライト
    [SerializeField]
    private Sprite targetSprite; // 目的地のスプライト
    [SerializeField]
    private Sprite playerSprite; // プレイヤーのスプライト
    [SerializeField]
    private Sprite blockSprite; // ブロックのスプライト
    [SerializeField]
    private Sprite wallSprite; // 壁のスプライト
    [SerializeField]
    private GameObject congra;//コングラチュレーションのスプライト
    [SerializeField]
    private Text score_text;//スコアのテキスト

    private GameObject player; // プレイヤーのゲームオブジェクト
    private Vector2 middleOffset; // 中心位置
    private int blockCount; // ブロックの数
    private bool _isClear; // ゲームをクリアした場合 true

    // 各位置に存在するゲームオブジェクトを管理する連想配列
    private Dictionary<GameObject, Vector2Int> gameObjectPosTable = new Dictionary<GameObject, Vector2Int>();
    // ゲーム開始時に呼び出される
    private void Start()
    {
        LoadTileData(); // タイルの情報を読み込む
        Debug.Log("aaa");
        Randamstage();
        Debug.Log("bbb");
        Setmaptile();
        Debug.Log("ccc");
        CreateStage(); // ステージを作成
        Debug.Log("ddd");
    }
    private enum StageMoveType
    {
        LEFT_MOVE,
        RIGHT_MOVE,
        UP_MOVE,
        DOWN_MOVE

    }
    private enum DirectionType
    {
        UP, // 上
        RIGHT, // 右
        DOWN, // 下
        LEFT, // 左
    }
    // タイルの種類
    private enum TileType
    {
        NONE, // 何も無い
        GROUND, // 地面
        TARGET, // 目的地
        PLAYER, // プレイヤー
        BLOCK, // ブロック

        PLAYER_ON_TARGET, // プレイヤー（目的地の上）
        BLOCK_ON_TARGET, // ブロック（目的地の上）
    }
    // タイルの情報を読み込む
    private void LoadTileData()
    {
        // タイルの情報を一行ごとに分割
        string[] lines = stageFile.text.Split
        (
            new[] { '\r', '\n' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        // タイルの列数を計算
        string[] nums = lines[0].Split(new[] { ',' });

        // タイルの列数と行数を保持
        rows = lines.Length; // 行数
        columns = nums.Length; // 列数

        max_loop_count = (rows-2) * (columns-2);//外周以外のタイルの数をカウント
        
        // タイル情報を int 型の２次元配列で保持
        tileList = new TileType[columns, rows];
        for (int y = 0; y < rows; y++)
        {
            // 一文字ずつ取得
            string st = lines[y];
            nums = st.Split(new[] { ',' });
            for (int x = 0; x < columns; x++)
            {
                // 読み込んだ文字を数値に変換して保持
                tileList[x, y] = (TileType)int.Parse(nums[x]);
            }
        }
    }
    //ランダムなステージを作成
    private void Randamstage()
    {

        int[] random_pos = { 0, 0 };
        int[] random_last_pos = { 0, 0 };
        random_pos[0] = Random.Range(1,rows-1);
        random_pos[1] = Random.Range(1,columns-1);
    }
    private void Setmaptile()
    {
        // 移動方向格納用
        int move_vec = -1;     // 移動方向
        int pre_move_vec = -1; // 一つ前の移動方向
                               // 位置格納用
        int[] box_pos = { 0, 0 };      // ボックス位置
        int[] walk_pos = new int[2];    // 歩行ポイント位置
        int[] pre_box_pos = { 0, 0 };  // 一つ前のボックス位置
        int[] pre_walk_pos = { 0, 0 }; // 一つ前の歩行ポイント位置
        int[] walk2_pos = { 0, 0 };    // 歩行ポイント位置２（経路用）
        int[] walk3_pos = { 0, 0 };    // 歩行ポイント位置３（経路用）
        /* ポイント位置を基準にマップの自動生成を行う*/
        // ポイント位置を基準とする
        pre_box_pos[0] = random_pos[0];
        pre_box_pos[1] = random_pos[1];
        // ループ回数初期化
        loop_count = 0;
        // ボックスと歩行ポイントをポイントの直線上に並べて置く
        while (true)
        {
            // ４方向をランダムで決める
            move_vec = Random.Range(0, 3);
            if (move_vec == ((int)StageMoveType.LEFT_MOVE))
            {
                box_pos[0] = pre_box_pos[0] - 1;
                box_pos[1] = pre_box_pos[1];
                walk_pos[0] = pre_box_pos[0] - 2;
                walk_pos[1] = pre_box_pos[1];
            }
            else if (move_vec == ((int)StageMoveType.RIGHT_MOVE))
            {
                box_pos[0] = pre_box_pos[0] + 1;
                box_pos[1] = pre_box_pos[1];
                walk_pos[0] = pre_box_pos[0] + 2;
                walk_pos[1] = pre_box_pos[1];
            }
            else if (move_vec == ((int)StageMoveType.UP_MOVE))
            {
                box_pos[0] = pre_box_pos[0];
                box_pos[1] = pre_box_pos[1] - 1;
                walk_pos[0] = pre_box_pos[0];
                walk_pos[1] = pre_box_pos[1] - 2;
            }
            else if (move_vec == ((int)StageMoveType.DOWN_MOVE))
            {
                box_pos[0] = pre_box_pos[0];
                box_pos[1] = pre_box_pos[1] + 1;
                walk_pos[0] = pre_box_pos[0];
                walk_pos[1] = pre_box_pos[1] + 2;
            }
            // 歩行ポイント（２マス先）が置ける範囲なら置く
            // マップの範囲 かつ ブランクか歩行経路のタイル
            if (0 < walk_pos[0] && walk_pos[0] < rows && 0 < walk_pos[1] && walk_pos[1] < columns 
            && (tileList[walk_pos[1], walk_pos[0]] == TileType.NONE || tileList[walk_pos[1],walk_pos[0]]== TileType.GROUND)
            && tileList[box_pos[1],box_pos[0]] == TileType.NONE || tileList[box_pos[1],box_pos[0]] == TileType.GROUND)
            {
                // 移動した方向を保持
                pre_move_vec = move_vec;
                break;
            }// ループ回数が最大数を超えたら処理終了
            loop_count++;
            if (loop_count >= max_loop_count)
            {
                // 最終的な位置はポイント位置として返却
                random_last_pos[0] = random_pos[0];
                random_last_pos[1] = random_pos[1];
                return;
            }

        }
        // ボックス、歩行ポイントを設定
        tileList[box_pos[1],box_pos[0]] = TileType.BLOCK;
        tileList[pre_box_pos[1],pre_box_pos[0]] = TileType.GROUND;
        tileList[walk_pos[1],walk_pos[0]] = TileType.GROUND;
        // 最後の歩行ポイントを設定
        random_last_pos[0] = walk_pos[0];
        random_last_pos[1] = walk_pos[1];
    }
    // ステージを作成
    private void CreateStage()
    {
        // ステージの中心位置を計算
        middleOffset.x = columns * tileSize * 0.5f - tileSize * 0.5f;
        middleOffset.y = rows * tileSize * 0.5f - tileSize * 0.5f; ;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                TileType val = tileList[x, y];

                

                // タイルの名前に行番号と列番号を付与
                string name = "tile" + y + "_" + x;

                // タイルのゲームオブジェクトを作成
                GameObject tile = new GameObject(name);

                // タイルにスプライトを描画する機能を追加
                SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();

                // タイルのスプライトを設定
                spriteRenderer.sprite = groundSprite;

                // タイルの位置を設定
                tile.transform.position = GetDisplayPosition(x, y);
                // 壁の場合
                if (val == TileType.NONE) 
                {
                    // 壁のゲームオブジェクトを作成
                    GameObject wall = new GameObject("wall");

                    // 壁にスプライトを描画する機能を追加
                    spriteRenderer = wall.AddComponent<SpriteRenderer>();

                    // 壁のスプライトを設定
                    spriteRenderer.sprite = wallSprite;

                    // 壁の描画順を手前にする
                    spriteRenderer.sortingOrder = 1;

                    // 壁の位置を設定
                    wall.transform.position = GetDisplayPosition(x, y);

                    
                }
                // 目的地の場合
                if (val == TileType.TARGET)
                {
                    // 目的地のゲームオブジェクトを作成
                    GameObject destination = new GameObject("destination");

                    // 目的地にスプライトを描画する機能を追加
                    spriteRenderer = destination.AddComponent<SpriteRenderer>();

                    // 目的地のスプライトを設定
                    spriteRenderer.sprite = targetSprite;

                    // 目的地の描画順を手前にする
                    spriteRenderer.sortingOrder = 1;

                    // 目的地の位置を設定
                    destination.transform.position = GetDisplayPosition(x, y);
                }
                // プレイヤーの場合
                if (val == TileType.PLAYER)
                {
                    // プレイヤーのゲームオブジェクトを作成
                    player = new GameObject("player");

                    // プレイヤーにスプライトを描画する機能を追加
                    spriteRenderer = player.AddComponent<SpriteRenderer>();

                    // プレイヤーのスプライトを設定
                    spriteRenderer.sprite = playerSprite;

                    // プレイヤーの描画順を手前にする
                    spriteRenderer.sortingOrder = 2;

                    // プレイヤーの位置を設定
                    player.transform.position = GetDisplayPosition(x, y);

                    // プレイヤーを連想配列に追加
                    gameObjectPosTable.Add(player, new Vector2Int(x, y));
                }
                // ブロックの場合
                else if (val == TileType.BLOCK)
                {
                    // ブロックの数を増やす
                    blockCount++;

                    // ブロックのゲームオブジェクトを作成
                    GameObject block = new GameObject("block" + blockCount);

                    // ブロックにスプライトを描画する機能を追加
                    spriteRenderer = block.AddComponent<SpriteRenderer>();

                    // ブロックのスプライトを設定
                    spriteRenderer.sprite = blockSprite;

                    // ブロックの描画順を手前にする
                    spriteRenderer.sortingOrder = 2;

                    // ブロックの位置を設定
                    block.transform.position = GetDisplayPosition(x, y);

                    // ブロックを連想配列に追加
                    gameObjectPosTable.Add(block, new Vector2Int(x, y));
                }
            }
        }
    }
    // 指定された行番号と列番号からスプライトの表示位置を計算して返す
    private Vector2 GetDisplayPosition(int x, int y)
    {
        return new Vector2
        (
            x * tileSize - middleOffset.x,
            y * -tileSize + middleOffset.y
        );
    }
    // 指定された位置に存在するゲームオブジェクトを返します
    private GameObject GetGameObjectAtPosition(Vector2Int pos)
    {
        foreach (KeyValuePair<GameObject,Vector2Int> pair in gameObjectPosTable)
        {
            // 指定された位置が見つかった場合
            if (pair.Value == pos)
            {
                // その位置に存在するゲームオブジェクトを返す
                return pair.Key;
            }
        }
        return null;
    }
    private bool IsValidPosition(Vector2Int pos)
    {
        if (0 <= pos.x && pos.x < columns && 0 <= pos.y && pos.y < rows)
        {
            return tileList[pos.x, pos.y] != TileType.NONE;
        }
        return false;
    }
    // 指定された位置のタイルがブロックなら true を返す
    private bool IsBlock(Vector2Int pos)
    {
        TileType cell = tileList[pos.x, pos.y];
        return cell == TileType.BLOCK || cell == TileType.BLOCK_ON_TARGET;
    }// 毎フレーム呼び出される
    private void Update()
    {
        score_text.text = "歩いた歩数:"　+ player_move_cnt.ToString();

        // ゲームクリアしている場合は操作できないようにする
        if (_isClear) return;

        // 上矢印が押された場合
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // プレイヤーが上に移動できるか検証
            TryMovePlayer(DirectionType.UP);
            Debug.Log(player_move_cnt);
        }
        // 右矢印が押された場合
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // プレイヤーが右に移動できるか検証
            TryMovePlayer(DirectionType.RIGHT);
            Debug.Log(player_move_cnt);
        }
        // 下矢印が押された場合
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // プレイヤーが下に移動できるか検証
            TryMovePlayer(DirectionType.DOWN);
            Debug.Log(player_move_cnt);
        }
        // 左矢印が押された場合
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // プレイヤーが左に移動できるか検証
            TryMovePlayer(DirectionType.LEFT);
            Debug.Log(player_move_cnt);
        }
    }
    // 指定された方向にプレイヤーが移動できるか検証
    // 移動できる場合は移動する
    private void TryMovePlayer(DirectionType direction)
    {
        // プレイヤーの現在地を取得
        Vector2Int currentPlayerPos = gameObjectPosTable[player];

        // プレイヤーの移動先の位置を計算
        Vector2Int nextPlayerPos = GetNextPositionAlong(currentPlayerPos, direction);

        // プレイヤーの移動先がステージ内ではない場合は無視
        if (!IsValidPosition(nextPlayerPos)) return;

        // プレイヤーの移動先にブロックが存在する場合
        if (IsBlock(nextPlayerPos))
        {
            // ブロックの移動先の位置を計算
            Vector2Int nextBlockPos = GetNextPositionAlong(nextPlayerPos, direction);

            // ブロックの移動先がステージ内の場合かつ
            // ブロックの移動先にブロックが存在しない場合
            if (IsValidPosition(nextBlockPos) && !IsBlock(nextBlockPos))
            {
                player_move_cnt++;//歩いた歩数をカウントする

                // 移動するブロックを取得
                GameObject block = GetGameObjectAtPosition(nextPlayerPos);

                // プレイヤーの移動先のタイルの情報を更新
                UpdateGameObjectPosition(nextPlayerPos);

                // ブロックを移動
                block.transform.position = GetDisplayPosition(nextBlockPos.x, nextBlockPos.y);

                // ブロックの位置を更新
                gameObjectPosTable[block] = nextBlockPos;

                // ブロックの移動先の番号を更新
                if (tileList[nextBlockPos.x, nextBlockPos.y] == TileType.GROUND)
                {
                    // 移動先が地面ならブロックの番号に更新
                    tileList[nextBlockPos.x, nextBlockPos.y] = TileType.BLOCK;
                }
                else if (tileList[nextBlockPos.x, nextBlockPos.y] == TileType.TARGET)
                {
                    // 移動先が目的地ならブロック（目的地の上）の番号に更新
                    tileList[nextBlockPos.x, nextBlockPos.y] = TileType.BLOCK_ON_TARGET;
                }

                // プレイヤーの現在地のタイルの情報を更新
                UpdateGameObjectPosition(currentPlayerPos);

                // プレイヤーを移動
                player.transform.position = GetDisplayPosition(nextPlayerPos.x, nextPlayerPos.y);

                // プレイヤーの位置を更新
                gameObjectPosTable[player] = nextPlayerPos;

                // プレイヤーの移動先の番号を更新
                if (tileList[nextPlayerPos.x, nextPlayerPos.y] == TileType.GROUND)
                {
                    // 移動先が地面ならプレイヤーの番号に更新
                    tileList[nextPlayerPos.x, nextPlayerPos.y] = TileType.PLAYER;
                    
                }
                else if (tileList[nextPlayerPos.x, nextPlayerPos.y] == TileType.TARGET)
                {
                    // 移動先が目的地ならプレイヤー（目的地の上）の番号に更新
                    tileList[nextPlayerPos.x, nextPlayerPos.y] = TileType.PLAYER_ON_TARGET;
                }
            }
        }
        // プレイヤーの移動先にブロックが存在しない場合
        else
        {
            // プレイヤーの現在地のタイルの情報を更新
            UpdateGameObjectPosition(currentPlayerPos);

            // プレイヤーを移動
            player.transform.position = GetDisplayPosition(nextPlayerPos.x, nextPlayerPos.y);

            // プレイヤーの位置を更新
            gameObjectPosTable[player] = nextPlayerPos;

            // プレイヤーの移動先の番号を更新
            if (tileList[nextPlayerPos.x, nextPlayerPos.y] == TileType.GROUND)
            {
                // 移動先が地面ならプレイヤーの番号に更新
                tileList[nextPlayerPos.x, nextPlayerPos.y] = TileType.PLAYER;
                player_move_cnt++;//歩いた歩数をカウントする
            }
            else if (tileList[nextPlayerPos.x, nextPlayerPos.y] == TileType.TARGET)
            {
                // 移動先が目的地ならプレイヤー（目的地の上）の番号に更新
                tileList[nextPlayerPos.x, nextPlayerPos.y] = TileType.PLAYER_ON_TARGET;
                player_move_cnt++;//歩いた歩数をカウントする
            }
        }

        // ゲームをクリアしたかどうか確認
        CheckCompletion();
    }
    // 指定された方向の位置を返す
    private Vector2Int GetNextPositionAlong(Vector2Int pos, DirectionType direction)
    {
        switch (direction)
        {
            // 上
            case DirectionType.UP:
                pos.y -= 1;
                break;

            // 右
            case DirectionType.RIGHT:
                pos.x += 1;
                break;

            // 下
            case DirectionType.DOWN:
                pos.y += 1;
                break;

            // 左
            case DirectionType.LEFT:
                pos.x -= 1;
                break;
        }
        return pos;
    }// 指定された位置のタイルを更新
    private void UpdateGameObjectPosition(Vector2Int pos)
    {
        // 指定された位置のタイルの番号を取得
        TileType cell = tileList[pos.x, pos.y];

        // プレイヤーもしくはブロックの場合
        if (cell == TileType.PLAYER || cell == TileType.BLOCK)
        {
            // 地面に変更
            tileList[pos.x, pos.y] = TileType.GROUND;
        }
        // 目的地に乗っているプレイヤーもしくはブロックの場合
        else if (cell == TileType.PLAYER_ON_TARGET || cell == TileType.BLOCK_ON_TARGET)
        {
            // 目的地に変更
            tileList[pos.x, pos.y] = TileType.TARGET;
        }
    }// ゲームをクリアしたかどうか確認
    private void CheckCompletion()
    {
        // 目的地に乗っているブロックの数を計算
        int blockOnTargetCount = 0;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (tileList[x, y] == TileType.BLOCK_ON_TARGET)
                {
                    blockOnTargetCount++;
                }
            }
        }

        // すべてのブロックが目的地の上に乗っている場合
        if (blockOnTargetCount == blockCount)
        {
            // ゲームクリア
            _isClear = true;
            congra.SetActive(true);
        }
    }

}
