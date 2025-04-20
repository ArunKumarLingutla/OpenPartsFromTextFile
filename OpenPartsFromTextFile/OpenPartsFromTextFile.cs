using NXOpen;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.BodyDes.OnestepUnformBuilder;

namespace OpenPartsFromTextFile
{
    public class OpenPartsFromTextFile
    {
        //class members
        private static Session theSession = null;
        private static UFSession theUFSession = null;
        private static UI theUI = null;
        private static List<string> unLoadedParts;
        private static int exitCode;
        static ListingWindow listingWindow;
        public static void Main(string[] args)
        {
            try
            {
                theSession = Session.GetSession();
                theUFSession=UFSession.GetUFSession();
                theUI = UI.GetUI();
                listingWindow=theSession.ListingWindow;
                listingWindow.Open();
                unLoadedParts = new List<string>();
                

                //Shows UI Block styler
                UIFileBrowser uiFileBrowser = new UIFileBrowser();
                uiFileBrowser.Show();

                //Check wether NX is TeamCenter integrated or Native NX
                theUFSession.UF.IsUgmanagerActive(out bool isUgmanagerActive);

                //If we need to open parts from local machine, Reads the text file and process it
                if (!string.IsNullOrEmpty(Globals.TextFilePath))
                {
                    //Closses all the parts, this will some times throw an exception, when the parts are open in background..
                    //theSession.Parts.CloseAll(BasePart.CloseModified.CloseModified, null);

                    //Goes through each line of text file
                    foreach (string partPath in AppUtilityFunctions.GetPartsFromTextFile(Globals.TextFilePath))
                    {
                        if (!string.IsNullOrEmpty(partPath))
                        {
                            //Gives file name by removing the path and extension
                            string partName = Path.GetFileNameWithoutExtension(partPath);

                            //Gets the status of part wether it is loaded fully partially or minimally or not loaded at all
                            //NXOpen.PartLoadState partLoadState = theSession.Parts.GetPartLoadStateOfFileName(partPath);

                            NXOpen.PartLoadStatus partLoadStatus=AppUtilityFunctions.OpenPart(partPath);
                            if (partLoadStatus!=null)
                            {
                                //Perform the required operation on the opened part
                                foreach (NXOpen.Features.Feature feature in theSession.Parts.Display.Features)
                                {
                                    //FeatureType will give what type of feature it is like POINT, DATUM_CSYS
                                    //GetFeatureName() will give the namevisible it beside like Point(1), Datum Coordinate System(2)
                                    //Name will give the custom name of that feature which shows in double quotes like "point on base"
                                    listingWindow.WriteLine("feature type- " + feature.FeatureType + " feature name- " + feature.GetFeatureName() + " feature general name- " + feature.Name.ToString());
                                }
                            }
                            else
                            {
                                unLoadedParts.Add(partName);
                                continue;
                            }
                        }
                    }
                }

                
                if (unLoadedParts.Count > 0)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    listingWindow.WriteLine("============================================================================================");
                    listingWindow.WriteLine("List of Unloaded Parts");
                    foreach (string unloadedPart in unLoadedParts)
                    {
                        listingWindow.WriteLine(unloadedPart);
                        sb.AppendLine(unloadedPart);
                    }
                    theUI.NXMessageBox.Show("Unloaded Parts",NXMessageBox.DialogType.Information,sb.ToString());
                }
            }
            catch (Exception ex)
            {
                theUI.NXMessageBox.Show("main", NXMessageBox.DialogType.Error, ex.Message);
            }
            finally 
            { 
                Environment.ExitCode=exitCode;
                listingWindow.Close();
                listingWindow = null;
                
            }
        }

        public static int GetUnloadOption(string arg)
        {
            //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
            return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }

        //------------------------------------------------------------------------------
        // Following method cleanup any housekeeping chores that may be needed.
        // This method is automatically called by NX.
        //------------------------------------------------------------------------------
        public static void UnloadLibrary(string arg)
        {
            try
            {
                //---- Enter your code here -----
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }
    }
}
