using System.Collections;
using System;
using log4net;
using ESRI.ArcGIS.Geometry;

namespace ESRIUK.DynamicLocators.BNGLocator
{
    /// <summary>
    /// Class from Productivity Suite that does BNG Transformation and Grid Lookup
    /// </summary>
    public class TranslateGridReference
    {
        // Create new instance of logger, 1 per class recommended        
        log4net.ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #region Member Variables

        private string[,] prefixArray;

        #endregion

        #region Constructors

        public TranslateGridReference()
        {
            _log.Debug("NGTranslate TranslateGridReference");
            try
            {
                loadArray();
            }
            catch
            {
            }
        }

        #endregion

        #region Private Methods

        private void loadArray()
        {
            _log.Debug("NGTranslate loadArray");
            try
            {
                //redimension the array
                prefixArray = new string[7, 13];
                prefixArray[0, 0] = "SV"; prefixArray[1, 0] = "SW";
                prefixArray[2, 0] = "SX"; prefixArray[3, 0] = "SY";
                prefixArray[4, 0] = "SZ"; prefixArray[5, 0] = "TV";
                prefixArray[6, 0] = "TW";

                prefixArray[0, 1] = "SQ"; prefixArray[1, 1] = "SR";
                prefixArray[2, 1] = "SS"; prefixArray[3, 1] = "ST";
                prefixArray[4, 1] = "SU"; prefixArray[5, 1] = "TQ";
                prefixArray[6, 1] = "TR";

                prefixArray[0, 2] = "SL"; prefixArray[1, 2] = "SM";
                prefixArray[2, 2] = "SN"; prefixArray[3, 2] = "SO";
                prefixArray[4, 2] = "SP"; prefixArray[5, 2] = "TL";
                prefixArray[6, 2] = "TM";

                prefixArray[0, 3] = "SF"; prefixArray[1, 3] = "SG";
                prefixArray[2, 3] = "SH"; prefixArray[3, 3] = "SJ";
                prefixArray[4, 3] = "SK"; prefixArray[5, 3] = "TF";
                prefixArray[6, 3] = "TG";

                prefixArray[0, 4] = "SA"; prefixArray[1, 4] = "SB";
                prefixArray[2, 4] = "SC"; prefixArray[3, 4] = "SD";
                prefixArray[4, 4] = "SE"; prefixArray[5, 4] = "TA";
                prefixArray[6, 4] = "TB";

                prefixArray[0, 5] = "NV"; prefixArray[1, 5] = "NW";
                prefixArray[2, 5] = "NX"; prefixArray[3, 5] = "NY";
                prefixArray[4, 5] = "NZ"; prefixArray[5, 5] = "OV";
                prefixArray[6, 5] = "OW";

                prefixArray[0, 6] = "NQ"; prefixArray[1, 6] = "NR";
                prefixArray[2, 6] = "NS"; prefixArray[3, 6] = "NT";
                prefixArray[4, 6] = "NU"; prefixArray[5, 6] = "OQ";
                prefixArray[6, 6] = "OR";

                prefixArray[0, 7] = "NL"; prefixArray[1, 7] = "NM";
                prefixArray[2, 7] = "NN"; prefixArray[3, 7] = "NO";
                prefixArray[4, 7] = "NP"; prefixArray[5, 7] = "OL";
                prefixArray[6, 7] = "OM";

                prefixArray[0, 8] = "NF"; prefixArray[1, 8] = "NG";
                prefixArray[2, 8] = "NH"; prefixArray[3, 8] = "NJ";
                prefixArray[4, 8] = "NK"; prefixArray[5, 8] = "OF";
                prefixArray[6, 8] = "OG";

                prefixArray[0, 9] = "NA"; prefixArray[1, 9] = "NB";
                prefixArray[2, 9] = "NC"; prefixArray[3, 9] = "ND";
                prefixArray[4, 9] = "NE"; prefixArray[5, 9] = "OA";
                prefixArray[6, 9] = "OB";

                prefixArray[0, 10] = "HV"; prefixArray[1, 10] = "HW";
                prefixArray[2, 10] = "HX"; prefixArray[3, 10] = "HY";
                prefixArray[4, 10] = "HZ"; prefixArray[5, 10] = "JV";
                prefixArray[6, 10] = "JW";

                prefixArray[0, 11] = "HQ"; prefixArray[1, 11] = "HR";
                prefixArray[2, 11] = "HS"; prefixArray[3, 11] = "HT";
                prefixArray[4, 11] = "HU"; prefixArray[5, 11] = "JQ";
                prefixArray[6, 11] = "JR";

                prefixArray[0, 12] = "HL"; prefixArray[1, 12] = "HM";
                prefixArray[2, 12] = "HN"; prefixArray[3, 12] = "HO";
                prefixArray[4, 12] = "HP"; prefixArray[5, 12] = "JL";
                prefixArray[6, 12] = "JM";
            }
            catch
            {
            }
        }
        #endregion

