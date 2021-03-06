﻿/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: ErrManager.cs                      |
* | v1.00, January 2018                      |
* | Author: Emilio Arango Delgado de Mendoza |
* |------------------------------------------|
*/
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

        private static ErrManager _ErrManagerSingleton = null;
        public  static ErrManager Instance
        {
            get
            {
                if (_ErrManagerSingleton == null)
                    _ErrManagerSingleton = new ErrManager();
                return _ErrManagerSingleton;
            }
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public ErrManager()
        {
	        bWarnings = true;
	        bInfos = true;

            if (_ErrManagerSingleton == null)
                _ErrManagerSingleton = this;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="bW"> Warning messages are (not) managed.</param>
        /// <param name="bI">Info messages are (not) managed.</param>
        public ErrManager(bool bW, bool bI)
        {
	        bWarnings = bW;
	        bInfos = bI;

            if (_ErrManagerSingleton == null)
                _ErrManagerSingleton = this;
        }

        /// <summary>
        /// Returns a value which indicates if an error have been found.
        /// </summary>
        /// <returns>Value which indicates if an error have been found.</returns>
        public bool existsError()
        {
	        return (errList.Count != 0);
        }

        /// <summary>
        /// An error message is stored into the manager.
        /// </summary>
        /// <param name="message">Message to add.</param>
        public void addError(string message)
        {
	        errList.Add("Error: " + message);
        }

        /// <summary>
        /// If warnings allowed, a warning message is stored into the manager.
        /// </summary>
        /// <param name="message">Message to add.</param>
        public void addWarning(string message)
        {
	        if (bWarnings) warList.Add("Warn: " + message);
        }

        /// <summary>
        /// If info messages allowed, a warning message is stored into the manager.
        /// </summary>
        /// <param name="message">Message to add.</param>
        public void addInfo(string message)
        {
	        if (bInfos) infList.Add("Info: " + message);
        }

        /// <summary>
        /// Get all the erroneous info found and stored into the manager.
        /// </summary>
        /// <returns>A formatted string containing all the relevant stored messages.</returns>
        public string errorsToString()
        {
	        string output = string.Empty;

            foreach (string item in errList)
                output += item + Environment.NewLine;

	        return output;
        }
        public string warningsToString()
        {
            string output = string.Empty;

            foreach (string item in warList)
                output += item + Environment.NewLine;

            return output;
        }
        public string infosToString()
        {
            string output = string.Empty;

            foreach (string item in infList)
                output += item + Environment.NewLine;

            return output;
        }

        /// <summary>
        ///  Displays all the relevant messages found and stored into the manager.
        /// </summary>
        public void printStatus()
        {
            string output = string.Empty;

            output = infosToString();
            if(!string.IsNullOrEmpty(output))
                Common.WriteLineConsoleColoured(output, ConsoleColor.Blue);

            output = warningsToString();
            if (!string.IsNullOrEmpty(output))
                Common.WriteLineConsoleColoured(output, ConsoleColor.Yellow);

            output = errorsToString();
            if (!string.IsNullOrEmpty(output))
                Common.WriteLineConsoleColoured(output, ConsoleColor.Red);
        }
    }
}
