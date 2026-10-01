# Online Movie Booking System - Database & Architecture Flow

This document details the database schema, stored procedure flow, and ADO.NET data access layer.

## 1. Database Schema (`App_Data/MovieBookingDB.mdf`)

The system uses SQL Server LocalDB with 4 normalized relational tables:
- **`Tbl_Movie_Category`**: Stores movie genres/categories (`Cat_ID` [PK], `Cat_Name`).
- **`Tbl_Movie`**: Stores movie records (`Movie_ID` [PK], `Movie_Name`, `Cat_ID` [FK], `ReleaseDate`, `Rate`).
- **`Tbl_User`**: Stores registered user accounts (`User_ID` [PK], `UserName`, `Email`, `Password`, `ContactNo`, `City`).
- **`Tbl_Booking`**: Stores transaction reservations (`booking_ID` [PK], `User_ID` [FK], `Cat_ID` [FK], `Movie_ID` [FK], `no_of_Tickets`, `amount`, `BookingDate`).

## 2. ADO.NET Data Access Flow

Controllers interact with the database exclusively using pure ADO.NET and Stored Procedures:
1. **Connection**: `SqlConnection` connects using connection string `MovieBookingConn` defined in `Web.config`.
2. **Command**: `SqlCommand` configured with `CommandType.StoredProcedure`.
3. **Parameter Binding**: Strongly-typed `SqlParameter` objects protect against SQL Injection.
4. **Execution Methods**:
   - `ExecuteReader()`: Iterates through row sets for dropdown lists and table views.
   - `ExecuteNonQuery()`: Executes INSERT, UPDATE, and DELETE operations.
   - `ExecuteScalar()`: Retrieves single values such as generated identity IDs or record counts.

## 3. Stored Procedures Workflow
- `sp_GetAllCategories`: Fetches all categories for the initial dropdown.
- `sp_GetMoviesByCategory`: Filters movies dynamically by `Cat_ID`.
- `sp_GetMovieById`: Retrieves specific movie ticket rate for real-time calculation.
- `sp_BookMovie`: Inserts a validated booking transaction into `Tbl_Booking`.
- `sp_GetUserBookings`: Fetches joined booking history for the logged-in user.
