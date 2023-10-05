using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.AI;
using UnityEngine.XR;
using Unity.Collections;

public class Chunk : MonoBehaviour
{
    private Mesh m_Mesh;
    public MeshRenderer MeshRenderer;
    public MeshFilter MeshFilter;
    public MeshCollider MeshCollider;
    //private GameObject collidersObj;

    public static readonly int Width = 16;
    public static readonly int Height = 256;

    private World world;

    private ChunkPos m_Position;
    public ChunkPos Position
    {
        get
        {
            return m_Position;
        }
        set
        {
            //Debug.Log(value.ToString());
            m_Position = value;
            transform.position = new Vector3(value.x * 16, value.y * 16, value.z * 16);
        }
    }

    /// <summary>
    /// [y, x, z]
    /// </summary>
    private int[,,] m_BlockMap = new int[Width, Height, Width];
    private bool m_MeshUpdateFlag = false;
    private bool m_MeshUpdatingFlag = false;

    //private int vertexIndex = 0;
    //private List<Vector3> vertices = new List<Vector3>();
    //private List<int> triangles = new List<int>();
    //private List<Vector2> uvs = new List<Vector2>();


    void Awake()
    {
        MeshRenderer.material.mainTexture = TextureManager.BlockAtlas;
    }

    void Start()
    {
        /*
        var sw = new System.Diagnostics.Stopwatch();     // ストップウォッチオブジェクト生成
        sw.Start();  //  時間計測スタート
        //UpdateMesh();
        UpdateMeshAsync();
        sw.Stop();   // 時間計測終了
        TimeSpan span = sw.Elapsed;    //  計測した時間を span に代入
        Debug.Log("time: " + span.TotalMilliseconds.ToString());
        */
        //m_MeshUpdateFlag = true;
        StartCoroutine(CoCreateChunckMesh());
    }

    private void Update()
    {
        if (m_MeshUpdateFlag && !m_MeshUpdatingFlag)
        {
            var sw = new System.Diagnostics.Stopwatch();     // ストップウォッチオブジェクト生成
            sw.Start();  //  時間計測スタート
            m_MeshUpdateFlag = false;
            m_MeshUpdatingFlag = true;
            StartCoroutine(CoCreateChunckMesh());
            //Debug.Log("fin");
            m_MeshUpdatingFlag = false;
            //UpdateMeshAsync();
            sw.Stop();   // 時間計測終了
            TimeSpan span = sw.Elapsed;    //  計測した時間を span に代入
            //Debug.Log("time: " + span.TotalMilliseconds.ToString());
        }
    }

    public static GameObject CreateChunkObject(ChunkPos pos, World world)
    {
        GameObject chunkObj = Instantiate(Resources.Load<GameObject>("Prefabs/ChunkPrefab"));
        ///GameObject chunkObj = Instantiate(chunkPrefab);
        //GameObject chunkObj = new GameObject("[" + pos.x + ", " + pos.y + ", " + pos.z + "]");
        chunkObj.name = "[" + pos.x + ", " + pos.y + ", " + pos.z + "]";
        Chunk chunk = chunkObj.GetComponent<Chunk>();
        chunk.world = world;
        chunk.Position = pos;
        //chunk.ChunkTerrainGeneration();
        //chunk.position = pos;
        return chunkObj;
    }

