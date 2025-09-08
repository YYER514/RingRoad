#region 模块信息
// **********************************************************************
// Copyright (C) 幻世界
// Please contact me if you have any questions
// File Name:              ParticleExporter
// Author:                幻世界
// **********************************************************************
#endregion

namespace HSJFramework
{
    using UnityEngine;
    using System;
    using System.IO;
    using System.Collections;

    public class ParticleExporter : MonoBehaviour
    {
        // 输出序列帧图的相对路径（工程目录根文件夹下）
        [Header("输出路径")]
        public string path ;
        //  导出图片的摄像机，使用RenderTexture
        private Camera exportCamera;

        //导出帧率，设置Time.captureFramerate会忽略真实时间，而使用此帧率
        [Header("截图帧率")]
        public int frameRate = 25;
        // 导出帧的数目
        [Header("导出帧数")]
        public float frameCount = 20;

        private int currentIndex = 0;

        public void OnEnable()
        {
            Time.captureFramerate = frameRate;
            exportCamera = GetComponent<Camera>();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        void Update()
        {
            if (currentIndex >= frameCount) { return; }
            // 每帧截屏
            StartCoroutine(CaptureFrame());
        }
        //开启截图携程
        IEnumerator CaptureFrame()
        {
            string filename = String.Format("{0}/{1:D04}.png", path, ++currentIndex);

            yield return new WaitForEndOfFrame();
            RenderTexture blackCamRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            exportCamera.targetTexture = blackCamRenderTexture;
            exportCamera.backgroundColor = Color.black;
            exportCamera.Render();
            RenderTexture.active = blackCamRenderTexture;
            Texture2D texb = GetTex2D();

            yield return new WaitForEndOfFrame();
            RenderTexture whiteCamRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            exportCamera.targetTexture = whiteCamRenderTexture;
            exportCamera.backgroundColor = Color.white;
            exportCamera.Render();
            RenderTexture.active = whiteCamRenderTexture;
            Texture2D texw = GetTex2D();

            // 将产生的输出纹理编码到字节数组中，然后写入文件
            Texture2D outputtex = PutTexture(texb, texw);
            byte[] pngShot = outputtex.EncodeToPNG();
            File.WriteAllBytes(filename, pngShot);
            Debug.Log(filename);
            pngShot = null;
            RenderTexture.active = null;
            System.GC.Collect();
        }
        //将两张纹理计算后输出带有透明通道的Tex
        private  Texture2D PutTexture( Texture2D texb, Texture2D texw)
        {
            Texture2D outputtex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
            //根据黑白相机渲染的差异创建Alpha
            for (int y = 0; y < outputtex.height; ++y)
            { // each row
                for (int x = 0; x < outputtex.width; ++x)
                { // each column
                    float alpha;
                    alpha = texw.GetPixel(x, y).r - texb.GetPixel(x, y).r;
                    alpha = 1.0f - alpha;
                    Color color;
                    if (alpha == 0)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = texb.GetPixel(x, y);
                    }
                    color.a = alpha;
                    outputtex.SetPixel(x, y, color);
                }
            }

            return outputtex;
        }

        // 从屏幕上获取纹理，渲染全部或只渲染相机的一半
        private Texture2D GetTex2D()
        {
            // 创建一个屏幕大小的纹理，RGB24格式
            int width = Screen.width;
            int height = Screen.height;
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            // 将屏幕内容读入纹理
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            return tex;
        }
    }
}