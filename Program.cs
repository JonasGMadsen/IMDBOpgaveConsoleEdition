using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

class Program
{
    static string connectionString = "Server=LAPTOP-83G828H4;Database=IMDB2;Trusted_Connection=True;TrustServerCertificate=true";

    static void Main(string[] args)
    {
        Console.WriteLine("IMDB asignment database");
        while (true)
        {
            Console.WriteLine("Select option:");
            Console.WriteLine("1. Search movie title");
            Console.WriteLine("2. Search person");
            Console.WriteLine("3. Add movie");
            Console.WriteLine("4. Add person");
            Console.WriteLine("5. Update movie info");

            int option = GetOption(1, 5);

            
            switch (option)
            {
                case 1:
                    SearchMovieByTitle();
                    break;
                case 2:
                    SearchPerson();
                    break;
                case 3:
                    InsertMovie();
                    break;
                case 4:
                    AddPerson();
                    break;
                case 5:
                    UpdateMovie();
                    break;
            }
        }
    }

    static void SearchMovieByTitle()
    {
        Console.WriteLine("Enter movie title:");
        string searchTerm = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "WildcardSearchTitles101";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.CommandTimeout = 120;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pattern", "%" + searchTerm + "%");
                
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine("Search results for movies:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["tconst"]}, {reader["primaryTitle"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No movies found.");
                    Thread.Sleep(800);
                    Console.Clear();
                }
            }
        }
        Console.ReadKey();

        Console.Clear();
    }


    static void SearchPerson()
    {
        Console.WriteLine("Enter the person's name: ");
        string searchTerm = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "WildcardSearchNames101";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@pattern", "%" + searchTerm + "%");
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine("Search results for person:");
                    while (reader.Read())
                    {
                        Console.WriteLine(reader["primaryName"]);
                    }
                }
                else
                {
                    Console.WriteLine("No person found.");
                }
            }
        }
        Console.ReadKey();

        Console.Clear();
    }
    static void InsertMovie()
    {
        Console.WriteLine("Enter person details:");
        Console.Write("Title Type name: ");
        string titleTypeName = Console.ReadLine();
        Console.Write("Primary title: ");
        string primaryTitle = Console.ReadLine();
        Console.Write("Original title: ");
        string originalTitle = Console.ReadLine();
        Console.Write("Is adult: ");
        string isAdult = Console.ReadLine();
        Console.Write("startyear: ");
        string startYear = Console.ReadLine();
        Console.Write("Endyear: ");
        string endYear = Console.ReadLine();
        Console.Write("Runtime: ");
        string runTimeMinutes = Console.ReadLine();
        Console.Write("Genre: ");
        string genre = Console.ReadLine();
        bool.TryParse(isAdult, out bool isAdultBool);
        int.TryParse(startYear, out int startYearInt);
        int.TryParse(endYear, out int endYearInt);
        int.TryParse(runTimeMinutes, out int runTimeMinutesInt);

        InsertMovieUsingStoredProcedure(titleTypeName, originalTitle, primaryTitle, isAdultBool, startYearInt, endYearInt, runTimeMinutesInt, genre);
    }
    
    static void InsertMovieUsingStoredProcedure(string titleTypeName, string title, string originalTitle, bool isAdult, int startYear, int endYear, int runTimeMinutes, string genre)
    {
        int isAdultInt;
        if (isAdult)
        {
            isAdultInt = 1;
        }
        else
        {
            isAdultInt = 0;
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("AddTitle", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@titleTypeName", titleTypeName);
                command.Parameters.AddWithValue("@primaryTitle", title);
                command.Parameters.AddWithValue("@originalTitle", originalTitle);
                command.Parameters.AddWithValue("@isAdult", isAdultInt);
                command.Parameters.AddWithValue("@startYear", startYear);
                command.Parameters.AddWithValue("@endYear", endYear);
                command.Parameters.AddWithValue("@runTimeMinutes", runTimeMinutes);
                command.Parameters.AddWithValue("@genre", genre);

                connection.Open();

                command.ExecuteNonQuery();
                connection.Close();


            }
        }
        Thread.Sleep(800);

        Console.Clear();
    }

    static void AddPerson()
    {
        Console.WriteLine("Enter person details:");
        Console.Write("Name: ");

        string name = Console.ReadLine();
        Console.Write("BirthYear: ");

        string birthYearString = Console.ReadLine();
        int birthYear;
        int.TryParse(birthYearString, out birthYear);

        Console.Write("DeathYear (if unknown, press Enter): ");
        string deathYearString = Console.ReadLine();

        int? deathYear = null;

        if (!string.IsNullOrEmpty(deathYearString))
        {
            int tempDeathYear;
            if (int.TryParse(deathYearString, out tempDeathYear))
            {
                deathYear = tempDeathYear;
            }
        }

        Console.WriteLine("Primary Profession. Write between 1 or 3 professions (Split the professions using ',')");
        string primaryProfession = Console.ReadLine();

        Console.WriteLine("Know for titles");
        string knownForTitles = Console.ReadLine();


        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("AddName", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@primaryName", name);
                command.Parameters.AddWithValue("@birthYear", birthYear);
                command.Parameters.AddWithValue("@primaryProfession", primaryProfession);
                command.Parameters.AddWithValue("@knownForTitles", knownForTitles);



                if (deathYear.HasValue)
                {
                    command.Parameters.AddWithValue("@deathYear", deathYear);
                }
                else
                {
                    command.Parameters.AddWithValue("@deathYear", DBNull.Value);
                }

                connection.Open();
                command.ExecuteNonQuery();           
            }
        }
        Thread.Sleep(800);

        Console.Clear();
    }

    static void UpdateMovie()
    {
        Console.WriteLine("Enter movie title to update:");
        string searchTerm = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT tconst, primaryTitle FROM Titles WHERE primaryTitle LIKE @searchTerm";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Tconst: {reader["tconst"]}, Title: {reader["primaryTitle"]}");
                    }
                    reader.Close();

                    Console.WriteLine("Enter the tconst of the movie you want to update:");
                    string tconst = Console.ReadLine();

                    Console.WriteLine("Enter new movie title:");
                    string newTitle = Console.ReadLine();

                    string updateQuery = "UPDATE Titles SET primaryTitle = @newTitle WHERE tconst = @tconst";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@newTitle", newTitle);
                        updateCommand.Parameters.AddWithValue("@tconst", tconst);

                        updateCommand.ExecuteNonQuery();

                    }
                }
                else
                {
                    Console.WriteLine("No movie found");
                }
            }
        }
        Thread.Sleep(800);
        Console.Clear();
    }



    static int GetOption(int min, int max)
    {
        int option;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out option))
            {
                if (option >= min && option <= max)
                {
                    return option;
                }
            }
            Console.WriteLine("Invalid option. Enter a valid option.");
        }
    }
}
