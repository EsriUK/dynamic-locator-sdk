using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Location;
using esriLocationPrivate;
using log4net;


namespace ESRIUK.DynamicLocators.Core
{
    /// <summary>
    /// This class is the base class for all locators created with the SDK.
    /// It contains the implementations of all of the interfaces that are required for an ArcGIS locator to work
    /// Many of the method implementations minimal to enalbe basic functionality and to allow further customisation
    /// 
    /// This class CANNOT be used on as is to create a locator. The methods CreateFields, Properties, FindAddressCandidates and ReverseGeocode 
    /// must be overridden in a child class at a minimum to create a locator
    /// </summary>
    public class LocatorWrapper : ILocator, IAddressInputs, IGeocodingProperties, IAddressGeocoding, IAddressCandidates, IClone, ISingleLineAddressInput,
      ILocatorImpl, ILocatorDataset, IESRILocatorReleaseInfo, IBatchGeocoding, IReverseGeocoding, IReverseGeocodingProperties, IGeocodeServerSingleLine
   {
    
       #region Member Variables
       // Create new instance of logger, 1 per class recommended        
       log4net.ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       protected IFields m_addressFields = new FieldsClass();
       protected IFields m_matchFields =  new FieldsClass();
       protected IFields m_candidateFields = new FieldsClass();
       protected ISpatialReference m_spatialReference;
       private string m_category = "Address";
       private string m_description = "Locator Wrapper";
       private string m_name = "Locator Wrapper";
       private bool m_addPercentAlongToMatchFields = false;
       private bool m_addReferenceIDToMatchFields = false;
       private bool m_addStandardizeStringToMatchFields = false;
       protected bool m_addXYCoordsToMatchFields = false;
       private IPropertySet m_defaultInputFieldNames;
       private int m_endOffset = 3;
       private string m_intersectionConnectors = "& | @";
       private bool m_matchIfScoresTie = true;
       private int m_minimumCandidateScore = 10;
       private int m_minimumMatchScore = 60;
       private double m_sideOffset = 20;
       private esriUnits m_sideOffsetUnits = esriUnits.esriFeet;
       private int m_spellingSensitivity = 80;
       private bool m_useRelativePaths = true;
       protected IPropertySet m_locatorProperties = new PropertySetClass();
       private bool m_needToUpdate = false;
       protected String m_searchTokenName = "Address";
       protected string m_searchTokenAlias = "Address";
       private double m_searchDistance = 100;
       private esriUnits m_searchDistanceUnits = esriUnits.esriFeet;

       #endregion

      /// <summary>
      /// The constructor initialises the log4net configuration and calls the CreateFields method
      /// </summary>
      public LocatorWrapper()
      {
          // This section is used to get the local configuration file so that the log4net configuration can be accessed for debugging
          string dllLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\","");
          FileInfo file = new FileInfo(dllLoc+"/ESRIUK.DynamicLocators.Core.dll.config");
          log4net.Config.XmlConfigurator.Configure(file);
    
          // Create fields is called to initialise all of the required fields for a locator
          CreateFields();
      }
    
      /// <summary>
      /// This initialises all of the different fields that are required for the locator to function
      /// It contains declarations for the Match and Candidate fields for the locator and so must be overridden in each child class
      /// </summary>
      protected virtual void CreateFields()
      {
          // Must be overridden in child class        
      }
    
      #region Implement IReverseGeocode
    
      /// <summary>
      /// Generate an address based on a point.
      /// </summary>
      /// <param name="location"></param>
      /// <param name="returnIntersection"></param>
      /// <returns></returns>
      public virtual IPropertySet ReverseGeocode(IPoint location, bool returnIntersection)
      {
          _log.Debug("IReverseGeocode ReverseGeocode");
          IPropertySet reverseGeocodedResult = new PropertySetClass();
          return reverseGeocodedResult;
      }
    
      #endregion
    
      #region Implement IAddressCandidates
    
      /// <summary>
      /// Fields contained in a list of candidates.
      /// </summary>
      public IFields CandidateFields
      {
          get
          {
              _log.Debug("IAddressCandidates CandidateFields");
                if (m_candidateFields == null)
                    m_candidateFields = new FieldsClass();
          return m_candidateFields;
          }
      }
    
      /// <summary>
      /// Generates candidates for an address.
      /// </summary>
      /// <param name="address"></param>
      /// <returns>Address candidates</returns>
      public virtual IArray FindAddressCandidates(IPropertySet address)
      {
          _log.Debug("IAddressCandidates FindAddressCandidates");
          IArray addressCandidates = new ArrayClass();
          return addressCandidates;
      }
    
      #endregion
    
      #region IClone Members
    
      /// <summary>
      /// Assigns the properties of src to the receiver.
      /// </summary>
      /// <param name="src"></param>
      public void Assign(IClone src)
      {
          _log.Debug("IClone Assign");
          throw new NotImplementedException();
      }
    
      /// <summary>
      /// Clones the receiver and assigns the result to *clone.
      /// </summary>
      /// <returns></returns>
      public IClone Clone()
      {
          _log.Debug("IClone Clone");
          LocatorWrapper veLocator = this;
          return veLocator as IClone;
      }
    
      /// <summary>
      /// Returns TRUE when the receiver and other have the same properties.
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool IsEqual(IClone other)
      {
          _log.Debug("IClone IsEqual");
          throw new NotImplementedException();
      }
    
      /// <summary>
      /// Returns TRUE when the receiver and other are the same object.
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public bool IsIdentical(IClone other)
      {
          _log.Debug("IClone IsIdentical");
          object clonedLocator = other as object;
          object thisLocator = this as object;
          return isSameObject(clonedLocator, thisLocator);
      }
    
      #endregion
    
      #region ILocator Implemented
    
      /// <summary>
      /// Category of the locator.
      /// </summary>
      public string Category
      {
          get
          {
              _log.Debug("ILocator Category");
          return m_category;
          }
          set
          {
              _log.Debug("ILocator Category");
          m_category = value;
          }
      }
    
      /// <summary>
      /// Description of the locator.
      /// </summary>
      public string Description
      {
          get
          {
              _log.Debug("ILocator Description");
          return m_description;
          }
          set
          {
              _log.Debug("ILocator Description");
          m_description = value;
          }
      }
    
      /// <summary>
      /// Name of the locator.
      /// </summary>
      public string Name
      {
          get
          {
              _log.Debug("ILocator Name");
          return m_name;
          }
          set
          {
              _log.Debug("ILocator Name");
          m_name = value;
          }
      }
    
      /// <summary>
      /// User interface for the locator.
      /// </summary>
      public virtual ILocatorUI UserInterface
      {
          get
          {
              _log.Debug("ILocator UserInterface");
              return new ESRI.ArcGIS.LocationUI.AddressLocatorUIClass();
       
          }
      }
    
      #endregion
    
      #region IAddressInputs Implemented
    
      /// <summary>
      /// Fields needed to geocode a table of addresses.
      /// </summary>
      public IFields AddressFields
      {
          get
          {
              _log.Debug("IAddressInputs AddressFields");
              return m_addressFields;
          }
      }
    
      /// <summary>
      /// Recognized names for an input field.
      /// </summary>
      /// <param name="addressField"></param>
      /// <returns></returns>
      public object get_DefaultInputFieldNames(string addressField)
      {
          _log.Debug("IAddressInputs get_DefaultInputFieldNames");
          return m_defaultInputFieldNames.GetProperty(addressField);
      }
    
      #endregion
    
      #region IGeocodingProperties Implemented
    
      /// <summary>
      /// Indicates if the percentage along the reference feature at which the address is located is included in the geocoding result.
      /// </summary>
      public bool AddPercentAlongToMatchFields
      {
          get
          {
              _log.Debug("IGeocodingProperties AddPercentAlongToMatchFields");
              return m_addPercentAlongToMatchFields;
          }
          set
          {
              _log.Debug("IGeocodingProperties AddPercentAlongToMatchFields");
              m_addPercentAlongToMatchFields = value;
          }
      }
    
      /// <summary>
      /// Indicates if the feature ID of the matched feature is included in the geocoding result.
      /// </summary>
      public bool AddReferenceIDToMatchFields
      {
          get
          {
              _log.Debug("IGeocodingProperties AddReferenceIDToMatchFields");
              return m_addReferenceIDToMatchFields;
          }
          set
          {
              _log.Debug("IGeocodingProperties AddReferenceIDToMatchFields");
              m_addReferenceIDToMatchFields = value;
          }
      }
    
      /// <summary>
      /// Indicates if the standardized address is included in the geocoding result.
      /// </summary>
      public bool AddStandardizeStringToMatchFields
      {
          get
          {
              _log.Debug("IGeocodingProperties AddStandardizeStringToMatchFields");
              return m_addStandardizeStringToMatchFields;
          }
          set
          {
              _log.Debug("IGeocodingProperties AddStandardizeStringToMatchFields");
              m_addStandardizeStringToMatchFields = value;
          }
      }
    
      /// <summary>
      /// Indicates if the x and y coordinates of the address location are included in the geocoding result.
      /// </summary>
      public virtual bool AddXYCoordsToMatchFields
      {
          get
          {
              _log.Debug("IGeocodingProperties AddXYCoordsToMatchFields");
              return m_addXYCoordsToMatchFields;
          }
          set
          {
              _log.Debug("IGeocodingProperties AddXYCoordsToMatchFields");
              m_addXYCoordsToMatchFields = value;
              IFieldsEdit newMatchFields = new FieldsClass();
              IFieldsEdit newCandidateFields = new FieldsClass();
              IFieldEdit XField = new FieldClass();
              IFieldEdit YField = new FieldClass();
    
              // Add the XY fields
              if (m_addXYCoordsToMatchFields)
              {
                  XField.Name_2 = "X";
                  YField.Name_2 = "Y";
                
                  XField.Type_2 = esriFieldType.esriFieldTypeDouble;
                  YField.Type_2 = esriFieldType.esriFieldTypeDouble;
                
                  XField.Scale_2 = 6;
                  YField.Scale_2 = 6;
                
                  YField.Precision_2 = 8;
                  XField.Precision_2 = 8;          
                
                  newMatchFields = m_matchFields as IFieldsEdit;
                  newCandidateFields = m_candidateFields as IFieldsEdit;
                
                  newMatchFields.AddField(XField);
                  newCandidateFields.AddField(XField);
                
                  newMatchFields.AddField(YField);
                  newCandidateFields.AddField(YField);
                
                  m_matchFields = newMatchFields;
                  m_candidateFields = newCandidateFields;
              }
              // Remove any existing XY fields
              else
              {
                  newMatchFields = m_matchFields as IFieldsEdit;
                  newCandidateFields = m_candidateFields as IFieldsEdit;
              
                  try
                  {
                      newMatchFields.DeleteField(newMatchFields.get_Field(newMatchFields.FindField("X")));
                      newCandidateFields.DeleteField(newCandidateFields.get_Field(newCandidateFields.FindField("X")));
    
                      newMatchFields.DeleteField(newMatchFields.get_Field(newMatchFields.FindField("Y")));
                      newCandidateFields.DeleteField(newCandidateFields.get_Field(newCandidateFields.FindField("Y")));
                  }
                  catch { }
    
                  m_matchFields = newMatchFields;
                  m_candidateFields = newCandidateFields;
              }
          }
      }
    
      /// <summary>
      /// Recognized names for a required input field.
      /// </summary>
      /// <param name="addressField"></param>
      /// <param name="inputFieldNames"></param>
      public void set_DefaultInputFieldNames(string addressField, object inputFieldNames)
      {
          _log.Debug("IGeocodingProperties set_DefaultInputFieldNames");
    
          //add single line
          if (m_defaultInputFieldNames == null)
          {
              m_defaultInputFieldNames = new PropertySetClass();
              m_defaultInputFieldNames.SetProperty("SingleLine", this.DefaultInputFieldNames);
          }
          else if (m_defaultInputFieldNames.GetProperty("SingleLine") == null)
          {
              m_defaultInputFieldNames.SetProperty("SingleLine", this.DefaultInputFieldNames);
          }
          m_defaultInputFieldNames.SetProperty(addressField, inputFieldNames);
      }
    
      /// <summary>
      /// End offset percentage.
      /// </summary>
      public int EndOffset
      {
          get
          {
              _log.Debug("IGeocodingProperties EndOffset");
              return m_endOffset;
          }
          set
          {
              _log.Debug("IGeocodingProperties EndOffset");
              m_endOffset = value;
          }
      }
    
      /// <summary>
      /// Connector strings used to designate intersections.
      /// </summary>
      public string IntersectionConnectors
      {
          get
          {
              _log.Debug("IGeocodingProperties IntersectionConnectors");
              return m_intersectionConnectors;
          }
          set
          {
              _log.Debug("IGeocodingProperties IntersectionConnectors");
              m_intersectionConnectors = value;
          }
      }
    
      /// <summary>
      /// Indicates whether addresses should be arbitrarily matched to a feature when two or more features have the same best score.
      /// </summary>
      public bool MatchIfScoresTie
      {
          get
          {
              _log.Debug("IGeocodingProperties MatchIfScoresTie");
              return m_matchIfScoresTie;
          }
          set
          {
              _log.Debug("IGeocodingProperties MatchIfScoresTie");
              m_matchIfScoresTie = value;
          }
      }
    
      /// <summary>
      /// Minimum candidate score setting.
      /// </summary>
      public int MinimumCandidateScore
      {
          get
          {
              _log.Debug("IGeocodingProperties MinimumCandidateScore");
              return m_minimumCandidateScore;
          }
          set
          {
              _log.Debug("IGeocodingProperties MinimumCandidateScore");
              m_minimumCandidateScore = value;
          }
      }
    
      /// <summary>
      /// Minimum match score setting.
      /// </summary>
      public int MinimumMatchScore
      {
          get
          {
              _log.Debug("IGeocodingProperties MinimumMatchScore");
              return m_minimumMatchScore;
          }
          set
          {
              _log.Debug("IGeocodingProperties MinimumMatchScore");
              m_minimumMatchScore = value;
          }
      }
    
      /// <summary>
      /// Side offset distance.
      /// </summary>
      public double SideOffset
      {
          get
          {
              _log.Debug("IGeocodingProperties SideOffset");
              return m_sideOffset;
          }
          set
          {
              _log.Debug("IGeocodingProperties SideOffset");
              m_sideOffset = value;
          }
    
      }
    
      /// <summary>
      /// Units used for the side offset.
      /// </summary>
      public esriUnits SideOffsetUnits
      {
          get
          {
              _log.Debug("IGeocodingProperties SideOffsetUnits");
              return m_sideOffsetUnits;
          }
          set
          {
              _log.Debug("IGeocodingProperties SideOffsetUnits");
              m_sideOffsetUnits = value;
          }
      }
    
      /// <summary>
      /// Spelling sensitivity setting.
      /// </summary>
      public int SpellingSensitivity
      {
          get
          {
              _log.Debug("IGeocodingProperties SpellingSensitivity");
              return m_spellingSensitivity;
          }
          set
          {
              _log.Debug("IGeocodingProperties SpellingSensitivity");
              m_spellingSensitivity = value;
          }
      }
    
      /// <summary>
      /// Indicates if the paths to the reference data should be stored relative to the locator.
      /// </summary>
      public bool UseRelativePaths
      {
          get
          {
              _log.Debug("IGeocodingProperties UseRelativePaths");
              return m_useRelativePaths;
          }
          set
          {
              _log.Debug("IGeocodingProperties UseRelativePaths");
              m_useRelativePaths = value;
          }
      }
    
      #endregion
    
      #region IReverseGeocodingProperties Implemented
    
      /// <summary>
      /// Search distance.
      /// </summary>
      public double SearchDistance
      {
          get
          {
              _log.Debug("IReverseGeocodingProperties SearchDistance");
              return m_searchDistance;
          }
          set
          {
              _log.Debug("IReverseGeocodingProperties SearchDistance");
              m_searchDistance = value;
          }
      }
    
      /// <summary>
      /// Units used for the search distance.
      /// </summary>
      public esriUnits SearchDistanceUnits
      {
          get
          {
              _log.Debug("IReverseGeocodingProperties SearchDistanceUnits");
              return m_searchDistanceUnits;
          }
          set
          {
              _log.Debug("IReverseGeocodingProperties SearchDistanceUnits");
              m_searchDistanceUnits = value;
          }
      }
    
      #endregion
    
      #region IAddressGeocoding Implemented
    
      /// <summary>
      /// Uses the FindAddressCandidates method to geocode a single address
      /// </summary>
      /// <param name="address">Input address</param>
      /// <returns>IPropertySet that contains the matched address</returns>
      public virtual IPropertySet MatchAddress(IPropertySet address)
      {
          _log.Debug("IAddressGeocoding MatchAddress");
          IPropertySet resultSet = new PropertySetClass();
          try
          {            
              IArray addressCandidates = FindAddressCandidates(address);
              if (addressCandidates.Count < 1)
                  throw new Exception();
              
              resultSet = addressCandidates.get_Element(0) as IPropertySet;
                          
              object names;
              object values;
              // Get the name and value of all the properties in the property set
              resultSet.GetAllProperties(out names, out values);
              String[] nameArray = names as String[];
              object[] valueArray = values as object[];
              
              _log.Debug("MatchAddress Input address columns:" + string.Concat(nameArray));
              // Add the Status Field to the Result
              List<string> matchNames = new List<string>(nameArray);
              List<object> matchValues = new List<object>(valueArray);
              matchNames.Insert(1, "Status");
              matchValues.Insert(1, "M");
    
              // Set the field names and values for the matched address
              names = matchNames.ToArray() as object;
              values = matchValues.ToArray() as object;
              resultSet.SetProperties(names, values);
              
              return resultSet;
          }
          catch (Exception ex)
          {
              _log.Error("An error ocurred during MatchAddress: " + ex.Message);
          
              // Return an empty address match to prevent errors
              IGeometry emptyPoint = new PointClass() as IGeometry;
              emptyPoint.SetEmpty();
              resultSet.SetProperties(new string[] { "Shape", "Status", "Score", "Match_addr" },
                                          new object[] { emptyPoint, "U", null, null });
              return resultSet;
          }
      }
    
      /// <summary>
      /// Fields contained in the geocoding result.
      /// </summary>
      public IFields MatchFields
      {
          get
          {
              _log.Debug("IAddressGeocoding MatchFields");
                if (m_matchFields == null)
                    m_matchFields = new FieldsClass();
                return m_matchFields;
          }
      }
    
      /// <summary>
      /// Geocodes a table of addresses
      /// </summary>
      /// <param name="addressTable">Input address table</param>
      /// <param name="addressFieldNames">Fields defined in the table</param>
      /// <param name="whereClause">Query filter where clause</param>
      /// <param name="outputFeatureClass">Output feature class for matched addresses</param>
      /// <param name="outputFieldNames">Output field names</param>
      /// <param name="fieldsToCopy"></param>
      /// <param name="cancelTracker"></param>
      public virtual void MatchTable(ITable addressTable, String addressFieldNames, String whereClause,
          IFeatureClass outputFeatureClass, String outputFieldNames, IPropertySet fieldsToCopy, ITrackCancel cancelTracker)
      {
          _log.Debug("IAddressGeocoding MatchTable");
          
          // Obtain the read and insert cursors
          IQueryFilter queryFilter = new QueryFilterClass();
          queryFilter.WhereClause = whereClause;
          ICursor readCursor = null;
          IFeatureCursor insertCursor = null;
          IFeatureCursor updateCursor = null;
    
          // m_needToUpdate will be True when a Rematch is being preformed
          if (m_needToUpdate)
          {
              // Create update cursor to update matched records
              updateCursor = outputFeatureClass.Update(queryFilter, false);
              if (isSameObject(addressTable, outputFeatureClass))
                  readCursor = updateCursor as ICursor;
              else
                  readCursor = addressTable.Search(queryFilter, true);
          }
          else
          {
              // Create insert cursor to add new records
              readCursor = addressTable.Search(queryFilter, true);
              insertCursor = outputFeatureClass.Insert(true);
          }
    
          int count = addressTable.RowCount(queryFilter);      
    
          // Progress dialog setup
          IStepProgressor progressor = null;
          if (cancelTracker != null)
              progressor = cancelTracker.Progressor as IStepProgressor;
          IProgressStatistics progStats = cancelTracker as IProgressStatistics;
          if (progressor != null)
          {
              progressor.StepValue = 1;
              progressor.MaxRange = addressTable.RowCount(null);
          }
    
          // Separate the input field names
          string[] multilineFields = addressFieldNames.Split(',');
    
          // Read the first row and get the address field
          IRow row = readCursor.NextRow();
    
          Dictionary<int, string> addressFieldIndexes = new Dictionary<int, string>();
    
          // Get the index of each valid field
          for (int i = 0; i < multilineFields.Length; i++)
          {
              if (multilineFields[i].Trim().Length > 0)
                  addressFieldIndexes.Add(row.Fields.FindField(multilineFields[i].Trim()), multilineFields[i].Trim());
          }
    
          string address;      
          IPropertySet addressProperties = new PropertySetClass();
          IPropertySet resultSet;
          IFeatureBuffer featureBuffer;
          object copyTo, copyFrom, key, value;
    
          // Get the name and value of all the properties in the property set
          fieldsToCopy.GetAllProperties(out copyTo, out copyFrom);
          string[] copyToArray = copyTo as string[];
          object[] copyFromArray = copyFrom as object[];
          string matchStatus = "U";
          
          // Populate the output feature class
          while (row != null)
          {
              featureBuffer = outputFeatureClass.CreateFeatureBuffer();
              foreach (KeyValuePair<int,string> entry in addressFieldIndexes)
              {
                  if (entry.Key != -1)
                      address = row.get_Value(entry.Key) as string;
                  else
                      address = row.get_Value(0) as string;
    
                  addressProperties.SetProperty(entry.Value, address);
              }
              
              resultSet = MatchAddress(addressProperties);
              
              // Get all of the fields and values of the result
              resultSet.GetAllProperties(out key, out value);
              string[] names = key as string[];
              object[] items = value as object[];
              
              // Output match fields and values
              // Insert the Feature into the output featureClass and get the next row
              if (m_needToUpdate)
              {            
                  _log.Debug("IAddressGeocoding Updating Feature" + row.OID.ToString());
                  
                  // Record is being rematched so find record to update
                  IFeature feature = outputFeatureClass.GetFeature(row.OID);
                  
                  for (int i = 0; i < names.Length; i++)
                  {
                      if (names[i] == "Shape")
                          feature.Shape = items[i] as IGeometry;
                      else
                      {
                          if (names[i] == "Status")
                          {
                              matchStatus = items[i] as string;
                              feature.set_Value(outputFeatureClass.FindField(names[i]), items[i]);
                          }
                      }                                       
                  }
    
                  // Set the match type
                  if (outputFeatureClass.FindField("Match_type") != -1)
                  {
                      feature.set_Value(outputFeatureClass.FindField("Match_type"), "A");
                  }
    
                  // Copy over values from address table
                  for (int j = 0; j < copyToArray.Length; j++)
                  {
                      feature.set_Value(outputFeatureClass.FindField(copyToArray[j]),
                      row.get_Value(addressTable.FindField(copyFromArray[j] as string)));
                  }
                  
                  updateCursor.UpdateFeature(feature);
              }
              else
              {
                  // set shape and status of matched record
                  for (int i = 0; i < names.Length; i++)
                  {
                      if (names[i] == "Shape")
                          featureBuffer.Shape = items[i] as IGeometry;
                      else
                      {
                          if (names[i] == "Status")
                          matchStatus = items[i] as string;
                          featureBuffer.set_Value(outputFeatureClass.FindField(names[i]), items[i]);
                      }                 
                  }
    
                  // Set the match type
                  if (outputFeatureClass.FindField("Match_type") != -1)
                  {
                      featureBuffer.set_Value(outputFeatureClass.FindField("Match_type"), "A");
                  }
    
                  // Copy over values from address table
                  for (int j = 0; j < copyToArray.Length; j++)
                  {
                      try
                      {
                          featureBuffer.set_Value(outputFeatureClass.FindField(copyToArray[j]),
                              row.get_Value(addressTable.FindField(copyFromArray[j] as string)));
                      }
                      catch (Exception ex)
                      {
                          _log.Error("An error occurred copying values from the address table: " + ex.Message);
                      }
                  }
                  
                  insertCursor.InsertFeature(featureBuffer);
              }
              
              row = readCursor.NextRow();
              
              // Update the MatchTable progress
              if (progStats != null)
              {
                  progStats.StepByValue(matchStatus);
                  progStats.Update();
              }
              if (cancelTracker != null)
              {
                  if (!cancelTracker.Continue())
                      break;
              }
          }
          
          _log.Debug("IAddressGeocoding MatchTable End of updating features.");
          
          // Clean up the cursors
          if (insertCursor != null)
          {
              insertCursor.Flush();
              Marshal.ReleaseComObject(insertCursor);
          }
          if (updateCursor != null)
          {
              updateCursor.Flush();
              Marshal.ReleaseComObject(updateCursor);
          }
          if (readCursor != null)
              Marshal.ReleaseComObject(readCursor);
      }
    
      /// <summary>
      /// Checks that the locator properties and locator dataset are present and valid.
      /// </summary>
      public void Validate() { }
    
      #endregion
    
      #region ILocatorImpl Implemented
    
      /// <summary>
      /// Getter and Setter for the Locators properties
      /// </summary>
      public virtual IPropertySet Properties
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
                
                  if (property != null)
                      m_searchTokenName = property as String;
    
                  set_DefaultInputFieldNames(m_searchTokenName, (new string[] { "FullAddress", "Address", "ADDRESS", "Search" }) as object);
    
                  // Create the addressFields
                  IFieldsEdit fieldsEdit = new FieldsClass();
                  IFieldEdit fieldEdit = new FieldClass();
                  fieldEdit.Name_2 = m_searchTokenName;
                  fieldEdit.AliasName_2 = m_searchTokenAlias;
                  fieldEdit.Required_2 = true;
                  fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                  fieldEdit.Length_2 = 50;
                  fieldsEdit.AddField(fieldEdit);
                  m_addressFields = fieldsEdit as IFields;
              }
              catch
              {
              }
          }
      }
    
      #endregion
    
      #region ILocatorDataset Implemented
    
      /// <summary>
      /// Contains a getter for the Locator name object which is taken from the Locator workspace
      /// </summary>
      public ILocatorName FullName
      {
          get
          {
              _log.Debug("ILocatorDataset FullName");
              object locatorStringObject = m_locatorProperties.GetProperty("LocatorWorkspaceString");
              string[] locatorStringArray = locatorStringObject as string[];
              string locatorString = locatorStringObject as string;
            
              // If the LocatorWorkspaceString had more than one row in the file
              if (locatorStringArray != null)
              {
                  locatorString = string.Join("", locatorStringArray);
              }
            
              _log.Debug("locatorString=" + locatorString);
              string[] workspacePropertiesArray = tokenizeLocatorWorkspaceString(locatorString);
            
              if (workspacePropertiesArray == null)
                  throw new Exception("Error parsing LocatorWorkspaceString.");
            
              string progID = workspacePropertiesArray[0];
              string path = workspacePropertiesArray[1];
              IWorkspace workspace = null;
              ILocatorWorkspace locatorWorkspace = null;
              ILocatorManager locatorManager = new LocatorManagerClass();
              ILocatorName locatorName = null;
            
              try
              {
                  // Set up the correct workspace type
                  if (progID.Contains("esriDataSourcesFile.ShapefileWorkspaceFactory"))
                  {
                      IWorkspaceFactory workpspaceFactory = new ShapefileWorkspaceFactoryClass();
                      workspace = workpspaceFactory.OpenFromFile(path, 0);
                      if (workspace == null)
                          return null;
                      locatorWorkspace = locatorManager.GetLocatorWorkspace(workspace);
                  }
                  else if (progID.Contains("esriDataSourcesGDB.FileGDBWorkspaceFactory"))
                  {
                      workspace = createGDBWorkspace(path, "gdb");
                      if (workspace == null)
                          return null;
                      locatorWorkspace = locatorManager.GetLocatorWorkspace(workspace);
                  }
                  else if (progID.Contains("esriDataSourcesGDB.AccessWorkspaceFactory"))
                  {
                      workspace = createGDBWorkspace(path, "mdb");
                      if (workspace == null)
                          return null;
                      locatorWorkspace = locatorManager.GetLocatorWorkspace(workspace);
                  }
                  else if (progID.Contains("esriDataSourcesGDB.SdeWorkspaceFactory"))
                  {
                      string connectionString = tokenizeSDEProperties(path);
                    
                      IWorkspaceName2 workspaceName = new WorkspaceNameClass();
                      workspaceName.WorkspaceFactoryProgID = progID;
                      workspaceName.ConnectionString = connectionString;
                      workspaceName.BrowseName = m_name;
                    
                      IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
                      workspace = workspaceFactory.Open(workspaceName.ConnectionProperties, 0);
                      locatorWorkspace = locatorManager.GetLocatorWorkspace(workspace);
                  }
                  else
                  {
                      throw new NotImplementedException();
                  }
                  locatorName = locatorWorkspace.GetLocatorName(m_name);
              }
              catch (Exception e)
              {
                  throw new Exception("Error opening the Locator Workspace.", e);
              }
              return locatorName;
          }
      }
    
      public ILocatorWorkspace LocatorWorkspace
      {
          get
          {
              _log.Debug("ILocatorDataset LocatorWorkspace");
          IName name = FullName.LocatorWorkspaceName as IName;
          return name.Open() as ILocatorWorkspace;
          }
      }
    
      #endregion
    
      #region IBatchGeocoding Members
    
      /// <summary>
      /// Uses MatchAddress to geocode a cursor of addresses to a RecordSet 
      /// This is called by ArcGIS Server Geocode Addresses
      /// </summary>
      /// <param name="addressCursor">Cursor containing address to be geocoded</param>
      /// <param name="addressFieldNames">The address fields that make up a record in the cursor</param>
      /// <param name="outputRecordSet">The output record set</param>
      /// <param name="outputFieldNames">The output field names</param>
      /// <param name="fieldsToCopy"></param>
      /// <param name="cancelTracker"></param>
      public void MatchRecordSet(ICursor addressCursor, string addressFieldNames, IRecordSetInit outputRecordSet,
          string outputFieldNames, IPropertySet fieldsToCopy, ITrackCancel cancelTracker)
      {
          _log.Debug("IBatchGeocoding MatchRecordSet");
    
          _log.Debug("MatchRecordSet addressFieldNames:" + addressFieldNames);
          _log.Debug("MatchRecordSet outputFieldNames:" + outputFieldNames);
    
          ICursor resultCursor = outputRecordSet.Insert();
          IRow row = addressCursor.NextRow();
          IRowBuffer rowBuffer = null;
          IFields fields = row.Fields;
          IFields bufferFields = null;
          String[] fieldNames = addressFieldNames.Split(',');
          String[] outputFields = outputFieldNames.Split(',');
          int addressFieldsSize = fieldNames.Length;
          int outputFieldsSize = outputFields.Length;
          int copyFieldsSize = 0;
          String addressValue = "";
          IPropertySet addressProperty;
          IPropertySet results;
          object value, values, names;
          String[] nameArray;
          object[] valueArray;
          Dictionary<string, int> addressFieldInds = new Dictionary<string,int>();
          Dictionary<string, int> outputFieldInds = new Dictionary<string, int>();
          string fieldName;
          string outFieldName;
    
          // Get all address field indexes
          for (int i = 0; i < addressFieldsSize; i++)
          {
              fieldName = fieldNames[i].Trim();
              if(!addressFieldInds.ContainsKey(fieldName))
                  addressFieldInds.Add(fieldName, fields.FindField(fieldName));
          }
    
          //loop through each record
          while (row != null)
          {
              addressProperty = new PropertySetClass();
              rowBuffer = outputRecordSet.CreateRowBuffer();
              bufferFields = rowBuffer.Fields;
    
              //populate a property set of search values
              for (int i = 0; i < addressFieldsSize; i++)
              {                
                  fieldName = fieldNames[i].Trim();                
                  addressValue = row.get_Value(addressFieldInds[fieldName])  as String;
    
                  if(!string.IsNullOrEmpty(addressValue))
                      addressProperty.SetProperty(fieldName, addressValue);                
              }
    
              // Geocode the Address
              results = MatchAddress(addressProperty);
    
              // Get all output field indexes, only do this once to save processing
              if (outputFieldInds.Count == 0)
              {
                  for (int i = 0; i < outputFieldsSize; i++)
                  {
                      outFieldName = outputFields[i].Trim();
                      outputFieldInds.Add(outFieldName, bufferFields.FindField(outFieldName));
                  }
              }
    
              //add the result to the recordset
              for (int i = 0; i < outputFieldsSize; i++)
              {
                  outFieldName = outputFields[i].Trim();
                  value = results.GetProperty(outFieldName);
                  _log.Debug("MatchRecordSet outputFields[i]:" + outFieldName);
                  rowBuffer.set_Value(outputFieldInds[outFieldName], value);
              }
    
              //copy extra fields
              fieldsToCopy.GetAllProperties(out names, out values);
              nameArray = names as String[];
              valueArray = values as object[];
              copyFieldsSize = nameArray.Length;
    
              for (int i = 0; i < copyFieldsSize; i++)
              {
                  string fieldToCopy = nameArray[i];
                  if(fieldToCopy == "ResultID")
                      rowBuffer.set_Value(bufferFields.FindField(fieldToCopy), row.OID);
                  else
                      rowBuffer.set_Value(bufferFields.FindField(fieldToCopy), row.get_Value(fields.FindField(fieldToCopy)));
              }
    
              //insert row
              resultCursor.InsertRow(rowBuffer);
    
              row = addressCursor.NextRow();
          }
    
          // Clean up the cursors
          resultCursor.Flush();
          Marshal.ReleaseComObject(resultCursor);
          Marshal.ReleaseComObject(addressCursor);
      }
    
      /// <summary>
      /// Uses MatchTable to rematch addresses in a feature class
      /// </summary>
      /// <param name="pInputTable">Input table containing addresses</param>
      /// <param name="inputFieldNames">In put tables fields</param>
      /// <param name="inputJoinFieldName">Input join field name</param>
      /// <param name="resultTable">The rematch result table</param>
      /// <param name="outputFieldNames">Output field names</param>
      /// <param name="outputJoinFieldName">Output join field name</param>
      /// <param name="whereClause">Where Clause for the Match Table method</param>
      /// <param name="cancelTracker"></param>
      public void RematchTable(ITable pInputTable, string inputFieldNames, string inputJoinFieldName, IFeatureClass resultTable,
          string outputFieldNames, string outputJoinFieldName, string whereClause, ITrackCancel cancelTracker)
      {
          _log.Debug("IBatchGeocoding RematchTable");
          
          if (inputJoinFieldName == null && inputJoinFieldName == "")
              inputJoinFieldName = pInputTable.OIDFieldName;
          
          if (outputJoinFieldName == null && outputJoinFieldName == "")
              outputJoinFieldName = resultTable.OIDFieldName;
    
          // Setup the edit session
          IWorkspaceEdit workspaceEdit = setupEditSession(resultTable);
          IPropertySet fieldsToCopy = new PropertySetClass();
    
          try
          {
              m_needToUpdate = true;
              MatchTable(pInputTable, inputFieldNames, whereClause, resultTable, outputFieldNames, fieldsToCopy, cancelTracker);
              m_needToUpdate = false;
          }
          catch (Exception e)
          {
              m_needToUpdate = false;
              workspaceEdit.AbortEditOperation();
              workspaceEdit.StopEditing(false);
              throw new Exception("Failed to Rematch the table. ", e);
          }
    
          workspaceEdit.StopEditOperation();
          workspaceEdit.StopEditing(true);
      }
    
      #endregion
    
      #region IESRILocatorReleaseInfo Implemented
    
      public string Release
      {
          get
          {
              _log.Debug("IESRILocatorReleaseInfo Release");
              return m_locatorProperties.GetProperty("Version") as string;
          }
      }
    
      #endregion
     
      #region ISingleLineAddressInput Implemented
    
      /// <summary>
      /// Recognized names for the single line input field.
      /// </summary>
      public object DefaultInputFieldNames
      {
          get
          {
              _log.Debug("ISingleLineAddressInput DefaultInputFieldNames Get");
              return (new string[] { "SingleLine", "SingleLineInput", "Address" }) as object;
          }
      }
    
      /// <summary>
      /// Getter for Single Line Address field
      /// </summary>
      public IField SingleLineAddressField
      {
          get
          {
              _log.Debug("ISingleLineAddressInput SingleLineAddressField Get");
              IFieldEdit singleLieField = new FieldClass();
              singleLieField.Name_2 = "SingleLine";
              singleLieField.Type_2 = esriFieldType.esriFieldTypeString;
              singleLieField.AliasName_2 = "Single Line Input";
              singleLieField.Required_2 = false;
              singleLieField.Length_2 = 50;
    
              return singleLieField;
          }
    
      }
    
      #endregion
    
      #region IGeocodeServerSingleLine Implemented
    
      /// <summary>
      /// Recognized names for the single line input field.
      /// </summary>
      /// <returns></returns>
      public object GetDefaultInputFieldNames () 
      {
          _log.Debug("IGeocodeServerSingleLine GetDefaultInputFieldNames");
          return DefaultInputFieldNames;
      }
    
      /// <summary>
      /// Field needed to geocode a single line address.
      /// </summary>
      /// <returns></returns>
      public IField GetSingleLineAddressField ()
      {
          _log.Debug("IGeocodeServerSingleLine GetSingleLineAddressField");
          return SingleLineAddressField;
      }
    
      #endregion
    
      #region Misc
      /// <summary>
      /// Compare two objects to see if they are identical
      /// </summary>
      /// <param name="a"></param>
      /// <param name="b"></param>
      /// <returns></returns>
      protected bool isSameObject(object a, object b)
      {
          _log.Debug("Misc isSameObject");
          if (a.Equals(b))
              return true;
          else
              return false;
      }
    
      /// <summary>
      /// Get the path for a locator stored in a file folder
      /// </summary>
      /// <param name="locatorWorkspaceString"></param>
      /// <returns></returns>
      private string[] tokenizeLocatorWorkspaceString(string locatorWorkspaceString)
      {
          _log.Debug("Misc tokenizeLocatorWorkspaceString");
          if (locatorWorkspaceString == null)
              throw new ArgumentNullException("The locatorWorkspaceString is null.");
          
          string progID = "";
          string path = "";
          try
          {
              string[] parsedString = locatorWorkspaceString.Split(new char[] { ';' }, 2);
          
              // First get the WorkspaceFactory Type
              if (parsedString.Length > 0)
              {
                  progID = parsedString[0];
                  string[] progIDArray = progID.Split('=');
          
                  // Get the workspaceFactory string and trim the whitespace
                  if (progIDArray.Length == 2)
                      progID = progIDArray[1].Trim();
              }
          
              // Next get the path
              if (parsedString.Length == 2)
              {
                  path = parsedString[1];
                  string[] pathArray = path.Split('=');
          
              // Get the path to the locator
                  if (pathArray.Length == 2)
                      path = pathArray[1].Trim();
              }
          
              if (progID == "" || path == "")
                  throw new Exception("Error parsing LocatorWorkspaceString.");
          }
          catch (System.ArgumentOutOfRangeException)
          {
              throw new Exception("Error parsing LocatorWorkspaceString.");
          }
          return new string[] { progID, path };
      }
    
      /// <summary>
      /// Get connection properties for FGDB and PGDB
      /// </summary>
      /// <param name="connection"></param>
      /// <returns></returns>
      private string[] tokenizeConnectionProperties(string connection)
      {
          _log.Debug("Misc tokenizeConnectionProperties");
          if (connection == null)
              throw new ArgumentNullException("The connection properties are null.");
          
          string[] connectionPath;
          try
          {
              string[] connectionArray = connection.Split(new char[] { '=' }, 2);
          
              if (connectionArray.Length == 2)
                  connection = connectionArray[1];
          
              connectionPath = connection.Split(';');
          }
          catch (System.ArgumentOutOfRangeException)
          {
              throw new Exception("Error parsing LocatorWorkspaceString.");
          }
          
          return connectionPath;
      }
    
      /// <summary>
      /// Builds a property set that contains the connection info for SDE
      /// </summary>
      /// <param name="connection"></param>
      /// <returns></returns>
      private string tokenizeSDEProperties(string connection)
      {
          _log.Debug("Misc tokenizeSDEProperties");
          if (connection == null)
              throw new ArgumentNullException("The SDE connection properties are null.");
          
          try
          {
              connection = connection.Split(new char[] { '=' }, 2)[1];
              connection = connection.Trim();
              connection = connection.Trim(new char[] { '(', ')' });
          }
          catch (System.ArgumentOutOfRangeException)
          {
              throw new Exception("Error parsing LocatorWorkspaceString.");
          }
          
          return connection;
      }
    
      /// <summary>
      /// Create a workspace for a FGDB or PGDB
      /// </summary>
      /// <param name="path"></param>
      /// <param name="gdbType"></param>
      /// <returns></returns>
      private IWorkspace createGDBWorkspace(string path, string gdbType)
      {
          _log.Debug("Misc createGDBWorkspace");
          string fullPath = "";
          string[] connectionProperties = tokenizeConnectionProperties(path);
          
          if (connectionProperties == null)
              throw new ArgumentNullException("The connection properties are null.");
          
          if (connectionProperties.Length > 0)
          {
              fullPath = connectionProperties[0];
              try
              {
                  string[] pathArray = fullPath.Split('=');
                  
                  // Get the path to the locator
                  if (pathArray.Length == 2)
                  {
                      fullPath = pathArray[1].TrimEnd(')');
                      fullPath = fullPath.Trim();
                  }
              }
              catch (Exception e)
              {
                  throw new Exception("Error parsing LocatorWorkspaceString.", e);
              }
          }
          
          IWorkspaceFactory workspaceFactory;
          
          if (gdbType.ToLower() == "mdb")
              workspaceFactory = new AccessWorkspaceFactoryClass();
          else if (gdbType.ToLower() == "gdb")
              workspaceFactory = new FileGDBWorkspaceFactoryClass();
          else
              throw new ArgumentException("Expected GDB Type equal to 'mdb' or 'gdb'.");
          
          return workspaceFactory.OpenFromFile(fullPath, 0);
      }
    
      /// <summary>
      /// Setup an edit Session for a table
      /// </summary>
      /// <param name="table"></param>
      /// <returns></returns>
      private IWorkspaceEdit setupEditSession(IClass table)
      {
          _log.Debug("Misc setupEditSession");
          IDataset dataset = table as IDataset;
          IWorkspace workspace = dataset.Workspace;
          IWorkspaceEdit workspaceEdit = null;
          IVersionedObject versionedObject = table as IVersionedObject;
          IObjectClassInfo2 objectClassInfo = table as IObjectClassInfo2;
          
          if (versionedObject != null && versionedObject.IsRegisteredAsVersioned ||
              objectClassInfo != null && !objectClassInfo.CanBypassEditSession())
              return workspace as IWorkspaceEdit;
          
          workspaceEdit = workspace as IWorkspaceEdit;
          if (workspaceEdit != null && !workspaceEdit.IsBeingEdited())
          {
              try
              {
                  workspaceEdit.StartEditing(true);
              }
              catch (Exception e)
              {
                  throw new Exception("Exception thrown trying to start the edit session. ", e);
              }
              try
              {
                  workspaceEdit.StartEditOperation();
              }
              catch (Exception e)
              {
                  throw new Exception("Exception thrown trying to start editing. ", e);
              }
          }
          
          return workspaceEdit;
      }
    
      /// <summary>
      /// Helper method for getting a property and returning null if it isn't found
      /// instead of throwing an exception.
      /// </summary>
      /// <param name="locatorProperties"></param>
      /// <param name="Name"></param>
      /// <returns>The property value if found.  Otherwise 'null'</returns>
      protected object getProperty(IPropertySet locatorProperties, String Name)
      {
          _log.Debug("Misc getProperty");
          try
          {
              return locatorProperties.GetProperty(Name);
          }
          catch
          {
              return null;
          }
      }
    
      /// <summary>
      /// Member that allows for getting and setting the Name for the Search Token.
      /// Ex.  On the Find Dialog, the text that will be displayed in front of the textbox.
      /// 
      /// Set this in the .loc file as Fields = YourString
      /// </summary>
      protected virtual String SearchPropertyName
      {
          get
          {
              _log.Debug("Misc SearchPropertyName");
              return m_searchTokenName;
          }
          set
          {
              _log.Debug("Misc SearchPropertyName");
              m_searchTokenName = value;
          }
      }
    
      protected virtual string SearchTokenAlias
      {
          get
          {
              _log.Debug("Misc SearchAliasName");
              return m_searchTokenAlias;
          }
          set
          {
              _log.Debug("Misc SearchAliasName");
              m_searchTokenAlias = value;
          }
      }
    
      #endregion

  }
}
