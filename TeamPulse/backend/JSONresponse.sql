WITH total_stats AS (
    SELECT
        COUNT(id) AS total_count,
        COALESCE(ROUND(AVG(score), 2), 0.00) AS average_score
    FROM pulseentry
),

score_list AS (
    SELECT
        vt.score AS score_value,
        COUNT(pe.id) AS score_count
    FROM (
        SELECT 1 AS score
        UNION ALL SELECT 2
        UNION ALL SELECT 3
        UNION ALL SELECT 4
        UNION ALL SELECT 5
    ) vt
    LEFT JOIN pulseentry pe
        ON vt.score = pe.score
    GROUP BY vt.score
),

category_list AS (
    SELECT
        pc.id AS category_id,
        pc.name AS category_name,
        COUNT(pe.id) AS entry_count
    FROM pulsecategory pc
    LEFT JOIN pulseentry pe
        ON pc.id = pe.categoryid
    GROUP BY pc.id, pc.name
),

scores_json AS (
    SELECT
        JSON_OBJECTAGG(CAST(sl.score_value AS CHAR), sl.score_count) AS scores_json
    FROM score_list sl
),

categories_json AS (
    SELECT
        JSON_ARRAYAGG(
            JSON_OBJECT(
                'id', cl.category_id,
                'name', cl.category_name,
                'count', cl.entry_count
            )
        ) AS categories_json
    FROM category_list cl
    ORDER BY cl.category_name
)

SELECT
    JSON_OBJECT(
        'count', t.total_count,
        'averageScore', t.average_score,
        'scores', s.scores_json,
        'categories', c.categories_json
    ) AS summary_json
FROM total_stats t
CROSS JOIN scores_json s
CROSS JOIN categories_json c;