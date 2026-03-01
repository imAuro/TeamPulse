SELECT
        pc.name AS category_name,
        COUNT(pe.id) AS entry_count
    FROM pulsecategory pc
    LEFT JOIN pulseentry pe
        ON pc.id = pe.categoryid
    GROUP BY pc.id, pc.name
