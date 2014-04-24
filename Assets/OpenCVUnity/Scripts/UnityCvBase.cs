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
            var doubles = new[]
        {
            (double) color.b*255,
            (double) color.g*255,
            (double) color.r*255
        };
            return new CvScalar(doubles[0], doubles[1], doubles[2]);
        }

    }
 
}