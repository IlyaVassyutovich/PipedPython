<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.IO.Pipes</Namespace>
</Query>

var pythonExecutorProcess = new Process()
{
	StartInfo = new ProcessStartInfo()
	{
		FileName = @"python.exe",
		Arguments = @"D:\vasutovich\Documents\PipedPython\executor.py",
		RedirectStandardOutput = true,
		RedirectStandardError = true,
		UseShellExecute = false,
		CreateNoWindow = true
	}
};

pythonExecutorProcess.Start();
pythonExecutorProcess.WaitForExit();
pythonExecutorProcess
	.StandardOutput
	.ReadToEnd()
	.Dump();
pythonExecutorProcess
	.StandardError
	.ReadToEnd()
	.Dump();
