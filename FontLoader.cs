using System;
using System.IO;
using SplashKitSDK;
using static SplashKitSDK.SplashKit;

public static class FontLoader
{
    private static readonly string[] FontCandidates =
    {
        @"C:\Windows\Fonts\arial.ttf",
        @"C:\Windows\Fonts\Arial.ttf",
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "/usr/share/fonts/truetype/liberation2/LiberationSans-Regular.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
        "./DejaVuSans.ttf",
        "./arial.ttf"
    };

    public static void EnsureLoaded()
    {
        string? fontPath = Array.Find(FontCandidates, File.Exists);

        if (string.IsNullOrWhiteSpace(fontPath))
        {
            try
            {
                LoadFont("Arial", "arial.ttf");
                LoadFont("arial", "arial.ttf");
            }
            catch
            {
                // Ignore missing font files; SplashKit may still have a default font available.
            }

            return;
        }

        try
        {
            LoadFont("Arial", fontPath);
            LoadFont("arial", fontPath);
            LoadFont("Default", fontPath);
        }
        catch
        {
            // Best-effort fallback: keep the app running even if the font cannot be reloaded.
        }
    }
}
