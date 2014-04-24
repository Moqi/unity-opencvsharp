using OpenCvSharp;
using Uk.Org.Adcock.Parallel;
using UnityEngine;
using System.Collections;

public class WebCamTextureProxy : MonoBehaviour
{

    public static WebCamTextureProxy instance;
    public static System.Action<IplImage> OnFrameReady;
    private WebCamTexture _webCamTexture;
    private IplImage _iplImage;

    private IEnumerator Capture()
    {
        var width = _webCamTexture.width;
        var height = _webCamTexture.height;
        var pixels = _webCamTexture.GetPixels();
        //Debug.Log("Capturing...");
        /*
        Parallel.For(0, height * width, x =>
        {
            var i = x / height; //y
            var j = x % width;  //x
            Debug.Log(string.Format("{0}|{1}|{2}", x, i, j));
            var pixel = pixels[j + i * width];
            var col = new CvScalar(
                (double)pixel.b * 255,
                (double)pixel.g * 255,
                (double)pixel.r * 255
            );

            _iplImage.Set2D(i, j, col);
        });
        for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
            {
                var pixel = pixels[j + i * width];
                var col = new CvScalar
                {
                    Val0 = (double)pixel.b * 255,
                    Val1 = (double)pixel.g * 255,
                    Val2 = (double)pixel.r * 255
                };

                _iplImage.Set2D(i, j, col);
            }
        */
        Parallel.For(0, height, i =>
        {
            for (var j = 0; j < width; j++)
            {
                var pixel = pixels[j + i * width];
                var col = new CvScalar
                {
                    Val0 = (double)pixel.b * 255,
                    Val1 = (double)pixel.g * 255,
                    Val2 = (double)pixel.r * 255
                };

                _iplImage.Set2D(i, j, col);
            }
        });
        if (OnFrameReady != null) OnFrameReady.Invoke(_iplImage);
        yield return null;
        StartCoroutine(Capture());
    }

    public WebCamTextureProxy SetTargetTexture(WebCamTexture texture)
    {
        _webCamTexture = texture;
        return this;
    }

    public WebCamTextureProxy SetTargetDepth(BitDepth bitDepth)
    {
        _iplImage = new IplImage(_webCamTexture.width, _webCamTexture.height, bitDepth, 3);
        return this;
    }

    public void StartCapture()
    {
        StartCoroutine(Capture());
    }

    public void Awake()
    {
        instance = this;
    }

    public void OnDestroy()
    {
        Cv.ReleaseImage(_iplImage);
        _iplImage = null;
    }

}
