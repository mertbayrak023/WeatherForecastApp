using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using WeatherForecastApp.HelperClasses;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Controllers
{
    //haritadan secmek, ve hava kalite indeksi eklemek
    public class HomeController : Controller
    {
        private ForecastDBEntities db = new ForecastDBEntities(); 

        // GET: Home
        public ActionResult Index(string Location)
        {
            Forecast forecast = new Forecast();

            if (string.IsNullOrEmpty(Location))
            {
                return View(db.Forecasts.Where(x => x.Location.Contains("Istanbul") ).ToList());
            }

            if (!(string.IsNullOrEmpty(Location)))
            {
                Location = Location.ToLower();
                string temp = Location.Substring(1);
                Location = Location[0].ToString().ToUpper() + temp;
            }

            if (Location != null)
            {
                var isLocationAlreadyExists = db.Forecasts.Any(x => x.Location == Location);
                if (!isLocationAlreadyExists)
                {
                    /*Calling API http://openweathermap.org/api */
                    string apiKey = "63693c5201cc965e89e63e7a3d8bad49";
                    HttpWebRequest apiRequest =
                    WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?q=" +
                    Location + "&units=metric&appid=" + apiKey) as HttpWebRequest;

                    string apiResponse = "";
                    using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        apiResponse = reader.ReadToEnd();
                    }
                    /*End*/

                    /*http://json2csharp.com*/
                    ResponseWeather rootObject = JsonConvert.DeserializeObject<ResponseWeather>(apiResponse);

                    //https://api.openweathermap.org/data/3.0/onecall?lat=19.0760&lon=72.8777&appid=63693c5201cc965e89e63e7a3d8bad49
                    HttpWebRequest apiRequest7 =
                    WebRequest.Create("https://api.openweathermap.org/data/3.0/onecall?" + "lat=" + rootObject.coord.lat +"&lon=" + rootObject.coord.lon + "&units=metric&appid=" + apiKey) as HttpWebRequest;

                    string apiResponse7 = "";
                    using (HttpWebResponse response7 = apiRequest7.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader7 = new StreamReader(response7.GetResponseStream());
                        apiResponse7 = reader7.ReadToEnd();
                    }

                    ResponseWeather7 rootObject7 = JsonConvert.DeserializeObject<ResponseWeather7>(apiResponse7);

                    int counter = 0;

                    forecast.Location = Location;
                   
                    foreach (var item in rootObject7.daily)
                    {
                        forecast.Day = "Pazartesi";
                        forecast.Degree = (int)item.temp.max;
                        forecast.Image = null;
                        forecast.Rain = (int)item.rain;
                        forecast.Wind = (int)item.wind_speed;

                        if (item.wind_deg == 0 && item.wind_deg == 360)
                        {
                            forecast.Direction = "Kuzey";
                        }

                        else if (item.wind_deg > 0 && item.wind_deg < 90)
                        {
                            forecast.Direction = "Kuzeydoğu";
                        }

                        else if (item.wind_deg == 90)
                        {
                            forecast.Direction = "Doğu";
                        }

                        else if (item.wind_deg > 90 && item.wind_deg < 180)
                        {
                            forecast.Direction = "Güneydoğu";
                        }

                        else if (rootObject.wind.deg == 180)
                        {
                            forecast.Direction = "Güney";
                        }

                        else if (item.wind_deg > 180 && item.wind_deg < 270)
                        {
                            forecast.Direction = "Güneybatı";
                        }

                        else if (item.wind_deg == 270)
                        {
                            forecast.Direction = "Batı";
                        }

                        else if (item.wind_deg > 270 && item.wind_deg < 360)
                        {
                            forecast.Direction = "Kuzeybatı";
                        }

                        forecast.Date = DateTime.Now.Date;

                        counter++;

                        if (counter == 1)
                        {
                            break;
                        }
                    }

                    counter = 0;

                    foreach(var item in rootObject7.daily)
                    {
                        counter++;

                        if (counter == 2)
                        {
                            forecast.DayTuesday = "Sali";
                            forecast.TuesdayDegree = (int)item.temp.max;
                            forecast.TuesdayRain = (int)item.rain;
                            break;
                        }
                    }

                    counter = 0;

                    foreach (var item in rootObject7.daily)
                    {
                        counter++;

                        if (counter == 3)
                        {
                            forecast.DayWednesday = "Carsamba";
                            forecast.WednesdayDegree = (int)item.temp.max;
                            forecast.WednesdayRain = (int)item.rain;
                            break;
                        }
                    }

                    counter = 0;

                    foreach (var item in rootObject7.daily)
                    {
                        counter++;

                        if (counter == 4)
                        {
                            forecast.DayThursday = "Persembe";
                            forecast.ThursdayDegree = (int)item.temp.max;
                            forecast.ThursdayRain = (int)item.rain;
                            break;
                        }
                    }

                    counter = 0;

                    foreach (var item in rootObject7.daily)
                    {
                        counter++;

                        if (counter == 5)
                        {
                            forecast.DayFriday = "Cuma";
                            forecast.FridayDegree = (int)item.temp.max;
                            forecast.FridayRain = (int)item.rain;
                            break;
                        }
                    }

                    counter = 0;

                    foreach (var item in rootObject7.daily)
                    {
                        counter++;

                        if (counter == 6)
                        {
                            forecast.DaySaturday = "Cumartesi";
                            forecast.SaturdayDegree = (int)item.temp.max;
                            forecast.SaturdayRain = (int)item.rain;
                            break;
                        }
                    }

                    counter = 0;

                    foreach (var item in rootObject7.daily)
                    {
                        counter++;

                        if (counter == 7)
                        {
                            forecast.DaySunday = "Pazar";
                            forecast.SundayDegree = (int)item.temp.max;
                            forecast.SundayRain = (int)item.rain;
                            break;
                        }
                    }

                    db.Forecasts.Add(forecast);
                    db.SaveChanges();
                }
                else
                {
                    /*Calling API http://openweathermap.org/api */
                    string apiKey = "63693c5201cc965e89e63e7a3d8bad49";
                    HttpWebRequest apiRequest =
                    WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?q=" +
                    Location + "&units=metric&appid=" + apiKey) as HttpWebRequest;

                    string apiResponse = "";
                    using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        apiResponse = reader.ReadToEnd();
                    }
                    /*End*/

                    /*http://json2csharp.com*/
                    ResponseWeather rootObject = JsonConvert.DeserializeObject<ResponseWeather>(apiResponse);

                    //https://api.openweathermap.org/data/3.0/onecall?lat=19.0760&lon=72.8777&appid=63693c5201cc965e89e63e7a3d8bad49
                    HttpWebRequest apiRequest7 =
                    WebRequest.Create("https://api.openweathermap.org/data/3.0/onecall?" + "lat=" + rootObject.coord.lat + "&lon=" + rootObject.coord.lon + "&units=metric&appid=" + apiKey) as HttpWebRequest;

                    string apiResponse7 = "";
                    using (HttpWebResponse response7 = apiRequest7.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader7 = new StreamReader(response7.GetResponseStream());
                        apiResponse7 = reader7.ReadToEnd();
                    }

                    ResponseWeather7 rootObject7 = JsonConvert.DeserializeObject<ResponseWeather7>(apiResponse7);

                    using (var context = new ForecastDBEntities())
                    {
                        int counter = 0;
                        // Use of lambda expression to access
                        // particular record from a database
                        var data = context.Forecasts.FirstOrDefault(x => x.Location == Location);

                        // Checking if any such record exist
                        if (data != null)
                        {
                            foreach (var item in rootObject7.daily)
                            {
                                data.Location = Location;
                                data.Day = "Pazartesi";
                                data.Degree = (int)item.temp.max;
                                data.Image = null;
                                data.Rain = (int)item.rain;
                                data.Wind = (int)item.wind_speed;
                                if (item.wind_deg == 0 && item.wind_deg == 360)
                                {
                                    data.Direction = "Kuzey";
                                }

                                else if (item.wind_deg > 0 && item.wind_deg < 90)
                                {
                                    data.Direction = "Kuzeydoğu";
                                }

                                else if (item.wind_deg == 90)
                                {
                                    data.Direction = "Doğu";
                                }

                                else if (item.wind_deg > 90 && item.wind_deg < 180)
                                {
                                    data.Direction = "Güneydoğu";
                                }

                                else if (rootObject.wind.deg == 180)
                                {
                                    data.Direction = "Güney";
                                }

                                else if (item.wind_deg > 180 && item.wind_deg < 270)
                                {
                                    data.Direction = "Güneybatı";
                                }

                                else if (item.wind_deg == 270)
                                {
                                    data.Direction = "Batı";
                                }

                                else if (item.wind_deg > 270 && item.wind_deg < 360)
                                {
                                    data.Direction = "Kuzeybatı";
                                }
                                data.Date = DateTime.Now.Date;
                                
                                counter++;

                                if(counter == 1)
                                {
                                    break;
                                }

                            }

                            counter = 0;

                            foreach (var item in rootObject7.daily)
                            {
                                counter++;

                                if (counter == 2)
                                {
                                    data.DayTuesday = "Sali";
                                    data.TuesdayDegree = (int)item.temp.max;
                                    data.TuesdayRain = (int)item.rain;
                                    break;
                                }
                            }

                            counter = 0;

                            foreach (var item in rootObject7.daily)
                            {
                                counter++;

                                if (counter == 3)
                                {
                                    data.DayWednesday = "Carsamba";
                                    data.WednesdayDegree = (int)item.temp.max;
                                    data.WednesdayRain = (int)item.rain;
                                    break;
                                }
                            }

                            counter = 0;

                            foreach (var item in rootObject7.daily)
                            {
                                counter++;

                                if (counter == 4)
                                {
                                    data.DayThursday = "Persembe";
                                    data.ThursdayDegree = (int)item.temp.max;
                                    data.ThursdayRain = (int)item.rain;
                                    break;
                                }
                            }

                            counter = 0;

                            foreach (var item in rootObject7.daily)
                            {
                                counter++;

                                if (counter == 5)
                                {
                                    data.DayFriday = "Cuma";
                                    data.FridayDegree = (int)item.temp.max;
                                    data.FridayRain = (int)item.rain;
                                    break;
                                }
                            }

                            counter = 0;

                            foreach (var item in rootObject7.daily)
                            {
                                counter++;

                                if (counter == 6)
                                {
                                    data.DaySaturday = "Cumartesi";
                                    data.SaturdayDegree = (int)item.temp.max;
                                    data.SaturdayRain = (int)item.rain;
                                    break;
                                }
                            }

                            counter = 0;

                            foreach (var item in rootObject7.daily)
                            {
                                counter++;

                                if (counter == 7)
                                {
                                    data.DaySunday = "Pazar";
                                    data.SundayDegree = (int)item.temp.max;
                                    data.SundayRain = (int)item.rain;
                                    break;
                                }
                            }

                            context.SaveChanges();
                            //update the rest 
                        }
                    }
                }
            }
            return View(db.Forecasts.Where(x => x.Location.Contains(Location) || Location == null).ToList());
        }

        public ActionResult Email(string email)
        {
            Email emailDb = new Email();

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", new { Location = "Istanbul" });
            }



            if (email != null)
            {
                var isEmailAlreadyExists = db.Email.Any(x => x.EmailName == email);
                if (!isEmailAlreadyExists)
                {
                    emailDb.EmailName = email;
                    db.Email.Add(emailDb);
                    db.SaveChanges();

                    return RedirectToAction("Index", new { Location = "Istanbul" });
                }

                else
                {
                    using (var context = new ForecastDBEntities())
                    {
                        // Use of lambda expression to access
                        // particular record from a database
                        var data = context.Email.FirstOrDefault(x => x.EmailName == email);

                        // Checking if any such record exist
                        if (data != null)
                        {
                            data.EmailName = email;
                            context.SaveChanges();
                        }
                    }       
                }
            }

            return RedirectToAction("Index", new { Location = "Istanbul" });
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult CreateContact()
        {
            return View(new Contact());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateContact(Contact model)
        {

            if (ModelState.IsValid)
            {
                db.Contact.Add(model);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { Location = "Istanbul" });
            }

            return View(model);
        }

        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forecast forecast = db.Forecasts.Find(id);
            if (forecast == null)
            {
                return HttpNotFound();
            }
            return View(forecast);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ForecastID,Location,Degree,Image,Rain,Wind,Direction,Date")] Forecast forecast)
        {
            if (ModelState.IsValid)
            {
                db.Forecasts.Add(forecast);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(forecast);
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forecast forecast = db.Forecasts.Find(id);
            if (forecast == null)
            {
                return HttpNotFound();
            }
            return View(forecast);
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ForecastID,Location,Degree,Image,Rain,Wind,Direction,Date")] Forecast forecast)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forecast).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(forecast);
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forecast forecast = db.Forecasts.Find(id);
            if (forecast == null)
            {
                return HttpNotFound();
            }
            return View(forecast);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Forecast forecast = db.Forecasts.Find(id);
            db.Forecasts.Remove(forecast);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
