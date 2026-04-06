#  **Book4You - .NET Project**


Book4You is a web platform for browsing and booking accommodations.
It is built with ASP.NET Core MVC and Entity Framework Core, with role-based functionality for users and admins.

Project Overview
Book4You allows users to discover stays, view accommodation details, and place bookings.
The project includes an admin area for managing users, bookings, and unavailable periods.

## **Implemented Features**
### User authentication (signup, login, profile edit)
### Accommodation listing with filters:
Location
Type
Max price
Guests
Check-in/check-out availability
### Accommodation details page with:
- Image gallery
- Amenities
- Booking form
- Reviews section
### Booking system:
- Create booking
- View personal bookings
- Date overlap validation
### Review system:
- Users can leave one review per accommodation
- Reviews are allowed only after completed stays
- Ratings are calculated from user reviews
### Admin features:
- View all bookings
- Cancel bookings
- Manage users and roles
- Manage unavailable periods

## **Tech Stack**
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (LocalDB)
- Razor Views
- CSS and JavaScript
- Session-based authentication

## **Project Structure**

- Controllers: request handling and business logic
- Models: domain entities and view models
- Data: database context, migrations, seeding
- Views: Razor pages for UI
- wwwroot: static files (CSS, JS, images)
  
## **Getting Started**

Prerequisites
- .NET SDK 10
- SQL Server LocalDB (or another SQL Server instance)

## **Run the project**

1. **Restore dependencies**:
dotnet restore
2. **Apply migrations**:
dotnet ef database update
3. **Run the app**:
dotnet run --project Project.csproj

## **Default admin account**

If seeding is enabled, an admin account is created automatically:

Email: admin@book4you.local

Password: Admin123!

## **Database**
The project uses EF Core migrations and includes seeded data for:

- Amenities
- Accommodations
- Images
- Accommodation amenities
- Admin user

 ## **Current Limitations**
  
- No real payment processing yet
- Notifications and email flows are not implemented
- Owner-specific dashboard is not implemented yet
- Some legal/support pages are placeholder/demo content

 ## **Roadmap**
  
- Owner dashboard and owner-specific accommodation management
- Extended booking lifecycle (confirmed/rejected/completed states)
- Amenities and image management UI in admin/owner flows
- Payment integration
- Email notifications

  
## **Author**:
Ghiluta Denis Cristian

## **License**
This project is for educational purposes
