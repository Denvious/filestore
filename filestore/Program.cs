using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

// Regex
using System.Text.RegularExpressions;

// bzip2 unzip
using ICSharpCode.SharpZipLib.BZip2;

namespace filestore {
    class Program {

        // Folder dialogue must be run on main thread
        [STAThread]
        static void Main(string[] args) {

            // Configuration section

            string startFormat = "Directory Name";
            string endFormat = "\"";

            Console.WriteLine("Please specify a folder path that contains bz or xml filelist documents");
            string filepath = Console.ReadLine();

            // Create a new explorer process for selecting folder
            Process explorerProcess = new Process();

            // Get current user's username directory and starts
            string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Using statement explicitly ensures that the instance is disposed of properly. 
            using (FolderBrowserDialog fileDialogue = new FolderBrowserDialog()) {

                // Saves the dialogue in an DialogResult instance
                DialogResult result = fileDialogue.ShowDialog();

                // Saves selected directory, and files in directory
                string fileDirectory = fileDialogue.SelectedPath.ToString();
                string[] fileArray = Directory.GetFiles(fileDialogue.SelectedPath);

                // TEMP, string of entire file
                string files = string.Join(" ", fileArray);

                foreach(string file in fileArray) {

                    // Stores the file in bzis
                    BZip2InputStream bzis = new BZip2InputStream(File.OpenRead(file));

                    // Create file at directory + txt
                    String fileName = fileDirectory + "\\temp.txt";
                    FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                    bzis.CopyTo(fileStream);
                    
                    String fileWrite = fileDirectory + "\\finalList.txt";
                    
                    fileStream.Close();

                    using (StreamReader sr = File.OpenText(fileName)) {

                        String line;
                        while ((line = sr.ReadLine()) != null) {

                            Regex reg = new Regex("\\B(<Directory Name=)\\B\".*?\"");
                            MatchCollection matches = reg.Matches(line);

                            // Count to see whether the directory is a subchild
                            int i = 0;
                            using (StreamWriter swr = new StreamWriter(fileWrite,  true)) {
                                foreach (var item in matches) {
                                    swr.WriteLine(item.ToString());
                                }
                            }
                        }
                    }

                    // Close
                    bzis.Close();

                    // Creates a new byte buffer, and stores the contents of the bzip into the buffer
                    //byte[] byteBuffer = new byte[bzis.Length];
                    //bzis.Read(byteBuffer, 0, byteBuffer.Length);

                    // Close the stream as we dont need it. 
                    //bzis.Close();

                    /*

                    // Encode into string before returning 
                    string tempBZString = bString.ToString();

                    Regex reg = new Regex("\".*?\"");
                    MatchCollection matches = reg.Matches(tempBZString);

                    using (StreamWriter streamWriterFile = new StreamWriter(fileDirectory + "\\temp.txt")) {
                        foreach (var item in matches) {
                            streamWriterFile.WriteLine(item.ToString());
                        }
                    }

                    */
                }
            }
            
            // Prevents the console from closing automatically, by inserting breakpoint
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
