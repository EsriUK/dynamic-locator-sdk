using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRIUK.DynamicLocators.BNGLocator;
using NUnit.Framework;

namespace ESRIUK.DynamicLocators.BNGLocatorTests
{
    [TestFixture()]
    [ExcludeFromCodeCoverage]
    public class NGTranslateTests
    {
        private IProjectedCoordinateSystem bngSRef;
        private IGeographicCoordinateSystem wgs84SRef;

        private void BindRunTime()
        {
            if (ESRI.ArcGIS.RuntimeManager.ActiveRuntime == null)
            {
                // Required to prevent Runtime.InteropServices.COMException
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
                bngSRef = new SpatialReferenceEnvironmentClass().CreateProjectedCoordinateSystem((int)esriSRProjCSType.esriSRProjCS_BritishNationalGrid);
                wgs84SRef = new SpatialReferenceEnvironmentClass().CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);            
            }
        }

        [Test()]
        [TestCase("TQ00000000SE", 500005.0, 100000.0)]
        [TestCase("NX509582", 250900.0, 558200.0)]
        [TestCase("TQ509582", 550900.0, 158200.0)]
        [TestCase("TQ0000NW", 500000.0, 100500.0)]
        [TestCase("TQ0000NE", 500500.0, 100500.0)]
        [TestCase("TQ0000SE", 500500.0, 100000.0)]
        [TestCase("NX00NW", 200000.0, 502500.0)]
        [TestCase("TL44", 540000.0, 240000.0)]        
        [TestCase("TQ", 500000.0, 100000.0)]   
        public void TestGetExtentsReturnsCorrectValues(string gridRef, double expectedX, double expectedY)
        {
            BindRunTime();

            TranslateGridReference translate = new TranslateGridReference();
            Envelope envelope = translate.GetExtents(gridRef);
            Assert.IsTrue(envelope.XMin == expectedX);
            Assert.IsTrue(envelope.YMin == expectedY);
        }

        [Test()]
        [TestCase("123 123 123")]
        [TestCase("NX5095822")]
        [TestCase("asd 123 asd")]
        [TestCase("TQ0SD0NW")]
        [TestCase("TQ00100NW")]        
        [TestCase("TQ0000000000SE")]
        [TestCase("TQ000000001SE")]
        [TestCase("TQ0010eeeeeee0NW")]
        [TestCase("TQ00S00NW")]
        [TestCase("TQNW")]
        [TestCase("")] 
        public void TestGetExtentsHandlesIncorrectValues(string gridRef)
        {
            BindRunTime();

            TranslateGridReference translate = new TranslateGridReference();
            Envelope envelope = translate.GetExtents(gridRef);
            Assert.IsTrue(envelope == null);
        }

        [Test()]
        [TestCase(450950, 458250, "BNG", "SE509582")]
        [TestCase(-3.910277, 55.357446, "LatLong", "NS789087")]
        public void TestGetGridReferenceCorrectValues(double x, double y, string sRef, string expectedRes)
        {
            BindRunTime();
            IPoint testPoint = new PointClass();
            testPoint.PutCoords(x, y);

            if (sRef == "BNG")
                testPoint.SpatialReference = bngSRef;
            else
                testPoint.SpatialReference = wgs84SRef;
            
            BNG_Locator bng = new BNG_Locator();
            IPropertySet results = bng.ReverseGeocode(testPoint, false);
            object obj = results.GetProperty("BNG");
            Assert.IsTrue(obj.ToString() == expectedRes);
        }


        [Test()]
        [TestCase(450950, 458250, "LatLong", "SE509582")]
        [TestCase(-3.910277, 553.357446, "LatLong", "")]
        public void TestGetGridReferenceInCorrectValues(double x, double y, string sRef, string expectedRes)
        {
            BindRunTime();
            IPoint testPoint = new PointClass();
            testPoint.PutCoords(x, y);

            if (sRef == "BNG")
                testPoint.SpatialReference = bngSRef;
            else
                testPoint.SpatialReference = wgs84SRef;

            BNG_Locator bng = new BNG_Locator();
            IPropertySet results = bng.ReverseGeocode(testPoint, false);
            object obj = results.GetProperty("BNG");
            Assert.IsTrue(obj.ToString() == expectedRes);
        }
    }
}
