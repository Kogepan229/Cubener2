using UnityEngine;

public class VR_PlayerController : MonoBehaviour
{
    public Player player;
    public Rigidbody body;

    public readonly int CHUNK_LAYER = 1 << 6;

    [SerializeField] private Transform RightConrtollerTransform;
    private Transform cameraRigTransform;
    private Ray m_Ray;
    private RaycastHit m_Hit;
    private GameObject m_BlockHighlight;

    private bool m_ExistLookingBlock = false;
    private float m_MiningInvervalCount = 0;
    private float m_PuttingInvervalCount = 0;

    private BlockPos? m_LookingAtChunkPos;
    private BlockPos? m_OldLookingAtChunkPos;

    void Start()
    {
        Transform viewAngleTransform = transform.Find("ViewAngle").GetComponent<Transform>();

        var cameraRig = viewAngleTransform.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
        cameraRigTransform = cameraRig.centerEyeAnchor;

        m_BlockHighlight = transform.parent.GetComponent<World>().transform.Find("BlockHighlight").gameObject;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        m_OldLookingAtChunkPos = m_LookingAtChunkPos;
        // Look At
        m_Ray.origin = RightConrtollerTransform.position;
        m_Ray.direction = RightConrtollerTransform.forward;
        m_ExistLookingBlock = Physics.Raycast(m_Ray, out m_Hit, player.Reach, CHUNK_LAYER);

        if (m_ExistLookingBlock)
        {
            m_LookingAtChunkPos = getPosLookingAt();
            Info.UpdateInfo("Look At", m_Hit.point.x.ToString() + " / " + m_Hit.point.y.ToString() + " / " + m_Hit.point.z.ToString() + " (" + m_LookingAtChunkPos.ToString() + ")");
            Info.UpdateInfo("Front Look At", getFrontPosLookingAt().ToString());
        }
        else
        {
            m_LookingAtChunkPos = null;
            Info.UpdateInfo("Look At", "None");
            Info.UpdateInfo("Front Look At", "None");
        }

        BlockHighlightProc();

        Info.UpdateInfo("Chunk At", new ChunkPos(new BlockPos(player.transform.position)).ToString());


        OVRInput.FixedUpdate();

        Move();
        MineBlock();
        PutBlock();
    }

    private void Move()
    {
        Vector3 move = Vector3.zero;

        // 右コントローラーのスティック
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        move += cameraRigTransform.right * stick.x * player.HorizontalSpeed;
        move += cameraRigTransform.forward * stick.y * player.HorizontalSpeed;

        move.y = 0;

        if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            // 右コントローラーのAボタンが押された時の処理
            move += Vector3.up * player.VerticalSpeed * -1;
        }
        if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            // 右コントローラーのBボタンが押された時の処理
            move += Vector3.up * player.VerticalSpeed;
        }

        

        body.velocity = move;
    }

    private void BlockHighlightProc()
    {
        // 見ているブロックが変わった場合
        if (m_LookingAtChunkPos != m_OldLookingAtChunkPos)
        {
            if (m_LookingAtChunkPos == null)
            {
                m_BlockHighlight.SetActive(false);
            }
            else
            {
                m_BlockHighlight.SetActive(true);
                m_BlockHighlight.transform.position = ((BlockPos)m_LookingAtChunkPos).AsVector3Int();
            }
        }
    }

    // X軸の回転を±90度に制限
    // 参考サイト https://www.popii33.com/unity-first-person-camera/
    public Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,zはベクトル（量と向き）：wはスカラー（座標とは無関係の量）)
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, -90f, 90f);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

    //World.EnumDirectionHorizontal getPlayerLookingDirectionHorizontal()
    //{
    //    if (45f <= viewAngleTransform.localEulerAngles.y && viewAngleTransform.localEulerAngles.y < 135f)
    //    {
    //        return World.EnumDirectionHorizontal.East;
    //    }
    //    else if (135f <= viewAngleTransform.localEulerAngles.y && viewAngleTransform.localEulerAngles.y < 225f)
    //    {
    //        return World.EnumDirectionHorizontal.North;
    //    }
    //    else if (225f <= viewAngleTransform.localEulerAngles.y && viewAngleTransform.localEulerAngles.y < 315f)
    //    {
    //        return World.EnumDirectionHorizontal.West;
    //    }
    //    else
    //    {
    //        return World.EnumDirectionHorizontal.South;
    //    }
    //}

    //World.EnumDirectionVertical getPlayerLookingAtDirectionVertical()
    //{
    //    Debug.Log(cameraTransform.localEulerAngles.x.ToString());
    //    if (cameraTransform.localEulerAngles.x > 90)
    //    {
    //        return World.EnumDirectionVertical.Up;
    //    }
    //    else
    //    {
    //        return World.EnumDirectionVertical.Down;
    //    }
    //}

    public BlockPos GetBlockPos()
    {
        return new BlockPos(transform.position);
    }

    public ChunkPos GetChunkPos()
    {
        return new ChunkPos(GetBlockPos());
    }

    BlockPos getPosLookingAt()
    {
        BlockPos pos = new BlockPos(m_Hit.point);
        if (RightConrtollerTransform.forward.y <= 0)  // Down
        {
            //Debug.Log("-");
            if (Mathf.Approximately(m_Hit.point.y, pos.y))
            {
                pos.y -= 1;
            }
        }
        if (RightConrtollerTransform.forward.x <= 0)
        {
            if (Mathf.Approximately(m_Hit.point.x, pos.x))
            {
                pos.x -= 1;
            }
        }
        if (RightConrtollerTransform.forward.z <= 0)
        {
            if (Mathf.Approximately(m_Hit.point.z, pos.z))
            {
                pos.z -= 1;
            }
        }

        return pos;
    }

    BlockPos getFrontPosLookingAt()
    {
        BlockPos pos = new BlockPos(m_Hit.point);
        if (m_Hit.point.y == pos.y || m_Hit.point.y == pos.y + 1)
        {
            if (RightConrtollerTransform.forward.y > 0)
            {
                pos.y -= 1;
            }
        }
        if (m_Hit.point.x == pos.x || m_Hit.point.x == pos.x + 1)
        {
            if (RightConrtollerTransform.forward.x > 0)
            {
                pos.x -= 1;
            }
        }
        if (m_Hit.point.z == pos.z || m_Hit.point.z == pos.z + 1)
        {
            if (RightConrtollerTransform.forward.z > 0)
            {
                pos.z -= 1;
            }
        }
        return pos;
    }

    private void MineBlock()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0)
        {
            if (m_ExistLookingBlock && (m_MiningInvervalCount == 0 || m_MiningInvervalCount > player.MiningInterval))
            {
                player.world.PutBlock(0, getPosLookingAt());
                m_MiningInvervalCount = 0;
            }
            m_MiningInvervalCount += Time.deltaTime;
        }
        else
        {
            m_MiningInvervalCount = 0;
        }
    }

    private void PutBlock()
    {
        int blockID = player.toolBar.GetBlockIdInFocus();
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0 && blockID != 0)
        {
            if (m_ExistLookingBlock && (m_PuttingInvervalCount == 0 || m_PuttingInvervalCount > player.PuttingInterval))
            {
                player.world.PutBlock(blockID, getFrontPosLookingAt());
                m_PuttingInvervalCount = 0;
            }
            m_PuttingInvervalCount += Time.deltaTime;
        }
        else
        {
            m_PuttingInvervalCount = 0;
        }
    }
}
