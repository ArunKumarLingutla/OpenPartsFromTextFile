using NXOpen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPartsFromTextFile
{
    public class OpenPartsFromTextFile
    {
        //class members
        private static Session theSession = null;
        private static UI theUI = null;

        public static void Main(string[] args)
        {
            try
            {
                theSession = Session.GetSession();
                theUI = UI.GetUI();
                ListingWindow listingWindow=theSession.ListingWindow;
                listingWindow.Open();

                //Shows UI Block styler
                UIFileBrowser uIFileBrowser = new UIFileBrowser();
                uIFileBrowser.Show();

                //Reads the text file and process it
                if (theSession != null && !string.IsNullOrEmpty(Globals.textFilePath))
                {
                    //Closses all the parts before opening parts in text file
                    theSession.Parts.CloseAll(BasePart.CloseModified.CloseModified, null);

                    //Goes through each line of text file
                    foreach (string partPath in AppUtilityFunctions.GetPartsFromTextFile(Globals.textFilePath))
                    {
                        if (!string.IsNullOrEmpty(partPath))
                        {
                            //Gives file name by removing the path and extension
                            string partName = Path.GetFileNameWithoutExtension(partPath);

                            //Gets the status of part wether it is loaded fully partially or minimally or not loaded at all
                            NXOpen.PartLoadState partLoadState = theSession.Parts.GetPartLoadStateOfFileName(partPath);

                            if (partLoadState == PartLoadState.NotLoaded && File.Exists(partPath))
                            {
                                //Loads the part and displays it in NX UI, DisplayPartOption used to open new or replace existing
                                NXOpen.BasePart part = theSession.Parts.OpenActiveDisplay(partPath, DisplayPartOption.AllowAdditional, out _) as NXOpen.BasePart;

                                //Loads the part but wont show it in NX UI
                                //NXOpen.Part part = theSession.Parts.Open(partPath, out _) as NXOpen.Part;

                                listingWindow.WriteLine("part - " + partName + " Is opened");
                                foreach (NXOpen.Features.Feature feature in part.Features)
                                {
                                    //FeatureType will give what type of feature it is like POINT, DATUM_CSYS
                                    //GetFeatureName() will give the namevisible it beside like Point(1), Datum Coordinate System(2)
                                    //Name will give the custom name of that feature which shows in double quotes like "point on base"
                                    listingWindow.WriteLine("feature type- " + feature.FeatureType + " feature name- " + feature.GetFeatureName()+" feature general name- "+ feature.Name.ToString());
                                }
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                theUI.NXMessageBox.Show("main", NXMessageBox.DialogType.Error, ex.Message);
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