    public async void GenerateChunkTerrain()
    {
        await Task.Run(() =>
        {
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetSeed(10);
            noise.SetFrequency(0.03f);

            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Width; z++)
                {
                    int _x = x + Position.x * Width;
                    int _z = z + Position.z * Width;
                    int _height = (int)(noise.GetNoise(_x, _z) * 100f / 8) + 50;
                    //Debug.Log(noise.GetNoise(_x, _z));
                    //int _height = (int)(Mathf.PerlinNoise(_x * _scale, _z * _scale) * 10f) + 30;
                    //Debug.Log(Mathf.PerlinNoise(_x * _scale, _z * _scale));
                    //Debug.Log(x * 0.01 + ", " + z * 0.01f);
                    //Debug.Log((int)(noise.GetNoise(x * 0.1f, z * 0.1f) * 1000f));
                    for (int y = 0; y < _height; y++)
                    {
                        PutBlock(4, new Vector3Int(x, y, z));
                    }
                }
            }


        });
        this.enabled = true;
    }

    public void GenerateChunkTerrainNew()
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetSeed(10);
        noise.SetFrequency(0.03f);

        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                int _x = x + Position.x * Width;
                int _z = z + Position.z * Width;
                int _height = (int)(noise.GetNoise(_x, _z) * 100f / 8) + 50;
                //Debug.Log(noise.GetNoise(_x, _z));
                //int _height = (int)(Mathf.PerlinNoise(_x * _scale, _z * _scale) * 10f) + 30;
                //Debug.Log(Mathf.PerlinNoise(_x * _scale, _z * _scale));
                //Debug.Log(x * 0.01 + ", " + z * 0.01f);
                //Debug.Log((int)(noise.GetNoise(x * 0.1f, z * 0.1f) * 1000f));
                for (int y = 0; y < _height; y++)
                {
                    if (y == _height - 1)
                    {
                        PutBlock(BlockManager.GetBlockId(Grass.NAME), new Vector3Int(x, y, z));
                    }
                    else
                    {
                        PutBlock(BlockManager.GetBlockId(Dirt.NAME), new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }

    /*
    void ChunkTerrainGeneration()
    {
        //Debug.Log("pos: " + Position.ToString());
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetSeed(10);
        noise.SetFrequency(0.03f);

        //noise.Set
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                int _x = x + Position.x * Width;
                int _z = z + Position.z * Width;
                int _height = (int)(noise.GetNoise(_x, _z) * 100f / 8) + 50;
                //Debug.Log(noise.GetNoise(_x, _z));
                //int _height = (int)(Mathf.PerlinNoise(_x * _scale, _z * _scale) * 10f) + 30;
                //Debug.Log(Mathf.PerlinNoise(_x * _scale, _z * _scale));
                //Debug.Log(x * 0.01 + ", " + z * 0.01f);
                //Debug.Log((int)(noise.GetNoise(x * 0.1f, z * 0.1f) * 1000f));
                for (int y = 0; y < _height; y++)
                {
                    PutBlock(4, new Vector3Int(x, y, z));
                }
            }
        }

        this.enabled = true;
    }
    */

    void PopulateVoxelMap()
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetSeed(1);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                //Debug.Log(x * 0.01 + ", " + z * 0.01f);
                Debug.Log((int)(noise.GetNoise(x * 0.1f, z * 0.1f) * 1000f));
            }
        }


        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Width; z++)
                {
                    PutBlock(BlockManager.GetBlockId(Glass.NAME), new BlockPos(x, y, z));
                    //m_BlockMap[x, y, z] = 6;
                }
            }
        }

        for (int y = 1; y < Height - 1; y++)
        {
            for (int x = 1; x < Width - 1; x++)
            {
                for (int z = 1; z < Width - 1; z++)
                {
                    if (x > Width / 2)
                    {
                        PutBlock(4, new BlockPos(x, y, z));
                        //m_BlockMap[x, y, z] = 4;
                    }
                    else
                    {
                        PutBlock(4, new BlockPos(x, y, z));
                        //m_BlockMap[x, y, z] = 4;
                    }
                }
            }
        }
        //m_MeshUpdateFlag = true;
        return;


        for (int _y = 0; _y < Height; _y++)
        {
            for (int _x = 0; _x < Width; _x++)
            {
                for (int _z = 0; _z < Width; _z++)
                {

                    if (_y < 1)
                        PutBlock(1, new BlockPos(_x, _y, _z));
                    //m_BlockMap[_x, _y, _z] = 1;
                    else if (_y == Height - 1)
                        PutBlock(4, new BlockPos(_x, _y, _z));
                    //m_BlockMap[_x, _y, _z] = 4;
                    else
                        PutBlock(2, new BlockPos(_x, _y, _z));
                    //m_BlockMap[_x, _y, _z] = 2;

                }
            }
        }

        PutBlock(0, new BlockPos(0, Height - 1, 0));
        PutBlock(5, new BlockPos(Width - 1, Height - 1, Width - 1));
        PutBlock(6, new BlockPos(Width - 1, Height - 1, Width - 2));
        //m_BlockMap[0, Height - 1, 0] = 0;
        //m_BlockMap[Width - 1, Height - 1, Width - 1] = 5;
        //m_BlockMap[Width - 1, Height - 1, Width - 2] = 6;
        //m_MeshUpdateFlag = true;
    }

    private Vector3Int getPosInChunk(BlockPos pos)
    {
        int x, z;
        if (pos.x < 0 && pos.x % Width != 0)
        {
            x = pos.x % Width + 16;
        }
        else
        {
            x = pos.x % Width;
        }

        if (pos.z < 0 && pos.z % Width != 0)
        {
            z = pos.z % Width + 16;
        }
        else
        {
            z = pos.z % Width;
        }
        return new Vector3Int(x, pos.y % Height, z);
    }

    public void PutBlock(int blockID, BlockPos pos)
    {
        Vector3Int _pos = getPosInChunk(pos);
        Debug.Log("Chunk: " + Position.ToString() + " InChunk: " + _pos.ToString());
        m_BlockMap[_pos.x, _pos.y, _pos.z] = blockID;
        /*
        try
        {
            if (blockData.Blocks[blockID].isSolid && m_BlockColliderMap[_pos.x, _pos.y, _pos.z] == null)
            {
                //Debug.Log("collider");
                GameObject obj = Instantiate(blockColliderPrefab, collidersObj.transform);
                obj.transform.localPosition = _pos;
                m_BlockColliderMap[_pos.x, _pos.y, _pos.z] = obj;
            }
            else if (blockData.Blocks[blockID].isSolid == false)
            {
                Destroy(m_BlockColliderMap[_pos.x, _pos.y, _pos.z]);
            }
        }catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log(_pos.ToString());
        }
        */

        m_MeshUpdateFlag = true;
    }
    public void PutBlock(int blockID, Vector3Int posInChunk)
    {
        m_BlockMap[posInChunk.x, posInChunk.y, posInChunk.z] = blockID;
        m_MeshUpdateFlag = true;
    }

    // 視界をさえぎるブロックかどうか判定
    bool CheckTransparent(Vector3 pos, Block.Faces face)
    {
        int id = CheckBlockID(pos);
        if (id == 0)
        {
            return true;
        }
        // チャンク境界
        if (id < 0)
        {
            return true;
        }
        return BlockManager.GetBlock(id).IsTransparent(face);
    }

    int CheckBlockID(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1 || z < 0 || z > Width - 1)
        {
            return -1;
        }
        return m_BlockMap[x, y, z];
    }

    //void UpdateMesh()
    //{
    //    //Debug.Log("Mesh Update");


    //    CreateMeshData();
    //    var sw = new System.Diagnostics.Stopwatch();     // ストップウォッチオブジェクト生成
    //    sw.Start();  //  時間計測スタート
    //    CreateMesh();
    //    sw.Stop();   // 時間計測終了
    //    TimeSpan span = sw.Elapsed;    //  計測した時間を span に代入
    //    //Debug.Log("time: " + span.TotalMilliseconds.ToString());

    //    m_MeshUpdateFlag = false;
    //}

    //async void UpdateMeshAsync()
    //{
    //    m_MeshUpdateFlag = false;
    //    m_MeshUpdatingFlag = true;
    //    await Task.Run(CreateMeshData);
    //    CreateMesh();
    //    m_MeshUpdatingFlag = false;
    //}

    class ChunkMeshData
    {
        public int vertexIndex = 0;
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector2> uvs = new List<Vector2>();
    }

    ChunkMeshData CreateMeshData()
    {
        ChunkMeshData chunkMeshData = new();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Width; z++)
                {

                    AddBlockRenderingDataToChunk(chunkMeshData, new Vector3(x, y, z));

                }
            }
        }
        return chunkMeshData;
    }

    void AddBlockRenderingDataToChunk(ChunkMeshData chunkMeshData, Vector3 pos)
    {
        if (CheckBlockID(pos) <= 0)
        {
            return;
        }
        var b = BlockManager.GetBlock(CheckBlockID(pos));
        for (int face = 0; face < 6; face++)
        {
            // その面が不透過の個体だった場合
            if (!CheckTransparent(pos + BlockRenderingData.faceChecks[face], (Block.Faces)face))
            {
                continue;
            }
            if (CheckTransparent(pos, (Block.Faces)face) && CheckBlockID(pos) == CheckBlockID(pos + BlockRenderingData.faceChecks[face]))
            {
                continue;
            }

            var t = b.GetTexture((Block.Faces)face);
            if (t == null)
            {
                continue;
            }
            var uv = TextureManager.GetTextureUV(t);
            chunkMeshData.uvs.Add(new Vector2(uv.x, uv.y));
            chunkMeshData.uvs.Add(new Vector2(uv.x, uv.y + uv.height));
            chunkMeshData.uvs.Add(new Vector2(uv.x + uv.width, uv.y));
            chunkMeshData.uvs.Add(new Vector2(uv.x + uv.width, uv.y + uv.height));
            chunkMeshData.vertices.Add(pos + BlockRenderingData.blockVerts[BlockRenderingData.voxelTris[(int)face, 0]]);
            chunkMeshData.vertices.Add(pos + BlockRenderingData.blockVerts[BlockRenderingData.voxelTris[(int)face, 1]]);
            chunkMeshData.vertices.Add(pos + BlockRenderingData.blockVerts[BlockRenderingData.voxelTris[(int)face, 2]]);
            chunkMeshData.vertices.Add(pos + BlockRenderingData.blockVerts[BlockRenderingData.voxelTris[(int)face, 3]]);
            chunkMeshData.triangles.Add(chunkMeshData.vertexIndex);
            chunkMeshData.triangles.Add(chunkMeshData.vertexIndex + 1);
            chunkMeshData.triangles.Add(chunkMeshData.vertexIndex + 2);
            chunkMeshData.triangles.Add(chunkMeshData.vertexIndex + 2);
            chunkMeshData.triangles.Add(chunkMeshData.vertexIndex + 1);
            chunkMeshData.triangles.Add(chunkMeshData.vertexIndex + 3);
            chunkMeshData.vertexIndex += 4;
        }

        return;
    }

    void AddTexture(ChunkMeshData chunkMeshData, int textureID)
    {
        float y = textureID / BlockRenderingData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * BlockRenderingData.TextureAtlasSizeInBlocks);

        x *= BlockRenderingData.NormalizedBlockTextureSize;
        y *= BlockRenderingData.NormalizedBlockTextureSize;

        y = 1f - y - BlockRenderingData.NormalizedBlockTextureSize;

        chunkMeshData.uvs.Add(new Vector2(x, y));
        chunkMeshData.uvs.Add(new Vector2(x, y + BlockRenderingData.NormalizedBlockTextureSize));
        chunkMeshData.uvs.Add(new Vector2(x + BlockRenderingData.NormalizedBlockTextureSize, y));
        chunkMeshData.uvs.Add(new Vector2(x + BlockRenderingData.NormalizedBlockTextureSize, y + BlockRenderingData.NormalizedBlockTextureSize));
    }

    //void CreateMesh()
    //{
    //    m_Mesh = new Mesh
    //    {
    //        vertices = vertices.ToArray(),
    //        triangles = triangles.ToArray(),
    //        uv = uvs.ToArray()
    //    };

    //    m_Mesh.RecalculateNormals();

    //    world.ChunkGenerateMeshColliderQueue.Enqueue((this, m_Mesh));
    //}

    private void OnDestroy()
    {
        Destroy(m_Mesh);
    }

    IEnumerator CoCreateChunckMesh()
    {
        //// 地形生成
        //var task = Task.Run(() => GenerateChunkTerrainNew());
        //while (!task.IsCompleted)
        //{
        //    yield return null;
        //}
        //task.Wait();


        var createMeshDatatask = Task.Run(() => CreateMeshData());
        while (!createMeshDatatask.IsCompleted)
        {
            //Debug.Log("skip create mesh");
            yield return null;
        }
        createMeshDatatask.Wait();
        var chunkMeshData = createMeshDatatask.Result;

        m_Mesh = new Mesh
        {
            vertices = chunkMeshData.vertices.ToArray(),
            triangles = chunkMeshData.triangles.ToArray(),
            uv = chunkMeshData.uvs.ToArray()
        };
        m_Mesh.RecalculateNormals();
        MeshFilter.sharedMesh = m_Mesh;


        NativeArray<int> meshIds = new NativeArray<int>(1, Allocator.Persistent);
        meshIds[0] = m_Mesh.GetInstanceID();
        var bakeMeshJob = new BakeMeshJob(meshIds);
        var bakeMeshJobHandle = bakeMeshJob.Schedule(meshIds.Length, 1);
        while (!bakeMeshJobHandle.IsCompleted)
        {
            yield return null;
        }
        bakeMeshJobHandle.Complete();
        meshIds.Dispose();
        MeshCollider.sharedMesh = m_Mesh;
    }
}
