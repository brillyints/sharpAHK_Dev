using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using sharpAHK;
using AHKExpressions;

namespace sharpAHK_Dev
{
    public partial class _Apps
    {

        public class ffMPEG
        {
            _AHK ahk = new _AHK();
            _Lists lst = new _Lists();
            _StatusBar statbar = new _StatusBar();
            _Threads thr = new _Threads();

            public class VideoFile
            {
                private string _Path;
                public string Path
                {
                    get
                    {
                        return _Path;
                    }
                    set
                    {
                        _Path = value;
                    }
                }
                public TimeSpan Duration { get; set; }
                public double BitRate { get; set; }
                public string RawAudioFormat { get; set; }
                public string AudioFormat { get; set; }
                public string RawVideoFormat { get; set; }
                public string VideoFormat { get; set; }
                public int Height { get; set; }
                public int Width { get; set; }
                public int FPS { get; set; }
                public string RawInfo { get; set; }
                public bool infoGathered { get; set; }

                public VideoFile(string path)
                {
                    _Path = path;
                }

            }

            public static string FFmpegPath { get; set; }

            public static TextBox txtRemainingMin { get; set; }

            public static TextBox txtJobTimeDisp { get; set; }

            public static TextBox txtCurrentProgress { get; set; }

            public static PictureBox pictureBox { get; set; }

            public static ProgressBar videoProgress { get; set; }


            // populate statusbar control text
            private void sb(string DisplayText, int Pos = 1, bool opt1 = false, bool op2 = false)
            {
                statbar.StatusBar(DisplayText, Pos);
            }

            private string Params { get; set; }
            private TimeSpan ExtractDuration(string rawInfo)
            {
                TimeSpan t = new TimeSpan(0);
                Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*)", RegexOptions.Compiled);
                Match m = re.Match(rawInfo);

                if (m.Success)
                {
                    string duration = m.Groups[1].Value;
                    string[] timepieces = duration.Split(new char[] { ':', '.' });
                    if (timepieces.Length == 4)
                    {
                        t = new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
                    }
                }

                return t;
            }
            private double ExtractBitrate(string rawInfo)
            {
                Regex re = new Regex("[B|b]itrate:.((\\d|:)*)", RegexOptions.Compiled);
                Match m = re.Match(rawInfo);
                double kb = 0.0;
                if (m.Success)
                {
                    Double.TryParse(m.Groups[1].Value, out kb);
                }
                return kb;
            }
            private string ExtractRawAudioFormat(string rawInfo)
            {
                string a = string.Empty;
                Regex re = new Regex("[A|a]udio:.*", RegexOptions.Compiled);
                Match m = re.Match(rawInfo);
                if (m.Success)
                {
                    a = m.Value;
                }
                return a.Replace("Audio: ", "");
            }
            private string ExtractAudioFormat(string rawAudioFormat)
            {
                string[] parts = rawAudioFormat.Split(new string[] { ", " }, StringSplitOptions.None);
                return parts[0].Replace("Audio: ", "");
            }
            private string ExtractRawVideoFormat(string rawInfo)
            {
                string v = string.Empty;
                Regex re = new Regex("[V|v]ideo:.*", RegexOptions.Compiled);
                Match m = re.Match(rawInfo);
                if (m.Success)
                {
                    v = m.Value;
                }
                return v.Replace("Video: ", ""); ;
            }
            private string ExtractVideoFormat(string rawVideoFormat)
            {
                string[] parts = rawVideoFormat.Split(new string[] { ", " }, StringSplitOptions.None);
                return parts[0].Replace("Video: ", "");
            }
            private int ExtractVideoWidth(string rawInfo)
            {
                int width = 0;
                Regex re = new Regex("(\\d{2,4})x(\\d{2,4})", RegexOptions.Compiled);
                Match m = re.Match(rawInfo);
                if (m.Success)
                {
                    int.TryParse(m.Groups[1].Value, out width);
                }
                return width;
            }
            private int ExtractVideoHeight(string rawInfo)
            {
                int height = 0;
                Regex re = new Regex("(\\d{2,4})x(\\d{2,4})", RegexOptions.Compiled);
                Match m = re.Match(rawInfo);
                if (m.Success)
                {
                    int.TryParse(m.Groups[2].Value, out height);
                }
                return height;
            }


