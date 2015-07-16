using System;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Types;

namespace wrapper
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ConsoleDriver.Run(new SocketIO());
		}
	}

	class SocketIO : ILibrary
	{
		public void Preprocess (Driver driver, ASTContext ctx)
		{
		}

		public void Postprocess (Driver driver, ASTContext ctx)
		{
		}

		public void Setup (Driver driver)
		{
			var options = driver.Options;
			options.Verbose = true;
			options.Quiet = false;
			options.GeneratorKind = GeneratorKind.CSharp;
			options.LibraryName = "SocketIO";

			options.addIncludeDirs ("/Users/matthew/projects/Socket.IO.Client/building/wrapper-cppsharp/src-out");
			options.Headers.Add("wrap_client.h");
			options.Headers.Add("wrap_message.h");
			options.Headers.Add("wrap_socket.h");

			options.addLibraryDirs ("/Users/matthew/projects/Socket.IO.Client/building/out/lib");
			options.Libraries.Add("libwrapper.a");
			options.StripLibPrefix = false;

			options.OutputDir = "/Users/matthew/projects/Socket.IO.Client/building/wrapper-cppsharp/gen";
		}

		public void SetupPasses (Driver driver)
		{
		}
	}
}
