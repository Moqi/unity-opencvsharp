using System.Collections;
using OpenCvSharp;
using Uk.Org.Adcock.Parallel;
using UnityCV;
using UnityEngine;
using Color = UnityEngine.Color;

// ReSharper disable once CheckNamespace
public class UnityCvTest : MonoBehaviour
{

    public BitDepth ImagesDepth = BitDepth.U8;
    public Color ThreshFromColor, ThreshToColor;
    public bool UpdateColorsEachFrame;
    public bool ShowThresholdedImage;
    public Renderer TargetRenderer;

    public int RequestedWidth;
    public int RequestedHeight;
    public int RequestedFps;

    public Vector2 ColorPosition;
    private CvScalar _cvScalarFrom, _cvScalarTo;

    public GameObject FollowerPrefab;
    private Transform _dummyTransform;

    private IplImage _scribbleImage;
    private Vector3 _smoothVel;
    public float SmoothTime;

    private IplImage GetThresholdedImage(IplImage img, CvScalar from, CvScalar to)
    {
        var imgHsv = Cv.CreateImage(Cv.GetSize(img), ImagesDepth, 3);
        Cv.CvtColor(img, imgHsv, ColorConversion.BgrToHsv);
        var imgThreshed = Cv.CreateImage(Cv.GetSize(img), ImagesDepth, 1);
        Cv.InRangeS(imgHsv, from, to, imgThreshed);
        Cv.ReleaseImage(imgHsv);
        if (ShowThresholdedImage) Cv.ShowImage("Threshold", imgThreshed);
        return imgThreshed;
    }

    public void Awake()
    {
        UnityCvBase.Init(WebCamTexture.devices[0].name, RequestedWidth, RequestedHeight, RequestedFps, ImagesDepth);
        TargetRenderer.sharedMaterial.mainTexture = UnityCvBase.WebCamTexture;

        Camera.main.orthographicSize = Screen.height / 2f;
        var tr = TargetRenderer.transform;
        tr.localScale = new Vector3(Screen.width, Screen.height);

        _cvScalarFrom = UnityCvUtils.ColorToBGRScalar(ThreshFromColor);
        _cvScalarTo = UnityCvUtils.ColorToBGRScalar(ThreshToColor);

        if (FollowerPrefab) _dummyTransform = ((GameObject)Instantiate(FollowerPrefab)).transform;

        if (ShowThresholdedImage) Cv.NamedWindow("Threshold", WindowMode.FreeRatio);

        WebCamTextureProxy.OnFrameReady += Process;
    }

    public void Process(IplImage frame)
    {
        if (UpdateColorsEachFrame)
        {
            _cvScalarFrom = UnityCvUtils.ColorToBGRScalar(ThreshFromColor);
            _cvScalarTo = UnityCvUtils.ColorToBGRScalar(ThreshToColor);
        }

        var imgThresh = GetThresholdedImage(frame, _cvScalarFrom, _cvScalarTo);

        CvMoments moments;
        Cv.Moments(imgThresh, out moments, true);

        var moment10 = Cv.GetSpatialMoment(moments, 1, 0);
        var moment01 = Cv.GetSpatialMoment(moments, 0, 1);
        var area = Cv.GetSpatialMoment(moments, 0, 0);
        ColorPosition = new Vector2((float)(moment10 / area), (float)(moment01 / area));
        ColorPosition.Scale(new Vector2((float)Screen.width / frame.Width,
           (float)Screen.height / frame.Height));

        ColorPosition -= new Vector2(Screen.width, Screen.height) * .5f;

        Cv.ReleaseImage(imgThresh);
    }

    public void OnDestroy()
    {
        WebCamTextureProxy.OnFrameReady -= Process;
        Cv.DestroyAllWindows();
        TargetRenderer.sharedMaterial.mainTexture = null;
    }

    public void Update()
    {
        if (ColorPosition.magnitude > 0 && _dummyTransform != null)
            _dummyTransform.position = Vector3.SmoothDamp(_dummyTransform.position, new Vector3(ColorPosition.x, ColorPosition.y), ref _smoothVel, SmoothTime);
    }

}
