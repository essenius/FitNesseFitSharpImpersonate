# FitNesseFitSharpImpersonate
Fixture to impersonate another user (e.g. for testing REST APIs under a different account).
Note that this won't work with Selenium or UI Automation, as those spawn other processes which use the original user.

# Getting Started
1. Download FitNesse (http://fitnesse.org) and install it to C:\Apps\FitNesse
2. Download FitSharp (https://github.com/jediwhale/fitsharp) and install it to C:\Apps\FitNesse\FitSharp.
3. Clone the repo to a local folder (C:\Data\FitNesseDemo)
4. Update plugins.properties to point to the FitSharp folder (if you took other folders than suggested)
5. Build all projects in the solution ImpersonmationFixture (Release)
6. Ensure you have Java installed (1.8 or higher)
7. Start FitNesse with the root repo folder as the data folder as well as the current directory:

	cd /D C:\Data\FitNesseDemo

	java -jar C:\Apps\FitNesse\fitnesse-standalone.jar -d .
    
8. Open a browser and enter the URL http://localhost:8080/FitSharpDemos.ImpersonationTest?test

# Contribute
Enter an issue or provide a pull request.