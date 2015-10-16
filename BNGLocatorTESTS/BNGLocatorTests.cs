using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRIUK.DynamicLocators.BNGLocator;
using ESRIUK.DynamicLocators.Core;
using NUnit.Framework;

namespace ESRIUK.DynamicLocators.BNGLocatorTests
{
    [TestFixture()]
    [ExcludeFromCodeCoverage]
    public class BNGLocatorTests
    {
        private void BindRunTime()
        {
            if (ESRI.ArcGIS.RuntimeManager.ActiveRuntime == null)
            {
                // Required to prevent Runtime.InteropServices.COMException
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
            }
        }

        [Test()]
        [TestCase("SE509582")]
        [TestCase("HT509582")]
        [TestCase("-3.910277 55.357446")]
        [TestCase("HT50915822NE")]
        public void TestFindAddressCandidatesCanMatchAddress(string address)
        {
            BindRunTime();

            // Get the input from the IPropertySet 
            object names = null;
            object values = null; 

            IPropertySet addressObj = new PropertySetClass();
            names = new string[] { "Single Line Address"};
            // Get centre point of Envelope for geocode location
            // Workaround for currnet non Point geometry issue
            values = new object[] { address };

            addressObj.SetProperties(names, values);

            BNG_Locator bng = new BNG_Locator();
            IArray matches = bng.FindAddressCandidates(addressObj);
            Assert.IsTrue(matches != null);
        }

        [Test()]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("sdjsjd")]
        public void TestFindAddressCandidatesCannotMatchAddress(string address)
        {
            BindRunTime();

            // Get the input from the IPropertySet 
            object names = null;
            object values = null;

            IPropertySet addressObj = new PropertySetClass();
            names = new string[] { "Single Line Address" };
            // Get centre point of Envelope for geocode location
            // Workaround for currnet non Point geometry issue
            values = new object[] { address };

            addressObj.SetProperties(names, values);

            BNG_Locator bng = new BNG_Locator();
            IArray matches = bng.FindAddressCandidates(addressObj);
            Assert.IsTrue(matches.Count == 0);
        }

        [Test()]
        public void TestFindAddressCandidatesWithEmptyInput()
        {
            BindRunTime();



            IPropertySet addressObj = new PropertySetClass();

            BNG_Locator bng = new BNG_Locator();
            IArray matches = bng.FindAddressCandidates(addressObj);
            Assert.IsTrue(matches.Count == 0);
        }
    }
}
