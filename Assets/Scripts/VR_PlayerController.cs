using UnityEngine;
using UnityEngine.InputSystem;

public class VR_PlayerController : MonoBehaviour
{
    public Player player;
    public Rigidbody body;

    public readonly int CHUNK_LAYER = 1 << 6;

    private Transform cameraTransform;
    private Transform viewAngleTransform;
    private Transform cameraRigTransform;
    private Ray m_Ray;
    private RaycastHit m_Hit;
    private GameObject m_BlockHighlight;

    private bool m_IsCursorMode = false;
    private bool m_ExistLookingBlock = false;
    private float m_MiningInvervalCount = 0;
    private float m_PuttingInvervalCount = 0;

    private BlockPos? m_LookingAtChunkPos;
    private BlockPos? m_OldLookingAtChunkPos;

    //BoxCollider m_Collider;

    void Start()
    {
        viewAngleTransform = transform.Find("ViewAngle").GetComponent<Transform>();
        cameraTransform = viewAngleTransform.Find("OVRCameraRig").GetComponent<Transform>();

        var cameraRig = viewAngleTransform.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
        cameraRigTransform = cameraRig.centerEyeAnchor;

        m_BlockHighlight = player.world.transform.Find("BlockHighlight").gameObject;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        m_IsCursorMode = false;
    }

    void Update()
    {
        m_OldLookingAtChunkPos = m_LookingAtChunkPos;

        //RoatateCameraProc();

        // Look At
        m_Ray.origin = cameraTransform.position;
        m_Ray.direction = cameraTransform.forward;
        m_ExistLookingBlock = Physics.Raycast(m_Ray, out m_Hit, player.Reach, CHUNK_LAYER);

        if (m_ExistLookingBlock)
        {
            Info.UpdateInfo("Look At", m_Hit.point.x.ToString() + " / " + m_Hit.point.y.ToString() + " / " + m_Hit.point.z.ToString() + " (" + getPosLookingAt().ToString() + ")");
            Info.UpdateInfo("Front Look At", getFrontPosLookingAt().ToString());
            m_LookingAtChunkPos = getPosLookingAt();
        }
        else
        {
            Info.UpdateInfo("Look At", "None");
            Info.UpdateInfo("Front Look At", "None");
            m_LookingAtChunkPos = null;
        }

        BlockHighlightProc();

        MineBlock();
        PutBlock();

        // INFO
        Info.UpdateInfo("X, Y, Z", transform.position.x.ToString() + " / " + transform.position.y.ToString() + " / " + transform.position.z.ToString());
        Info.UpdateInfo("Rotation", cameraTransform.localEulerAngles.x.ToString() + " / " + viewAngleTransform.localEulerAngles.y.ToString() + " / " + cameraTransform.localEulerAngles.z.ToString());
        Info.UpdateInfo("Direction", getPlayerLookingDirectionHorizontal().ToString());
        Info.UpdateInfo("Chunk At", new ChunkPos(new BlockPos(transform.position)).ToString());
    }

    void FixedUpdate()
    {
        Vector3 move = Vector3.zero;

        OVRInput.FixedUpdate();
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

        // 右コントローラーのスティック
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        move += cameraRigTransform.right * stick.x * player.HorizontalSpeed;
        move += cameraRigTransform.forward * stick.y * player.HorizontalSpeed;

        body.velocity = move;
        //MoveProc();
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

    World.EnumDirectionHorizontal getPlayerLookingDirectionHorizontal()
    {
        if (45f <= viewAngleTransform.localEulerAngles.y && viewAngleTransform.localEulerAngles.y < 135f)
        {
            return World.EnumDirectionHorizontal.East;
        }
        else if (135f <= viewAngleTransform.localEulerAngles.y && viewAngleTransform.localEulerAngles.y < 225f)
        {
            return World.EnumDirectionHorizontal.North;
        }
        else if (225f <= viewAngleTransform.localEulerAngles.y && viewAngleTransform.localEulerAngles.y < 315f)
        {
            return World.EnumDirectionHorizontal.West;
        }
        else
        {
            return World.EnumDirectionHorizontal.South;
        }
    }

    World.EnumDirectionVertical getPlayerLookingAtDirectionVertical()
    {
        Debug.Log(cameraTransform.localEulerAngles.x.ToString());
        if (cameraTransform.localEulerAngles.x > 90)
        {
            return World.EnumDirectionVertical.Up;
        }
        else
        {
            return World.EnumDirectionVertical.Down;
        }
    }

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
        if (cameraTransform.forward.y <= 0)  // Down
        {
            //Debug.Log("-");
            if (Mathf.Approximately(m_Hit.point.y, pos.y))
            {
                pos.y -= 1;
            }
        }
        if (cameraTransform.forward.x <= 0)
        {
            if (Mathf.Approximately(m_Hit.point.x, pos.x))
            {
                pos.x -= 1;
            }
        }
        if (cameraTransform.forward.z <= 0)
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
            if (cameraTransform.forward.y > 0)
            {
                pos.y -= 1;
            }
        }
        if (m_Hit.point.x == pos.x || m_Hit.point.x == pos.x + 1)
        {
            if (cameraTransform.forward.x > 0)
            {
                pos.x -= 1;
            }
        }
        if (m_Hit.point.z == pos.z || m_Hit.point.z == pos.z + 1)
        {
            if (cameraTransform.forward.z > 0)
            {
                pos.z -= 1;
            }
        }
        return pos;
    }

    public void MineBlock()
    {
    }

    private void PutBlock()
    {
    }
}
