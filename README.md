# Resort Management System

This is a **Resort Management System** that enables users to manage villa bookings, check availability, and handle reservations dynamically. The system supports multiple user roles such as **Admin** and **Customer**, allowing for villa management, booking confirmation, and invoice generation. It is built with `.NET MVC` architecture and uses `MySQL` for database management.

## Features

- **User Roles:**
  - **Admin**: Can manage villas, view booking details, check in/out guests, and cancel bookings.
  - **Customer**: Can view villa details, check availability, and make bookings.
  
- **Booking Management:**
  - Book villas based on availability.
  - Confirm bookings with payment integration (Stripe API).
  - Admin can cancel bookings or update booking statuses.
  - Generate invoices in PDF and Word formats.
  
- **Dynamic Availability:**
  - The system dynamically assigns available villa numbers based on villa type and current occupancy.

- **Stripe Payment Integration:**
  - Integrated with Stripe for payment processing and validation.

## Technologies Used

- **Frontend:** .NET MVC with Razor Views
- **Backend:** .NET (C#)
- **Database:** MySQL
- **Payment Processing:** Stripe API
- **Frontend Libraries:** Bootstrap 5

## Setup Instructions

1. Clone the repository:

   ```bash
   git clone https://github.com/your-username/resort-management-system.git
2. Navigate to the project directory:
    cd resort-management-system
3. Set up the database:
Create a MySQL database and configure the connection string in appsettings.json.
4. Install the required packages:
dotnet restore
5. Run the application:
6. Open your browser and navigate to http://localhost:5000 to access the system.




How to Use
Admin Functions:

Login with admin credentials.
Manage villas (add, update, delete).
View, confirm, or cancel bookings.
Check in and check out guests.
Customer Functions:

View available villas.
Make a booking.
Receive booking confirmation via email after payment.
Future Enhancements
Implement user registration and authentication for customers.
Add more payment gateways.
Enhance the villa search functionality with filters and sorting.
Introduce reporting features for admins.
