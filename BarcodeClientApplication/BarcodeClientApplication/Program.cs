using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Configuration;

namespace BarcodeClientApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            string barcodeFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Barcodes");

            while (true)
            {
                try
                {
                    // List images in the Barcodes folder
                    string[] imageFiles = Directory.GetFiles(barcodeFolderPath, "*.png"); // Add other image extensions if needed
                    if (imageFiles.Length == 0)
                    {
                        Console.WriteLine("No images found in the Barcodes folder.");
                        break;
                    }

                    for (int i = 0; i < imageFiles.Length; i++)
                    {
                        Console.WriteLine($"{i + 1}. {Path.GetFileName(imageFiles[i])}");
                    }

                    // User selects an image
                    Console.WriteLine("Enter the number of the image to process (or 'exit' to quit):");
                    string input = Console.ReadLine();
                    if (input.ToLower() == "exit")
                    {
                        break;
                    }

                    if (!int.TryParse(input, out int imageIndex) || imageIndex < 1 || imageIndex > imageFiles.Length)
                    {
                        Console.WriteLine("Invalid selection. Please try again.");
                        continue;
                    }

                    string selectedImagePath = imageFiles[imageIndex - 1];

                    // Connect to server and process image
                    using (TcpClient client = new TcpClient(serverIP, serverPort))
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        writer.WriteLine(selectedImagePath);
                        writer.Flush();

                        string response = reader.ReadLine();
                        Console.WriteLine("Response from server: " + response);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine("\nPress any key to continue or type 'exit' to quit.");
                if (Console.ReadLine().ToLower() == "exit")
                {
                    break;
                }
            }
        }
    }
}

