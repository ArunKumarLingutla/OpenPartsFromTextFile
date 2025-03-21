using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpenUI;


namespace OpenPartsFromTextFile
{
    public class AppUtilityFunctions
    {
        public static NXOpen.UI theUI = NXOpen.UI.GetUI();
        public static List<string> GetPartsFromTextFile(string filePath)
        {
            List<string> parts = new List<string>();
            try
            {
                if (filePath!=null)
                {
                    parts = File.ReadAllLines(filePath).ToList();
                }
            }
            catch (Exception ex)
            {
                theUI.NXMessageBox.Show("Getting Parts From text file",NXMessageBox.DialogType.Error,ex.Message);
            }
            return parts;
        }
    }
}
