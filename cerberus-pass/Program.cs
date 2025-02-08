using cerberus_pass;

var manager = new PasswordManager();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Willkommen zu Cerberus-Pass!");
Console.ResetColor();

do
{

  Console.WriteLine("Wählen was du tun willst:");

  Console.WriteLine("""
    1. Password-Liste ausgeben
    2. Password mit ID ausgeben
    3. Neues Password bearbeiten
    4. Vorhandenes Password bearbeiten
    5. Password löschen
""");

  var userInput = Console.ReadLine();

  int menuChoice;

  if (!int.TryParse(userInput, out menuChoice))
    continue;

  switch ((MenuOptions)menuChoice)
  {
    case MenuOptions.List:
      var vault = manager.GetAll();
      foreach (var item in vault)
      {
        Console.WriteLine(item);
      }
      break;
    case MenuOptions.GetOne:
      // todo: Exception wenn title nicht existiert
      Console.WriteLine("Welchen Eintrag willst du ansehen? (Title):");
      var titleToPrint = Console.ReadLine();
      var entry = manager.GetEntry(titleToPrint);
      Console.WriteLine(entry + $"\t{entry.Password}");
      break;
    case MenuOptions.Create:
      Console.WriteLine("Gebe einen Titel für den Eintrag an:");
      var title = Console.ReadLine();
      Console.WriteLine("Gebe einen Login für den Eintrag an:");
      var login = Console.ReadLine();
      Console.WriteLine("Gebe ein Passwort für den Eintrag an:");
      var password = Console.ReadLine();
      var newEntry = manager.CreateEntry(title, login, password);
      if (newEntry is null)
      {
        Console.WriteLine($"Eintrag mit {title} existiert bereits.\nWolltest du diesen Updaten? Oder erstelle einen neuen mit einem anderen Titel.");
      }
      else
      {
        Console.WriteLine("Neuer Eintrag erfolgreich erstellt:");
        Console.WriteLine(newEntry); // Gibt Type aus;
      }
      break;
    case MenuOptions.Update:
      Console.WriteLine("Welchen Eintrag willst du ändern? (Title):");
      var title_to_change = Console.ReadLine();
      Console.WriteLine(
        "Gebe einen neuen Titel für den Eintrag an (Leer um nichts zu ändern):");
      var new_title = Console.ReadLine();
      Console.WriteLine(
        "Gebe einen neuen Login für den Eintrag an (Leer um nichts zu ändern):");
      var new_login = Console.ReadLine();
      Console.WriteLine(
        "Gebe ein neues Passwort für den Eintrag an (Leer um nichts zu ändern):");
      var new_password = Console.ReadLine();
      var oldEntry = manager.GetEntry(title_to_change);
      var updatedEntry = manager.UpdateEntry(title_to_change, new PasswordEntry(
        String.IsNullOrEmpty(new_title) ? oldEntry.Title : new_title,
        String.IsNullOrEmpty(new_login) ? oldEntry.Login : new_login,
        String.IsNullOrEmpty(new_password) ? oldEntry.Password : new_password
      ));
      Console.WriteLine($"Eintrag {updatedEntry.Title} wurde erfolgreich aktuallisiert.");
      break;
    case MenuOptions.Delete:
      Console.WriteLine("Welchen Eintrag willst du Löschen? (Title):");
      var titleToDelete = Console.ReadLine();
      if (manager.DeleteEntry(titleToDelete))
        Console.WriteLine($"Eintrag {titleToDelete} wurde erfolgreich entfernt");
      else
        Console.WriteLine(
          $"Fehler beim löschen des Eintrags: {titleToDelete} wurde nicht gefunden!");
      break;
    default:
      // Fehler anzeigen -> Eingabe-Hint (1-5)
      // Eingabe wiederholen
      Console.WriteLine("Falsche Eingabe! Valide Optionen => 1-5");
      break;
  }
  Console.ReadKey();
  Console.Clear();
} while (true);