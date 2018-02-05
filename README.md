# How to get started

First, this project requires Visual Studio 2017 with the ASP.NET and web tools workload.

For detailed instructions, see [the blog post for this project](https://blogs.msdn.microsoft.com/webdev/2018/02/05/learn-how-to-do-image-recognition-with-cognitive-services-and-asp-net), which walks through everything with pretty pictures.

1. Get an Azure subscription (it's free to start, and cheap for something as simple as this).
2. Create a Cognitive Services resource.
3. Create an Azure Storage resource.

## API keys

Open the **Web.Config** file and modify the placeholder values with API keys that you grabbed from the Azure Portal.

## Publishing to Azure

The easiest way to publish is via Right-click > Publish to Azure. However, you can publish a number of ways, including by configuring this project to run via a [CI/CD pipeline](https://docs.microsoft.com/en-us/vsts/build-release/apps/aspnet/build-aspnet-4?tabs=vsts).
