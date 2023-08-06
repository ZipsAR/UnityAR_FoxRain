using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraFrameAccess : MonoBehaviour
{
    [Header("Camera Feed")] public RawImage CameraRawImage;

    public bool RenderUsingYUVPlanes;

    private ARCameraManager _cameraManager;
    private Texture2D _cameraTexture;
    private bool _feedPaused;

    private XRCameraIntrinsics _intrinsics;
    private XRCpuImage _lastCpuImage;
    
    public void Awake()
    {
        _cameraManager = FindObjectOfType<ARCameraManager>();
        _feedPaused = false;
    }

    private void Start()
    {
        _cameraManager.frameReceived += OnFrameReceived;
        
    }

    private void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        if (_feedPaused)
        {
            return;
        }

        if (!_cameraManager.TryAcquireLatestCpuImage(out _lastCpuImage))
        {
            Debug.Log("Failed to acquire latest cpu image.");
            return;
        }

        UpdateCameraTexture(_lastCpuImage, RenderUsingYUVPlanes);
        // Update intrinsics on every frame, as intrinsics can change over time
        // UpdateCameraIntrinsics();
    }
    
    private unsafe void UpdateCameraTexture(XRCpuImage image, bool convertYuvManually)
    {
        var format = TextureFormat.RGBA32;

        if (_cameraTexture == null || _cameraTexture.width != image.width || _cameraTexture.height != image.height)
        {
            _cameraTexture = new Texture2D(image.width, image.height, format, false);
        }

        var conversionParams = new XRCpuImage.ConversionParams(image, format);
        var rawTextureData = _cameraTexture.GetRawTextureData<byte>();
        var rawTexturePtr = new IntPtr(rawTextureData.GetUnsafePtr());

        if (convertYuvManually)
        {
            ConvertYuvImageIntoBuffer(image, rawTexturePtr, format);
        }
        else
        {
            try
            {
                image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            }
            finally
            {
                image.Dispose();
            }
        }

        _cameraTexture.Apply();
        CameraRawImage.texture = _cameraTexture;
    }
    
    private void ConvertYuvImageIntoBuffer(XRCpuImage image, IntPtr targetBuffer, TextureFormat format)
        {
            var bufferSize = image.height * image.width * 4;
            var rgbBuffer = new byte[bufferSize];

            // Populate buffer with YUV => RGB conversion

            var yPlane = image.GetPlane(0);
            var uvPlane = image.GetPlane(1);

            // Known offset values of test devices.
            // AR Foundation's XRCpuImage.Plane does not expose buffer offsets or image offsets
            var planeOffsetY = 0;
            var planeOffsetUV = 0;
            var imageOffsetY = 0;
            var imageOffsetX = 0;

            for (int row = 0; row < image.height; row++)
            {
                for (int col = 0; col < image.width; col++)
                {
                    byte y = yPlane.data[planeOffsetY + ((imageOffsetY + row) * yPlane.rowStride) + imageOffsetX + col];

                    var rowOffset = (imageOffsetY + row) / 2 * uvPlane.rowStride;
                    var colOffset = (imageOffsetX + col) / 2 * 2;
                    var offset = planeOffsetUV + rowOffset + colOffset;

                    // YUV NV12 to RGB conversion (A3 wrongly reports NV21, no need to swap U & V)
                    // https://en.wikipedia.org/wiki/YUV#Y%E2%80%B2UV420sp_(NV21)_to_RGB_conversion_(Android)

                    sbyte u = (sbyte)(uvPlane.data[offset] - 128);
                    sbyte v = (sbyte)(uvPlane.data[offset + 1] - 128);

                    var r = y + (1.370705f * v);
                    var g = y - (0.698001f * v) - (0.337633f * u);
                    var b = y + (1.732446f * u);

                    r = r > 255 ? 255 : r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b < 0 ? 0 : b;

                    int pixelIndex = ((image.height - row - 1) * image.width) + col;

                    switch (format)
                    {
                        case TextureFormat.RGB24:
                            rgbBuffer[4 * pixelIndex] = (byte)r;
                            rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            rgbBuffer[(4 * pixelIndex) + 2] = (byte)b;
                            break;
                        case TextureFormat.RGBA32:
                            rgbBuffer[4 * pixelIndex] = (byte)r;
                            rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            rgbBuffer[(4 * pixelIndex) + 2] = (byte)b;
                            rgbBuffer[(4 * pixelIndex) + 3] = 255;
                            break;
                        case TextureFormat.BGRA32:
                            rgbBuffer[4 * pixelIndex] = (byte)b;
                            rgbBuffer[(4 * pixelIndex) + 1] = (byte)g;
                            rgbBuffer[(4 * pixelIndex) + 2] = (byte)r;
                            rgbBuffer[(4 * pixelIndex) + 3] = 255;
                            break;
                    }
                }
            }
            
            Marshal.Copy(rgbBuffer, 0, targetBuffer, bufferSize);
        }
}
