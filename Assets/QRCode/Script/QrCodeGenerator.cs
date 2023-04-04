using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class QrCodeGenerator : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImgReceiver;
    [SerializeField]
    private TMP_InputField _inputField;

    private Texture2D _encodedText;
    // Start is called before the first frame update
    void Start()
    {
        _encodedText = new Texture2D(256, 256);

        //get ip address
        Process process = new Process();
        process.StartInfo.FileName = "ipconfig";
        process.StartInfo.Arguments = "/all";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        Regex regex = new Regex(@"IPv4 Address[.\s\S]*?:\s*(?<ipAddress>\d+\.\d+\.\d+\.\d+)");
        Match match = regex.Match(output);

        if (match.Success)
        {
            // Parse the IPv4 address
            string ipAddressString = match.Groups["ipAddress"].Value;
            IPAddress ipAddress = IPAddress.Parse(ipAddressString);

            // Determine the address family (IPv4 or IPv6)
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                UnityEngine.Debug.LogWarning(string.Format("IPv4 Address: {0}", ipAddressString));
                string url = "http://" + ipAddressString + ":5500/";
                EncodeTextToQrCode(url);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Not an IPv4 address");
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("IPv4 address not found");
        }
        UnityEngine.Debug.LogWarning(output);
    }

    private Color32[] Encode(string textToEncode, int width, int height)
    {
        BarcodeWriter writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };


        return writer.Write(textToEncode);
    }

    public void OnClickEncode()
    {
        //EncodeTextToQrCode();
    }

    private void EncodeTextToQrCode(string textWrite)
    {
        //string textWrite = string.IsNullOrEmpty(_inputField.text) ? "You should write something" : _inputField.text;

        Color32[] _convertPixelToTexture = Encode(textWrite, _encodedText.width, _encodedText.height);
        _encodedText.SetPixels32(_convertPixelToTexture);
        _encodedText.Apply();

        _rawImgReceiver.texture = _encodedText;
    }


        


}
