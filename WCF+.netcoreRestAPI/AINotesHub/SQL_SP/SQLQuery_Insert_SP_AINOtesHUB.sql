CREATE PROCEDURE sp_InsertNote
(
    @Id UNIQUEIDENTIFIER,
    @Title NVARCHAR(MAX),
    @Content NVARCHAR(MAX),
    @Category NVARCHAR(MAX),
    @CreatedAt DATETIME2(7),
    @UpdatedAt DATETIME2(7),
    @CardBackground NVARCHAR(MAX),
    @CreatedBy NVARCHAR(MAX) = NULL,
    @UserId UNIQUEIDENTIFIER,
    @IsDeleted BIT,
    @UpdatedBy NVARCHAR(MAX) = NULL,
    @IsArchived BIT,
    @IsImportant BIT,
    @IsStarred BIT,
    @ReminderDateTime DATETIME2(7) = NULL,
    @IsReminderOn BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Notes
    (
        Id,
        Title,
        [Content],
        Category,
        CreatedAt,
        UpdatedAt,
        CardBackground,
        CreatedBy,
        UserId,
        IsDeleted,
        UpdatedBy,
        IsArchived,
        IsImportant,
        IsStarred,
        ReminderDateTime,
        IsReminderOn
    )
    VALUES
    (
        @Id,
        @Title,
        @Content,
        @Category,
        @CreatedAt,
        @UpdatedAt,
        @CardBackground,
        @CreatedBy,
        @UserId,
        @IsDeleted,
        @UpdatedBy,
        @IsArchived,
        @IsImportant,
        @IsStarred,
        @ReminderDateTime,
        @IsReminderOn
    );
END
GO