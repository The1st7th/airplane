using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Flights.Models;

namespace Flights.Controllers
{
    public class ItemsController : Controller
    {
        [HttpGet("/departures")]
        public ActionResult Departures()
        {
            List<Departure> allItems = Departure.GetAll();
            return View(allItems);
        }

        [HttpGet("/departures/new")]
        public ActionResult Createdeparture()
        {
            List<Arrival> allItems = Arrival.GetAll();
            return View(allItems);
        }

        [HttpPost("/departures")]
        public ActionResult Create()
        {
          Departure newItem = new Departure (Request.Form["new-departure"]);
          newItem.Save();
          Arrival selectedarrival = Arrival.Find(int.Parse(Request.Form["arrival"]));
          newItem.AddArrival(selectedarrival);
          List<Departure> allItems = Departure.GetAll();
          return View("departures", allItems);
        }
        [HttpGet("/")]
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost("/arrivals")]
        public ActionResult Indexc()
        {
            Arrival newItem = new Arrival (Request.Form["new-arrival"]);
            newItem.Save();
            List<Arrival> allItems = Arrival.GetAll();
            return View("arrivalc",allItems);
        }
        [HttpGet("/arrival/new")]
        public ActionResult Createarrival()
        {
            return View();
        }
        [HttpGet("/departures/delete/{id}")]
        public ActionResult Delete(int id)
        {
            Departure.Delete(id);
            List<Departure> allItems = Departure.GetAll();
            return View("departures", allItems);
        }
        [HttpPost("/depature/delete")]
        public ActionResult DeleteAll(int id)
        {
            Departure.DeleteAll();
            List<Departure> allItems = Departure.GetAll();
            return View("departures", allItems);
        }
        [HttpPost("/arrival/search")]
        public ActionResult Resultcategory()
        {
          Arrival selectedCategory = Arrival.Find(int.Parse(Request.Form["category"]));
          return View("departures", selectedCategory.GetArrival());
        }
        [HttpGet("/arrival/search")]
        public ActionResult Searcharrival()
        {
          List<Arrival> allItems = Arrival.GetAll();
          return View(allItems);
        }
        [HttpGet("/arrival")]
        public ActionResult Displaycategory()
        {
            List<Arrival> allItems = Arrival.GetAll();
            return View("arrivalc", allItems);
        }
        [HttpPost("/departure/search")]
        public ActionResult Resultitems()
        {
          Departure selecteditems = Departure.Find(int.Parse(Request.Form["departure"]));
          return View("Arrivalc", selecteditems.GetDeparture());
        }
        [HttpGet("/items/search")]
        public ActionResult Searchcatfromitems()
        {
          List<Departure> allItems = Departure.GetAll();
          return View(allItems);
        }


    }
}
