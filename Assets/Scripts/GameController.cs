using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
  private Transform canvas, containers, platformContainer, sqliteContainer;
  private TextMeshProUGUI consoleText;
  private RuntimePlatform platform;
  public enum PlatformDATA {
    PlatformUnity,
    PlatformSystem,
    OSVersion
  }
  public enum SQLiteDATA {
    SQLConnectionString,
    SQLInitiation,
    SQLCreateTable,
    SQLInsertInto,
    SQLUpdate,
    SQLDelete,
    SQLDropTable
  }

  private static readonly Color dangerColor = new Color32 (0xCC, 0, 0, 0xFF);
  private static readonly Color warningColor = new Color32 (0xFF, 0x88, 0, 0xFF);
  private static readonly Color successColor = new Color32 (0, 0x7E, 0x33, 0xFF);

  void Awake () {
    if (Application.isMobilePlatform) {
      Screen.SetResolution (720, 1280, false);
    }

    Screen.orientation = ScreenOrientation.Portrait;
    Screen.autorotateToLandscapeLeft = false;
    Screen.autorotateToLandscapeRight = false;
    Screen.autorotateToPortrait = true;

    platform = Application.platform;

    canvas = GameObject.Find ("Canvas").transform;
    containers = canvas.Find ("Containers");
    Transform contentContainer = containers.Find ("ContentContainer");
    platformContainer = contentContainer.GetChild (0).GetChild (1);
    sqliteContainer = contentContainer.GetChild (1).GetChild (1);
    consoleText = contentContainer.GetChild (2).GetChild (1).GetChild (0).GetChild (1).GetComponent<TextMeshProUGUI> ();
  }

  void Start () {
    containers.Find ("PlatformContainer").GetChild (0).GetComponent<Image> ().sprite = GetPlatformLogo ();

    PlatformTest ();
    SQLiteTest ();
  }

  private Sprite GetPlatformLogo () {
    switch (platform) {
      case RuntimePlatform.Android:
        return Resources.Load<Sprite> ("android");
      case RuntimePlatform.LinuxPlayer:
        return Resources.Load<Sprite> ("linux");
      case RuntimePlatform.LinuxEditor:
        return Resources.Load<Sprite> ("linux");
      case RuntimePlatform.IPhonePlayer:
        return Resources.Load<Sprite> ("apple");
      case RuntimePlatform.OSXPlayer:
        return Resources.Load<Sprite> ("apple");
      case RuntimePlatform.OSXEditor:
        return Resources.Load<Sprite> ("apple");
      case RuntimePlatform.WindowsEditor:
        return Resources.Load<Sprite> ("windows");
      case RuntimePlatform.WebGLPlayer:
        return Resources.Load<Sprite> ("web");
      case RuntimePlatform.XboxOne:
        return Resources.Load<Sprite> ("xbox-one");
      case RuntimePlatform.XBOX360:
        return Resources.Load<Sprite> ("xbox-360");
      case RuntimePlatform.PS4:
        return Resources.Load<Sprite> ("ps4");
      case RuntimePlatform.PS3:
        return Resources.Load<Sprite> ("ps3");
      default:
        return Resources.Load<Sprite> ("default-os");
    }
  }

  void PlatformTest () {
    // Test Platform data
    SetText (PlatformDATA.PlatformUnity, Application.platform.ToString ());
    SetText (PlatformDATA.PlatformSystem, Environment.OSVersion.Platform.ToString ());
    SetText (PlatformDATA.OSVersion, Environment.OSVersion.VersionString);
  }

  void SQLiteTest () {
    // Test SQLite initiation
    try {
      SQLite.Initiate ();
      SetText (SQLiteDATA.SQLConnectionString, "Success");
      SetTextColor (SQLiteDATA.SQLConnectionString, successColor);

      SetText (SQLiteDATA.SQLInitiation, "Success");
      SetTextColor (SQLiteDATA.SQLInitiation, successColor);
      Debug.Log ("SQLite initiated with success");
    } catch (ArgumentException exception) {
      SetText (SQLiteDATA.SQLConnectionString, "Failed");
      SetTextColor (SQLiteDATA.SQLConnectionString, dangerColor);

      onSQLiteFail (SQLiteDATA.SQLInitiation);
      Debug.LogError ("SQLiteConnectionString: " + exception.Message);
      consoleText.text = exception.ToString ();
      return;
    } catch (SqliteException exception) {
      SetText (SQLiteDATA.SQLConnectionString, "Success");
      SetTextColor (SQLiteDATA.SQLConnectionString, successColor);

      SetText (SQLiteDATA.SQLInitiation, "Failed");
      SetTextColor (SQLiteDATA.SQLInitiation, dangerColor);

      onSQLiteFail (SQLiteDATA.SQLCreateTable);
      Debug.LogError ("SQLCreateTable: " + exception.Message);
      consoleText.text = exception.ToString ();
      return;
    } catch (Exception exception) {
      SetText (SQLiteDATA.SQLConnectionString, "Failed");
      SetTextColor (SQLiteDATA.SQLConnectionString, dangerColor);

      SetText (SQLiteDATA.SQLInitiation, "Failed");
      SetTextColor (SQLiteDATA.SQLInitiation, dangerColor);

      onSQLiteFail (SQLiteDATA.SQLCreateTable);
      Debug.LogError ("SQLException: " + exception.Message);
      consoleText.text = exception.ToString ();
      return;
    }

    // Test SQLite table creation
    try {
      SQLite.CreateTable ("demo_table");

      SetText (SQLiteDATA.SQLCreateTable, "Success");
      SetTextColor (SQLiteDATA.SQLCreateTable, successColor);
      Debug.Log ("SQLite table created with success");
    } catch (Exception exception) {
      SetText (SQLiteDATA.SQLCreateTable, "Failed");
      SetTextColor (SQLiteDATA.SQLCreateTable, dangerColor);

      onSQLiteFail (SQLiteDATA.SQLInsertInto);
      Debug.LogError ("SQLException: " + exception.Message);
      consoleText.text = exception.ToString ();
      return;
    }

    // Test SQLite insert into
    try {
      SQLite.InsertInto ("demo_table", "demo_name", 0);

      SetText (SQLiteDATA.SQLInsertInto, "Success");
      SetTextColor (SQLiteDATA.SQLInsertInto, successColor);
      Debug.Log ("SQLite insert into with success");
    } catch (Exception exception) {
      SetText (SQLiteDATA.SQLInsertInto, "Failed");
      SetTextColor (SQLiteDATA.SQLInsertInto, dangerColor);

      onSQLiteFail (SQLiteDATA.SQLUpdate);
      Debug.LogError ("SQLException: " + exception.Message);
      consoleText.text = exception.ToString ();
      return;
    }

    // Test SQLite update
    try {
      SQLite.UpdateWhere ("demo_table", "demo_name", 1);

      SetText (SQLiteDATA.SQLUpdate, "Success");
      SetTextColor (SQLiteDATA.SQLUpdate, successColor);
      Debug.Log ("SQLite update with success");
    } catch (Exception exception) {
      SetText (SQLiteDATA.SQLUpdate, "Failed");
      SetTextColor (SQLiteDATA.SQLUpdate, dangerColor);

      onSQLiteFail (SQLiteDATA.SQLDelete);
      Debug.LogError ("SQLException: " + exception.Message);
      consoleText.text = exception.ToString ();
      return;
    }

    // Test SQLite delete
    try {
      SQLite.Delete ("demo_table", "demo_name");

      SetText (SQLiteDATA.SQLDelete, "Success");
      SetTextColor (SQLiteDATA.SQLDelete, successColor);
      Debug.Log ("SQLite delete with success");
    } catch (Exception exception) {
      SetText (SQLiteDATA.SQLDelete, "Failed");
      SetTextColor (SQLiteDATA.SQLDelete, dangerColor);

      onSQLiteFail (SQLiteDATA.SQLDropTable);
      Debug.LogError ("SQLException: " + exception.Message);
      consoleText.text = exception.ToString ();
      return;
    }

    // Test SQLite drop table
    try {
      SQLite.DropTable ("demo_table");

      SetText (SQLiteDATA.SQLDropTable, "Success");
      SetTextColor (SQLiteDATA.SQLDropTable, successColor);
      Debug.Log ("SQLite drop table with success");
    } catch (Exception exception) {
      SetText (SQLiteDATA.SQLDropTable, "Failed");
      SetTextColor (SQLiteDATA.SQLDropTable, dangerColor);

      Debug.LogError ("SQLException: " + exception.Message);
      consoleText.text = exception.ToString ();
      return;
    }
  }

  private void onSQLiteFail (SQLiteDATA startData) {
    int startIndex = (int) startData;
    int[] arr = Enumerable.Range (startIndex, sqliteContainer.childCount - startIndex).ToArray ();
    foreach (int index in arr) {
      SetText ((SQLiteDATA) index, "Aborted");
      SetTextColor ((SQLiteDATA) index, warningColor);
    }
  }

  private void SetText (PlatformDATA data, string info) {
    platformContainer.GetChild ((int) data).GetChild (1).GetComponent<TextMeshProUGUI> ().text = info;
    platformContainer.GetChild ((int) data).GetChild (1).GetComponent<TextMeshProUGUI> ().color = Color.white;
    platformContainer.GetChild ((int) data).GetChild (1).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Normal;
  }

  private void SetTextColor (PlatformDATA data, Color color) {
    platformContainer.GetChild ((int) data).GetChild (1).GetComponent<TextMeshProUGUI> ().color = color;
  }

  private void SetText (SQLiteDATA data, string info) {
    sqliteContainer.GetChild ((int) data).GetChild (1).GetComponent<TextMeshProUGUI> ().text = info;
    sqliteContainer.GetChild ((int) data).GetChild (1).GetComponent<TextMeshProUGUI> ().color = Color.white;
    sqliteContainer.GetChild ((int) data).GetChild (1).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Normal;
  }

  private void SetTextColor (SQLiteDATA data, Color color) {
    sqliteContainer.GetChild ((int) data).GetChild (1).GetComponent<TextMeshProUGUI> ().color = color;
  }

  public void ClearConsole () {
    consoleText.text = "";
  }
}