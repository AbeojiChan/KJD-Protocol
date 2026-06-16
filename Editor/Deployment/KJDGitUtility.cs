#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;

namespace KJD.Framework.Deployment
{
    public static class KJDGitUtility
    {
        public static string RunCommand(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            using (Process process = Process.Start(processInfo))
            {
                if (process == null) return "Error: Terminal down.";

                string output = process.StandardOutput.ReadToEnd().Trim();
                string error = process.StandardError.ReadToEnd().Trim();
                process.WaitForExit();

                return string.IsNullOrEmpty(error) || error.StartsWith("Note:") ? output : $"Error: {error}";
            }
        }
    }
}
#endif