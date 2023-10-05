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
    public float MiningInterval = 0.3f;

    public readonly int CHUNK_LAYER = 1 << 6;

    private World world;

    private Transform cameraTransform;
    private Transform viewAngleTransform;
    private Ray m_Ray;
    private RaycastHit m_Hit;
    private GameObject m_BlockHighlight;

    private bool m_IsCursorMode = false;
    private bool m_ExistLookingBlock = false;
    private float m_MiningInvervalCount = 0;

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
        //cameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        world = transform.parent.GetComponent<World>();
        m_BlockHighlight = world.transform.Find("BlockHighlight").gameObject;
        //m_Collider = transform.gameObject.GetComponent<BoxCollider>();
        //m_Collider.contactOffset = 5f;

        viewAngleTransform = transform.Find("ViewAngle").GetComponent<Transform>();
        if (transform.Find("ViewAngle") == null )
        {
            Debug.Log("null1");
        }
        else
        {
            Debug.Log("found");
        }
        cameraTransform = viewAngleTransform.Find("PlayerCamera").GetComponent<Transform>();
        if (viewAngleTransform.Find("PlayerCamera") == null)
        {
            Debug.Log("null2");
        }
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

        MoveProc();

        // Look At
        m_Ray.origin = cameraTransform.position;
        m_Ray.direction = cameraTransform.forward;
        m_ExistLookingBlock = Physics.Raycast(m_Ray, out m_Hit, Reach, CHUNK_LAYER);

        if (m_ExistLookingBlock)
        {
            Info.InfoData["Look At"] = m_Hit.point.x.ToString() + " / " + m_Hit.point.y.ToString() + " / " + m_Hit.point.z.ToString() + " (" + getPosLookingAt().ToString() + ")";
            Info.InfoData["Front Look At"] = getFrontPosLookingAt().ToString();
            m_LookingAtChunkPos = getPosLookingAt();
        }
        else
        {
            Info.InfoData["Look At"] = "None";
            Info.InfoData["Front Look At"] = "None";
            m_LookingAtChunkPos = null;
        }

        BlockHighlightProc();

        MiningProc();

        // INFO
        Info.InfoData["X, Y, Z"] = transform.position.x.ToString() + " / " + transform.position.y.ToString() + " / " + transform.position.z.ToString();
        Info.InfoData["Rotation"] = cameraTransform.localEulerAngles.x.ToString() + " / " + viewAngleTransform.localEulerAngles.y.ToString() + " / " + cameraTransform.localEulerAngles.z.ToString();
        Info.InfoData["Direction"] = getPlayerLookingDirectionHorizontal().ToString();
        Info.InfoData["Chunk"] = new ChunkPos(new BlockPos(transform.position)).ToString();
    }





    private void UpdatePlayerInput()
    {
        InputSystem.Update();
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
        //inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputHorizontal = (m_Keyboard.dKey.isPressed ? 1 : 0) + (m_Keyboard.aKey.isPressed ? -1 : 0);
        inputVertical = (m_Keyboard.wKey.isPressed ? 1 : 0) + (m_Keyboard.sKey.isPressed ? -1 : 0);
        inputLShift = m_Keyboard.leftShiftKey.isPressed ? 1 : 0;
        inputSpace = m_Keyboard.spaceKey.isPressed ? 1 : 0;
        inputLCtrl = Input.GetKeyUp(KeyCode.LeftControl) ? 1 : 0;
        inputMouse0 = m_Mouse.leftButton.isPressed ? 1 : 0;
        //Debug.Log(inputMouse0);
        inputMouse1 = Input.GetKeyDown(KeyCode.Mouse1) ? 1 : 0;

        //inputVertical = Input.GetAxisRaw("Vertical");
        //inputLShift = Input.GetAxisRaw("Fire3");
        //inputSpace = Input.GetAxisRaw("Jump");
        //inputMouse0 = Input.GetKey(KeyCode.Mouse0) ? 1 : 0;
    }

    private void ChangeCursorMode()
    {
        if (inputLCtrl == 1)
        {
            if (m_IsCursorMode)
            {
                Cursor.lockState = CursorLockMode.Locked;
                //Cursor.visible = false;
                m_IsCursorMode = false;
                Debug.Log("LCtrl: Locked");
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
                m_IsCursorMode = true;
                Debug.Log("LCtrl: Unlocked");
            }
        }
    }

    private void RoatateCameraProc()
    {
        if (m_IsCursorMode == false)
        {
            //Vector3 _tmp = cameraTransform.localEulerAngles;
            viewAngleTransform.Rotate(Vector3.up * mouseHorizontal * HorizontalMouseSpeed/*, Space.World*/);
            cameraTransform.Rotate(Vector3.right * -mouseVertical * VerticalMouseSpeed/*, Space.Self*/);
            cameraTransform.localRotation = ClampRotation(cameraTransform.localRotation);
        }
    }

    private void MoveProc()
    {
        // Player Move
        Vector3 move = Vector3.zero;
        //Debug.Log(cameraTransform.right);
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
            if (Mathf.Approximately(m_Hit.point.y, pos.y) /*m_Hit.point.y == pos.y*/)
            {
                pos.y -= 1;
            }
        }
        if (cameraTransform.forward.x <= 0)
        {
            if (Mathf.Approximately(m_Hit.point.x, pos.x) /*m_Hit.point.x == pos.x*/)
            {
                pos.x -= 1;
            }
        }
        if (cameraTransform.forward.z <= 0)
        {
            if (Mathf.Approximately(m_Hit.point.z, pos.z) /*m_Hit.point.z == pos.z*/)
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

    public void MiningProc()
    {
        if (inputMouse0 == 1)
        {
            if (m_ExistLookingBlock && (m_MiningInvervalCount == 0 || m_MiningInvervalCount > MiningInterval))
            {
                Debug.Log("put: " + getPosLookingAt().ToString());
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

    public void Teleport(BlockPos pos)
    {
        transform.position = pos.AsVector3Int();
    }
}
