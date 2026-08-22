# recipesAtYourFingertipsRev0
This is the Rev 0 of the final project for openU
Remeber, if working on a new codespace, it is very well posiablle that we need to add again locally the user secrets (which contain the DB connection creditials)

You need to excute this command from the main directory of the project:
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=ep-royal-dawn-b2k5kx9x.c-6.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=<the password for your neon postgerSQL DB, you have kept it on your computer and can also rotate it from neon web console where you have logged using your github account>;SSL Mode=Require"
