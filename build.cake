var target = Argument("target", "Deploy-Bot");
var config = Argument("config", "Release");

Task("Deploy-Bot")
  .Does(() =>
  {
    DotNetCorePublish("DragonBot", new DotNetCorePublishSettings
    {
      Configuration = config,
      SelfContained = true
    });
  });

RunTarget(target);
