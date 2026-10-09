# RescuePet

An ASP.NET MVC web app for a pet rescue organisation. Visitors can browse pets that need a home, filter them, adopt a pet, post a new pet for adoption, and make a donation.

Built for INF 272 at the University of Pretoria.

## Features

- **Home**: an overview of the rescue and featured pets.
- **Pets**: browse every listed pet and filter by type, breed and location.
- **Adopt**: choose a pet and record the adoption against an adopter.
- **Post a Pet**: list a new pet with details, a short and a long story, and a photo upload.
- **Donations**: make a donation and see a confirmation page.

## Tech stack

- ASP.NET MVC 5 on .NET Framework 4.7.2 (C#)
- SQL Server LocalDB, accessed with ADO.NET (`SqlConnection`)
- Razor views, Bootstrap 5 and jQuery

## Database

`DB_Pets.sql` creates the `DB_Pets` database with these tables: Users, Phones, PetTypes, Breeds, Locations, Pets, Adoptions and Donations. It also loads sample data, including pet photos stored in the database.

## Running locally

1. Open `domRescuePet.sln` in Visual Studio 2022 with the **ASP.NET and web development** workload installed. That workload includes SQL Server LocalDB.
2. Create the database: open `DB_Pets.sql` in Visual Studio (or SQL Server Management Studio), connect to `(localdb)\MSSQLLocalDB` and run the script.
3. Press **F5**. NuGet packages are restored automatically on the first build.

The connection string is `ConnectionString_Pets` in `domRescuePet/Web.config`.

## Project structure

```
domRescuePet/
├── Controllers/HomeController.cs   # All pages and form handling
├── Models/                         # Pet, Breed, Adoption, Donation, etc.
│   └── DefaultDataService.cs       # Database access
├── Views/Home/                     # Index, Pets, Adopt, Post, Donate
└── Content/Images/                 # Site images
DB_Pets.sql                         # Database creation and seed script
```
