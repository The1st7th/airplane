using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Flight;
using System;


namespace Flights.Models
{
    public class Departure
    {
        private string _departure;
        private int _id;
        // We no longer declare _categoryId here

        public Departure(string departure, int id = 0)
        {
            _departure = departure;
            _id = id;
           // categoryId is removed from the constructor
        }

        public override bool Equals(System.Object otherItem)
        {
          if (!(otherItem is Departure))
          {
            return false;
          }
          else
          {
             Departure newItem = (Departure) otherItem;
             bool idEquality = this.GetId() == newItem.GetId();
             bool descriptionEquality = this.GetDeparture() == newItem.GetDeparture();
             // We no longer compare Items' categoryIds in a categoryEquality bool here.
             return (idEquality && descriptionEquality);
           }
        }
        public override int GetHashCode()
        {
             return this.GetDeparture().GetHashCode();
        }

        public string GetDeparture()
        {
            return _departure;
        }

        public int GetId()
        {
            return _id;
        }

        // We've removed the GetCategoryId() method entirely.

        public void Save()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();

            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO departure (departure) VALUES (@departure);";

            MySqlParameter departure = new MySqlParameter();
            departure.ParameterName = "@departure";
            departure.Value = this._departure;
            cmd.Parameters.Add(departure);

            // Code to declare, set, and add values to a categoryId SQL parameters has also been removed.

            cmd.ExecuteNonQuery();
            _id = (int) cmd.LastInsertedId;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public static List<Departure> GetAll()
        {
            List<Departure> allItems = new List<Departure> {};
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM departure;";
            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            while(rdr.Read())
            {
              int itemId = rdr.GetInt32(0);
              string itemDescription = rdr.GetString(1);
              // We no longer need to read categoryIds from our items table here.
              // Constructor below no longer includes a itemCategoryId parameter:
              Departure newItem = new Departure(itemDescription, itemId);
              allItems.Add(newItem);
            }
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
            return allItems;
        }

        public static Departure Find(int id)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM departure WHERE id = (@searchId);";

            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = id;
            cmd.Parameters.Add(searchId);

            var rdr = cmd.ExecuteReader() as MySqlDataReader;
            int itemId = 0;
            string itemName = "";
            // We remove the line setting a itemCategoryId value here.

            while(rdr.Read())
            {
              itemId = rdr.GetInt32(0);
              itemName = rdr.GetString(1);
              // We no longer read the itemCategoryId here, either.
            }

            // Constructor below no longer includes a itemCategoryId parameter:
            Departure newItem = new Departure(itemName, itemId);
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

            return newItem;
        }

        public void UpdateDescription(string newDeparture)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"UPDATE departure SET departure = @newDeparture WHERE id = @searchId;";

            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = _id;
            cmd.Parameters.Add(searchId);

            MySqlParameter departure = new MySqlParameter();
            departure.ParameterName = "@newDeparture";
            departure.Value = newDeparture;
            cmd.Parameters.Add(departure);

            cmd.ExecuteNonQuery();
            _departure = newDeparture;
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }

        }

        public static void Delete(int id)
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM departure WHERE id = @searchId; DELETE FROM departure_arrival WHERE departure_id = @searchId;";

            MySqlParameter searchId = new MySqlParameter();
            searchId.ParameterName = "@searchId";
            searchId.Value = id;
            cmd.Parameters.Add(searchId);

            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        public static void DeleteAll()
        {
            MySqlConnection conn = DB.Connection();
            conn.Open();
            var cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM departure; DELETE FROM departure_arrival";
            cmd.ExecuteNonQuery();
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
        public void AddArrival(Arrival newCategory)
        {
          MySqlConnection conn = DB.Connection();
          conn.Open();
          var cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"INSERT INTO departure_arrival (arrival_id, departure_id) VALUES (@CategoryId, @ItemId);";

          MySqlParameter arrival_id = new MySqlParameter();
          arrival_id.ParameterName = "@CategoryId";
          arrival_id.Value = newCategory.GetId();
          cmd.Parameters.Add(arrival_id);

          MySqlParameter departure_id = new MySqlParameter();
          departure_id.ParameterName = "@ItemId";
          departure_id.Value = _id;
          cmd.Parameters.Add(departure_id);

          cmd.ExecuteNonQuery();
          conn.Close();
          if (conn != null)
          {
              conn.Dispose();
          }
        }
        public List<Arrival> GetArrivals()
         {
           MySqlConnection conn = DB.Connection();
           conn.Open();
           var cmd = conn.CreateCommand() as MySqlCommand;
           cmd.CommandText = @"SELECT arrival_id FROM departure_arrival WHERE departure_id = @itemId;";

           MySqlParameter itemIdParameter = new MySqlParameter();
           itemIdParameter.ParameterName = "@itemId";
           itemIdParameter.Value = _id;
           cmd.Parameters.Add(itemIdParameter);

           var rdr = cmd.ExecuteReader() as MySqlDataReader;

           List<int> arrivalIds = new List<int> {};
           while(rdr.Read())
           {
               int arrivalId = rdr.GetInt32(0);
               arrivalIds.Add(arrivalId);
           }
           rdr.Dispose();

           List<Arrival> arrivals = new List<Arrival> {};
           foreach (int arrivalId in arrivalIds)
           {
               var categoryQuery = conn.CreateCommand() as MySqlCommand;
               categoryQuery.CommandText = @"SELECT * FROM arrival WHERE id = @arrivalId;";

               MySqlParameter arrivalIdParameter = new MySqlParameter();
               arrivalIdParameter.ParameterName = "@CategoryId";
               arrivalIdParameter.Value = arrivalId;
               categoryQuery.Parameters.Add(arrivalIdParameter);

               var categoryQueryRdr = categoryQuery.ExecuteReader() as MySqlDataReader;
               while(categoryQueryRdr.Read())
               {
                   int thisarrivalId = categoryQueryRdr.GetInt32(0);
                   string arrivalName = categoryQueryRdr.GetString(1);
                   Arrival foundarrival = new Arrival(arrivalName, thisarrivalId);
                   arrivals.Add(foundarrival);
               }
               categoryQueryRdr.Dispose();
           }
           conn.Close();
           if (conn != null)
           {
               conn.Dispose();
           }
           return arrivals;
         }

    }
}
