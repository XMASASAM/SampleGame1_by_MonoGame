using System;
using System.Collections.Generic;
using System.Text;

namespace TextRead
{
    public class Text
    {
        string[] data;

        public Text(string[] data)
        {
            this.data = data;
        }


        public string[] GetStrings()
        {
            return data;
        }

        public string GetAt(int point)
        {
            return data[point];
        }

        public override string ToString()
        {
            string s="";
            foreach (string i in data) s += i + ";";
            return s;
        }
    }
}
