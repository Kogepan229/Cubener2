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

        // �v���C���[������`�����N���ς�����Ƃ��ɐ����ƍ폜������
        if (beforeFramePlayerChunkPos != player.GetChunkPos())
        {
            Debug.Log("Another Chunk");
            GenerateChunks();
            UnloadOutOfRangeChunks();
        }
    }

    void LateUpdate()
    {
        beforeFramePlayerChunkPos = player.GetChunkPos();
    }

    IEnumerator CoCreateChunck(ChunkPos chunkPos)
    {
        if (chunkMap[chunkPos.y].ContainsKey(chunkPos.AsLong()))
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
        // ChunkLoadDistance��3�ȏ�̊�ł���悤��
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
            // ��
            for (int j = -i; j <= i; j++)
            {
                StartCoroutine(CoCreateChunck(new ChunkPos(i, 0, j) + player.GetChunkPos()));
            }
            // ��
            for (int j = -i; j <= i; j++)
            {
                StartCoroutine(CoCreateChunck(new ChunkPos(-i, 0, j) + player.GetChunkPos()));
            }
            // �E
            for (int j = -(i - 1); j < i; j++)
            {
                StartCoroutine(CoCreateChunck(new ChunkPos(j, 0, i) + player.GetChunkPos()));
            }
            // ��
            for (int j = -(i - 1); j < i; j++)
            {
                StartCoroutine(CoCreateChunck(new ChunkPos(j, 0, -i) + player.GetChunkPos()));
            }
        }
    }

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
                // �`�����N�̃��[�h��������2�`�����N���ꂽ��폜
                // �`�����N�̋��E�����������Ƃ��ɍ폜�Ɛ������J��Ԃ���Ȃ��悤�ɂ��邽��
                if (distance > ChunkLoadDistance + 2)
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
