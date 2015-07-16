using ObjCRuntime;

[assembly: LinkWith(
	"../libwrapper.a", 
	LinkTarget = LinkTarget.Arm64 | LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Simulator | LinkTarget.Simulator64,
	ForceLoad = true,
	IsCxx = true)]
[assembly: LinkWith(
	"../libsioclient.a", 
	LinkTarget = LinkTarget.Arm64 | LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Simulator | LinkTarget.Simulator64,
	ForceLoad = true,
	IsCxx = true)]
[assembly: LinkWith(
	"../libboost.a", 
	LinkTarget = LinkTarget.Arm64 | LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Simulator | LinkTarget.Simulator64,
	ForceLoad = true,
	IsCxx = true)]
