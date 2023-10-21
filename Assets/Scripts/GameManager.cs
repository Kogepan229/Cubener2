using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        TextureManager.LoadTextures();
        BlockManager.ResistAllBlock();
        Debug.Log("loaded");
        GameObject.Find("World").GetComponent<World>().enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float fps = 1f / Time.deltaTime;
        Info.UpdateInfo("fps", fps.ToString());
    }
}
