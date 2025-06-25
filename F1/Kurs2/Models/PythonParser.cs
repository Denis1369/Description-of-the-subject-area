using System.Diagnostics;
using System.Threading.Tasks;

namespace Kurs2.Services
{
    public class PythonResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }
    }

    public class PythonParser
    {
        private readonly string _pythonPath;
        private readonly string _scriptPath;

        public PythonParser(string pythonPath, string scriptPath)
        {
            _pythonPath = pythonPath;
            _scriptPath = scriptPath;
        }

        public async Task<PythonResult> ExecuteAsync(int raceId, string host, string user, string password, string database)
        {
            var arguments = $"\"{_scriptPath}\" --race-id {raceId} " +
                           $"--host {host} --user {user} --password {password} --database {database}";

            var psi = new ProcessStartInfo(_pythonPath, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) throw new Exception("Не удалось запустить процесс");

            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            var completedTask = await Task.WhenAny(
                process.WaitForExitAsync(cts.Token),
                Task.Delay(TimeSpan.FromMinutes(3), cts.Token)
            );

            if (!completedTask.IsCompletedSuccessfully)
            {
                process.Kill();
                throw new TimeoutException("Таймаут выполнения скрипта");
            }

            return new PythonResult
            {
                ExitCode = process.ExitCode,
                Output = await outputTask,
                Error = await errorTask
            };
        }
    }
}