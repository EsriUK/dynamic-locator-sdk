# Dynamic Locator SDK

## Introduction
The Dynamic Locator SDK is an alternative method for creating ArcGIS Locators. It is a Visual Studio C# solution that implements ArcObjects interfaces to allow a third party API or other code to be wrapped in such a way that the third party API or code can be integrated with the ArcGIS platform as an ArcGIS Locator.
This SDK can be used for any locator that cannot be created using a Stylefile, for example a mathematical locator that accepts different coordinate systems, or, as mentioned earlier if a location API has some functionality that is desired in an ArcGIS locator the SDK can be used to wrap the API so that it can be used as an ArcGIS Locator. This locator can then be used through ArcGIS for Desktop and/or published to ArcGIS for Server which can then be referenced and used in ArcGIS Online.

## Known Issues

There is a known issue with the field data types when geocoding a file where the input fields are numeric, these fields must be wrapped in quotation marks so to force them to be read as strings otherwise the field values will not be read correctly and will not be geocoded.


## The SDK Solution

The Visual Studio solution contains four projects. The DynamicLocatorCore project is the base project that contains implementations of all of the ArcGIS interfaces required to build a Locator.
The DynamicLocatorsCoreTEST project contains some basic unit tests to cover the code in the DynamicLocatorsCore project.
The BNGLocator project contains custom code to create a Locator that can geocode BNG grid references. The BNGLocatorTEST project contains some unit tests to cover the code in the BNGLocator project.

The solution targets .Net 4.5.1. The SDK has been built against ArcGIS 10.4, and once compiled work against any greater version over 10.4.

## DynamicLocatorCore Project

This project must be referenced in the new project that will contain the actual functionality for the new locator, the DynamicLocatorCore project itself cannot be used directly to create a locator.

IProgressStatistics – This file contains an interface which is used to create a progress dialog.

LocatorWrapper – This file contains a basic implementation of all of the interfaces required to create a locator. 

## BNGLocator Project

This an example project that shows how a locator can be created and how custom functionality can be implemented. The locator created by this project is a British National Grid locator that accepts BNG values or Lat/Long coordinates and returns a location. It demonstrates how Geosearching, Geocoding and Reverse Geocoding can be implemented and the types of object that must be returned for the results to be correctly interpreted by the ArcGIS platform.

Bng_locator.loc – This file contains standard ArcGIS locator properties and a CLSID that is used to link this file to the DLL that is created when the BNGLocator project is built. This is very important as this file is how the locator is used to interact with the ArcGIS platform. 

##### Bng_locator.loc extract:

```
; Basic locator and file properties
LocFileUseUTF8 = True
Category = Address
Description = British National Grid locator
Fields = BNG
UseRelativePaths = false

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;  Properties Required by ArcGIS
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

CLSID = {A1B28934-33BA-4c32-960B-1931BD24BBF7}
UICLSID = {AE5A3A0E-F756-11D2-9F4F-00C04F8ED1C4}

Interpolate.SideValue.Left = L
Interpolate.SideValue.Right = R

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;  Esri Geocoder Misc Optional Properties
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

IntersectionConnectors = & @ | and at

; The minimum score a match must have to be returned
MinimumMatchScore = 85

; The minimum score a candidate must have to be considered
MinimumCandidateScore = 10

EndOffset = 3
SideOffset = 20
SideOffsetUnits = Feet
```

For example in ArcGIS for Desktop this LOC file is what will be displayed in the ArcCatalog window when a user wants to use the locator or publish it to server.

BNGLocator – This is the main file of the BNGLocator project, it is the link between the ArcGIS API and any custom code or APIs that are needed to provide the desired functionality.  It overrides the CreateFields, Properties, FindAddressCandidates and ReverseGeocode methods, these methods must be overridden at a minimum to provide all of the functionality that an ArcGIS locator normally provides i.e.  Geosearching, Geocoding and Reverse Geocoding.
One thing to notice about this file is that it contains a Guid that matches the CLSID in the LOC file, this is how the link is created between the LOC file and the DLL.

NGTranslate – This is a completely custom file that contains methods to provide the functionality specifically required by the BNG locator. This shows how custom functionality can be wrapped and then used in the ArcGIS platform.

## Creating a new Locator

*Basic steps*
* Add a new project to the solution.
* Create a new LOC file with a new CLSID setting the locator properties as required.
* Create a new class and add a Guid to the top of the class with the same value as the CLSID in the LOC file.
* Override the CreateFields, Properties, FindAddressCandidates and ReverseGeocode methods at a minimum.
* Build the project

