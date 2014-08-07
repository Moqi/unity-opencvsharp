using OpenCvSharp;
using UnityEngine;
using System.Collections;

namespace UnityCV
{
    public class UnityCvBase
    {

        public static WebCamTexture WebCamTexture { get; private set; }

        public static void Init(string deviceName, int requestedWidth, int requestedHeight, int requestedFps, BitDepth targetBitDepth)
        {
            WebCamTexture = new WebCamTexture(deviceName, requestedWidth, requestedHeight, requestedFps);
            WebCamTexture.Play();
            if (!WebCamTextureProxy.instance)
                new GameObject("_webcamtextureproxy") { hideFlags = HideFlags.HideAndDontSave }.AddComponent<WebCamTextureProxy>().SetTargetTexture(WebCamTexture).SetTargetDepth(targetBitDepth).StartCapture();
        }

    }

    public static class UnityCvUtils
    {

        public static CvScalar ColorToBGRScalar(Color color)
        {
            return new CvScalar(
                color.b * 255d,
                color.g * 255d,
                color.r * 255d
                );
        }

    }

}