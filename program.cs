using System;
using System.Drawing;
using System.Text;

class Steganography
{
    public static void EncodeMessage(string inputImagePath, string outputImagePath, string message)
    {
        Bitmap bmp = new Bitmap(inputImagePath);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message + "\0"); // Null terminator for message end
        int messageIndex = 0;
        bool messageEncoded = false;

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixel = bmp.GetPixel(x, y);
                if (messageIndex < messageBytes.Length)
                {
                    byte r = (byte)((pixel.R & 0xFE) | ((messageBytes[messageIndex] >> 7) & 1));
                    byte g = (byte)((pixel.G & 0xFE) | ((messageBytes[messageIndex] >> 6) & 1));
                    byte b = (byte)((pixel.B & 0xFE) | ((messageBytes[messageIndex] >> 5) & 1));
                    messageBytes[messageIndex] <<= 3;
                    messageIndex++;
                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
                else
                {
                    messageEncoded = true;
                    break;
                }
            }
            if (messageEncoded) break;
        }
        bmp.Save(outputImagePath);
        Console.WriteLine("Message encoded successfully!");
    }

    public static string DecodeMessage(string inputImagePath)
    {
        Bitmap bmp = new Bitmap(inputImagePath);
        StringBuilder extractedMessage = new StringBuilder();
        byte extractedByte = 0;
        int bitCount = 0;

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixel = bmp.GetPixel(x, y);
                extractedByte = (byte)((extractedByte << 1) | (pixel.R & 1));
                extractedByte = (byte)((extractedByte << 1) | (pixel.G & 1));
                extractedByte = (byte)((extractedByte << 1) | (pixel.B & 1));
                bitCount += 3;

                if (bitCount >= 8)
                {
                    bitCount = 0;
                    if (extractedByte == 0) // Null terminator
                        return extractedMessage.ToString();
                    extractedMessage.Append((char)extractedByte);
                    extractedByte = 0;
                }
            }
        }
        return extractedMessage.ToString();
    }

    static void Main()
    {
        Console.WriteLine("1. Encode Message\n2. Decode Message");
        Console.Write("Choose an option: ");
        int option = int.Parse(Console.ReadLine());

        if (option == 1)
        {
            Console.Write("Enter input image path: ");
            string inputImage = Console.ReadLine();
            Console.Write("Enter output image path: ");
            string outputImage = Console.ReadLine();
            Console.Write("Enter message to encode: ");
            string message = Console.ReadLine();
            EncodeMessage(inputImage, outputImage, message);
        }
        else if (option == 2)
        {
            Console.Write("Enter image path: ");
            string imagePath = Console.ReadLine();
            string hiddenMessage = DecodeMessage(imagePath);
            Console.WriteLine("Decoded Message: " + hiddenMessage);
        }
        else
        {
            Console.WriteLine("Invalid option");
        }
    }
}
