// See https://aka.ms/new-console-template for more information
using repo2srv;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

Console.WriteLine("Hello, World!");

var content = File.ReadAllText(args[0]);

var settings = JsonSerializer.Deserialize<Root>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

var h = new repo2srv.Handler(settings.Apps);
Console.WriteLine("Preparing");
h.Prepare();
Console.WriteLine("Prepared");
Console.WriteLine("Compressing");
h.Compress();
Console.WriteLine("Compressed");

Console.WriteLine("Copying");
h.CopyToDest();
Console.WriteLine("Copied");

Console.WriteLine("press ay key to exit");
Console.ReadLine();