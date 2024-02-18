using GingerMintSoft.DayTime;

namespace GingerMintSoft.DayTimeTest
{
    [TestClass]
    public class DayTimeCalcYearTest
    {
        [TestMethod]
        public void TestSunriseSunSetTimeForOneYear()
        {
            var home = new Coordinates()
            {
                Latitude = 48.10507778308992,
                Longitude = 7.90856839921184
            };

            // iterate thru whole year 
            var actDate = new DateTime(DateTime.Now.Year, 1, 1);
            var nextYear = DateTime.Now.Year + 1;   

            while(actDate.Year < nextYear)
            {
                // Parameters : year - month - day - lat - long
                var actDay = new CalcDayTime().SunriseSunset(actDate, home);
            
                Console.WriteLine($"{actDate.Day:00}.{actDate.Month:00}.{actDate.Year} Sunrise: {actDay.SunRise}");
                Console.WriteLine($"           Sunset:  {actDay.SunSet}");
                Console.WriteLine($"        DayLength:  {actDay.DayLength.Hours:00}:{actDay.DayLength.Minutes:00}:{actDay.DayLength.Seconds:00}");
                Console.WriteLine("---------------------------------------");

                actDate = actDate.AddDays(1);
            }

            Assert.IsTrue(new DateTime(DateTime.Now.Year + 1, 1, 1) == actDate);
        }
    }
}