            /// <summary>
            /// FFMpeg Create Preview Images From Video Dir  
            /// </summary>
            /// <param name="VideoDir"></param>
            /// <param name="SaveDir"></param>
            public void VideoScreenCaps(string VideoDir, ProgressBar progressB, bool NewThread = true, string SaveDir = "", int TilesWide = 3, int TilesTall = 3, string Scale = "320:240", string SaveFormat = "jpg", bool OverWriteExisting = false, string CaptureOpt = "1000")
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => VideoScreenCaps(VideoDir, progressB, false, SaveDir, TilesWide, TilesTall, Scale, SaveFormat, OverWriteExisting, CaptureOpt));
                    thr.Start(newThread, "VidThread");
                    //newThread.Start();
                }
                else
                {
                    // string videoFilePath, string saveDir = "", int TilesWide = 3, int TilesTall = 3, string Scale = "320:240", string SaveFormat = "jpg"

                    if (!pathDefined) { pathDefined = FFMpeg_DefinePaths(); if (!pathDefined) { return; } }


                    List<string> files = new List<string>();

                    if (ahk.isFile(VideoDir)) { files.Add(VideoDir); }
                    else { files = lst.FileList(VideoDir, "*.*", true, false, true); }

                    if (progressB.InvokeRequired) { progressB.BeginInvoke((MethodInvoker)delegate () { progressB.Maximum = files.Count(); }); }
                    else { progressB.Maximum = files.Count(); }


                    SaveDir = VideoDir + "\\Previews";

                    Dir_Watch(SaveDir);  // monitor dir for new images and display as created

                    Stopwatch watch = new Stopwatch();

                    int i = 0;
                    foreach (string file in files)
                    {
                        watch.Start();
                        sb(file);

                        i++; sb(i + "/" + files.Count.ToString(), 2);
                        if (progressB.InvokeRequired) { progressB.BeginInvoke((MethodInvoker)delegate () { progressB.Value = i; }); } else { progressB.Value = i; }

                        ahk.DisplayText(ahk.FileName(file) + " | " + i + "/" + files.Count, txtCurrentProgress);

                        // calculate time left until folder completed 

                        // number of files left in dir to vidCap    // reemaining jobs times last job time = time remaining
                        int remaining = files.Count() - i; remaining = remaining * lastJobTime;
                        TimeSpan t = TimeSpan.FromSeconds(remaining);
                        string remains = t.TotalMinutes.ToString();
                        if (txtRemainingMin.InvokeRequired) { txtRemainingMin.BeginInvoke((MethodInvoker)delegate () { txtRemainingMin.Text = remains + " Min Left"; }); } else { txtRemainingMin.Text = remains + " Min Left"; }


                        // check to see if preview image already exists
                        string PreviewImage = SaveDir + "\\" + ahk.FileNameNoExt(file) + "." + SaveFormat;


                        if (!OverWriteExisting)
                        {
                            if (!File.Exists(PreviewImage))
                            {
                                sb("Generating Video Preview Image... " + ahk.FileName(PreviewImage));
                                string previewImg = VidCap_Tiles(file, "", TilesWide, TilesTall, Scale, SaveFormat, CaptureOpt);

                                //if (pictureBox != null) { pictureBox.Image = previewImg.ToImg(); }  // display preview image if viewer visible
                            }
                        }
                        else
                        {
                            ahk.FileDelete(PreviewImage);
                            sb("Generating Video Preview Image... " + ahk.FileName(PreviewImage));
                            string previewImg = VidCap_Tiles(file, "", TilesWide, TilesTall, Scale, SaveFormat, CaptureOpt);

                            //if (pictureBox != null) { pictureBox.Image = previewImg.ToImg(); }  // display preview image if viewer visible
                        }



                        string stopWatchTime = watch.Elapsed.Seconds.ToString(); lastJobTime = watch.Elapsed.Seconds;
                        ahk.DisplayText(stopWatchTime + " seconds", txtJobTimeDisp);
                        watch.Reset();
                    }

                    ahk.OpenDir(SaveDir);
                    ahk.MsgBox("Finished Creating VideoScreenCaps");
                }

            }

            bool ProgressSetup = false;
            public void SetupVideoProgress(int Total = 11)
            {
                ProgressSetup = true;

                if (videoProgress != null)
                {
                    if (videoProgress.InvokeRequired) { videoProgress.BeginInvoke((MethodInvoker)delegate () { videoProgress.Maximum = Total; }); }
                    else { videoProgress.Maximum = Total; }
                }
            }

            public void UpdateVideoProgress(int i = 1)
            {
                if (!ProgressSetup) { SetupVideoProgress(11); }

                if (videoProgress != null)
                {
                    if (videoProgress.InvokeRequired) { videoProgress.BeginInvoke((MethodInvoker)delegate () { videoProgress.Value = i; }); } else { videoProgress.Value = i; }
                }
            }


            // display preview image if viewer visible
            public void DisplayImage(string ImagePath)
            {
                if (pictureBox != null)
                {
                    if (pictureBox.Visible) { pictureBox.Image = ImagePath.ToImg(); }
                }
            }

            private bool pathDefined = false;

            public int lastJobTime;

            // takes tiled screen shot to generate a single preview image - returns path to new preview image (blank if failed)
            public string VidCap_Tiles(string videoFilePath, string saveDir = "", int TilesWide = 3, int TilesTall = 3, string Scale = "320:240", string SaveFormat = "jpg", string CaptureOpt = "1000")
            {
                if (!ahk.isVideo(videoFilePath))
                {
                    sb(videoFilePath + " Not Recognized Video Format... Skipping");
                    return "";
                }

                VideoFile vidInfo = GetVideoInfo(videoFilePath);

                // Extract FPS From video Info returned

                //h264(Main)(avc1 / 0x31637661), yuv420p, 600x450, 750 kb / s, SAR 1:1 DAR 4:3, 29.97 fps, 29.97 tbr, 30k tbn, 59.94 tbc(default)



                int seconds = vidInfo.Duration.TotalSeconds.ToInt();  //getDuration(videoFilePath);
                int egs = TilesWide * TilesTall;  // ex 3 x 3 = 9 segs - dfivide total time by 9 for even timing
                int shotDistance = seconds / egs;

                //TimeSpan t = TimeSpan.FromMilliseconds(shotDistance);
                TimeSpan t = TimeSpan.FromSeconds(shotDistance);

                int MinBetween = t.Minutes;       // # of minutes between tile shots (in video)
                int SecondsBetween = t.Seconds;  // # of seconds between tile shots (in video)
                int distanceBetweenFrames = (seconds * vidInfo.FPS).ToInt();

                //string SaveFormat = "jpg";
                //Scale = "0:0";
                int FrameCount = 1;

                //Scale = "320:240"; 

                //int TileX = 2;
                //int TileY = 3;
                //string Scale = "320:240"; 
                //string[] shots = new string[numShots];

                // default save dir under neath original file
                if (saveDir == "") { saveDir = ahk.FileDir(videoFilePath) + "\\Previews"; }

                string outImage = saveDir + "\\" + ahk.FileNameNoExt(videoFilePath) + "." + SaveFormat;
                if (File.Exists(outImage)) { return ""; }


                outImage = saveDir + "\\" + ahk.FileNameNoExt(videoFilePath) + "_%03d" + SaveFormat;

                ahk.FileCreateDir(saveDir);

                string args = "";
                string opt = CaptureOpt;



                //-i video.mkv -vf scale=500:-1 -t 10 -r 10 image.gif

                // one every 1000 frames, works but didn't alway fill all the boxes
                if (opt == "1000") { args = " -ss 00:" + MinBetween + ":" + SecondsBetween + " -i \"" + videoFilePath + "\" -frames: " + FrameCount + " -vf \"select=not(mod(n\\,1000)),scale=" + Scale + ",tile=" + TilesWide + "x" + TilesTall + "\" \"" + outImage + "\""; }

                // one every 3000 frames - works but SLOWER
                if (opt == "3000") { args = " -ss 00:" + MinBetween + ":" + SecondsBetween + " -i \"" + videoFilePath + "\" -vf \"select=not(mod(n\\,3000)),scale=" + Scale + ",tile=" + TilesWide + "x" + TilesTall + "\" \"" + outImage + "\""; }

                //string args = " -ss 00:" + MinBetween + ":" + SecondsBetween + " -i \"" + videoFilePath + "\" -vf \"-r 0.0033,scale=" + Scale + ",tile=" + TilesWide + "x" + TilesTall + "\" \"" + outImage + "\"";
                //string args = " -ss 00:" + MinBetween + ":" + SecondsBetween + " -i \"" + videoFilePath + "\" -vf \"select=not(mod(n\\,3500)),scale=" + Scale + ",tile=" + TilesWide + "x" + TilesTall + "\" \"" + outImage + "\"";

                // one frame per video (thumbnails no rows)
                if (opt == "1") { args = " -ss 00:" + MinBetween + ":" + SecondsBetween + " -i \"" + videoFilePath + "\" -vf thumbnail,scale=" + Scale + " -frames:v 1 \"" + outImage + "\""; }

                // export ALL action frames (about 1K images in 40min example video)
                if (opt == "ActionFrames") { args = "-i \"" + videoFilePath + "\" -vf \"select = 'eq(pict_type,PICT_TYPE_I)'\" -vsync vfr \"" + saveDir + "\\" + ahk.FileNameNoExt(videoFilePath) + "_%04d." + SaveFormat + "\""; }

                // One screen shot at specific time stamp
                if (opt == "TimeShot") { args = " -i \"" + videoFilePath + "\" -ss 00:02:05.000 -vframes 1 \"" + saveDir + "\\" + ahk.FileNameNoExt(videoFilePath) + "." + SaveFormat + "\""; }

                // mosasic option - works
                if (opt == "Mosasic") { args = " -i \"" + videoFilePath + "\" -vf select='gt(scene\\,0.5)',scale=" + Scale + ",tile -frames:v 5  \"" + outImage + "\""; }


                //args = "-ss 3 -i \"" + videoFilePath + "\" -vf \"select = gt(scene\\, 0.5)\" -frames:v 5 -vsync vfr \"" + outImage + "\"";
                //args = "-i \"" + videoFilePath + "\" -vf  \"select = gt(scene\\, 0.4),scale = 640:360\" -frames:v 5 \"" + outImage + "\"";
                //args = "-i \"" + videoFilePath + "\" -vf fps=1/60 \"" + outImage + "\""; // one image per minute
                //int dur = getDuration(videoFilePath);
                // 2,266 = 37 min video


                string FPS = "";  // number of frames to capture
                try
                {
                    int totalFPS = vidInfo.Duration.TotalSeconds.ToInt() / 10;  // divide total seconds by 10
                    FPS = "1/" + totalFPS.ToString();
                }
                catch { FPS = "1/60"; }  // 1 frame per min



                args = "-i \"" + videoFilePath + "\" -vf fps=" + FPS + " \"" + outImage + "\""; // 


                ahk.FileAppend(videoFilePath, ahk.AppDir() + "\\FFMpeg_Log.txt");
                StartFFMpeg(args);


                if (File.Exists(outImage)) { return outImage; }
                else { return ""; }
            }


            private string StartFFMpeg(string args)
            {
                FFMpeg_DefinePaths();

                string Args = args.Replace(",", "`,");

                string pid = ahk.RunWait(ffmpeg + " " + Args, "", "Hide");

                //string pid = ahk.Run(ffmpeg + " " + Args, "", "Hide");
                //string pid = ahk.Run(ffmpeg + " " + Args);

                //ahk.Run(ffmpeg + " " + Args);

                //_Lists lst = new _Lists();

                //Process runningProc = new Process();
                //List<Process> procs = lst.Process_List();
                //foreach(Process proc in procs)
                //{
                //    if (proc.Id.ToString() == pid)
                //    {
                //        runningProc = proc;
                //        break;
                //    }
                //}

                ////Create the output and streamreader to get the output
                //string output = null; StreamReader srOutput = null;

                ////try the process
                //try
                //{
                //    //run the process
                //    //Process proc = System.Diagnostics.Process.Start(oInfo);

                //    runningProc.WaitForExit();

                //    //get the output
                //    srOutput = runningProc.StandardError;

                //    //now put it in a string
                //    output = srOutput.ReadToEnd();

                //    runningProc.Close();
                //}
                //catch (Exception ex)
                //{
                //    output = string.Empty;
                //    ahk.MsgBox("Exception Caught: " + ex.ToString());
                //}
                //finally
                //{
                //    //now, if we succeded, close out the streamreader
                //    if (srOutput != null)
                //    {
                //        srOutput.Close();
                //        srOutput.Dispose();
                //    }
                //}
                //return output;







                //try
                //{
                //    Process p = new Process();
                //    p.StartInfo.UseShellExecute = false;
                //    p.StartInfo.CreateNoWindow = true;
                //    p.StartInfo.FileName = ffmpeg;
                //    p.StartInfo.RedirectStandardError = true;

                //    //string args = " -ss 00:" + MinBetween + ":" + SecondsBetween + " -i \"" + videoFilePath + "\" -frames " + FrameCount + " -vf \"select=not(mod(n\\,1000)),scale=" + Scale + ",tile=" + TilesWide + "x" + TilesTall + "\" \"" + outImage + "\"";

                //    p.StartInfo.Arguments = args;
                //    p.Start();
                //    p.WaitForExit();
                //    p.Dispose();
                //}
                //catch { }

                return pid;
            }

            ProgressBar progressB = new ProgressBar();
            public string ffmpeg;


            // returns duration in seconds of video file (works)
            public int getDuration(string videoFilePath)
            {
                if (!pathDefined) { pathDefined = FFMpeg_DefinePaths(); if (!pathDefined) { return -1; } }

                // start process that runs in hidden window, grabs command line output
                int duration = 0;
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = ffmpeg;
                p.StartInfo.Arguments = " -i \"" + videoFilePath + "\"";
                string output = null;
                StreamReader sr = null;

                try
                {
                    p.Start();
                    sr = p.StandardError;
                    output = sr.ReadToEnd();
                    string[] hms;
                    Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*).*\n");
                    Match m = re.Match(output);
                    if (m.Success)
                    {
                        hms = m.Groups[1].Value.Split(new char[] { ':', '.' });
                        duration = int.Parse(hms[0]) * 3600 + int.Parse(hms[1]) * 60 + int.Parse(hms[2]);
                    }


                    //string RawInfo = output;
                    //double BitRate = ExtractBitrate(RawInfo);
                    //string RawAudioFormat = ExtractRawAudioFormat(RawInfo);
                    //string AudioFormat = ExtractAudioFormat(RawAudioFormat);
                    //string RawVideoFormat = ExtractRawVideoFormat(RawInfo);
                    //string VideoFormat = ExtractVideoFormat(RawVideoFormat);
                    //int vidWidth = ExtractVideoWidth(RawInfo);
                    //int vidHeight = ExtractVideoHeight(RawInfo);


                    p.WaitForExit(10000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    //throw ex;
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                        sr.Dispose();
                    }
                }
                return duration;
            }

            private bool FFMpeg_DefinePaths()
            {
                //sb("Confirming FFMpeg Resources Exist...");

                if (File.Exists(ahk.AppDir() + "\\ffMpeg\\ffmpeg.exe")) { ffmpeg = ahk.AppDir() + "\\ffMpeg\\ffmpeg.exe"; }

                if (!File.Exists(ffmpeg)) { ahk.MsgBox("ERROR: " + ffmpeg + " NOT FOUND"); return false; }
                else
                {
                    return true;
                }
            }

            public Encoded EncodeVideo(VideoFile input, string encodingCommand, string outputFile, bool getVideoThumbnail)
            {
                Encoded encoded = new Encoded();

                Params = string.Format(@"-i ""{0}"" {1} ""{2}""", input.Path, "-y -b 300k", outputFile);
                string output = FFMPEG(Params);
                encoded.EncodingLog = output;
                encoded.EncodedVideoPath = outputFile;

                if (File.Exists(outputFile))
                {
                    encoded.Success = true;

                    //get thumbnail?
                    if (getVideoThumbnail)
                    {
                        string saveThumbnailTo = outputFile + "_thumb.jpg";

                        if (GetVideoThumbnail(input, saveThumbnailTo))
                        {
                            encoded.ThumbnailPath = saveThumbnailTo;
                        }
                    }
                }
                else
                {
                    encoded.Success = false;
                }

                return encoded;

            }
            public bool GetVideoThumbnail(VideoFile input, string saveThumbnailTo)
            {
                if (!input.infoGathered)
                {
                    GetVideoInfo(input.Path);
                }
                int secs;
                //divide the duration in 3 to get a preview image in the middle of the clip
                //instead of a black image from the beginning.
                secs = (int)Math.Round(TimeSpan.FromTicks(input.Duration.Ticks / 3).TotalSeconds, 0);
                string Params = string.Format("-i {0} {1} -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, saveThumbnailTo, secs);
                string output = FFMPEG(Params);

                if (File.Exists(saveThumbnailTo))
                {
                    return true;
                }
                else
                {
                    //try running again at frame 1 to get something
                    Params = string.Format("-i {0} {1} -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, saveThumbnailTo, 1);
                    output = FFMPEG(Params);

                    if (File.Exists(saveThumbnailTo))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }


            }
            public VideoFile GetVideoInfo(string videoFilePath, bool Hidden = true, int TimeOut = 5000)  // WORKS
            {
                string Params = string.Format("-i \"{0}\"", videoFilePath);

                string output = FFMPEG(Params, Hidden, TimeOut);
                //string output = FFProbe(Params);

                VideoFile input = new VideoFile(videoFilePath);
                input.Path = videoFilePath;
                input.RawInfo = output;
                input.Duration = ExtractDuration(input.RawInfo);
                input.BitRate = ExtractBitrate(input.RawInfo);
                input.RawAudioFormat = ExtractRawAudioFormat(input.RawInfo);
                input.AudioFormat = ExtractAudioFormat(input.RawAudioFormat);
                input.RawVideoFormat = ExtractRawVideoFormat(input.RawInfo);
                input.VideoFormat = ExtractVideoFormat(input.RawVideoFormat);
                input.Width = ExtractVideoWidth(input.RawInfo);
                input.Height = ExtractVideoHeight(input.RawInfo);
                input.infoGathered = true;


                string fpsS = "0"; int fps = 0;
                List<string> segs = ahk.StringSplit_List(output, ",");
                foreach (string seg in segs)
                {
                    if (seg.Contains("fps"))
                    {
                        fpsS = seg.Replace(" fps", "");
                        fpsS = fpsS.Trim();
                        input.FPS = fpsS.ToInt();
                    }
                }

                return input;
            }

            public string FFMPEG(string Parameters, bool Hidden = true, int TimeOut = 10000)  // Launch FFMpeg in Hidden Window
            {
                //string pid = StartFFMpeg(Parameters);


                // define ffmpeg path
                string ffmpeg = "";
                if (File.Exists(ahk.AppDir() + "\\ffMpeg\\ffmpeg.exe")) { ffmpeg = ahk.AppDir() + "\\ffMpeg\\ffmpeg.exe"; }
                else
                {
                    string localPath = @"C:\_Code\LucidProjects\ADBindex\LiveIntheMedia\LiveInTheMedia\bin\Debug\ffmpeg\ffmpeg.exe";
                    if (File.Exists(localPath)) { ffmpeg = localPath; }
                }


                //create a process info
                //ProcessStartInfo oInfo = new ProcessStartInfo(this.FFmpegPath, Parameters);
                ProcessStartInfo oInfo = new ProcessStartInfo(ffmpeg, Parameters);
                oInfo.UseShellExecute = false;

                oInfo.RedirectStandardOutput = true;
                oInfo.RedirectStandardError = true;

                oInfo.UseShellExecute = false;  //testing
                oInfo.CreateNoWindow = Hidden;  //testing


                //Create the output and streamreader to get the output
                string output = null; StreamReader srOutput = null;

                //try the process
                try
                {
                    //run the process
                    Process proc = System.Diagnostics.Process.Start(oInfo);

                    proc.WaitForExit(TimeOut); // 10 seconds timeout

                    //get the output
                    srOutput = proc.StandardError;

                    //now put it in a string
                    output = srOutput.ReadToEnd();

                    proc.Close();
                }
                catch (Exception ex)
                {
                    output = string.Empty;
                    ahk.MsgBox("Exception Caught: " + ex.ToString());
                }
                finally
                {
                    //now, if we succeded, close out the streamreader
                    if (srOutput != null)
                    {
                        srOutput.Close();
                        srOutput.Dispose();
                    }
                }
                return output;
            }

            public string FFProbe(string Parameters)
            {
                //string pid = StartFFMpeg(Parameters);


                // define ffmpeg path
                string ffprobe = "";
                if (File.Exists(ahk.AppDir() + "\\ffMpeg\\ffprobe.exe")) { ffprobe = ahk.AppDir() + "\\ffMpeg\\ffprobe.exe"; }
                //else { if (File.Exists(txtFFMpegPath.Text)) { ffmpeg = txtFFMpegPath.Text; } }


                //create a process info
                ProcessStartInfo oInfo = new ProcessStartInfo(ffprobe, Parameters);
                oInfo.UseShellExecute = false;
                oInfo.RedirectStandardOutput = true;
                oInfo.RedirectStandardError = true;
                oInfo.UseShellExecute = false;  //testing
                oInfo.CreateNoWindow = true;  //testing


                //Create the output and streamreader to get the output
                string output = null; StreamReader srOutput = null;

                //try the process
                try
                {
                    //run the process
                    Process proc = System.Diagnostics.Process.Start(oInfo);

                    proc.WaitForExit();

                    //get the output
                    srOutput = proc.StandardError;

                    //now put it in a string
                    output = srOutput.ReadToEnd();

                    proc.Close();
                }
                catch (Exception ex)
                {
                    output = string.Empty;
                    ahk.MsgBox("Exception Caught: " + ex.ToString());
                }
                finally
                {
                    //now, if we succeded, close out the streamreader
                    if (srOutput != null)
                    {
                        srOutput.Close();
                        srOutput.Dispose();
                    }
                }
                return output;
            }


            public class Encoded
            {
                public string EncodedVideoPath { get; set; }
                public string ThumbnailPath { get; set; }
                public string EncodingLog { get; set; }
                public bool Success { get; set; }
            }


            /*
            # Take a screencap every second
            ./ffmpeg -i jack-slow.mpeg -vf fps = 1 video-caps/cap%d.png

            # Take a screencap every minute
            ./ffmpeg -i jack-slow.mpeg -vf fps = 1 / 60 video-caps/cap%03d.png

            # Take a screencap every ten minutes
            ./ffmpeg -i jack-slow.mpeg -vf fps = 1 / 600 video-caps/cap%04d.png
            */


            public void VideoToGif(string VidPath)
            {
                string args = "-i \"" + VidPath + "\" -vf scale=500:-1 -t 10 -r 10 " + ahk.FileNameNoExt(VidPath) + ".gif";

                StartFFMpeg(args);
            }







            #region === File System Watcher ===

            FileSystemWatcher watcher = new FileSystemWatcher();
            FileSystemWatcher sqliteFiles = new FileSystemWatcher();


            //  Directory Watch - File Change Event Notifications 

            List<string> DirWatch_ModifiedFiles = new List<string>();  // running list of files modified in watch directory

            List<string> DirWatch_CreatedFiles = new List<string>();  // running list of files modified in watch directory

            public void Dir_Watch(string watchDir)  // Start Monitoring Directory for File Changes & Define Event Handler Actions
            {
                try
                {
                    // Create a new FileSystemWatcher and set its properties.
                    watcher = new FileSystemWatcher();
                    watcher.Path = watchDir;

                    // Watch both files and subdirectories.
                    watcher.IncludeSubdirectories = true;

                    // Watch for all changes specified in the NotifyFilters enumeration.
                    watcher.NotifyFilter = NotifyFilters.Attributes |
                    NotifyFilters.CreationTime |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.FileName |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Security |
                    NotifyFilters.Size;

                    // Watch all files.
                    watcher.Filter = "*.*";

                    // Add event handlers.
                    watcher.Changed += new FileSystemEventHandler(DirWatch_OnChanged);
                    watcher.Created += new FileSystemEventHandler(DirWatch_OnChanged);
                    watcher.Deleted += new FileSystemEventHandler(DirWatch_OnChanged);
                    watcher.Renamed += new RenamedEventHandler(DirWatch_OnRenamed);

                    //Start monitoring.
                    watcher.EnableRaisingEvents = true;
                }
                catch (IOException e)
                {
                    Console.WriteLine("A Exception Occurred :" + e);
                }

                catch (Exception oe)
                {
                    Console.WriteLine("An Exception Occurred :" + oe);
                }
            }


            public void DirWatch_OnChanged(object source, FileSystemEventArgs e)  // Event Handler for DirWatch - OnChanged Events
            {
                string ChangeValue = e.ChangeType.ToString();
                if (ChangeValue == "Created")  // file created in watchdir
                {
                    try  //Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);
                    {
                        ((FileSystemWatcher)source).EnableRaisingEvents = false;   // work around for event firing twice

                        DirWatch_CreatedFiles.Add(e.FullPath);

                        DisplayImage(e.FullPath);

                        string baseName = ahk.FileNameNoExt(e.FullPath);
                        string baseNum = ahk.StringSplit(baseName, "_", 0, true, true);
                        int basenum = baseNum.ToInt();
                        UpdateVideoProgress(basenum);


                        sb("Created: " + e.FullPath);
                    }
                    finally { ((FileSystemWatcher)source).EnableRaisingEvents = true; } // work around for event firing twice

                    return;
                }
                if (ChangeValue == "Deleted")  // file deleted from watchdir
                {
                    try  //Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);
                    {
                        ((FileSystemWatcher)source).EnableRaisingEvents = false;   // work around for event firing twice

                        //if (cbLogToSQLite.Checked) { Insert_Into_DirWatch(DbFile, cbDebugMode.Checked, e.FullPath, ahk.FileExt(e.FullPath), "Deleted", "", DateTime.Now.ToString()); }

                        //if (cbAutoRefresh.Checked) { Load_Grid(txtSqlite.Text); }

                        sb("Deleted: " + e.FullPath);
                    }
                    finally { ((FileSystemWatcher)source).EnableRaisingEvents = true; } // work around for event firing twice

                    return;
                }
                if (ChangeValue == "Changed")  // file changed in watchdir
                {
                    try  //Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);
                    {
                        ((FileSystemWatcher)source).EnableRaisingEvents = false;   // work around for event firing twice

                        //if (cbLogToSQLite.Checked) { Insert_Into_DirWatch(DbFile, cbDebugMode.Checked, e.FullPath, ahk.FileExt(e.FullPath), "Modified", "", DateTime.Now.ToString()); }

                        //if (cbAutoRefresh.Checked) { Load_Grid(txtSqlite.Text); }

                        sb("Modified: " + e.FullPath);
                    }
                    finally { ((FileSystemWatcher)source).EnableRaisingEvents = true; } // work around for event firing twice

                    return;
                }
            }


            public void DirWatch_OnRenamed(object source, RenamedEventArgs e) // Event Handler for DirWatch - File Renamed Event
            {
                // Specify what is done when a file is renamed.
                try // Console.WriteLine(" {0} renamed to {1}", e.OldFullPath, e.FullPath);
                {
                    ((FileSystemWatcher)source).EnableRaisingEvents = false;   // work around for event firing twice

                    //if (cbLogToSQLite.Checked) { Insert_Into_DirWatch(DbFile, cbDebugMode.Checked, e.FullPath, ahk.FileExt(e.FullPath), "Renamed", e.OldFullPath, DateTime.Now.ToString()); }

                    //if (cbAutoRefresh.Checked) { Load_Grid(txtSqlite.Text); }

                    sb("Renamed: " + e.OldFullPath + " To " + e.FullPath);
                }
                finally { ((FileSystemWatcher)source).EnableRaisingEvents = true; } // work around for event firing twice
            }


            string gWatchFile;
            public void File_Watch(string watchFile)  // Start Monitoring Directory for Watch File Changes & Define Event Handler Actions
            {
                try
                {
                    gWatchFile = watchFile; // store watch file var globally

                    // Create a new FileSystemWatcher and set its properties.
                    FileSystemWatcher fwatcher = new FileSystemWatcher();

                    string watchDir = ahk.FileDir(watchFile);
                    fwatcher.Path = watchDir;

                    // Watch both files and subdirectories.
                    fwatcher.IncludeSubdirectories = true;

                    // Watch for all changes specified in the NotifyFilters
                    //enumeration.
                    fwatcher.NotifyFilter = NotifyFilters.Attributes |
                    NotifyFilters.CreationTime |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.FileName |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Security |
                    NotifyFilters.Size;

                    // Watch all files.
                    fwatcher.Filter = "*.*";

                    // Add event handlers.
                    fwatcher.Changed += new FileSystemEventHandler(FileWatch_OnChanged);
                    fwatcher.Created += new FileSystemEventHandler(FileWatch_OnChanged);
                    fwatcher.Deleted += new FileSystemEventHandler(FileWatch_OnChanged);
                    fwatcher.Renamed += new RenamedEventHandler(FileWatch_OnRenamed);

                    //Start monitoring.
                    fwatcher.EnableRaisingEvents = true;
                }
                catch (IOException e)
                {
                    Console.WriteLine("A Exception Occurred :" + e);
                }

                catch (Exception oe)
                {
                    Console.WriteLine("An Exception Occurred :" + oe);
                }
            }

            public void FileWatch_OnChanged(object source, FileSystemEventArgs e)  // Event Handler for DirWatch - OnChanged Events
            {
                if (e.FullPath == gWatchFile)  // if file changed is our watch file
                {
                    string ChangeValue = e.ChangeType.ToString();

                    if (ChangeValue == "Created")  // file created in watchdir
                    {
                        Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);

                        try
                        {
                            ((FileSystemWatcher)source).EnableRaisingEvents = false;   // work around for event firing twice
                                                                                       //Insert_Into_DirWatch(Properties.Settings.Default.DbFile, false, e.FullPath, ahk.FileExt(e.FullPath), "Created", "", DateTime.Now.ToString());
                            sb("Created: " + e.FullPath);
                        }
                        finally { ((FileSystemWatcher)source).EnableRaisingEvents = true; } // work around for event firing twice

                        return;
                    }

                    if (ChangeValue == "Deleted")  // file deleted from watchdir
                    {
                        Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);

                        try
                        {
                            ((FileSystemWatcher)source).EnableRaisingEvents = false;   // work around for event firing twice
                                                                                       //Insert_Into_DirWatch(Properties.Settings.Default.DbFile, false, e.FullPath, ahk.FileExt(e.FullPath), "Deleted", "", DateTime.Now.ToString());
                            sb("Deleted: " + e.FullPath);
                        }
                        finally { ((FileSystemWatcher)source).EnableRaisingEvents = true; } // work around for event firing twice


                        return;
                    }
                    if (ChangeValue == "Changed")  // file changed in watchdir
                    {
                        Console.WriteLine("{0}, with path {1} has been {2}", e.Name, e.FullPath, e.ChangeType);

                        try
                        {
                            ((FileSystemWatcher)source).EnableRaisingEvents = false;   // work around for event firing twice
                                                                                       //Insert_Into_DirWatch(Properties.Settings.Default.DbFile, false, e.FullPath, ahk.FileExt(e.FullPath), "Modified", "", DateTime.Now.ToString());
                            sb("Modified: " + e.FullPath);
                        }
                        finally { ((FileSystemWatcher)source).EnableRaisingEvents = true; } // work around for event firing twice

                        return;
                    }
                }

            }

            public void FileWatch_OnRenamed(object source, RenamedEventArgs e) // Event Handler for DirWatch - File Renamed Event
            {
                if (e.OldFullPath == gWatchFile)  // if file changed is our watch file
                {
                    // Specify what is done when a file is renamed.
                    Console.WriteLine(" {0} renamed to {1}", e.OldFullPath, e.FullPath);

                    try
                    {
                        gWatchFile = e.FullPath; // continue to monitor new file name 
                        ((FileSystemWatcher)source).EnableRaisingEvents = false;   // work around for event firing twice
                                                                                   //Insert_Into_DirWatch(Properties.Settings.Default.DbFile, false, e.FullPath, ahk.FileExt(e.FullPath), "Renamed", e.OldFullPath, DateTime.Now.ToString());
                        sb("Renamed: " + e.OldFullPath + " To " + e.FullPath);
                    }
                    finally { ((FileSystemWatcher)source).EnableRaisingEvents = true; } // work around for event firing twice
                }

            }

            #endregion

        }

    }
}
