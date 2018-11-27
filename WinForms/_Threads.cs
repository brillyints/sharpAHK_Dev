using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Windows.Forms;
using System.Threading;
using Telerik.WinControls.UI;

namespace sharpAHK_Dev
{
    public class _Threads
    {
        _TelerikLib tel = new _TelerikLib();
        _AHK ahk = new _AHK();

        public void Setup(TelerikTree DispTree)
        {
            ThreadList = new List<Thread>();

            TelTree = DispTree;
        }

        public void Start(Thread thread, string Name = "")
        {
            if (ThreadList == null)
            {
                ThreadList = new List<Thread>();
                //ahk.MsgBox("Must Add \"Setup(); Reference To Initialize ThreadList First");
                //return;
            }

            thread.IsBackground = true;


            if (Name != "") { thread.Name = Name; }
            thread.Start();
            ThreadList.Add(thread);  // Save Thread to List of Running Threads

            TelTreeDisp();
        }

        public static TelerikTree TelTree { get; set; }

        public static List<Thread> ThreadList { get; set; }
        public static Telerik.WinControls.UI.RadTextBox txtThreadStatus { get; set; }
        Dictionary<int, string> ThreadDetails = new Dictionary<int, string>(); // lookup by thread unique id, field to store info per thread to display

        //#region === Threads ===



        //// Checks Thread List for # of Threads Alive

        //public int RunningThreadCount(string State = "Running")

        //{

        //    int Running = 0;

        //    foreach (Thread thread in ThreadList) { if (thread.ThreadState.ToString() == State) { Running++; } }

        //    return Running;

        //}



        //// Loop To Stop All Running Threads

        //public void StopThreads(object sender = null, EventArgs e = null)

        //{

        //    // Loop through list of started threads for current status, update display

        //    if (ThreadList != null)

        //    {

        //        foreach (Thread thread in ThreadList) { if (thread.ThreadState.ToString() == "Running") { thread.Abort(); } }

        //    }



        //    ThreadCleanUp();

        //    UpdateThreadStatusDisp();

        //}



        //public void ThreadCleanUp()

        //{

        //    // reduce memory usage to the level before starting the threads.

        //    GC.Collect();

        //    GC.WaitForPendingFinalizers();

        //    GC.Collect();

        //}



        //public void UpdateThreadStatusDisp(bool RunningOnly = false)

        //{

        //    tree.ClearTree(RelatedTablesTree);



        //    // Loop through list of started threads for current status, update display

        //    if (ThreadList != null)

        //    {

        //        foreach (Thread thread in ThreadList)

        //        {

        //            if (RunningOnly)

        //            {

        //                if (thread.ThreadState.ToString() == "Running")

        //                {

        //                    RadTreeNode Node2 = new RadTreeNode();

        //                    Node2.Text = thread.Name + " (" + thread.ThreadState + ")";

        //                    Node2.Tag = thread.Name;

        //                    tree.AddNode(RelatedTablesTree, Node2);

        //                }

        //            }

        //            else

        //            {

        //                RadTreeNode Node2 = new RadTreeNode();

        //                Node2.Text = thread.Name + " (" + thread.ThreadState + ")";

        //                Node2.Tag = thread.Name;

        //                tree.AddNode(RelatedTablesTree, Node2);

        //            }

        //        }

        //    }

        //}



        //private void radMenuItem7_Click(object sender, EventArgs e)

        //{

        //    StopThreads();

        //    Sb("All Threads Stopped");

        //}



        //private void menuThreadList_Click(object sender, EventArgs e)

        //{

        //    UpdateThreadStatusDisp(false);

        //}





        //#endregion








        // all new threads stored in public list

        //public static List<Thread> ThreadList = new List<Thread>();


        public void ThreadAction(string JobName = "")
        {



        }


