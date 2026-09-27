using System;
using System.IO;

public static class AppPaths
{
    public static readonly string InstallDirectory = System.AppContext.BaseDirectory;

    private static readonly Lazy<string> _databaseFile =
        new Lazy<string>(() => Resolve("Data/StockData.db", "StockData.db", null));

    private static readonly Lazy<string> _walletFile =
        new Lazy<string>(() => Resolve("wallet.txt", "wallet.txt", "0"));

    public static string DatabaseFile => _databaseFile.Value;

    public static string WalletFile => _walletFile.Value;

    private static string Resolve(string installRelativePath, string dataFileName, string? defaultContent)
    {
        string installPath = Path.Combine(InstallDirectory, installRelativePath);
        string installDirectory = Path.GetDirectoryName(installPath) ?? InstallDirectory;

        if (Directory.Exists(installDirectory) && CanWrite(installDirectory))
        {
            return installPath;
        }

        string dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StockApp");
        Directory.CreateDirectory(dataDirectory);

        string dataPath = Path.Combine(dataDirectory, dataFileName);

        if (!File.Exists(dataPath))
        {
            if (File.Exists(installPath))
            {
                File.Copy(installPath, dataPath);
            }
            else if (defaultContent != null)
            {
                File.WriteAllText(dataPath, defaultContent);
            }
        }

        return dataPath;
    }

    private static bool CanWrite(string directory)
    {
        try
        {
            string probe = Path.Combine(directory, ".stockapp-write-test.tmp");
            File.WriteAllText(probe, string.Empty);
            File.Delete(probe);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
