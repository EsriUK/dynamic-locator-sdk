/// ASB: This class provided by Esri Inc - just changed the name of the Locator (m_name)
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Location;

namespace Esriuk.Geocoding
{
    // This class doesn't actually seem to get used, no methods below were called while debugging with desktop
  public sealed class LocatorNameWrapper : ILocatorName, IName
  {
    #region Memeber Variables

    private LocatorWrapper m_locator;
    private string m_category;
    private string m_description;
    private ILocatorWorkspaceName m_locatorWorkspaceName = new LocatorWorkspaceNameClass();
    private string m_name = "";
    private bool m_style = false;
    private string m_nameString;

    #endregion

    public LocatorNameWrapper(LocatorWrapper veLocator)
    {
      m_locator = veLocator;
    }

    #region ILocatorName Implemented

    public string Category
    {
      get
      {
        return m_locator.Category;
      }
      set
      {
        m_category = value;
      }
    }

    public string Description
    {
      get
      {
        return m_locator.Description;
      }
      set
      {
        m_description = value;
      }
    }

    public ILocatorWorkspaceName LocatorWorkspaceName
    {
      get
      {
        return m_locatorWorkspaceName;
      }
      set
      {
        m_locatorWorkspaceName = value;
      }
    }

    public string Name
    {
      get
      {
        return m_locator.Name;
      }
      set
      {
        m_name = value;
      }
    }

    public bool Style
    {
      get
      {
        return m_style;
      }
      set
      {
        m_style = value;
      }
    }

    #endregion

    #region IName Members

    public string NameString
    {
      get
      {
        return m_nameString;
      }
      set
      {
        m_nameString = value;
      }
    }

    public object Open()
    {
      return m_locator;
    }

    #endregion
  }
}
