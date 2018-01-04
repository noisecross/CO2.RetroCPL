using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class ErrManager
    {
        private bool         bWarnings;
        private bool         bInfos;
        private List<string> errList = new List<string>();
        private List<string> warList = new List<string>();
        private List<string> infList = new List<string>();

        private static ErrManager _uniqueErrManager = null;
        public  static ErrManager Instance
        {
            get
            {
                if (_uniqueErrManager == null)
                    _uniqueErrManager = new ErrManager();
                return _uniqueErrManager;
            }
        }


        public ErrManager()
        {
	        bWarnings = true;
	        bInfos = true;

            if (_uniqueErrManager == null)
                _uniqueErrManager = this;
        }

        public ErrManager(bool bW, bool bI)
        {
	        bWarnings = bW;
	        bInfos = bI;

            if (_uniqueErrManager == null)
                _uniqueErrManager = this;
        }

        public bool existsError()
        {
	        return (errList.Count != 0);
        }

        public void addError(string message)
        {
	        errList.Add("Error: " + message);
        }

        public void addWarning(string message)
        {
	        if (bWarnings) warList.Add("Warn: " + message);
        }

        public void addInfo(string message)
        {
	        if (bInfos) infList.Add("Info: " + message);
        }

        public string toString()
        {
	        string output = string.Empty;

            foreach (string item in errList)
                output += item + Environment.NewLine;

            foreach (string item in warList)
                output += item + Environment.NewLine;

            foreach (string item in infList)
                output += item + Environment.NewLine;

	        return output;
        }
    }
}
