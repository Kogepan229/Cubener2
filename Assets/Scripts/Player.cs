using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody body;

    public enum EnumGameMode
    {
        Survival,
        Creative,
    }

    public EnumGameMode GameMode = EnumGameMode.Survival;
    public float Reach = 5f;
    public float HorizontalSpeed = 1.0f;
    public float VerticalSpeed = 1.0f;
    public float HorizontalMouseSpeed = 1.0f;
    public float VerticalMouseSpeed = 1.0f;
    public float MiningInterval = 0.2f;
    public float PuttingInterval = 0.2f;

    public readonly int CHUNK_LAYER = 1 << 6;

    private World world;
    private ToolBar toolBar;

    private Transform cameraTransform;
    private Transform viewAngleTransform;
    private Ray m_Ray;
    private RaycastHit m_Hit;
    private GameObject m_BlockHighlight;

    private bool m_IsCursorMode = false;
    private bool m_ExistLookingBlock = false;
    private float m_MiningInvervalCount = 0;
    private float m_PuttingInvervalCount = 0;

    private Mouse m_Mouse;
    private Keyboard m_Keyboard;

    private float mouseHorizontal;
    private float mouseVertical;
    private float inputHorizontal;
    private float inputVertical;
    private float inputLShift;
    private float inputSpace;
    private float inputLCtrl;
    private float inputMouse0;
    private float inputMouse1;

    private BlockPos? m_LookingAtChunkPos;
    private BlockPos? m_OldLookingAtChunkPos;

    //BoxCollider m_Collider;

    void Start()
    {
        m_Mouse = Mouse.current;
        m_Keyboard = Keyboard.current;

        world = transform.parent.GetComponent<World>();


        viewAngleTransform = transform.Find("ViewAngle").GetComponent<Transform>();
        cameraTransform = viewAngleTransform.Find("PlayerCamera").GetComponent<Transform>();

        m_BlockHighlight = world.transform.Find("BlockHighlight").gameObject;
        toolBar = GameObject.Find("ToolBar").GetComponent<ToolBar>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        m_IsCursorMode = false;
    }

    void Update()
    {
        m_OldLookingAtChunkPos = m_LookingAtChunkPos;

        UpdatePlayerInput();

        ChangeCursorMode();

        RoatateCameraProc();



        // Look At
        m_Ray.origin = cameraTransform.position;
        m_Ray.direction = cameraTransform.forward;
        m_ExistLookingBlock = Physics.Raycast(m_Ray, out m_Hit, Reach, CHUNK_LAYER);

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
        MoveProc();
    }





    private void UpdatePlayerInput()
    {
        InputSystem.Update();
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        inputHorizontal = (m_Keyboard.dKey.isPressed ? 1 : 0) + (m_Keyboard.aKey.isPressed ? -1 : 0);
        inputVertical = (m_Keyboard.wKey.isPressed ? 1 : 0) + (m_Keyboard.sKey.isPressed ? -1 : 0);
        inputLShift = m_Keyboard.leftShiftKey.isPressed ? 1 : 0;
        inputSpace = m_Keyboard.spaceKey.isPressed ? 1 : 0;
        inputLCtrl = Input.GetKeyUp(KeyCode.LeftControl) ? 1 : 0;
        inputMouse0 = m_Mouse.leftButton.isPressed ? 1 : 0;
        inputMouse1 = m_Mouse.rightButton.isPressed ? 1 : 0;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        toolBar.ShiftNumberInFocus(scroll);
    }

    private void ChangeCursorMode()
    {
        if (inputLCtrl == 1)
        {
            if (m_IsCursorMode)
            {
                Cursor.lockState = CursorLockMode.Locked;
                m_IsCursorMode = false;
                Debug.Log("LCtrl: Locked");
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                m_IsCursorMode = true;
                Debug.Log("LCtrl: Unlocked");
            }
        }
    }

    private void RoatateCameraProc()
    {
        if (m_IsCursorMode == false)
        {
            viewAngleTransform.Rotate(Vector3.up * mouseHorizontal * HorizontalMouseSpeed);
            cameraTransform.Rotate(Vector3.right * -mouseVertical * VerticalMouseSpeed);
            cameraTransform.localRotation = ClampRotation(cameraTransform.localRotation);
        }
    }

    private void MoveProc()
    {
        // Player Move
        Vector3 move = Vector3.zero;
        move += viewAngleTransform.right * inputHorizontal * HorizontalSpeed * 10;
        move += viewAngleTransform.forward * inputVertical * HorizontalSpeed * 10;
        move += Vector3.up * -inputLShift * VerticalSpeed * 10;
        move += Vector3.up * inputSpace * VerticalSpeed * 10;
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
        if (inputMouse0 == 1)
        {
            if (m_ExistLookingBlock && (m_MiningInvervalCount == 0 || m_MiningInvervalCount > MiningInterval))
            {
                world.PutBlock(0, getPosLookingAt());
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
        int blockID = toolBar.GetBlockIdInFocus();
        if (inputMouse1 == 1 && blockID != 0)
        {
            if (m_ExistLookingBlock && (m_PuttingInvervalCount == 0 || m_PuttingInvervalCount > PuttingInterval))
            {
                world.PutBlock(blockID, getFrontPosLookingAt());
                m_PuttingInvervalCount = 0;
            }
            m_PuttingInvervalCount += Time.deltaTime;
        }
        else
        {
            m_PuttingInvervalCount = 0;
        }
    }

    public void Teleport(BlockPos pos)
    {
        transform.position = pos.AsVector3Int();
    }
}
