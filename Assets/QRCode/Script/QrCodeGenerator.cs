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
using System.Net.NetworkInformation;

public class QrCodeGenerator : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImgReceiver;
    [SerializeField]
    int portNumber;

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

        // Extract the IPv4 address that is inside the wireless Lan adapter
        Regex regex = new Regex(@"Wireless LAN adapter Wi(?:-)?Fi:[\s\S]*?Connection-specific DNS Suffix\s*\. :\s*(?<dnsSuffix>[^\s]+)[\s\S]*?IPv4 Address[.\s\S]*?:\s*(?<ipAddress>\d+\.\d+\.\d+\.\d+)[\s\S]*?Default Gateway[.\s\S]*?:\s*(?<defaultGateway>\d+\.\d+\.\d+\.\d+)");
        Match match = regex.Match(output);

        if (match.Success)
        {
            // Parse the IPv4 address
            string ipAddressString = match.Groups["ipAddress"].Value;
            IPAddress ipAddress = IPAddress.Parse(ipAddressString);

            // Determine the address family (IPv4 or IPv6)
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                string url = "http://" + ipAddressString + ":" + portNumber + "/";
                EncodeTextToQrCode(url);
            }
        }
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

    private void EncodeTextToQrCode(string textWrite)
    {
        Color32[] _convertPixelToTexture = Encode(textWrite, _encodedText.width, _encodedText.height);
        _encodedText.SetPixels32(_convertPixelToTexture);
        _encodedText.Apply();

        _rawImgReceiver.texture = _encodedText;
    }

}
