using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
using Flight;

namespace Flights.Models
{
    public class Arrival
    {
        private string _arrival;
        private int _id;

        public Arrival(string arrival, int id = 0)
        {
            _arrival = arrival;
            _id = id;
        }
        public override bool Equals(System.Object otherCategory)
        {
            if (!(otherCategory is Arrival))
            {
                return false;
            }
            else
            {
                Arrival newCategory = (Arrival) otherCategory;
                return this.GetId().Equals(newCategory.GetId());
            }
        }
        public override int GetHashCode()
        {
            return this.GetId().GetHashCode();
        }
        public string GetArrival()
        {
            return _arrival;
        }
        public int GetId()
        {
            return _id;
        }
        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO arrival (arrival) VALUES (@arrival);";

            MySqlParameter arrival = new MySqlParameter();
            arrival.ParameterName = "@arrival";
            arrival.Value = this._arrival;
            cmd.Parameters.Add(arrival);

            cmd.ExecuteNonQuery();
            _id = (int) cmd.LastInsertedId;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

        }
        public static List<Arrival> GetAll()
        {
            List<Arrival> allarrivals = new List<Arrival> {};
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM arrival;";
            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
              int arrivalId = rdr.GetInt32(0);
              string arrivalName = rdr.GetString(1);
              Arrival newarrival = new Arrival(arrivalName, arrivalId);
              allarrivals.Add(newarrival);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return allarrivals;
        }
        public static Arrival Find(int id)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM arrival WHERE id = (@searchId);";

            MySqlParameter arrival = new MySqlParameter();
            arrival.ParameterName = "@searchId";
            arrival.Value = id;
            cmd.Parameters.Add(arrival);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            int arrivalId = 0;
            string arrivalName = "";

            while(rdr.Read())
            {
              arrivalId = rdr.GetInt32(0);
              arrivalName = rdr.GetString(1);
            }
            Arrival newarrival = new Arrival(arrivalName, arrivalId);
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return newarrival;
        }
        public static void DeleteAll()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM arrival;";
            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
        public List<Departure> GetItemsnojoin()
        {
            List<Departure> allCategoryItems = new List<Departure> {};
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM items WHERE category_id = @category_id;";

            MySqlParameter categoryId = new MySqlParameter();
            categoryId.ParameterName = "@category_id";
            categoryId.Value = this._id;
            cmd.Parameters.Add(categoryId);


            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
              int itemId = rdr.GetInt32(0);
              string itemDescription = rdr.GetString(1);
              int itemCategoryId = rdr.GetInt32(2);
              Departure newItem = new Departure(itemDescription, itemId);
              allCategoryItems.Add(newItem);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return allCategoryItems;
        }
        public List<Departure> GetDeparture()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT departure.* FROM arrivals
                JOIN departure_arrival ON (arrival.id = departure_arrival.arrival_id)
                JOIN items ON (departure_arrival.departure_id = departure.id)
                WHERE arrival.id = @arrivalId;";

            MySqlParameter arrivalIdParameter = new MySqlParameter();
            arrivalIdParameter.ParameterName = "@arrivalId";
            arrivalIdParameter.Value = _id;
            cmd.Parameters.Add(arrivalIdParameter);

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
            List<Departure> departures = new List<Departure>{};

            while(rdr.Read())
            {
              int departureId = rdr.GetInt32(0);
              string departurecity = rdr.GetString(1);
              Departure newdeparture = new Departure(departurecity, departureId);
              departures.Add(newdeparture);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return departures;
        }
    }
}
