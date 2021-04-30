using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrafficSimulator
{
    public class OutputData
    {
        public List<string> Output = new List<string>();
        public List<string> NewData = new List<string>();
        public bool NewDataAvailable = false;

        public void PushNewData(string data)
        {
            NewData.Add(data);
            NewDataAvailable = true;
        }

        public List<string> GetNewDataLines()
        {
            List<string> nd = new List<string>();
            foreach (string line in NewData)
            {
                nd.Add(line);
            }
            //
            foreach (string line in NewData)
            {
                Output.Add(line);
            }
            //
            NewData.Clear();
            NewDataAvailable = false;
            //
            return nd;
        }

        public string GetNewData()
        {
            string nd = "";
            foreach (string line in NewData)
            {
                nd += line + "\n";
            }
            //
            foreach (string line in NewData)
            {
                Output.Add(line);
            }
            //
            NewData.Clear();
            NewDataAvailable = false;
            //
            return nd;
        }


    }
}
