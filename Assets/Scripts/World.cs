using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    /// <summary>
    /// West X+,  East X-,  North Z-,  South Z+
    /// </summary>
    public enum EnumDirectionHorizontal
    {
        West,     // X+
        East,     // X-
        North,    // Z-
        South,    // Z+
    }
    public enum EnumDirectionVertical
    {
        Up,       // Y+
        Down,     // Y-
    }

    private Player player;
    private Transform chunksTransform;


    public int ChunkLoadDistance = 7;



    /// <summary>
    /// [y][xz]
    /// </summary>
    private Dictionary<long, Chunk>[] chunkMap = new Dictionary<long, Chunk>[16];

    List<ChunkPos> chunkGenerationQueue = new List<ChunkPos>();
    private Queue<Chunk> chunkDeleteQueue = new Queue<Chunk>();
    public Queue<(Chunk, Mesh)> ChunkGenerateMeshColliderQueue = new Queue<(Chunk, Mesh)>();


    private ChunkPos beforeFramePlayerChunkPos;

    void Awake()
    {
        // Init chunkMap
        for (int i = 0; i < chunkMap.Length; i++)
        {
            chunkMap[i] = new Dictionary<long, Chunk>();
        }


        chunksTransform = transform.Find("ChunkContainer");
        player = transform.Find("Player").GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateChunks();
    }

    // Update is called once per frame
    void Update()
    {
        deleteChunksInQueue();
        //if (ChunkGenerateMeshColliderQueue.Count > 0)
        //{
        //    (Chunk, Mesh) item = ChunkGenerateMeshColliderQueue.Dequeue();
        //    item.Item1.MeshCollider.sharedMesh = item.Item2;
        //    item.Item1.MeshFilter.sharedMesh = item.Item2;
        //}

        // プレイヤーがいるチャンクが変わったときに生成と削除をする
        if (beforeFramePlayerChunkPos != player.GetChunkPos())
        {
            Debug.Log("Another Chunk");
            GenerateChunks();
            UnloadOutOfRangeChunks();
        }

        // チャンクの地形生成キューを消化
        //generateChunksInQueue();
    }

    void LateUpdate()
    {
        beforeFramePlayerChunkPos = player.GetChunkPos();
    }

    IEnumerator CoCreateChunck(ChunkPos chunkPos)
    {
        if (chunkMap[chunkPos.y].ContainsKey(ChunkPos.AsLong(chunkPos.x, chunkPos.z)))
        {
            yield break;
        }

        GameObject chunkObj = Chunk.CreateChunkObject(chunkPos, this);
        Chunk chunk = chunkObj.GetComponent<Chunk>();

        var generateTerrainTask = Task.Run(() => chunk.GenerateChunkTerrainNew());
        while (!generateTerrainTask.IsCompleted)
        {
            yield return null;
        }
        generateTerrainTask.Wait();

        AddChunkToWorld(chunk);
    }

    public void GenerateChunks()
    {
        // ChunkLoadDistanceが3以上の奇数であるように
        if (ChunkLoadDistance < 3)
        {
            ChunkLoadDistance = 3;
        }
        else if (ChunkLoadDistance % 2 == 0)
        {
            ChunkLoadDistance -= 1;
        }

        // 0
        // Proc

        StartCoroutine(CoCreateChunck(player.GetChunkPos()));

        for (int i = 1; i <= ChunkLoadDistance; i++)
        {
            // 上
            for (int j = -i; j <= i; j++)
            {
                StartCoroutine(CoCreateChunck(new ChunkPos(i, 0, j) + player.GetChunkPos()));
            }
            // 下
            for (int j = -i; j <= i; j++)
            {
                StartCoroutine(CoCreateChunck(new ChunkPos(-i, 0, j) + player.GetChunkPos()));
            }
            // 右
            for (int j = -(i - 1); j < i; j++)
            {
                StartCoroutine(CoCreateChunck(new ChunkPos(j, 0, i) + player.GetChunkPos()));
            }
            // 左
            for (int j = -(i - 1); j < i; j++)
            {
                StartCoroutine(CoCreateChunck(new ChunkPos(j, 0, -i) + player.GetChunkPos()));
            }
        }
    }

    //private void AddChunkGenerationQueue(ChunkPos pos)
    //{
    //    if (chunkMap[pos.y].ContainsKey(ChunkPos.AsLong(pos.x, pos.z)) == false && chunkGenerationQueue.Contains(pos) == false)
    //    {
    //        chunkGenerationQueue.Add(pos);
    //    }
    //}

    //private void generateChunksInQueue()
    //{
    //    List<ChunkPos> generatedChunks = new List<ChunkPos>();

    //    // チャンクの地形生成のキューを消化
    //    foreach (ChunkPos chunkPos in chunkGenerationQueue)
    //    {
    //        generateChunk(chunkPos);
    //        generatedChunks.Add(chunkPos);
    //    }

    //    foreach (ChunkPos chunkPos in generatedChunks)
    //    {
    //        chunkGenerationQueue.Remove(chunkPos);
    //    }
    //}

    //private void generateChunk(ChunkPos chunkPos)
    //{
    //    GameObject chunkObj = Chunk.CreateChunkObject(chunkPos, this);
    //    Chunk chunk = chunkObj.GetComponent<Chunk>();
    //    AddChunkToWorld(chunk);
    //    chunk.GenerateChunkTerrain();
    //}

    public void AddChunkToWorld(Chunk chunk)
    {
        chunk.transform.SetParent(chunksTransform);
        chunkMap[chunk.Position.y][ChunkPos.AsLong(chunk.Position.x, chunk.Position.z)] = chunk;
    }

    public void UnloadOutOfRangeChunks()
    {
        for (int i = 0; i < chunkMap.Length; i++)
        {
            foreach (KeyValuePair<long, Chunk> item in chunkMap[i])
            {
                ChunkPos tmp = item.Value.Position - player.GetChunkPos();
                int distance = Mathf.Max(Mathf.Abs(tmp.x), Mathf.Abs(tmp.z));
                if (distance > ChunkLoadDistance)
                {
                    chunkDeleteQueue.Enqueue(item.Value);
                }
            }
        }
    }

    private void deleteChunksInQueue()
    {
        while (chunkDeleteQueue.Count > 0)
        {
            Chunk chunk = chunkDeleteQueue.Dequeue();
            DeleteChunkFromWorld(chunk);
        }
    }


    public void DeleteChunkFromWorld(Chunk chunk)
    {
        //Debug.Log("2" + chunk.Position.ToString());
        chunkMap[chunk.Position.y].Remove(ChunkPos.AsLong(chunk.Position.x, chunk.Position.z));
        Destroy(chunk.gameObject);
    }
    public void DeleteChunkFromWorld(ChunkPos chunkPos)
    {
        DeleteChunkFromWorld(chunkMap[chunkPos.y][ChunkPos.AsLong(chunkPos.x, chunkPos.z)]);
        //Debug.Log("1" + chunkPos.ToString());
    }



    public void PutBlock(int BlockID, BlockPos pos)
    {
        ChunkPos chunkPos = new ChunkPos(pos);
        chunkMap[chunkPos.y][ChunkPos.AsLong(chunkPos.x, chunkPos.z)].PutBlock(BlockID, pos);
    }
}
