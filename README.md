# recipesAtYourFingertipsRev0
This is the Rev 0 of the final project for openU
Remeber, if working on a new codespace, it is very well posiablle that we need to add again locally the user secrets (which contain the DB connection creditials, google cloud auth0 creditials)

You need to excute this commands from the main directory of the project:

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=ep-royal-dawn-b2k5kx9x.c-6.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=<the password for your neon postgerSQL DB, you have kept it on your computer and can also rotate it from neon web console where you have logged using your github account>;SSL Mode=Require"

dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"

dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"

you can print to terminal all the current secrets to verify by running
dotnet user-secrets list


this app uses the following service providers:
digitalOcean - a platform to deploy our app to a public domain. login via github, payment method was set to be apple pay.
Neon - provides our postgerSQL DB - login via github
Google cloud console - for auth0 - login via your google account, credit card was set as the payment method