GO
CREATE DATABASE FPTClubs
GO
USE FPTClubs
GO
CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(50) UNIQUE NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    password_hash NVARCHAR(255) NOT NULL,
    profile_picture NVARCHAR(255),
    created_at DATETIME DEFAULT GETDATE()
);
GO
CREATE TABLE Clubs (
    club_id INT PRIMARY KEY IDENTITY(1,1),
    club_name NVARCHAR(100) NOT NULL,
    description TEXT,
    created_at DATETIME DEFAULT GETDATE(),
);
GO
CREATE TABLE Roles (
    role_id INT PRIMARY KEY IDENTITY(1,1),
    role_name NVARCHAR(50) UNIQUE NOT NULL
);
GO
CREATE TABLE ClubMembers (
    membership_id INT PRIMARY KEY IDENTITY(1,1),
    club_id INT NOT NULL,
    user_id INT NOT NULL,
    role_id INT NOT NULL,
    joined_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (club_id) REFERENCES Clubs(club_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (role_id) REFERENCES Roles(role_id) ON DELETE CASCADE
);
GO
CREATE TABLE Posts (
    post_id INT PRIMARY KEY IDENTITY(1,1),
    created_by INT NOT NULL,
    content TEXT NOT NULL,
    image_url NVARCHAR(255),
    created_at DATETIME DEFAULT GETDATE(),
    status NVARCHAR(20) DEFAULT 'Pending',
    FOREIGN KEY (created_by) REFERENCES ClubMembers(membership_id)  
);
GO
CREATE TABLE PostInteractions (
    interaction_id INT PRIMARY KEY IDENTITY(1,1),
    post_id INT NOT NULL,
    user_id INT NOT NULL,
    type NVARCHAR(20) NOT NULL,
    comment_text TEXT,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (post_id) REFERENCES Posts(post_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) 
);
GO
CREATE TABLE Events (
    event_id INT PRIMARY KEY IDENTITY(1,1),
    created_by INT NOT NULL,
    event_title NVARCHAR(255) NOT NULL,
    event_description TEXT,
    event_date DATETIME NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (created_by) REFERENCES ClubMembers(membership_id)  
);

GO
CREATE TABLE Tasks (
    task_id INT PRIMARY KEY IDENTITY(1,1),
    task_description TEXT NOT NULL,
    status NVARCHAR(20) DEFAULT 'Pending',
    due_date DATETIME,
    created_at DATETIME DEFAULT GETDATE(),
	created_by INT NOT NULL,
	FOREIGN KEY (created_by) REFERENCES ClubMembers(membership_id)  
);
GO
CREATE TABLE TaskAssignments (
    assignment_id INT PRIMARY KEY IDENTITY(1,1),
    task_id INT NOT NULL,
    membership_id INT NOT NULL,
    assigned_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (task_id) REFERENCES Tasks(task_id) ON DELETE CASCADE,
    FOREIGN KEY (membership_id) REFERENCES ClubMembers(membership_id) 
);
GO
CREATE TABLE Notifications (
    notification_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    message TEXT NOT NULL,
    is_read BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES Users(user_id) 
);
GO
CREATE TABLE ClubRequests (
    request_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    club_name NVARCHAR(100) NOT NULL,
    description TEXT,
    status NVARCHAR(20) DEFAULT 'Pending',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES Users(user_id) 
);
