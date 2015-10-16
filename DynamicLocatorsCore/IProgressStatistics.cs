using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;

namespace esriLocationPrivate
{
  [Guid("5A20F63C-3791-47AE-B5D9-55901DAC9FA0")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IProgressStatistics
  {
    void AddClass(string className, string classValue, string classDescription, out int classToken);
    void SetRangeMin(int rangeMinLo, int rangeMinHi);
    void SetRangeMax(int rangeMax, int rangeMaxHi);
    [DispId(1610678275)]
    int StepSize { set; }
    [DispId(1610678276)]
    int UpdateFrequency { set; }
    void SetPos(int classToken, int posLo, int posHi);
    void SetPosByValue(string classValue, int posLo, int posHi);
    void GetPos(int classToken, out int posLo, out int posHi);
    void GetPosByValue(string classValue, out int posLo, out int posHi);
    void Step(int classTokens);
    void StepByValue(string classValue);
    void StartWait(string message);
    void StopWait();
    void Update();
    void SetStatsFromTable(ITable pTable, string fieldName);
  }
}