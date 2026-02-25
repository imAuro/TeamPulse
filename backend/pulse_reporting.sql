
SET NOCOUNT ON;



IF OBJECT_ID('dbo.PulseEntry', 'U') IS NOT NULL DROP TABLE dbo.PulseEntry;
IF OBJECT_ID('dbo.PulseCategory', 'U') IS NOT NULL DROP TABLE dbo.PulseCategory;
GO

CREATE TABLE dbo.PulseCategory
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_PulseCategory PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE dbo.PulseEntry
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_PulseEntry PRIMARY KEY,

    CategoryId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT FK_PulseEntry_Category
            FOREIGN KEY REFERENCES dbo.PulseCategory(Id),

    Score TINYINT NOT NULL
        CONSTRAINT CK_PulseEntry_Score CHECK (Score BETWEEN 1 AND 5),

    Comment NVARCHAR(500) NULL,

    CreatedAt DATETIMEOFFSET(3) NOT NULL
);
GO
--------------------------------------------------
/* Summary query matching /api/pulse/summary */
;WITH Totals AS
(
    SELECT
        TotalCount   = COUNT_BIG(1),
        AverageScore = CAST(
            ISNULL(AVG(CAST(Score AS DECIMAL(9,4))), 0.0)
            AS DECIMAL(3,2)
        )
    FROM dbo.PulseEntry
),
ScoreDist AS
(
    -- Ensure 1–5 always appear even if no rows exist
    SELECT
        v.Score,
        Cnt = COUNT(pe.Id)
    FROM (VALUES (1),(2),(3),(4),(5)) v(Score)
    LEFT JOIN dbo.PulseEntry pe
        ON pe.Score = v.Score
    GROUP BY v.Score
),
CategoryCounts AS
(
    SELECT
        c.Id,
        c.[Name],
        [Count] = COUNT(pe.Id)
    FROM dbo.PulseCategory c
    LEFT JOIN dbo.PulseEntry pe
        ON pe.CategoryId = c.Id
    GROUP BY c.Id, c.[Name]
)
SELECT
    -- total count
    t.TotalCount AS [count],

    -- averageScore decimal(3,2)
    t.AverageScore AS [averageScore],

    -- scores object {"1":0,"2":0,...}
    JSON_QUERY(
        '{' +
        STRING_AGG(
            '"' + CAST(sd.Score AS VARCHAR(10)) + '":' +
            CAST(sd.Cnt AS VARCHAR(20)),
            ','
        ) +
        '}'
    ) AS [scores],

    -- categories array
    JSON_QUERY(
        (
            SELECT
                cc.Id   AS [id],
                cc.[Name] AS [name],
                cc.[Count] AS [count]
            FROM CategoryCounts cc
            ORDER BY cc.[Name]
            FOR JSON PATH
        )
    ) AS [categories]

FROM Totals t
CROSS JOIN ScoreDist sd
GROUP BY t.TotalCount, t.AverageScore
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;

----------------------------------
SELECT
    c.Id,
    c.[Name],
    [Count] = COUNT(pe.Id)
FROM dbo.PulseCategory c
LEFT JOIN dbo.PulseEntry pe
    ON pe.CategoryId = c.Id

GROUP BY c.Id, c.[Name]
ORDER BY [Count] DESC, c.[Name] ASC;
GO