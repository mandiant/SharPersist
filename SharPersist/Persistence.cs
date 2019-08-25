using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharPersist
{
    public class Persistence
    {

        private string persistMethod = "";
        private string command = "";
        private string commandArg = "";
        private string filePath = "";
        private string theKey = "";
        private string theVal = "";
        private string theName = "";
        private string status = "";
        private string option = "";

        // default constructor
        public Persistence()
        {

        }
        
        // constructor
        public Persistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {
            this.persistMethod = persistMethod;
            this.command = command;
            this.commandArg = commandArg;
            this.theKey = theKey;
            this.theVal = theVal;
            this.theName = theName;
            this.filePath = filePath;
            this.status = status;
            this.option = option;

        }


        // getters
        public string getPersistenceMethod()
        {
            return persistMethod;
        }

        public string getCommand()
        {
            return command;

        }

        public string getCommandArg()
        {
            return commandArg;

        }

        public string getTheKey()
        {
            return theKey; ;

        }

        public string getTheVal()
        {
            return theVal;

        }

        public string getTheName()
        {
            return theName;

        }

        public string getFilePath()
        {
            return filePath;

        }

        public string getStatus()
        {
            return status;


        }

        public string getOption()
        {
            return option;


        }

        // setters
        public void setPersistenceMethod(string persistMethod)
        {
            this.persistMethod = persistMethod;
        }

        public void setCommand(string command)
        {
            this.command = command;

        }

        public void setCommandArg(string commandArg)
        {
            this.commandArg = commandArg;

        }

        public void setTheKey(string theKey)
        {
            this.theKey = theKey;

        }

        public void setTheVal(string theVal)
        {
            this.theVal = theVal;

        }

        public void setTheName(string theName)
        {
            this.theName = theName;

        }

        public void setFilePath(string filePath)
        {
            this.filePath = filePath;

        }

        public void setStatus(string status)
        {
            this.status = status;
        }

        public void setOption(string option)
        {
            this.option = option;
        }


    } // end class 


} // end persistence
