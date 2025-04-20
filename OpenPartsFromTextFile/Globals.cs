using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPartsFromTextFile
{
    public class Globals
    {
        private static string _textFilePath;
        public static string TextFilePath
        {
            get { return _textFilePath; }
            set { _textFilePath = value; }
        }
        
        private static bool _isMultiPart;
        public static bool IsMultiPart
        {
            get { return _isMultiPart; }
            set { _isMultiPart = value; }
        }

        //public static string TextFilePath=string.Empty;
    }
}
