// RedPanda

using Common;

RedPandaServer server = new RedPandaServer();
await server.InitializeAsync();
Console.WriteLine("Press any key to stop the server");
Console.ReadLine();