        public void StartNewThread(string JobName = "")
        {
            Thread NewThread = new Thread(() => ThreadAction(JobName)); // Function To Execute
            NewThread.IsBackground = true;

            int threadNum = 1; if (ThreadList != null) { threadNum = ThreadList.Count; threadNum++; } // next available thread # to use
            NewThread.Name = JobName + threadNum.ToString(); // unique thread name (jobname + next available num)
            NewThread.Start();

            ThreadList.Add(NewThread);  // Save Thread to List of Running Threads

            UpdateThreadStatusDisp(); // Update Gui with Thread Details
        }


        // Loop To Stop All Running Threads

        public void StopThreads(object sender = null, EventArgs e = null)
        {
            // Loop through list of started threads for current status, update display

            if (ThreadList != null)
            {
                foreach (Thread thread in ThreadList) { if (thread.ThreadState.ToString() == "Running") { thread.Abort(); } }
            }

            UpdateThreadStatusDisp();
        }



        // Checks Thread List for # of Threads Alive

        public int ThreadCount(string State = "Running")
        {

            int Running = 0;

            foreach (Thread thread in ThreadList) { if (thread.ThreadState.ToString() == State) { Running++; } }

            return Running;
        }


        // Check Thread Statuses and Update GUI

        public void UpdateThreadStatusDisp()
        {

            //if (!TeleLog.minimalGui)
            //{

            // Loop through list of started threads for current status, update display

            if (ThreadList != null)
            {
                int threadCount = ThreadCount(); // number of "Running" threads

                string threadStatusMsg = "Running Thread Count : " + threadCount + " | " + DateTime.Now.ToShortDateString();


                // assemble output to display thread status / messages

                foreach (Thread thread in ThreadList)
                {
                    string line = "Name: " + thread.Name + " | State: " + thread.ThreadState.ToString() + " | ID: " + thread.ManagedThreadId;


                    // Check to see if thread details dictionary contains entry for this thread, if so add to display
                    if (ThreadDetails.ContainsKey(thread.ManagedThreadId))
                    {
                        line = line + "\n" + ThreadDetails[thread.ManagedThreadId];
                    }



                    //if (ThreadMsg != "") { line = line + "\n" + ThreadMsg; } // add optional text passed in param



                    if (thread.ThreadState.ToString() != "Aborted")  // add line to display message if not previously aborted
                    {
                        threadStatusMsg = threadStatusMsg + line + "\n";
                    }


                    if (txtThreadStatus != null) { tel.Update(txtThreadStatus, threadStatusMsg); }

                }

            }

            //}

        }

        public void TelTreeDisp()
        {
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();


            if (TelTree != null)
            {

                // Loop through list of started threads for current status, update display

                if (ThreadList != null)
                {
                    int threadCount = ThreadCount(); // number of "Running" threads

                    string threadStatusMsg = "Running Thread Count : " + threadCount + " | " + DateTime.Now.ToShortDateString();


                    // assemble output to display thread status / messages

                    foreach (Thread thread in ThreadList)
                    {
                        string line = "Name: " + thread.Name + " | State: " + thread.ThreadState.ToString() + " | ID: " + thread.ManagedThreadId;


                        // Check to see if thread details dictionary contains entry for this thread, if so add to display
                        if (ThreadDetails.ContainsKey(thread.ManagedThreadId))
                        {
                            line = line + "\n" + ThreadDetails[thread.ManagedThreadId];
                        }



                        //if (ThreadMsg != "") { line = line + "\n" + ThreadMsg; } // add optional text passed in param



                        if (thread.ThreadState.ToString() != "Aborted")  // add line to display message if not previously aborted
                        {
                            threadStatusMsg = threadStatusMsg + line + "\n";
                        }


                        if (txtThreadStatus != null) { tel.Update(txtThreadStatus, threadStatusMsg); }

                        RadTreeNode node = new RadTreeNode();
                        node.Text = line;
                        tree.AddNode(TelTree._RadTREE, node);
                    }

                }

            }

        }

        public void ThreadCleanUp()
        {
            // reduce memory usage to the level before starting the threads.

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }



    }

}
