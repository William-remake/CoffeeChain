using CoffeeChain.Api;

var app = ApiHost.Build(args, "http://127.0.0.1:5187");
await app.RunAsync();