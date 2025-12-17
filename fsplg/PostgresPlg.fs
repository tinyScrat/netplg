module PostgresPlg

open System
open Npgsql

let testDb connStr =
    use conn = new NpgsqlConnection(connStr)
    conn.Open()

    use cmd = new NpgsqlCommand("SELECT version()", conn)
    use reader = cmd.ExecuteReader()

    while reader.Read() do
        Console.WriteLine(reader.GetString 0)
