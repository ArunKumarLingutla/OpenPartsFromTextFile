using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NXOpen;
using NXOpenUI;
using System.Runtime.CompilerServices;
using NXOpen.UF;


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

        public static NXOpen.PartLoadStatus OpenPart(string partName)
        {
            NXOpen.UF.UFSession theUFSession = NXOpen.UF.UFSession.GetUFSession();
            string fileName=partName;
            NXOpen.PartLoadStatus partLoadStatus=null;

            theUFSession.UF.IsUgmanagerActive(out bool isUgmanagerActive);
            if (isUgmanagerActive)
            {
                string[] SlashSplit= partName.Split('/');
                NXOpen.Tag tag; theUFSession.Ugmgr.AskPartTag(SlashSplit[0],out tag);
                theUFSession.Ugmgr.ListPartRevisions(tag, out int noOfRev, out NXOpen.Tag[] revArray);
                theUFSession.Ugmgr.AskPartRevisionId(revArray[revArray.Length - 1], out string revId);
                fileName = "@DB/" + SlashSplit[0] + "/" + revId;
                //fileName = "@DB/" + partName;
            }

            try
            {
                NXOpen.BasePart part = NXOpen.Session.GetSession().Parts.FindObject(fileName);
                if (part != null) 
                {
                    NXOpen.Session.GetSession().Parts.SetActiveDisplay(part,NXOpen.DisplayPartOption.AllowAdditional,NXOpen.PartDisplayPartWorkPartOption.SameAsDisplay,out partLoadStatus);
                }
            }
            catch (Exception)
            {
                try
                {
                    NXOpen.BasePart part = NXOpen.Session.GetSession().Parts.OpenActiveDisplay(fileName,NXOpen.DisplayPartOption.AllowAdditional,out partLoadStatus);

                    NXOpen.UF.PartLoadState partLoadState;  
                    theUFSession.UF.AskLoadStateForPartFile(fileName, out partLoadState);
                    if (partLoadState==NXOpen.UF.PartLoadState.PartNotLoaded)
                    {
                        partLoadStatus = null;
                    }
                }
                catch (Exception)
                {
                    partLoadStatus=null;
                }
            }

            return partLoadStatus;
        }
    }
}
