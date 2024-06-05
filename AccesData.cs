using System.Data;
using System.Data.OleDb;
using System.Runtime.Intrinsics.X86;

namespace schedule_bot;

public class AccessDatabase
{
    public static string connectString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=TGBot_pups_0_1ver.mdb";

    private static OleDbConnection _oleDbConnection = new OleDbConnection(connectString);
    private OleDbCommand _oleDbCommand = _oleDbConnection.CreateCommand();
    public void ReadData()
    {
        /*_oleDbConnection = new OleDbConnection(connectString);
        _oleDbConnection.Open();*/
        
        string sqlQuery =
            "SELECT рассписание.[день недели], рассписание.время, предметы.[назв пред], препод.ФИО, рассписание.аудитория, рассписание.[неделя(цвет)]" +
            "FROM факультет INNER JOIN (препод INNER JOIN ((группа INNER JOIN предметы ON группа.id = предметы.группа) INNER JOIN рассписание ON предметы.id = рассписание.предмет) ON препод.id = предметы.препод) ON факультет.id = группа.Факультет " +
            "WHERE рассписание.день недели = 'dayOfWeek'" +
            "{ORDER BY рассписание.время";
        
        try
        {
            using (OleDbConnection connection = new OleDbConnection(connectString))
            {
                connection.Open();

                // Create a command with the SQL query
                OleDbCommand command = new OleDbCommand(sqlQuery, connection);

                // Execute the query and read the results
                OleDbDataReader reader = command.ExecuteReader();

                Console.WriteLine(reader.GetString(0));
                
                for (int j = 0; reader.Read(); j++)
                {
                    // Process rows from the result set

                    string dayOfWeek = reader.GetString(0);
                    string time = reader.GetString(1);
                    string lessons = reader.GetString(2);
                    string teacher = reader.GetString(3);
                    string classroom = reader.GetString(4);
                    
                    Console.WriteLine($"{time} {lessons} {teacher} {classroom}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        
        /*_oleDbCommand.CommandText =
            "SELECT рассписание.[день недели], рассписание.время, предметы.[назв пред], препод.ФИО, рассписание.аудитория, рассписание.[неделя(цвет)]" +
            "FROM факультет INNER JOIN (препод INNER JOIN ((группа INNER JOIN предметы ON группа.id = предметы.группа) INNER JOIN рассписание ON предметы.id = рассписание.предмет) ON препод.id = предметы.препод) ON факультет.id = группа.Факультет " +
            "WHERE рассписание.день недели = 'dayOfWeek'" +
            "{ORDER BY рассписание.время";
        */
    }
}