        /// <summary>
        /// Gets the extents from the "in_ngr_coord" string.
        /// </summary>
        /// <param name="in_coord">The in_ngr_coord.</param>
        /// <returns></returns>
        public Envelope GetExtents(string in_coord)
        {
            _log.Debug("NGTranslate GetExtents");

            Point point;
            string quadrant;
            int sub_width;
            string ngr_coord;
            Envelope result;
            String numberPart = "";

            try
            {
                // Check if entered a valid two letter prefix. If not
                // return nothing otherwise workout the OS 100km grid square

                ngr_coord = in_coord.Trim();

                point = ClassifyTwoLetters(ngr_coord.Substring(0, 2).ToUpper());

                // If the OS grid square wasn't found then return a null

                if (point == null)
                {
                    return null;
                }

                //  If the string is only the two letter prefix OS square then
                //  return the envelope i.e. zoom to the grid square extents

                if (ngr_coord.Length == 2)
                {
                    result = MakeRectangle(point.X, point.Y, 100000, 100000);
                    return result;
                }

                // Remove the first two letter prefix so that we can handle the
                // rest of the string. If whatever is left doesn't have an even
                // length then it cannot be a coordinate.

                ngr_coord = ngr_coord.Substring(2, ngr_coord.Length - 2).Trim();

                if ((ngr_coord.Length % 2) == 1)
                {
                    return null;
                }

                // If using the "ngr_coord" string ends with a quadrant, check that 
                // the preceding string is a number and has an EVEN length.

                quadrant = ngr_coord.Substring(ngr_coord.Length - 2, 2).ToUpper();

                if (quadrant == "SW" | quadrant == "SE" | quadrant == "NW" | quadrant == "NE")
                {
                    numberPart = ngr_coord.Substring(0, ngr_coord.Length - 2).Trim();

                    // This is kludge to fix co-ordinate being return if 10 characters
                    // are in the number which does return the correct value with a 
                    // quadrant.

                    if (numberPart.Length > 8)
                    {
                        return null;
                    }

                    // If the length is ODD then we return since it cannot be a 
                    // co-ordinate since we need at least of x and y

                    if ((numberPart.Length % 2) == 1)
                    {
                        return null;
                    }

                    if (IsInteger(numberPart) == false)
                    {
                        return null;
                    }
                }

                // If the "quadrant" is an integer then we check that the preceding
                // string is a number and is even length.

                if (IsInteger(quadrant) == false)
                {
                    if (quadrant == "SW" | quadrant == "SE" | quadrant == "NW" | quadrant == "NE")
                    {
                        //Remove the right letter prefixes
                        ngr_coord = ngr_coord.Substring(0, ngr_coord.Length - 2).Trim();

                        //Shift the point according to the numbers
                        //and work out the cell width.
                        sub_width = ShiftCoordinate(ref point, ngr_coord);

                        // TQnn is 20km square (ie twice the usual rule)
                        // TQnnSW is 5km square, (ie follows the usual rule)
                        // ShiftCoordinate allows for the 20km square
                        if (ngr_coord.Length == 2)
                            sub_width = sub_width / 2;

                        // As using quadrants move coordinate to
                        // one of the four quadrant corners and half
                        // the cell widh.
                        if (((sub_width != -1) || (sub_width != 1)))
                        {
                            sub_width = sub_width / 2;
                            switch (quadrant)
                            {
                                case "NE":
                                    {
                                        point.Y = point.Y + sub_width;
                                        point.X = point.X + sub_width;
                                        break;
                                    }
                                case "NW":
                                    {
                                        point.Y = point.Y + sub_width;
                                        break;
                                    }
                                case "SE":
                                    {
                                        point.X = point.X + sub_width;
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        sub_width = -1;
                    }
                }
                else
                {
                    // if IsInteger(quadrant) == true it must be in the format TL 2728
                    sub_width = ShiftCoordinate(ref point, ngr_coord);
                }

                switch (sub_width)
                {
                    case 1:
                        {
                            // 1 metre reference so make it an exact point
                            return MakeRectangle(point.X, point.Y, 1, 1);

                        }
                    case -1:
                        {
                            // Invalid reference so return nothing
                            return null;

                        }
                    default:
                        {
                            // calculate extents from width
                            return MakeRectangle(point.X, point.Y, (double)sub_width, (double)sub_width);
                        }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Shifts the coordinate.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="numbers">The numbers.</param>
        /// <returns></returns>
        private int ShiftCoordinate(ref Point point, string numbers)
        {
            _log.Debug("NGTranslate ShiftCoordinate");
            try
            {
                switch (numbers.Length)
                {
                    case 0:
                        return 100000;
                    case 2:
                        point.X = point.X + Convert.ToInt32(numbers.Substring(0, 1)) * 10000;
                        point.Y = point.Y + Convert.ToInt32(numbers.Substring(1, 1)) * 10000;
                        return 10000;
                    case 4:
                        point.X = point.X + Convert.ToInt32(numbers.Substring(0, 2)) * 1000;
                        point.Y = point.Y + Convert.ToInt32(numbers.Substring(2, 2)) * 1000;
                        return 1000;
                    case 6:
                        point.X = point.X + Convert.ToInt32(numbers.Substring(0, 3)) * 100;
                        point.Y = point.Y + Convert.ToInt32(numbers.Substring(3, 3)) * 100;
                        return 100;
                    case 8:
                        point.X = point.X + Convert.ToInt32(numbers.Substring(0, 4)) * 10;
                        point.Y = point.Y + Convert.ToInt32(numbers.Substring(4, 4)) * 10;
                        return 10;
                    case 10:
                        point.X = point.X + Convert.ToInt32(numbers.Substring(0, 5));
                        point.Y = point.Y + Convert.ToInt32(numbers.Substring(5, 5));
                        return 1;
                    default:
                        return -1;
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Create an Envelope
        /// </summary>
        /// <param name="X">X coordinate</param>
        /// <param name="Y">Y coordinate</param>
        /// <param name="width">Envelope width</param>
        /// <param name="height">Envelope heigh</param>
        /// <returns></returns>
        private Envelope MakeRectangle(double X, double Y, double width, double height)
        {
            _log.Debug("NGTranslate MakeRectangle");
            try
            {
                // Modified from LocatorHub original
                Envelope rect = new EnvelopeClass();
                rect.YMin = Y;
                rect.YMax = (Y + height);
                rect.XMin = X;
                rect.XMax = (X + width);
                return (Envelope)rect;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Determine what Grid Ref letters match the coordinates
        /// </summary>
        /// <param name="letterRef"></param>
        /// <returns></returns>
        private Point ClassifyTwoLetters(string letterRef)
        {
            _log.Debug("NGTranslate ClassifyTwoLetters");
            try
            {
                Point point = new Point(); 

                for (int count = 0; count <= 6; count++)
                {
                    for (int count2 = 0; count2 <= 12; count2++)
                    {
                        if (prefixArray[count, count2] == letterRef)
                        {
                            // Modified from LocatorHub original
                            point.X = (100000 * count);
                            point.Y = (100000 * count2);
                            return (Point)point;
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Check if a value is an Int
        /// </summary>
        /// <param name="stringValue">Input Value</param>
        /// <returns>Boolen indicating if value is Int</returns>
        private bool IsInteger(string stringValue)
        {
            _log.Debug("NGTranslate IsInteger");
            try
            {
                Convert.ToInt32(stringValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Accecpts a point and converts it to a UK grid reference, this is not 100% accurate
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public string GetGridReference(IPoint location)
        {
            string xPart = Math.Round(location.X).ToString();
            string yPart = Math.Round(location.Y).ToString();
            string prefix = "";
            string xLetterInd;
            string yLetterInd;
            string gridRefX;
            string gridRefY;

            // Get index for the X letter
            if (xPart.Length < 6)
            {
                xLetterInd = "0";
                gridRefX = xPart.Substring(0, 3);
            }
            else if (xPart.Length % 2 == 0)
            {
                xLetterInd = xPart.Substring(0, 1);
                gridRefX = xPart.Substring(1, 3);
            }
            else
            {
                xLetterInd = xPart.Substring(0, 2);
                gridRefX = xPart.Substring(2, 3);
            }

            // Get index for the Y letter
            if (yPart.Length < 6)
            {
                yLetterInd = "0";
                gridRefY = yPart.Substring(0, 3);
            }
            else if (yPart.Length % 2 == 0)
            {
                yLetterInd = yPart.Substring(0, 1);
                gridRefY = yPart.Substring(1, 3);
            }
            else
            {
                yLetterInd = yPart.Substring(0, 2);
                gridRefY = yPart.Substring(2, 3);
            }

            try
            {
                // Using the X and Y indexes to find the grid ref letters
                prefix = prefixArray[Convert.ToInt32(xLetterInd), Convert.ToInt32(yLetterInd)];
            }
            catch(Exception ex)
            {
                _log.Error("An error occurred finding the Grid prefix: " + ex.Message);
                return null;
            }
            
            return prefix + gridRefX + gridRefY;
        }
    }

}
