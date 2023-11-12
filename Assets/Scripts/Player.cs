using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum EnumGameMode
    {
        Survival,
        Creative,
    }

    public EnumGameMode GameMode = EnumGameMode.Survival;
    public float Reach = 5f;
    public float HorizontalSpeed = 10.0f;
    public float VerticalSpeed = 10.0f;
    public float MiningInterval = 0.2f;
    public float PuttingInterval = 0.2f;

    public readonly int CHUNK_LAYER = 1 << 6;

    public World world;
    public ToolBar toolBar;

    private Transform cameraTransform;
    private Transform viewAngleTransform;

    //BoxCollider m_Collider;

    void Start()
    {

        world = transform.parent.GetComponent<World>();
        toolBar = GameObject.Find("ToolBar").GetComponent<ToolBar>();


        viewAngleTransform = transform.Find("ViewAngle").GetComponent<Transform>();
        cameraTransform = viewAngleTransform.Find("PlayerCamera").GetComponent<Transform>();

        //m_BlockHighlight = world.transform.Find("BlockHighlight").gameObject;
    }

    void Update()
    {
    }


    public BlockPos GetBlockPos()
    {
        return new BlockPos(transform.position);
    }

    public ChunkPos GetChunkPos()
    {
        return new ChunkPos(GetBlockPos());
    }

    public void Teleport(BlockPos pos)
    {
        transform.position = pos.AsVector3Int();
    }
}
