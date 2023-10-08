using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Ripple : MonoBehaviour
{
    public Camera mainCamera;
    public RenderTexture PrevRT;//上一帧
    public RenderTexture CurrRT;//当前帧
    public RenderTexture TempRT;

    public Shader DrawShader;
    public Shader RippleShader;
    public Material DrawMat;
    public Material RippleMat;

    [Range(0, 0.2f)]
    public float DrawRadius = 0.2f;
    public int TextureSize = 512;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.GetComponent<Camera>();
        CurrRT = CreatRT();
        TempRT = CreatRT();
        PrevRT = CreatRT();

        DrawMat = new Material(DrawShader);
        RippleMat = new Material(RippleShader);

        GetComponent<Renderer>().material.mainTexture = CurrRT;
    }

    public RenderTexture CreatRT()
    {
        RenderTexture rt = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);
        rt.Create();
        return rt;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius">半径</param>

    private void DrawAt(float x, float y, float radius)
    {
        DrawMat.SetTexture("_SourceTex", CurrRT);
        DrawMat.SetVector("_Pos", new Vector4(x, y, radius));

        Graphics.Blit(null, TempRT, DrawMat);

        RenderTexture rt = TempRT;
        TempRT = CurrRT;
        CurrRT = rt;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                DrawAt(hit.textureCoord.x, hit.textureCoord.y, DrawRadius);

            }
        }

        //计算涟漪
        RippleMat.SetTexture("_PrevRT", PrevRT);
        RippleMat.SetTexture("_CurrentRT", CurrRT);

        Graphics.Blit(null, TempRT, RippleMat);
        Graphics.Blit(TempRT, PrevRT);

        RenderTexture rt = PrevRT;
        PrevRT = CurrRT;
        CurrRT = rt;
    }
}
