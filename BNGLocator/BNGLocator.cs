using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRIUK.DynamicLocators.Core;
using System.Runtime.InteropServices;

namespace ESRIUK.DynamicLocators.BNGLocator
{
    /// <summary>
    /// This class is a BNG Locator, extending a base class
    /// This file must point to a .loc file which references the GUID specified below
    /// e.g. CLSID = {A1B28934-33BA-4c32-960B-1931BD24BBF7}
    /// </summary>
    [Guid("A1B28934-33BA-4c32-960B-1931BD24BBF7")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ESRIUK.DynamicLocators.BNGLocator")]
    public class BNG_Locator : LocatorWrapper 
    {
        // Create new instance of logger, 1 per class recommended        
        log4net.ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The constructor just calls the base constructor
        /// </summary>
        public BNG_Locator()
            : base()
        {
        }

        /// <summary>
        /// This initialises all of the different fields that are required for the locator to function
        /// It contains declarations for the Match and Candidate fields for the locator
        /// </summary>
        protected override void CreateFields()
        {
            IFieldsEdit fieldsEdit = new FieldsClass();
            IFieldEdit shapeField = new FieldClass();
            IFieldEdit statusField = new FieldClass();
            IFieldEdit scoreField = new FieldClass();
            IFieldEdit xField = new FieldClass();
            IFieldEdit yField = new FieldClass();
            IFieldEdit matchField = new FieldClass();

            ISpatialReferenceFactory spatEnv = new SpatialReferenceEnvironmentClass();
            m_spatialReference = spatEnv.CreateProjectedCoordinateSystem((int)esriSRProjCSType.esriSRProjCS_BritishNationalGrid);
            SearchDistanceUnits = esriUnits.esriMeters;            
            // Set up the spatial Reference and Geometry Definition            
            IGeometryDefEdit geometryDefEdit = new GeometryDefClass();
            geometryDefEdit.SpatialReference_2 = m_spatialReference;
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;

            // Create the matchFields
            shapeField.Name_2 = "Shape";
            shapeField.Type_2 = esriFieldType.esriFieldTypeGeometry;
            shapeField.GeometryDef_2 = geometryDefEdit as IGeometryDef;
            fieldsEdit.AddField(shapeField);
            
            statusField.Name_2 = "Status";
            statusField.Type_2 = esriFieldType.esriFieldTypeString;
            statusField.Length_2 = 1;
            fieldsEdit.AddField(statusField);
            
            scoreField.Name_2 = "Score";
            scoreField.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldsEdit.AddField(scoreField);
            
            matchField.Name_2 = "Match_addr";
            matchField.Type_2 = esriFieldType.esriFieldTypeString;
            matchField.Length_2 = 50;
            fieldsEdit.AddField(matchField);
            
            xField.Name_2 = "X";
            xField.Type_2 = esriFieldType.esriFieldTypeDouble;
            xField.Length_2 = 16;
            xField.Precision_2 = 10;
            fieldsEdit.AddField(xField);
            
            yField.Name_2 = "Y";
            yField.Type_2 = esriFieldType.esriFieldTypeDouble;
            yField.Length_2 = 16;
            yField.Precision_2 = 10;
            fieldsEdit.AddField(yField);

            IFieldEdit XMinField = new FieldClass();
            IFieldEdit YMinField = new FieldClass();
            IFieldEdit XMaxField = new FieldClass();
            IFieldEdit YMaxField = new FieldClass();
            IFieldEdit addrType = new FieldClass();
            
            XMinField.Name_2 = "XMin";
            YMinField.Name_2 = "YMin";
            XMaxField.Name_2 = "XMax";
            YMaxField.Name_2 = "YMax";
            addrType.Name_2 = "Addr_type";
            
            XMinField.Type_2 = esriFieldType.esriFieldTypeDouble;
            YMinField.Type_2 = esriFieldType.esriFieldTypeDouble;
            XMaxField.Type_2 = esriFieldType.esriFieldTypeDouble;
            YMaxField.Type_2 = esriFieldType.esriFieldTypeDouble;
            addrType.Type_2 = esriFieldType.esriFieldTypeString;
            
            XMinField.Precision_2 = 8;
            YMinField.Precision_2 = 8;
            XMaxField.Precision_2 = 8;
            YMaxField.Precision_2 = 8;
            addrType.Length_2 = 20;
            
            fieldsEdit.AddField(XMinField);
            fieldsEdit.AddField(YMinField);
            fieldsEdit.AddField(XMaxField);
            fieldsEdit.AddField(YMaxField);
            fieldsEdit.AddField(addrType);

            m_matchFields = fieldsEdit as IFields;

            fieldsEdit = new FieldsClass();
            shapeField = new FieldClass();
            scoreField = new FieldClass();
            xField = new FieldClass();
            yField = new FieldClass();
            matchField = new FieldClass();

            // Create the CandidateFields
            shapeField.Name_2 = "Shape";
            shapeField.Type_2 = esriFieldType.esriFieldTypeGeometry;
            shapeField.GeometryDef_2 = geometryDefEdit as GeometryDef;
            fieldsEdit.AddField(shapeField);
            
            scoreField.Name_2 = "Score";
            scoreField.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldsEdit.AddField(scoreField);
            
            matchField.Name_2 = "Match_addr";
            matchField.Type_2 = esriFieldType.esriFieldTypeString;
            matchField.Length_2 = 50;
            fieldsEdit.AddField(matchField);
            
            xField.Name_2 = "X";
            xField.Type_2 = esriFieldType.esriFieldTypeDouble;
            xField.Length_2 = 16;
            xField.Precision_2 = 10;
            fieldsEdit.AddField(xField);
            
            yField.Name_2 = "Y";
            yField.Type_2 = esriFieldType.esriFieldTypeDouble;
            yField.Length_2 = 16;
            yField.Precision_2 = 10;
            fieldsEdit.AddField(yField);
            
            fieldsEdit.AddField(XMinField);
            fieldsEdit.AddField(YMinField);
            fieldsEdit.AddField(XMaxField);
            fieldsEdit.AddField(YMaxField);
            fieldsEdit.AddField(addrType);

            m_candidateFields = fieldsEdit as IFields;
        }

        /// <summary>
        /// Getter and Setter for the Locators properties
        /// </summary>
        public override IPropertySet Properties
        {
            get
            {
                _log.Debug("ILocatorImpl Properties");
                return m_locatorProperties;
            }
            set
            {
                try
                {
                    _log.Debug("ILocatorImpl Properties");
                    object names, values;

                    m_searchTokenName = "BNG";
                    m_searchTokenAlias = "BNG Grid Reference";
                    
                    // Get the name and value of all the properties in the property set
                    value.GetAllProperties(out names, out values);
                    string[] nameArray = names as string[];
                    object[] valueArray = values as object[];

                    // Set the property names and values
                    for (int i = 0; i < value.Count; i++)
                    {
                        m_locatorProperties.SetProperty(nameArray[i], valueArray[i]);
                    }

                    object property = getProperty(m_locatorProperties, "WriteXYCoordFields");
                    if (property != null)
                        m_addXYCoordsToMatchFields = bool.Parse(property as String);
                    property = getProperty(m_locatorProperties, "Fields");

                    // Create the Input fields for the locator
                    set_DefaultInputFieldNames(m_searchTokenName, (new string[] { "Grid Reference", "Grid Ref", "Address", "Search" }) as object);       
                    set_DefaultInputFieldNames("X Field", (new string[] { "X Field", "X", "XCoord",  }) as object);
                    set_DefaultInputFieldNames("Y Field", (new string[] { "Y Field", "Y", "YCoord" }) as object);

                    // Create the addressFields
                    IFieldsEdit fieldsEdit = new FieldsClass();
                    IFieldEdit bng = new FieldClass();
                    IFieldEdit xField = new FieldClass();
                    IFieldEdit yField = new FieldClass();

                    bng.Name_2 = m_searchTokenName;
                    bng.AliasName_2 = m_searchTokenAlias;
                    bng.Required_2 = false;
                    bng.Type_2 = esriFieldType.esriFieldTypeString;
                    bng.Length_2 = 15;
                    fieldsEdit.AddField(bng);
                    
                    xField.Name_2 = "X Field";
                    xField.AliasName_2 = "X Field";
                    xField.Required_2 = false;
                    xField.Type_2 = esriFieldType.esriFieldTypeString;
                    xField.Length_2 = 15;
                    fieldsEdit.AddField(xField);

                    yField.Name_2 = "Y Field";
                    yField.AliasName_2 = "Y Field";
                    yField.Required_2 = false;
                    yField.Type_2 = esriFieldType.esriFieldTypeString;
                    yField.Length_2 = 15;
                    fieldsEdit.AddField(yField);

                    m_addressFields = fieldsEdit as IFields;
                }
                catch(Exception ex)
                {
                    _log.Error("An error occured while setting the locator properties: " + ex.Message);
                }
            }
        }

        #region IAddressCandidates
        
        /// <summary>
        /// This method is called when a query is made to the locator. 
        /// This must be implemented in such a way that Single and Multi line searches can be preformaed
        /// </summary>
        /// <param name="address">Review code for the structure of this property set</param>
        /// <returns>A single-line array containing a property set. Review code for the structure of this property set</returns>
        public override IArray FindAddressCandidates(IPropertySet address)
        {
            _log.Debug("BNGLocator  IAddressCandidates FindAddressCandidates");
            IArray addressCandidates = new ArrayClass();

            // Get the input from the IPropertySet 
            object names = null;
            object values = null; 
            
            address.GetAllProperties(out names, out values);
            
            string[] nameArray = (string[])names;
            object[] valueArray = (object[])values;

            _log.Debug("Input address columns:" + string.Concat( nameArray));
            

            //make sure there is at least one value
            if (nameArray.Length > 0)
            {
                string addressValue;

                if(nameArray.Length == 1)                
                    addressValue = valueArray[0].ToString();   
                else
                    addressValue = valueArray[0].ToString() + "," + valueArray[1].ToString();   

                _log.Debug("Lookup Value:" + addressValue);

                Envelope enve = DoMatchLookup(addressValue);
                // Get centre point of Envelope for geocode location
                // ONLY Point geometries can be returned successfully 
                Point point = CentrePoint(enve);

                if (point != null)
                {
                    // Ensure spatial reference is set on this envelope returned by the search function
                    (point as IGeometry).SpatialReference = base.m_spatialReference;

                    // Build the required output array
                    IPropertySet match = new PropertySetClass();
                    names = new string[] { "Shape", "Status", "Score", "X", "Y", "XMin", "YMin", "XMax", "YMax", "Match_addr", "Addr_type" };                                      
                    values = new object[] { point, "M", 100, point.X, point.Y, enve.XMin, enve.YMin, enve.XMax, enve.YMax, addressValue.ToUpper(), "BNG" };

                    match.SetProperties(names, values);
                    addressCandidates.Add(match);
                }
            }
            

            return addressCandidates;
        }
        
        #endregion

        #region IReverseGeocoding

        /// <summary>
        /// Generate an address based on a point.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="returnIntersection"></param>
        /// <returns></returns>
        public override IPropertySet ReverseGeocode(IPoint location, bool returnIntersection)
        {
            _log.Debug("IReverseGeocode ReverseGeocode");
            string matchText;
            IPropertySet reverseGeocodedResult = new PropertySetClass();
            Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
            System.Object obj = Activator.CreateInstance(factoryType);
            var srf = obj as ISpatialReferenceFactory3;
            var wgs84GCS = srf.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            
            // Sometimes LatLong is incorrectly identified as BNG so it must be checked
            if ((location.X > -9 && location.X < 2) && (location.Y > 49 && location.Y < 61))
            {
                location.SpatialReference = wgs84GCS;
            }

            // Only supports coords in WGS 84 or BNG 
            if (location.SpatialReference.Name == "GCS_WGS_1984")
            {
                #region Project coordinates to BNG accurately                

                // Create Transformation from WGS84 to OSGB86
                var geoTrans = srf.CreateGeoTransformation((int)esriSRGeoTransformationType.esriSRGeoTransformation_OSGB1936_To_WGS1984Petrol) as IGeoTransformation;

                ISpatialReference fromSpatialReference;
                ISpatialReference toSpatialReference;
                geoTrans.GetSpatialReferences(out fromSpatialReference, out toSpatialReference);

                // Use correct coord systems to ensure accuracy                
                var bngPCS = srf.CreateProjectedCoordinateSystem((int)esriSRProjCSType.esriSRProjCS_BritishNationalGrid);
                if ((wgs84GCS.FactoryCode != toSpatialReference.FactoryCode)
                    || (bngPCS.GeographicCoordinateSystem.FactoryCode != fromSpatialReference.FactoryCode))
                {
                    throw new Exception("invalid geotransformation");
                }

                IGeometry2 geometry;
                geometry = location as IGeometry2;
                geometry.SpatialReference = wgs84GCS;

                geometry.ProjectEx(bngPCS, esriTransformDirection.esriTransformReverse, geoTrans, false, 0.0, 0.0);
                location = geometry as IPoint;

                #endregion
            }
            else if (location.SpatialReference.Name != "British_National_Grid")
            {           
                // Unaccepted spatial reference, do not process
                return reverseGeocodedResult;
            }

            TranslateGridReference translate = new TranslateGridReference();
            // Translate the BNG coords to a British Grid Reference
            matchText = translate.GetGridReference(location);
            object names = null;
            object values = null;

            // The values being returned must include a geometry, any extra info fields ("X", "Y", "Addr_type") and at least 
            // one field with the same name as an input field, "BNG" in this case, to hold the result fields, mathcText in this case
            names = new string[] { "Shape", "X Field", "Y Field", "BNG" , "Addr_type", "Match_addr" };
            values = new object[] { location, location.X, location.Y, matchText.ToString(), "BNG", matchText.ToString() };

            reverseGeocodedResult.SetProperties(names, values);
                        
            return reverseGeocodedResult;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="queryString">The string to search for, either XXXXXX,YYYYY or with sheet SSXXXXYYYY</param>
        /// <returns>An ArcObjects envelope object</returns>
        private Envelope DoMatchLookup(string queryString)
        {
            _log.Debug("BNGLocator DoMatchLookup");
            Envelope envelope = null;
            string matchText;
            string[] parts;

            // Removes '," and spaces and split into parts based on a "," delimiter
            matchText = queryString.Replace("'", "");
            matchText = matchText.Replace("\"", "");
            matchText = matchText.TrimStart(' ').TrimEnd(' ');

            // Split the search string
            parts = matchText.Split(',');

            if (parts.Length == 1)
            {
                parts = matchText.Split(' ');
            }

            // If there are two parts then it's a co-ordinate X,Y so we construct an
            // envelope and return it. If there is one part then we assume it's a 
            // tile reference e.g. TQ1234 so we call TranslateGridReference to get 
            // the OS envelope.

            if (parts.Length == 2)
            {
                double xCoordinate;
                double yCoordinate;

                if ((double.TryParse(parts[0], out xCoordinate) == true) & (double.TryParse(parts[1], out yCoordinate) == true))
                {
                    try
                    {
                        envelope = new EnvelopeClass();
                        envelope.XMin = xCoordinate;
                        envelope.YMin = yCoordinate;
                        envelope.XMax = xCoordinate;
                        envelope.YMax = yCoordinate;
                    }
                    catch (Exception)
                    {
                        envelope = null;
                    }
                }
            }
            else
            {
                TranslateGridReference translate = new TranslateGridReference();
                matchText = matchText.Replace(" ", string.Empty);
                envelope = translate.GetExtents(matchText);
            }

            return envelope;
        }

        #region Misc

        /// <summary>
        /// Returns a Point at the Centre of the Envelope.
        /// </summary>
        /// <returns>A Point</returns>
        public Point CentrePoint(Envelope envelope)
        {
            Point point = null;
            if (envelope == null) _log.Debug("envelope is null");
            if (envelope != null)
            {
                double xCentre;
                double yCentre;

                xCentre = envelope.XMin + ((envelope.XMax - envelope.XMin) / 2.0);
                yCentre = envelope.YMin + ((envelope.YMax - envelope.YMin) / 2.0);

                point = new Point();
                point.X = xCentre;
                point.Y = yCentre;
                point.SpatialReference = base.m_spatialReference;
            }
            return point;
        }

        #endregion
    }
}