*Result return format*
The IPropertySet objects that are returned from the FindAddressCandidates and ReverseGeocode methods must have a particular format so that they can be understood by the ArcGIS platform, a full example of this can be seen in the BNGLocator class. The returned object from the FindAddressCandidates method must have the following fields defined in it, other fields can also be added but these must be defined: 
1. Shape – A point geometry for the geocoded location
2. Status – States if the input value was matched or the type of match that was made
3. Score – A percentage value that indicates the accuracy of the match
4. Match_addr – The value that will be displayed in the Find tool in ArcMap or as the main match value returned from the REST endpoint
5. Match_type – The type of result that is being returned e.g. Address, Postcode.

```
IPropertySet reverseGeocodedResult = new PropertySetClass();
object names = null;
object values = null;
names = new string[] { "Shape", "X Field", "Y Field", "BNG" , "Addr_type", "Match_addr" };
values = new object[] { location, location.X, location.Y, matchText.ToString(), "BNG", matchText.ToString() };

reverseGeocodedResult.SetProperties(names, values);
```

The Shape and Match_addr value are also required for the ReverseGeocode return object but to get the results to display as expected in ArcMap or on the REST endpoint field names with the same name and value types as the input fields for the locator must be defined. 
So using the BNG locator as an example its possible input fields are called BNG, X Field and Y Field therefore these three fields must be defined, along with the Shape and Match_addr fields, in the IPropertySet object that is being returned.

Open the properties of the DynamicLocatorCore project and go to the Debug tab and verify that the “Start External program” is set to the correct location for your installation of ArcMap.exe, this will cause Visual Studio to open ArcMap when the DynamicLocatorCore project is run and allow you to debug the code. 
An alternate method is to use the “Attach to Process” function of Visual Studio to attach to the ArcMap process and debug the code. 

## Deployment

To actually deploy/use one of these locators in a live or test environment the any DLLs associated with any custom locator e.g. ESRIUK.DynamicLocators.BNGLocator.dll, must be registered with ArcGIS for Desktop and ArcGIS for Server so that they know that these DLLs exist when you load the LOC file into Desktop or try to publish the locator to Server.

###### To register the DLLs use the following commands in Command Prompt for each DLL:

*For Server:*

"~:\Program Files\Common Files\ArcGIS\bin\ESRIRegAsm.exe" /p:server "DLL_DIRECTORY\ESRIUK.DynamicLocators.Core.dll"

*For ArcMap:*

"~:\Program Files (x86)\Common Files\ArcGIS\bin\ESRIRegAsm.exe" /p:desktop "DLL_DIRECTORY\ESRIUK.DynamicLocators.Core.dll"

###### To unregister the DLLs use the following command:

*For Server:*

"~:\Program Files\Common Files\ArcGIS\bin\ESRIRegAsm.exe" /p:server /u "DLL_DIRECTORY\ESRIUK.DynamicLocators.Core.dll"

*For ArcMap:* 

"~:\Program Files (x86)\Common Files\ArcGIS\bin\ESRIRegAsm.exe" /p:desktop /u "DLL_DIRECTORY\ESRIUK.DynamicLocators.Core.dll"

More information about the EsriRegasm tool can be found here: [ESRIRegAsm utility](http://resources.arcgis.com/en/help/arcobjects-net/conceptualhelp/index.html#//0001000004n6000000)

Once all of the DLLs have been registered a connection to the LOC file directory can be made in the Catalog window in ArcGIS for Desktop and the LOC file will be displayed as a locator, this locator can then be used by simply dragging the locator into the map window or by loading it through the Find tool or through the Address Locator Manager found on the Geocoding toolbar. More information on this can be found here:

[Adding an Address Locator](http://desktop.arcgis.com/en/desktop/latest/guide-books/geocoding/adding-an-address-locator-to-an-arcmap-document.htm)
or
[Using the Find Tool](http://desktop.arcgis.com/en/desktop/latest/map/working-with-layers/using-the-find-tool.htm)

To publish the Locator to ArcGIS for Server follow the steps described here: [Publishing a Geocode Service](https://desktop.arcgis.com/en/desktop/latest/guide-books/geocoding/publishing-a-geocode-service.htm)

## Licensing

Copyright 2015 Esri UK

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

[](Esri Tags: Esri C-Sharp )
[](Esri Language: C-Sharp)

