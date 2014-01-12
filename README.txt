This is a simple scripting library to help with generating plots from ROOT files in some
sort of reproducable way.

Early alpha - just trying to see if this method even works.

Deployment: this is deployed as a 1-click app. See:
	https://plotlingo.codeplex.com/releases/view/115518

To deploy:
  Codeplex docs can be found here: https://codeplex.codeplex.com/wikipage?title=ClickOnce

	1. Set the build to Release
	2. Look at the properties of the app and make sure that the version number is right, along with all manifest info.
	2. Right click on the "PlotLingoConsole" project and build.
	3. Right click and choose the publish option.
		Make sure the website URL is as above
		Make sure you publish to an offline spot somewhere you know.
	4. In the publish directory, select the three files there, and send to zip file from the Explorer context menu.
	5. Upload.