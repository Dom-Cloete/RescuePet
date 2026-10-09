using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using domRescuePet.Models;

namespace domRescuePet.Models
{
    // establish connection string
    public class DefaultDataService
    {
        private String ConnectionString;

        public DefaultDataService()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString_Pets"].ConnectionString;
        }

        // home page = get total adoptions count
        public int GetAdoptedPetsCount()
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM Pets WHERE Status = 'Adopted'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }
            return count;
        }

        // home page = get adoptions summary for table
        public List<AdoptionDetail> GetAdoptions()
        {
            var adoptions = new List<AdoptionDetail>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"
                    SELECT u.FirstName + ' ' + u.LastName AS AdopterName,
                           p.PetName,
                           a.AdoptionDate
                    FROM Adoptions a
                    INNER JOIN Users u ON a.AdopterUserId = u.UserId
                    INNER JOIN Pets p ON a.PetId = p.PetId
                    ORDER BY a.AdoptionDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    adoptions.Add(new AdoptionDetail
                    {
                        AdopterName = reader["AdopterName"].ToString(),
                        PetName = reader["PetName"].ToString(),
                        AdoptionDate = Convert.ToDateTime(reader["AdoptionDate"])
                    });
                }
            }
            return adoptions;
        }

        // pets page = get all pets + then filter
        public List<Pet> GetPets(int typeId = 0, int breedId = 0, int locationId = 0)
        {
            var pets = new List<Pet>();
            string query = @"SELECT p.PetId, p.PetName, p.TypeId, p.BreedId, p.LocationId, p.Age, p.Weight, 
                                    p.Gender, p.PetStory_Short, p.PetStory_Long, p.Status, p.PosterUserId, p.Image,
                                    u.FirstName AS PosterFirstName, u.LastName AS PosterLastName,
                                    t.TypeName, b.BreedName, l.LocationName,
                                    a.AdoptionDate, au.FirstName AS AdopterFirstName, au.LastName AS AdopterLastName
                            FROM Pets p
                            INNER JOIN Users u ON p.PosterUserId = u.UserId
                            INNER JOIN PetTypes t ON p.TypeId = t.TypeId
                            INNER JOIN Breeds b ON p.BreedId = b.BreedId
                            INNER JOIN Locations l ON p.LocationId = l.LocationId
                            LEFT JOIN Adoptions a ON a.PetId = p.PetId
                            LEFT JOIN Users au ON au.UserId = a.AdopterUserId
                            WHERE (@TypeId = 0 OR p.TypeId = @TypeId)
                                AND (@BreedId = 0 OR p.BreedId = @BreedId)
                                AND (@LocationId = 0 OR p.LocationId = @LocationId)";

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TypeId", typeId);
                cmd.Parameters.AddWithValue("@BreedId", breedId);
                cmd.Parameters.AddWithValue("@LocationId", locationId);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        pets.Add(new Pet
                        {
                            PetId = Convert.ToInt32(reader["PetId"]),
                            PetName = reader["PetName"].ToString(),
                            TypeId = Convert.ToInt32(reader["TypeId"]),
                            BreedId = Convert.ToInt32(reader["BreedId"]),
                            LocationId = Convert.ToInt32(reader["LocationId"]),
                            Age = Convert.ToInt32(reader["Age"]),
                            Weight = Convert.ToDecimal(reader["Weight"]),
                            Gender = reader["Gender"].ToString(),
                            PetStory_Short = reader["PetStory_Short"].ToString(),
                            PetStory_Long = reader["PetStory_Long"].ToString(),
                            Status = reader["Status"].ToString(),
                            PosterUserId = Convert.ToInt32(reader["PosterUserId"]),
                            Image = reader["Image"].ToString(),
                            PosterName = reader["PosterFirstName"].ToString() + " " + reader["PosterLastName"].ToString(),
                            TypeName = reader["TypeName"].ToString(),
                            BreedName = reader["BreedName"].ToString(),
                            LocationName = reader["LocationName"].ToString(),
                            AdoptionDate = reader["AdoptionDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["AdoptionDate"]),
                            AdopterName = reader["AdopterFirstName"] == DBNull.Value ? null : reader["AdopterFirstName"].ToString() + " " + reader["AdopterLastName"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

                finally { con.Close(); }
            }
            return pets;
        }

        // pets page = pet types filter
        public List<PetType> GetPetTypes()
        {
            var types = new List<PetType>();
            string query = "SELECT * FROM PetTypes";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        types.Add(new PetType
                        {
                            TypeId = Convert.ToInt32(reader["TypeId"]),
                            TypeName = reader["TypeName"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

                finally { con.Close(); }
            }
            return types;
        }

        // pets page = breeds filter
        public List<Breed> GetBreeds(int typeId = 0)
        {
            var breeds = new List<Breed>();
            string query = "SELECT * FROM Breeds WHERE (@TypeId = 0 OR TypeId = @TypeId)";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TypeId", typeId);
                
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        breeds.Add(new Breed
                        {
                            BreedId = Convert.ToInt32(reader["BreedId"]),
                            TypeId = Convert.ToInt32(reader["TypeId"]),
                            BreedName = reader["BreedName"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

                finally { con.Close(); }
            }
            return breeds;
        }

        // pets page = locations filter
        public List<Location> GetLocations()
        {
            var locations = new List<Location>();
            string query = "SELECT * FROM Locations";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        locations.Add(new Location
                        {
                            LocationId = Convert.ToInt32(reader["LocationId"]),
                            LocationName = reader["LocationName"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

                finally { con.Close(); }
            }
            return locations;
        }

        // pets page = get images from db
        public string GetImage(int petId)
        {
            List<Pet> pets = GetPets().Where(p => p.PetId == petId).ToList();
            if (pets.Any())
            {
                return pets.First().Image;
            }
            return null;
        }

        // adopt page = get users from db
        public List<User> GetUsers()
        {
            var list = new List<User>();
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT UserId, FirstName, LastName FROM Users", con))
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new User
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString()
                    });
                }
            }
            return list;
        }

        // adopt page = filter phone nrs based on user selected (from db)
        public List<Phone> GetPhones(int userId)
        {
            var list = new List<Phone>();
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT PhoneId, PhoneNumber FROM Phones WHERE UserId = @UserId", con))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Phone
                        {
                            PhoneId = Convert.ToInt32(reader["PhoneId"]),
                            PhoneNumber = reader["PhoneNumber"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

                finally { con.Close(); }
            }
            return list;
        }

        // adopt page = save adoption to db
        public bool SaveAdoption(int petId, int adopterUserId, int adopterPhoneId)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(
                @"INSERT INTO Adoptions (PetId, AdopterUserId, AdopterPhoneId, AdoptionDate) 
                  VALUES (@PetId, @UserId, @PhoneId, GETDATE());
                  UPDATE Pets SET Status = 'Adopted' WHERE PetId = @PetId;", con))
            {
                cmd.Parameters.AddWithValue("@PetId", petId);
                cmd.Parameters.AddWithValue("@UserId", adopterUserId);
                cmd.Parameters.AddWithValue("@PhoneId", adopterPhoneId);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // post page = save pet to db
        public bool SavePet(Pet pet, int posterPhoneId)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(
                @"INSERT INTO Pets 
                  (PetName, TypeId, BreedId, LocationId, Age, Weight, Gender, PetStory_Short, PetStory_Long, Status, PosterUserId, Image)
                  VALUES
                  (@PetName, @TypeId, @BreedId, @LocationId, @Age, @Weight, @Gender, @Short, @Long, @Status, @UserId, @Image)", con))
            {
                cmd.Parameters.AddWithValue("@PetName", pet.PetName);
                cmd.Parameters.AddWithValue("@TypeId", pet.TypeId);
                cmd.Parameters.AddWithValue("@BreedId", pet.BreedId);
                cmd.Parameters.AddWithValue("@LocationId", pet.LocationId);
                cmd.Parameters.AddWithValue("@Age", pet.Age);
                cmd.Parameters.AddWithValue("@Weight", pet.Weight);
                cmd.Parameters.AddWithValue("@Gender", pet.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Short", (object)pet.PetStory_Short ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Long", (object)pet.PetStory_Long ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", "Available");
                cmd.Parameters.AddWithValue("@UserId", pet.PosterUserId);
                cmd.Parameters.AddWithValue("@Image", (object)pet.Image ?? DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // donate page = calculate total of donations
        public decimal GetTotalDonations()
        {
            decimal total = 0;
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("SELECT ISNULL(SUM(Amount), 0) FROM Donations", con))
            {
                con.Open();
                total = Convert.ToDecimal(cmd.ExecuteScalar());
            }
            return total;
        }

        // donate page = save donation to db
        public bool SaveDonation(int userId, int phoneId, decimal amount)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(
                @"INSERT INTO Donations (DonorUserId, DonorPhoneId, Amount, DonationDate)
                VALUES (@UserId, @PhoneId, @Amount, GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@PhoneId", phoneId);
                cmd.Parameters.AddWithValue("@Amount", amount);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}