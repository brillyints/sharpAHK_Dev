using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using static sharpAHK_Dev._Apps.Mpc;

namespace sharpAHK_Dev
{
    public static class Dev_Expressions
    {
        


        public static string MpcAction(this mpcWin MPCWIN, mpcActions action)
        {
            string ReturnVal = "";

            _Apps.Mpc mpc = new _Apps.Mpc();

            MPCWIN.mpcID = "ahk_PID " + MPCWIN.PID; // define mpcID value

            mpc.MpcAction(MPCWIN.mpcID, action);


            return ReturnVal;
        }



    }
}
