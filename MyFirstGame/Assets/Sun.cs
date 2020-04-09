using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Sun : MonoBehaviour
{
    float degrees;
    float azimuth;
    double sunDistance;
    System.DateTime date;
    GameObject lighting;
    Clock GlobalClock;

    // Start is called before the first frame update
    void Start()
    {
        sunDistance = 7;
        lighting = new GameObject("Lighting");
        Light lightComp = lighting.AddComponent<Light>();
        lightComp.color = Color.white;
        lightComp.intensity = 20;
        transform.position = new Vector3(1, 1, 1);
        lighting.transform.position = new Vector3(1, 1, 1);

        GlobalClock = GameObject.Find("Global Clock").GetComponent(typeof(Clock)) as Clock;
        date = GlobalClock.GetTime();
    }

    // Update is called once per frame
    void Update()
    {
        date = GlobalClock.GetTime();
        updatePosition(date);
        IDictionary<string, double> result = SunPosition.CalculateSunPosition(date, 34, 118);
        Debug.Log(date);
        
    }

    void updatePosition(System.DateTime date) {
        IDictionary<string, double> result = SunPosition.CalculateSunPosition(date, 34, 118);
        float X = (float)(sunDistance * System.Math.Cos(result["azimuth"]));
        float Y = (float)(sunDistance * System.Math.Sin(result["azimuth"]));
        float Z = (float)(sunDistance * System.Math.Sin(result["altitude"]));
        transform.position = new Vector3(X, Y, Z);
        lighting.transform.position = new Vector3(X, Y, Z);
    }
}

//Reference: http://guideving.blogspot.com/2010/08/sun-position-in-c.html
public static class SunPosition  
{  
    private const double Deg2Rad = System.Math.PI / 180.0;  
    private const double Rad2Deg = 180.0 / System.Math.PI;  
  
    /*! 
     * \brief Calculates the sun light. 
     * 
     * CalcSunPosition calculates the suns "position" based on a 
     * given date and time in local time, latitude and longitude 
     * expressed in decimal degrees. It is based on the method 
     * found here: 
     * http://www.astro.uio.no/~bgranslo/aares/calculate.html 
     * The calculation is only satisfiably correct for dates in 
     * the range March 1 1900 to February 28 2100. 
     * \param System.DateTime Time and date in local time. 
     * \param latitude Latitude expressed in decimal degrees. 
     * \param longitude Longitude expressed in decimal degrees. 
     */  
    public static IDictionary<string, double> CalculateSunPosition(  
        System.DateTime dateTime, double latitude, double longitude)  
    {  
        // Convert to UTC  
        dateTime = dateTime.ToUniversalTime();  
  
        // Number of days from J2000.0.  
        double julianDate = 367 * dateTime.Year -  
            (int)((7.0 / 4.0) * (dateTime.Year +   
            (int)((dateTime.Month + 9.0) / 12.0))) +  
            (int)((275.0 * dateTime.Month) / 9.0) +  
            dateTime.Day - 730531.5;  
          
        double julianCenturies = julianDate / 36525.0;  
  
        // Sidereal Time  
        double siderealTimeHours = 6.6974 + 2400.0513 * julianCenturies;  
          
        double siderealTimeUT = siderealTimeHours +  
            (366.2422 / 365.2422) * (double)dateTime.TimeOfDay.TotalHours;  
          
        double siderealTime = siderealTimeUT * 15 + longitude;  
  
        // Refine to number of days (fractional) to specific time.  
        julianDate += (double)dateTime.TimeOfDay.TotalHours / 24.0;  
        julianCenturies = julianDate / 36525.0;  
  
        // Solar Coordinates  
        double meanLongitude = CorrectAngle(Deg2Rad *  
            (280.466 + 36000.77 * julianCenturies));  
          
        double meanAnomaly = CorrectAngle(Deg2Rad *  
            (357.529 + 35999.05 * julianCenturies));  
          
        double equationOfCenter = Deg2Rad * ((1.915 - 0.005 * julianCenturies) *   
            System.Math.Sin(meanAnomaly) + 0.02 * System.Math.Sin(2 * meanAnomaly));  
          
        double elipticalLongitude =  
            CorrectAngle(meanLongitude + equationOfCenter);  
          
        double obliquity = (23.439 - 0.013 * julianCenturies) * Deg2Rad;  
  
        // Right Ascension  
        double rightAscension = System.Math.Atan2(  
            System.Math.Cos(obliquity) * System.Math.Sin(elipticalLongitude),  
            System.Math.Cos(elipticalLongitude));  
          
        double declination = System.Math.Asin(  
            System.Math.Sin(rightAscension) * System.Math.Sin(obliquity));  
  
        // Horizontal Coordinates  
        double hourAngle = CorrectAngle(siderealTime * Deg2Rad) - rightAscension;  
          
        if (hourAngle > System.Math.PI)  
        {  
            hourAngle -= 2 * System.Math.PI;  
        }  
  
        double altitude = System.Math.Asin(System.Math.Sin(latitude * Deg2Rad) *  
            System.Math.Sin(declination) + System.Math.Cos(latitude * Deg2Rad) *  
            System.Math.Cos(declination) * System.Math.Cos(hourAngle));  
  
        // Nominator and denominator for calculating Azimuth  
        // angle. Needed to test which quadrant the angle is in.  
        double aziNom = -System.Math.Sin(hourAngle);  
        double aziDenom =  
            System.Math.Tan(declination) * System.Math.Cos(latitude * Deg2Rad) -  
            System.Math.Sin(latitude * Deg2Rad) * System.Math.Cos(hourAngle);  
          
        double azimuth = System.Math.Atan(aziNom / aziDenom);  
          
        if (aziDenom < 0) // In 2nd or 3rd quadrant  
        {  
            azimuth += System.Math.PI;  
        }  
        else if (aziNom < 0) // In 4th quadrant  
        {  
            azimuth += 2 * System.Math.PI;  
        }  

        IDictionary<string, double> result = new Dictionary<string, double>();
        result.Add("altitude", altitude);
        result.Add("azimuth", azimuth);
        // Altitude  
        System.Console.WriteLine("Altitude: " + altitude * Rad2Deg);  
  
        // Azimut  
        System.Console.WriteLine("Azimuth: " + azimuth * Rad2Deg);  

        return result;
    }  
  
    /*! 
    * \brief Corrects an angle. 
    * 
    * \param angleInRadians An angle expressed in radians. 
    * \return An angle in the range 0 to 2*PI. 
    */  
    private static double CorrectAngle(double angleInRadians)  
    {  
        if (angleInRadians < 0)  
        {  
            return 2 * System.Math.PI - (System.Math.Abs(angleInRadians) % (2 * System.Math.PI));  
        }  
        else if (angleInRadians > 2 * System.Math.PI)  
        {  
            return angleInRadians % (2 * System.Math.PI);  
        }  
        else  
        {  
            return angleInRadians;  
        }  
    }  
}  