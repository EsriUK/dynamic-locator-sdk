using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRIUK.DynamicLocators.Core;
using NUnit.Framework;


namespace ESRIUK.DynamicLocators.DynamicLocatorsCoreTests
{

    /// <summary>
    /// This class contains very basic unit tests for all methods that do not have any reliance on external resources.
    /// Some of the methods not tested are MatchAddress, MatchTable and MatchRecordSet
    /// </summary>
    [TestFixture()]
    [ExcludeFromCodeCoverage]
    public class DynamicLocatorsCore_Tests
    {
        // Needed for every test
        private LocatorWrapper locWrapper;

        private void BindRunTime()
        {
            if(ESRI.ArcGIS.RuntimeManager.ActiveRuntime == null)
            {
                // Required to prevent Runtime.InteropServices.COMException
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
            }
        }

        private LocatorWrapper GetlocWrapperInstance()
        {
            if (locWrapper == null)
            {
                locWrapper = new LocatorWrapper();
            }

            return locWrapper;
        }

        [Test()]
        public void TestCore_ReverseGeocode()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            Assert.IsTrue(locWrapper.ReverseGeocode(new Point(),false) != null);
        }



        [Test()]
        public void TestCore_FindAddressCandidates()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            IPropertySet search = new PropertySet();

            Assert.IsTrue(locWrapper.FindAddressCandidates(search) != null);            
        }

        [Test()]
        public void TestCore_Clone()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.Clone() != null);
        }

        [Test()]
        public void TestCore_IsIdentical()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.IsIdentical(locWrapper));
            LocatorWrapper locWrapper2 = new LocatorWrapper();
            locWrapper2.set_DefaultInputFieldNames("Address", (new string[] { "FullAddress", "ADDRESS", "Search" }) as object);
            Assert.IsFalse(locWrapper.IsIdentical(locWrapper2));
        }

        [Test()]
        public void TestCore_GetCategory()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.Category == "Address");
        }

        [Test()]
        public void TestCore_SetCategory()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.Category = "Postcode";
            Assert.IsTrue(locWrapper.Category == "Postcode");
        }

        [Test()]
        public void TestCore_GetDescription()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.Description == "Locator Wrapper");
        }

        [Test()]
        public void TestCore_SetDescription()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.Description = "Unit Test";
            Assert.IsTrue(locWrapper.Description == "Unit Test");
        }

        [Test()]
        public void TestCore_GetName()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.Name == "Locator Wrapper");
        }

        [Test()]
        public void TestCore_SetName()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.Name = "Unit Test";
            Assert.IsTrue(locWrapper.Name == "Unit Test");
        }

        [Test()]
        public void TestCore_UserInterface()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.UserInterface is ESRI.ArcGIS.LocationUI.AddressLocatorUIClass);
        }

        [Test()]
        public void TestCore_get_DefaultInputFieldNames()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.set_DefaultInputFieldNames("Address", (new string[] { "FullAddress", "ADDRESS", "Search" }) as object);
            string[] temp = locWrapper.get_DefaultInputFieldNames("Address") as string[];
            Assert.IsTrue(temp[0] == "FullAddress");
        }

        [Test()]
        public void TestCore_GetAddPercentAlongToMatchFields()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsFalse(locWrapper.AddPercentAlongToMatchFields);
        }

        [Test()]
        public void TestCore_SetAddPercentAlongToMatchFields()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.AddPercentAlongToMatchFields = true;
            Assert.IsTrue(locWrapper.AddPercentAlongToMatchFields);
        }

        [Test()]
        public void TestCore_GetAddReferenceIDToMatchFields()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsFalse(locWrapper.AddReferenceIDToMatchFields);
        }

        [Test()]
        public void TestCore_SetAddReferenceIDToMatchFields()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.AddReferenceIDToMatchFields = true;
            Assert.IsTrue(locWrapper.AddReferenceIDToMatchFields);
        }

        [Test()]
        public void TestCore_GetAddStandardizeStringToMatchFields()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsFalse(locWrapper.AddStandardizeStringToMatchFields);
        }

        [Test()]
        public void TestCore_SetAddStandardizeStringToMatchFields()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.AddStandardizeStringToMatchFields = true;
            Assert.IsTrue(locWrapper.AddStandardizeStringToMatchFields);
        }

        [Test()]
        public void TestCore_GetAddXYCoordsToMatchFields()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsFalse(locWrapper.AddXYCoordsToMatchFields);
        }

        [Test()]
        public void TestCore_SetAddXYCoordsToMatchFields()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.AddXYCoordsToMatchFields = true;
            Assert.IsTrue(locWrapper.AddXYCoordsToMatchFields);
        }

        [Test()]
        public void TestCore_SetAddXYCoordsToMatchFields_Flase()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.AddXYCoordsToMatchFields = false;
            Assert.IsFalse(locWrapper.AddXYCoordsToMatchFields);
        }

        [Test()]
        public void TestCore_GetEndOffset()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.EndOffset == 3);
        }

        [Test()]
        public void TestCore_SetEndOffset()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.EndOffset = 5;
            Assert.IsTrue(locWrapper.EndOffset == 5);
        }

        [Test()]
        public void TestCore_GetIntersectionConnectors()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.IntersectionConnectors == "& | @");
        }

        [Test()]
        public void TestCore_SetIntersectionConnectors()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.IntersectionConnectors = "* & %";
            Assert.IsTrue(locWrapper.IntersectionConnectors == "* & %");
        }

        [Test()]
        public void TestCore_GetMatchIfScoresTie()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.MatchIfScoresTie);
        }

        [Test()]
        public void TestCore_SetMatchIfScoresTie()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.MatchIfScoresTie = false;
            Assert.IsFalse(locWrapper.MatchIfScoresTie);
        }

        [Test()]
        public void TestCore_GetMinimumCandidateScore()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.MinimumCandidateScore == 10);
        }

        [Test()]
        public void TestCore_SetMinimumCandidateScore()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.MinimumCandidateScore = 25;
            Assert.IsTrue(locWrapper.MinimumCandidateScore == 25);
        }

        [Test()]
        public void TestCore_GetMinimumMatchScore()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.MinimumMatchScore == 60);
        }

        [Test()]
        public void TestCore_SetMinimumMatchScore()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.MinimumMatchScore = 25;
            Assert.IsTrue(locWrapper.MinimumMatchScore == 25);
        } 

        [Test()]
        public void TestCore_GetSideOffsetUnits()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.SideOffsetUnits == esriUnits.esriFeet);
        }

        [Test()]
        public void TestCore_SetSideOffsetUnits()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.SideOffsetUnits = esriUnits.esriMeters;
            Assert.IsTrue(locWrapper.SideOffsetUnits == esriUnits.esriMeters);
        }

        [Test()]
        public void TestCore_GetSpellingSensitivity()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.SpellingSensitivity == 80);
        }

        [Test()]
        public void TestCore_SetSpellingSensitivity()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.SpellingSensitivity = 50;
            Assert.IsTrue(locWrapper.SpellingSensitivity == 50);
        }

        [Test()]
        public void TestCore_GetUseRelativePaths()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();

            Assert.IsTrue(locWrapper.UseRelativePaths);
        }

        [Test()]
        public void TestCore_SetUseRelativePaths()
        {
            BindRunTime();
            locWrapper = GetlocWrapperInstance();
            locWrapper.UseRelativePaths = false;
            Assert.IsFalse(locWrapper.UseRelativePaths);
        }

        
    }
}
