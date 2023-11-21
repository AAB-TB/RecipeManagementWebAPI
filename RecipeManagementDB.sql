-- Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    CONSTRAINT UK_Users_Username UNIQUE (Username),
    CONSTRAINT UK_Users_Email UNIQUE (Email)
);

-- Roles Table
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL
);

-- UserRoles Table (to associate users with roles)
CREATE TABLE UserRoles (
    UserRoleId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    RoleId INT,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    CONSTRAINT UC_UserRoles UNIQUE (UserId, RoleId) -- Ensure unique combinations
);

-- Categories Table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL
);

-- Recipes Table
CREATE TABLE Recipes (
    RecipeId INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    Ingredients TEXT,
    CategoryId INT,
    UserId INT,
    AverageRating DECIMAL(3, 2) DEFAULT 0,
    CONSTRAINT FK_Recipes_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT FK_Recipes_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Ratings Table
CREATE TABLE Ratings (
    RatingId INT IDENTITY(1,1) PRIMARY KEY,
    RecipeId INT,
    UserId INT,
    RatingValue INT NOT NULL CHECK (RatingValue >= 1 AND RatingValue <= 5),
    CONSTRAINT FK_Ratings_Recipes FOREIGN KEY (RecipeId) REFERENCES Recipes(RecipeId),
    CONSTRAINT FK_Ratings_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    UNIQUE(RecipeId, UserId)
);
