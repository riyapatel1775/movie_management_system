-- ==============================================================================
-- PROJECT: Online Movie Booking Management System
-- DATABASE: MovieBookingDB.mdf (Internal LocalDB in App_Data)
-- PURPOSE: Table Creation, Foreign Keys, Sample Data & Stored Procedures
-- ==============================================================================

-- ==============================================================================
-- 1. DROP EXISTING TABLES (IF RE-RUNNING SCRIPT)
-- ==============================================================================
IF OBJECT_ID('dbo.Tbl_Booking', 'U') IS NOT NULL DROP TABLE dbo.Tbl_Booking;
IF OBJECT_ID('dbo.Tbl_Movie', 'U') IS NOT NULL DROP TABLE dbo.Tbl_Movie;
IF OBJECT_ID('dbo.Tbl_Movie_Category', 'U') IS NOT NULL DROP TABLE dbo.Tbl_Movie_Category;
IF OBJECT_ID('dbo.Tbl_User', 'U') IS NOT NULL DROP TABLE dbo.Tbl_User;
GO

-- ==============================================================================
-- 2. CREATE TABLES
-- ==============================================================================

-- Table 1: Tbl_Movie_Category
CREATE TABLE Tbl_Movie_Category
(
    Cat_ID INT PRIMARY KEY IDENTITY(1,1),
    Cat_Type VARCHAR(100) NOT NULL
);
GO

-- Table 2: Tbl_Movie
CREATE TABLE Tbl_Movie
(
    Movie_ID INT PRIMARY KEY IDENTITY(1,1),
    Movie_name VARCHAR(200) NOT NULL,
    Release_Date DATE NOT NULL,
    Cat_ID INT NOT NULL,
    rate DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Movie_Category FOREIGN KEY (Cat_ID) 
        REFERENCES Tbl_Movie_Category(Cat_ID)
);
GO

-- Table 3: Tbl_User
CREATE TABLE Tbl_User
(
    User_ID INT PRIMARY KEY IDENTITY(1,1),
    User_Name VARCHAR(100) NOT NULL,
    Email_ID VARCHAR(150) NOT NULL UNIQUE,
    User_password VARCHAR(100) NOT NULL,
    City VARCHAR(100),
    PhoneNo VARCHAR(20)
);
GO

-- Table 4: Tbl_Booking
CREATE TABLE Tbl_Booking
(
    booking_ID INT PRIMARY KEY IDENTITY(1,1),
    User_ID INT NOT NULL,
    Cat_ID INT NOT NULL,
    Movie_ID INT NOT NULL,
    no_of_Tickets INT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Booking_User FOREIGN KEY (User_ID) 
        REFERENCES Tbl_User(User_ID),
    CONSTRAINT FK_Booking_Category FOREIGN KEY (Cat_ID) 
        REFERENCES Tbl_Movie_Category(Cat_ID),
    CONSTRAINT FK_Booking_Movie FOREIGN KEY (Movie_ID) 
        REFERENCES Tbl_Movie(Movie_ID)
);
GO

-- ==============================================================================
-- 3. INSERT INITIAL SAMPLE DATA
-- ==============================================================================

-- Categories
INSERT INTO Tbl_Movie_Category (Cat_Type) VALUES ('Action');
INSERT INTO Tbl_Movie_Category (Cat_Type) VALUES ('Comedy');
INSERT INTO Tbl_Movie_Category (Cat_Type) VALUES ('Sci-Fi');
INSERT INTO Tbl_Movie_Category (Cat_Type) VALUES ('Drama');
INSERT INTO Tbl_Movie_Category (Cat_Type) VALUES ('Animation');
GO

-- Movies
INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
VALUES ('Avengers: Endgame', '2024-05-10', 1, 250.00);

INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
VALUES ('Fast X', '2024-06-15', 1, 220.00);

INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
VALUES ('The Hangover', '2024-04-20', 2, 180.00);

INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
VALUES ('Johnny English', '2024-07-01', 2, 160.00);

INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
VALUES ('Interstellar', '2024-03-12', 3, 280.00);

INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
VALUES ('Inception', '2024-08-05', 3, 260.00);

INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
VALUES ('Oppenheimer', '2024-02-14', 4, 240.00);

INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
VALUES ('Spider-Man: Across the Spider-Verse', '2024-09-01', 5, 200.00);
GO

-- Sample User for testing (Email: rahul@example.com, Password: 123456)
INSERT INTO Tbl_User (User_Name, Email_ID, User_password, City, PhoneNo)
VALUES ('Rahul Sharma', 'rahul@example.com', '123456', 'Mumbai', '9876543210');
GO

-- ==============================================================================
-- 4. STORED PROCEDURES - CATEGORY MODULE
-- ==============================================================================

-- 1. sp_GetCategories
IF OBJECT_ID('dbo.sp_GetCategories', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetCategories;
GO
CREATE PROCEDURE sp_GetCategories
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Cat_ID, Cat_Type 
    FROM Tbl_Movie_Category
    ORDER BY Cat_Type ASC;
END
GO

-- 2. sp_GetCategoryById
IF OBJECT_ID('dbo.sp_GetCategoryById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetCategoryById;
GO
CREATE PROCEDURE sp_GetCategoryById
    @Cat_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Cat_ID, Cat_Type 
    FROM Tbl_Movie_Category
    WHERE Cat_ID = @Cat_ID;
END
GO

-- 3. sp_AddCategory
IF OBJECT_ID('dbo.sp_AddCategory', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AddCategory;
GO
CREATE PROCEDURE sp_AddCategory
    @Cat_Type VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Tbl_Movie_Category (Cat_Type)
    VALUES (@Cat_Type);
END
GO

-- 4. sp_UpdateCategory
IF OBJECT_ID('dbo.sp_UpdateCategory', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_UpdateCategory;
GO
CREATE PROCEDURE sp_UpdateCategory
    @Cat_ID INT,
    @Cat_Type VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Tbl_Movie_Category
    SET Cat_Type = @Cat_Type
    WHERE Cat_ID = @Cat_ID;
END
GO

-- 5. sp_DeleteCategory
IF OBJECT_ID('dbo.sp_DeleteCategory', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_DeleteCategory;
GO
CREATE PROCEDURE sp_DeleteCategory
    @Cat_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Tbl_Movie WHERE Cat_ID = @Cat_ID)
    BEGIN
        RAISERROR('Cannot delete category because movies are assigned to it.', 16, 1);
        RETURN;
    END

    DELETE FROM Tbl_Movie_Category
    WHERE Cat_ID = @Cat_ID;
END
GO

-- ==============================================================================
-- 5. STORED PROCEDURES - MOVIE MODULE
-- ==============================================================================

-- 6. sp_GetMovies
IF OBJECT_ID('dbo.sp_GetMovies', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetMovies;
GO
CREATE PROCEDURE sp_GetMovies
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        m.Movie_ID,
        m.Movie_name,
        m.Release_Date,
        m.Cat_ID,
        c.Cat_Type,
        m.rate
    FROM Tbl_Movie m
    INNER JOIN Tbl_Movie_Category c ON m.Cat_ID = c.Cat_ID
    ORDER BY m.Movie_ID DESC;
END
GO

-- 7. sp_GetMovieById
IF OBJECT_ID('dbo.sp_GetMovieById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetMovieById;
GO
CREATE PROCEDURE sp_GetMovieById
    @Movie_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        m.Movie_ID,
        m.Movie_name,
        m.Release_Date,
        m.Cat_ID,
        c.Cat_Type,
        m.rate
    FROM Tbl_Movie m
    INNER JOIN Tbl_Movie_Category c ON m.Cat_ID = c.Cat_ID
    WHERE m.Movie_ID = @Movie_ID;
END
GO

-- 8. sp_AddMovie
IF OBJECT_ID('dbo.sp_AddMovie', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AddMovie;
GO
CREATE PROCEDURE sp_AddMovie
    @Movie_name VARCHAR(200),
    @Release_Date DATE,
    @Cat_ID INT,
    @rate DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Tbl_Movie (Movie_name, Release_Date, Cat_ID, rate)
    VALUES (@Movie_name, @Release_Date, @Cat_ID, @rate);
END
GO

-- 9. sp_UpdateMovie
IF OBJECT_ID('dbo.sp_UpdateMovie', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_UpdateMovie;
GO
CREATE PROCEDURE sp_UpdateMovie
    @Movie_ID INT,
    @Movie_name VARCHAR(200),
    @Release_Date DATE,
    @Cat_ID INT,
    @rate DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Tbl_Movie
    SET Movie_name = @Movie_name,
        Release_Date = @Release_Date,
        Cat_ID = @Cat_ID,
        rate = @rate
    WHERE Movie_ID = @Movie_ID;
END
GO

-- 10. sp_DeleteMovie
IF OBJECT_ID('dbo.sp_DeleteMovie', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_DeleteMovie;
GO
CREATE PROCEDURE sp_DeleteMovie
    @Movie_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Tbl_Booking WHERE Movie_ID = @Movie_ID)
    BEGIN
        SELECT 0 AS Result, 'Cannot delete this movie because bookings exist.' AS Message;
        RETURN;
    END

    DELETE FROM Tbl_Movie
    WHERE Movie_ID = @Movie_ID;

    SELECT 1 AS Result, 'Movie deleted successfully.' AS Message;
END
GO

-- 11. sp_GetMoviesByCategory
IF OBJECT_ID('dbo.sp_GetMoviesByCategory', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetMoviesByCategory;
GO
CREATE PROCEDURE sp_GetMoviesByCategory
    @Cat_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        m.Movie_ID,
        m.Movie_name,
        m.Release_Date,
        m.Cat_ID,
        c.Cat_Type,
        m.rate
    FROM Tbl_Movie m
    INNER JOIN Tbl_Movie_Category c ON m.Cat_ID = c.Cat_ID
    WHERE m.Cat_ID = @Cat_ID
    ORDER BY m.Movie_name ASC;
END
GO

-- 12. sp_GetMovieRate
IF OBJECT_ID('dbo.sp_GetMovieRate', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetMovieRate;
GO
CREATE PROCEDURE sp_GetMovieRate
    @Movie_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT rate
    FROM Tbl_Movie
    WHERE Movie_ID = @Movie_ID;
END
GO

-- ==============================================================================
-- 6. STORED PROCEDURES - USER & AUTHENTICATION MODULE
-- ==============================================================================

-- 13. sp_RegisterUser
IF OBJECT_ID('dbo.sp_RegisterUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_RegisterUser;
GO
CREATE PROCEDURE sp_RegisterUser
    @User_Name VARCHAR(100),
    @Email_ID VARCHAR(150),
    @User_password VARCHAR(100),
    @City VARCHAR(100),
    @PhoneNo VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Tbl_User WHERE Email_ID = @Email_ID)
    BEGIN
        SELECT 0 AS Result, 'Email is already registered. Please login.' AS Message;
        RETURN;
    END

    INSERT INTO Tbl_User (User_Name, Email_ID, User_password, City, PhoneNo)
    VALUES (@User_Name, @Email_ID, @User_password, @City, @PhoneNo);

    SELECT 1 AS Result, 'Registration successful!' AS Message;
END
GO

-- 14. sp_LoginUser
IF OBJECT_ID('dbo.sp_LoginUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LoginUser;
GO
CREATE PROCEDURE sp_LoginUser
    @Email_ID VARCHAR(150),
    @User_password VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT User_ID, User_Name, Email_ID
    FROM Tbl_User
    WHERE Email_ID = @Email_ID
      AND User_password = @User_password;
END
GO

-- 15. sp_GetUserById
IF OBJECT_ID('dbo.sp_GetUserById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetUserById;
GO
CREATE PROCEDURE sp_GetUserById
    @User_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT User_ID, User_Name, Email_ID, User_password, City, PhoneNo
    FROM Tbl_User
    WHERE User_ID = @User_ID;
END
GO

-- 16. sp_UpdateUserProfile
IF OBJECT_ID('dbo.sp_UpdateUserProfile', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_UpdateUserProfile;
GO
CREATE PROCEDURE sp_UpdateUserProfile
    @User_ID INT,
    @User_Name VARCHAR(100),
    @Email_ID VARCHAR(150),
    @City VARCHAR(100),
    @PhoneNo VARCHAR(20),
    @User_password VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @User_password IS NOT NULL AND LEN(@User_password) > 0
    BEGIN
        UPDATE Tbl_User
        SET User_Name = @User_Name,
            Email_ID = @Email_ID,
            City = @City,
            PhoneNo = @PhoneNo,
            User_password = @User_password
        WHERE User_ID = @User_ID;
    END
    ELSE
    BEGIN
        UPDATE Tbl_User
        SET User_Name = @User_Name,
            Email_ID = @Email_ID,
            City = @City,
            PhoneNo = @PhoneNo
        WHERE User_ID = @User_ID;
    END
END
GO

-- ==============================================================================
-- 7. STORED PROCEDURES - BOOKING MODULE
-- ==============================================================================

-- 17. sp_AddBooking
IF OBJECT_ID('dbo.sp_AddBooking', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_AddBooking;
GO
CREATE PROCEDURE sp_AddBooking
    @User_ID INT,
    @Cat_ID INT,
    @Movie_ID INT,
    @no_of_Tickets INT,
    @amount DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Tbl_Booking (User_ID, Cat_ID, Movie_ID, no_of_Tickets, amount)
    VALUES (@User_ID, @Cat_ID, @Movie_ID, @no_of_Tickets, @amount);

    SELECT SCOPE_IDENTITY() AS NewBookingID;
END
GO

-- 18. sp_GetBookingsByUser
IF OBJECT_ID('dbo.sp_GetBookingsByUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetBookingsByUser;
GO
CREATE PROCEDURE sp_GetBookingsByUser
    @User_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        b.booking_ID,
        b.User_ID,
        b.Movie_ID,
        m.Movie_name,
        b.Cat_ID,
        c.Cat_Type,
        m.rate,
        b.no_of_Tickets,
        b.amount
    FROM Tbl_Booking b
    INNER JOIN Tbl_Movie m ON b.Movie_ID = m.Movie_ID
    INNER JOIN Tbl_Movie_Category c ON b.Cat_ID = c.Cat_ID
    WHERE b.User_ID = @User_ID
    ORDER BY b.booking_ID DESC;
END
GO

-- 19. sp_GetBookingById
IF OBJECT_ID('dbo.sp_GetBookingById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetBookingById;
GO
CREATE PROCEDURE sp_GetBookingById
    @booking_ID INT,
    @User_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        b.booking_ID,
        b.User_ID,
        b.Movie_ID,
        m.Movie_name,
        b.Cat_ID,
        c.Cat_Type,
        m.rate,
        b.no_of_Tickets,
        b.amount
    FROM Tbl_Booking b
    INNER JOIN Tbl_Movie m ON b.Movie_ID = m.Movie_ID
    INNER JOIN Tbl_Movie_Category c ON b.Cat_ID = c.Cat_ID
    WHERE b.booking_ID = @booking_ID 
      AND b.User_ID = @User_ID;
END
GO

-- 20. sp_UpdateBooking
IF OBJECT_ID('dbo.sp_UpdateBooking', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_UpdateBooking;
GO
CREATE PROCEDURE sp_UpdateBooking
    @booking_ID INT,
    @User_ID INT,
    @no_of_Tickets INT,
    @amount DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Tbl_Booking
    SET no_of_Tickets = @no_of_Tickets,
        amount = @amount
    WHERE booking_ID = @booking_ID 
      AND User_ID = @User_ID;
END
GO

-- 21. sp_DeleteBooking
IF OBJECT_ID('dbo.sp_DeleteBooking', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_DeleteBooking;
GO
CREATE PROCEDURE sp_DeleteBooking
    @booking_ID INT,
    @User_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Tbl_Booking
    WHERE booking_ID = @booking_ID 
      AND User_ID = @User_ID;
END
GO
