using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;

namespace sharpAHK_Dev
{
    public partial class _Web
    {

        public class FTP
        {
            private static _AHK ahk = new sharpAHK._AHK();
            private static _Lists lst = new _Lists();

            // source: https://www.codeproject.com/Tips/443588/Simple-Csharp-FTP-Class
            // credit: metastruct

            private string host = null;
            private string user = null;
            private string pass = null;
            private FtpWebRequest ftpRequest = null;
            private FtpWebResponse ftpResponse = null;
            private Stream ftpStream = null;
            private int bufferSize = 2048;

            /* Construct Object */
            public void ftp(string hostIP, string userName, string password) { host = hostIP; user = userName; pass = password; }

            /* Download File */
            public void download(string remoteFile, string localFile)
            {
                try
                {
                    /* Create an FTP Request */
                    ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    /* When in doubt, use these options */
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = true;
                    ftpRequest.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    /* Establish Return Communication with the FTP Server */
                    ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    /* Get the FTP Server's Response Stream */
                    ftpStream = ftpResponse.GetResponseStream();
                    /* Open a File Stream to Write the Downloaded File */
                    FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                    /* Buffer for the Downloaded Data */
                    byte[] byteBuffer = new byte[bufferSize];
                    int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                    try
                    {
                        while (bytesRead > 0)
                        {
                            localFileStream.Write(byteBuffer, 0, bytesRead);
                            bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    /* Resource Cleanup */
                    localFileStream.Close();
                    ftpStream.Close();
                    ftpResponse.Close();
                    ftpRequest = null;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                return;
            }

            /* Upload File */
            public bool upload(string remoteFile = "", string localFile = "")
            {
                //sharpAHK_Dev._Web.FTP ftp = new sharpAHK_Dev._Web.FTP();
                //ftp.ftp("ftp://88.88.188.115/", "sharpUser", "sharpPass");
                //List<string> dirList = ftp.directoryListSimple("Projects/");
                //List<string> dirList = ftp.directoryListDetailed("Projects/");
                //ftp.upload("Projects/sharpAHK_Dll_2017_03_24.zip", @"C:\_Code\sharpAHK_Dll\webTFS\webTFS\bin\Debug\ProjectZips\sharpAHK_Dll_2017_03_24.zip");
                //ftp.upload("Projects/sharpAHK_Examples_2017_03_24.zip", @"C:\_Code\sharpAHK_Dll\webTFS\webTFS\bin\Debug\ProjectZips\sharpAHK_Examples_2017_03_24.zip");



                // Get the object used to communicate with the server.  
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://88.88.188.115/Projects/" + localFile);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.  
                request.Credentials = new NetworkCredential("sharpUser", "sharpPass");

                // Copy the contents of the file to the request stream.  
                StreamReader sourceStream = new StreamReader(@"C:\_Code\sharpAHK_Dll\" + localFile);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

                response.Close();

                return true;

                //try
                //{
                //    /* Create an FTP Request */
                //    ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                //    /* Log in to the FTP Server with the User Name and Password Provided */
                //    ftpRequest.Credentials = new NetworkCredential(user, pass);
                //    /* When in doubt, use these options */
                //    ftpRequest.UseBinary = true;
                //    ftpRequest.UsePassive = true;
                //    ftpRequest.KeepAlive = true;
                //    /* Specify the Type of FTP Request */
                //    ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                //    /* Establish Return Communication with the FTP Server */
                //    ftpStream = ftpRequest.GetRequestStream();
                //    /* Open a File Stream to Read the File for Upload */
                //    //FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                //    var localFileStream = new FileStream(localFile, FileMode.Open);
                //    /* Buffer for the Downloaded Data */
                //    byte[] byteBuffer = new byte[bufferSize];
                //    int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                //    /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                //    try
                //    {
                //        while (bytesSent != 0)
                //        {
                //            ftpStream.Write(byteBuffer, 0, bytesSent);
                //            bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        ahk.MsgBox(ex.ToString()); 
                //        Console.WriteLine(ex.ToString());
                //        return false; 
                //    }
                //    /* Resource Cleanup */
                //    localFileStream.Close();
                //    ftpStream.Close();
                //    ftpRequest = null;
                //    return true; 
                //}
                //catch (Exception ex)
                //{
                //    ahk.MsgBox(ex.ToString());
                //    Console.WriteLine(ex.ToString());
                //    return false; 
                //}
                //return true; 
            }

            /* Delete File */
            public void delete(string deleteFile)
            {
                try
                {
                    /* Create an FTP Request */
                    ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + deleteFile);
                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    /* When in doubt, use these options */
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = true;
                    ftpRequest.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                    /* Establish Return Communication with the FTP Server */
                    ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    /* Resource Cleanup */
                    ftpResponse.Close();
                    ftpRequest = null;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                return;
            }

            /* Rename File */
            public void rename(string currentFileNameAndPath, string newFileName)
            {
                try
                {
                    /* Create an FTP Request */
                    ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + currentFileNameAndPath);
                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    /* When in doubt, use these options */
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = true;
                    ftpRequest.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                    /* Rename the File */
                    ftpRequest.RenameTo = newFileName;
                    /* Establish Return Communication with the FTP Server */
                    ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    /* Resource Cleanup */
                    ftpResponse.Close();
                    ftpRequest = null;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                return;
            }

            /* Create a New Directory on the FTP Server */
            public void createDirectory(string newDirectory)
            {
                try
                {
                    /* Create an FTP Request */
                    ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + newDirectory);
                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    /* When in doubt, use these options */
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = true;
                    ftpRequest.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                    /* Establish Return Communication with the FTP Server */
                    ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    /* Resource Cleanup */
                    ftpResponse.Close();
                    ftpRequest = null;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                return;
            }

            /* Get the Date/Time a File was Created */
            public string getFileCreatedDateTime(string fileName)
            {
                try
                {
                    /* Create an FTP Request */
                    ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName);
                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    /* When in doubt, use these options */
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = true;
                    ftpRequest.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                    /* Establish Return Communication with the FTP Server */
                    ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    /* Establish Return Communication with the FTP Server */
                    ftpStream = ftpResponse.GetResponseStream();
                    /* Get the FTP Server's Response Stream */
                    StreamReader ftpReader = new StreamReader(ftpStream);
                    /* Store the Raw Response */
                    string fileInfo = null;
                    /* Read the Full Response Stream */
                    try { fileInfo = ftpReader.ReadToEnd(); }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    /* Resource Cleanup */
                    ftpReader.Close();
                    ftpStream.Close();
                    ftpResponse.Close();
                    ftpRequest = null;
                    /* Return File Created Date Time */
                    return fileInfo;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Return an Empty string Array if an Exception Occurs */
                return "";
            }

            /* Get the Size of a File */
            public string getFileSize(string fileName)
            {
                try
                {
                    /* Create an FTP Request */
                    ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName);
                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    /* When in doubt, use these options */
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = true;
                    ftpRequest.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                    /* Establish Return Communication with the FTP Server */
                    ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    /* Establish Return Communication with the FTP Server */
                    ftpStream = ftpResponse.GetResponseStream();
                    /* Get the FTP Server's Response Stream */
                    StreamReader ftpReader = new StreamReader(ftpStream);
                    /* Store the Raw Response */
                    string fileInfo = null;
                    /* Read the Full Response Stream */
                    try { while (ftpReader.Peek() != -1) { fileInfo = ftpReader.ReadToEnd(); } }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    /* Resource Cleanup */
                    ftpReader.Close();
                    ftpStream.Close();
                    ftpResponse.Close();
                    ftpRequest = null;
                    /* Return File Size */
                    return fileInfo;
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Return an Empty string Array if an Exception Occurs */
                return "";
            }

            /* List Directory Contents File/Folder Name Only */
            public List<string> directoryListSimple(string directory)
            {
                try
                {
                    /* Create an FTP Request */
                    ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    /* When in doubt, use these options */
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = true;
                    ftpRequest.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                    /* Establish Return Communication with the FTP Server */
                    ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    /* Establish Return Communication with the FTP Server */
                    ftpStream = ftpResponse.GetResponseStream();
                    /* Get the FTP Server's Response Stream */
                    StreamReader ftpReader = new StreamReader(ftpStream);
                    /* Store the Raw Response */
                    string directoryRaw = null;
                    /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                    try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    /* Resource Cleanup */
                    ftpReader.Close();
                    ftpStream.Close();
                    ftpResponse.Close();
                    ftpRequest = null;
                    /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                    try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList.ToList(); }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Return an Empty string Array if an Exception Occurs */
                return new List<string>();
            }

            /* List Directory Contents in Detail (Name, Size, Created, etc.) */
            public List<string> directoryListDetailed(string directory)
            {
                try
                {
                    /* Create an FTP Request */
                    ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    /* When in doubt, use these options */
                    ftpRequest.UseBinary = true;
                    ftpRequest.UsePassive = true;
                    ftpRequest.KeepAlive = true;
                    /* Specify the Type of FTP Request */
                    ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    /* Establish Return Communication with the FTP Server */
                    ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    /* Establish Return Communication with the FTP Server */
                    ftpStream = ftpResponse.GetResponseStream();
                    /* Get the FTP Server's Response Stream */
                    StreamReader ftpReader = new StreamReader(ftpStream);
                    /* Store the Raw Response */
                    string directoryRaw = null;
                    /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                    try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                    /* Resource Cleanup */
                    ftpReader.Close();
                    ftpStream.Close();
                    ftpResponse.Close();
                    ftpRequest = null;
                    /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                    try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList.ToList(); }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Return an Empty string Array if an Exception Occurs */
                return new List<string>();
            }

        }


    }
}
