# Online Movie Booking System - User & Booking Workflow

This document explains the end-to-end booking flow implemented in the ASP.NET MVC 5 application.

## 1. System Navigation Flow
1. **Home Page (`/Home/Index`)**:
   - Displays project overview, quick links to Movies, Categories, and Booking.

2. **User Authentication (`/Account/Register` & `/Account/Login`)**:
   - New users register their details into `Tbl_User`.
   - Existing users log in; `User_ID` and user details are stored securely in ASP.NET `Session["User_ID"]`.

## 2. Movie Booking Process (`/Booking/BookMovie`)
1. **Category Selection**:
   - The user selects a movie category (e.g., Action, Comedy, Drama) from a dropdown list populated by `sp_GetAllCategories`.
2. **Cascading Movie Filter**:
   - Selecting a category triggers an AJAX call to `/Booking/GetMoviesByCategory?catId=...`.
   - The movie dropdown dynamically updates to display only movies belonging to the selected category.
3. **Automatic Price Calculation**:
   - When a movie is selected, its ticket rate is fetched.
   - User inputs number of tickets (`no_of_tickets`).
   - Total amount is calculated dynamically using:
     `Total Amount = Rate * No_Of_Tickets`
4. **Booking Confirmation**:
   - On clicking "Confirm Booking", a POST request executes stored procedure `sp_BookMovie`.
   - The booking record is inserted into `Tbl_Booking` with foreign keys (`User_ID`, `Cat_ID`, `Movie_ID`).

## 3. Post-Booking & Management (`/Booking/MyBookings`)
- Users can view all their past and upcoming movie reservations.
- Records show Booking ID, Movie Name, Category, Ticket Count, Total Amount, and Date.
- Users can edit or cancel reservations if needed.
