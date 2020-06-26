using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class SQLite {
  private static string urlDataBase = "URI=file:" + Application.persistentDataPath + "MasterSQLite.db";
  private static IDbCommand dbcmd;
  private static IDataReader reader;

  public static void Initiate () {
    IDbConnection _connection = new SqliteConnection (urlDataBase);
    dbcmd = _connection.CreateCommand ();
    _connection.Open ();
  }

  public static void CreateTable (string tableName) {
    string sql = string.Format ("CREATE TABLE IF NOT EXISTS {0} (_id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR (20), score INT);", tableName);
    dbcmd.CommandText = sql;

    dbcmd.ExecuteNonQuery ();
  }

  public static void InsertInto (string tableName, string name, int score) {
    string sql = string.Format ("INSERT INTO {0} (name, score) VALUES ('{1}', {2})", tableName, name, score);
    dbcmd.CommandText = sql;
    dbcmd.ExecuteNonQuery ();
  }

  public static void Delete (string tableName, string name) {
    string sql = string.Format ("DELETE FROM {0} WHERE name='{1}'", tableName, name);
    dbcmd.CommandText = sql;
    dbcmd.ExecuteNonQuery ();
  }

  public static void DropTable (string tableName) {
    string sql = string.Format ("DROP TABLE {0}", tableName);
    dbcmd.CommandText = sql;
    dbcmd.ExecuteNonQuery ();
  }

  public static void UpdateWhere (string tableName, string name, int score) {
    string sql = string.Format ("UPDATE {0} SET score={1} WHERE name='{1}'", tableName, score, name);
    dbcmd.CommandText = sql;
    dbcmd.ExecuteNonQuery ();
  }

  public static string Recuperar () {
    string sqlQuery = "SELECT name, score FROM highscores";
    dbcmd.CommandText = sqlQuery;
    reader = dbcmd.ExecuteReader ();
    while (reader.Read ()) {
      string name = reader.GetString (0);
      int score = reader.GetInt32 (1);
      reader.Close ();
      return ("value = name = " + name + " score = " + score);
    }
    reader.Close ();
    return null;
  }
}