
---- USER table ----

CREATE TABLE AppUsers
(
    UserID INT IDENTITY PRIMARY KEY,
    Username VARCHAR(50),
    Password VARCHAR(50),
    IsActive BIT
)

---- Inserting Test Data
INSERT INTO AppUsers (Username, Password, IsActive)
VALUES ('admin', 'admin@123', 1),
       ('user1', 'user@123', 1);

--- LOAN Type Table Creation -----
CREATE TABLE [dbo].[LoanType] (
	[TypeId] [int] Identity (1,1) NOT NULL
	,[TypeName] [varchar] (20) NOT NULL
	,[InterestRate] [decimal] (10,2) NOT NULL
CONSTRAINT [PK_LoanType] PRIMARY KEY CLUSTERED
(
[TypeId] ASC
) 
)

---- Inserting Test Data to Loan Types
INSERT INTO LoanType (TypeName, InterestRate)
VALUES ('Personal', 13.5),
       ('Housing', 10),
	   ('Vehicle', 17.26);

------ Login Attempt Data Storing Table ----

CREATE TABLE LoginAttemptLog
(
    LoginAttemptID INT IDENTITY(1,1) PRIMARY KEY,
    Username        VARCHAR(50),
    IsSuccess       BIT,
    IPAddress       VARCHAR(45),
    AttemptDate     DATETIME DEFAULT GETDATE()
);

------ LOAN Application Table Creation -----

CREATE TABLE [dbo].[LoanApplication] (
	[LoanId] [int] Identity (100,1) NOT NULL
	,[CustomerName] [varchar] (100) NOT NULL
	,[NIC] [varchar] (100) NOT NULL
	,[LoanTypeId] [int] NOT NULL
	,[InterestRate] [decimal] (10,2) NOT NULL
	,[LoanAmount] [decimal] (10,2) NOT NULL
	,[Duration] [int] NOT NULL
	,[RegisteredDate] [DateTime] NOT NULL
	,[Status] [varchar] (20) NOT NULL
CONSTRAINT [PK_LoanApplication] PRIMARY KEY CLUSTERED
(
[LoanId] ASC
) 
CONSTRAINT [FK_LoanApplication_LoanType]
	FOREIGN KEY (LoanTypeId)
	REFERENCES [dbo].[LoanType] (TypeId)
)

----- Inserting to test Data for loan Application ----

INSERT INTO LoanApplication (CustomerName, NIC, LoanTypeId, InterestRate, LoanAmount, Duration, RegisteredDate, Status)
VALUES ('Sawsiri Sampath','8911000875V', 1, 13.5, 25657.45, 24, GETDATE(), 'New');

-----STORED Procedures -------

---- Get All Applications ----

CREATE PROC [dbo].[USP_GetAllLoanApplication]
AS
BEGIN
	SELECT LoanId, CustomerName, NIC, LoanTypeId, lt.TypeName, la.InterestRate, la.RegisteredDate, LoanAmount, Duration, Status 
	FROM [dbo].[LoanApplication] la WITH (NOLOCK)
	INNER JOIN [dbo].[LoanType] lt WITH (NOLOCK) ON lt.TypeId = la.LoanTypeId
END

---- Get Loan Application by ID ----

CREATE PROC [dbo].[USP_GetLoanApplicationById]
(
	@LoanId INT = 0
)
AS
BEGIN
	SELECT LoanId, CustomerName, NIC, LoanTypeId, lt.TypeName, la.InterestRate,  la.RegisteredDate, LoanAmount, Duration, Status 
	FROM [dbo].[LoanApplication] la WITH (NOLOCK)
	INNER JOIN [dbo].[LoanType] lt WITH (NOLOCK) ON lt.TypeId = la.LoanTypeId
	WHERE LoanId = @LoanId
END

-------- Get Loan types ----

CREATE PROC [dbo].[USP_GetLoanTypes]
AS
BEGIN
	SELECT TypeId,TypeName ,InterestRate
	FROM [dbo].[LoanType] WITH (NOLOCK)
END



---- Save Loan Applications -----

CREATE PROC [dbo].[USP_RegisterLoanApplication]
(
	@CustomerName		VARCHAR(100)
	,@NIC				VARCHAR(50)
	,@LoanTypeId		INT
	,@InterestRate		DECIMAL (10,2)
	,@LoanAmount		DECIMAL (10,2)
	,@Duration			INT
	,@Status			VARCHAR(20)
)
AS
BEGIN

BEGIN TRY
	BEGIN TRAN
		INSERT INTO [dbo].[LoanApplication] (CustomerName, NIC, LoanTypeId, InterestRate, LoanAmount, Duration, RegisteredDate, Status)
		VALUES
			(
				@CustomerName
				,@NIC
				,@LoanTypeId	
				,@InterestRate	
				,@LoanAmount	
				,@Duration
				,GETDATE()
				,@Status		
			)
	COMMIT TRAN
END TRY
BEGIN CATCH
	ROLLBACK TRAN
END CATCH
END


---- Update Loan Applications by the ID-----

CREATE PROC [dbo].[USP_UpdateLoanApplication]
(
	@LoanId				INT = 0
	,@Status			VARCHAR(20)
)
AS
BEGIN
	DECLARE @RowCount INT = 0

	BEGIN TRY
		SET @RowCount = (SELECT COUNT(1) FROM [dbo].[LoanApplication] WITH (NOLOCK) 
		WHERE LoanId = @LoanId)

		IF (@RowCount > 0)
			BEGIN
				BEGIN TRAN
					UPDATE [dbo].[LoanApplication] 
					SET Status = @Status
					WHERE LoanId = @LoanId
				COMMIT TRAN
			END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
	END CATCH
END


------- User login Validation ----

CREATE PROCEDURE [dbo].[USP_ValidateUserLogin]
    @Username VARCHAR(50),
    @Password VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM AppUsers WITH (NOLOCK)
        WHERE Username = @Username
          AND Password = @Password
          AND IsActive = 1
    )
        SELECT 1 AS IsValid;
    ELSE
        SELECT 0 AS IsValid;
END

--- Login Attempt SP ----
CREATE PROCEDURE [dbo].[USP_InsertLoginAttempt]
    @Username   VARCHAR(50),
    @IsSuccess  BIT
   
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO LoginAttemptLog
    (
        Username,
        IsSuccess       
    )
    VALUES
    (
        @Username,
        @IsSuccess
    );
END

SELECT * FROM LoginAttemptLog

SELECT * FROM LoanApplication