using System.Text.Json;

namespace cerberus_pass;

public class PasswordManager
{
  private List<PasswordEntry>? vault;
  private const string vaultFilePath = "vault.cerberus";
  public PasswordManager()
  {
    // vault = new List<PasswordEntry>();
    // vault = new();
    // vault = [];
    if (File.Exists(vaultFilePath))
      LoadVault();
    else
      vault = [];
  }

  public List<PasswordEntry> GetAll() => vault;

  public PasswordEntry? CreateEntry(
    string title,
    string login,
    string password,
    string website = "",
    string note = "")
  {
    if (vault.Any(x => x.Title == title))
    {
      return null;
    }
    var newEntry = new PasswordEntry(
      title,
      login,
      password,
      website,
      note
    );
    vault.Add(newEntry);
    SaveVault();
    return newEntry;
  }

  // GetEntry
  public PasswordEntry GetEntry(string title) =>
    vault.Find(x => x.Title == title);


  // UpdateEntry
  public PasswordEntry UpdateEntry(string titleToChange, PasswordEntry newEntry)
  {
    var indexToUpdate = vault.FindIndex(
      x => x.Title == titleToChange);
    vault[indexToUpdate] = newEntry;
    SaveVault();
    return vault[indexToUpdate];

    // var entryToChange = vault.Find(x => x.Title == titleToChange);
    // entryToChange = newEntry;
  }

  // DeleteEntry
  public bool DeleteEntry(string titleToDelete)
  {
    var success = vault.RemoveAll(x => x.Title == titleToDelete) > 0;
    if (success)
      SaveVault();
    return success;
  }

  private void SaveVault()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true
    };
    var json = JsonSerializer.Serialize(
      vault, options
    );
    File.WriteAllText(vaultFilePath, json);
  }

  // Load from File
  // Wer callt diese Funktion?
  private void LoadVault()
  {
    var json = File.ReadAllText(vaultFilePath);
    vault = JsonSerializer.
      Deserialize<List<PasswordEntry>>(json) ?? [];
    // ?? => Null-Coalescing Operator
    // x = entweder ?? oder
    // ==> wenn "entweder" == null, dann ist x = "oder"
    // decrypt
  }
}