<Query Kind="Program">
  <Namespace>System.IO.Pipes</Namespace>
</Query>

void Main()
{
	using (var controlPipe = GetExecutionControlPipe())
	using (var resultsPipe = GetResultsPipe())
	{
		var arguments = new[]
		{
			PathToExecutor,
			TenantId,
			ExecutionId
		}
			.StringJoin(" ");
		var pythonExecutorProcess = new Process()
		{
			StartInfo = new ProcessStartInfo()
			{
				FileName = @"python.exe",
				Arguments = arguments,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			}
		};
		
		pythonExecutorProcess.Start();
		controlPipe.WaitForConnection();
		
		using (var streamWriter = new StreamWriter(controlPipe, Utf8WithoutBOM))
		{
			streamWriter.WriteLine(@"C:\Caprica\executions\test_tenant_test_execution\");
			streamWriter.WriteLine("test_algorithm");
			streamWriter.WriteLine("{\"test_parameter\": \"test_value\"}");
		}
		
		resultsPipe.WaitForConnection();
		var results = new List<string>();
		using (var streamReader = new StreamReader(resultsPipe, Utf8WithoutBOM))
		{
			while (!streamReader.EndOfStream)
				results.Add(streamReader.ReadLine());
		}
		
		results.Dump();
			
		pythonExecutorProcess.WaitForExit();
		pythonExecutorProcess
			.StandardOutput
			.ReadToEnd()
			.Dump();
		pythonExecutorProcess
			.StandardError
			.ReadToEnd()
			.Dump();
		
		resultsPipe.Close();
		controlPipe.Close();
	}
}

string PathToExecutor => @"D:\vasutovich\Documents\PipedPython\executor.py";

string TenantId => "testTenant";
string ExecutionId => "testExecution001";

private static readonly Encoding Utf8WithoutBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

NamedPipeServerStream GetExecutionControlPipe()
{
	return new NamedPipeServerStream(
		$"{TenantId}_{ExecutionId}_control",
		PipeDirection.Out,
		maxNumberOfServerInstances: 1,
		PipeTransmissionMode.Byte);
}

NamedPipeServerStream GetResultsPipe()
{
	return new NamedPipeServerStream(
		$"{TenantId}_{ExecutionId}_results",
		PipeDirection.In,
		maxNumberOfServerInstances: 1,
		PipeTransmissionMode.Byte);
}