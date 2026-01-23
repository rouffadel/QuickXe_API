USE [QuickXeCRUD];
GO

-- 1. Add CurrencySymbol column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Country]') AND name = 'CurrencySymbol')
BEGIN
    ALTER TABLE [dbo].[Country] ADD [CurrencySymbol] NVARCHAR(10) NULL;
END
GO

-- 2. Update CurrencySymbol based on CountryCode (supports 2-letter and 3-letter codes)
UPDATE [dbo].[Country]
SET [CurrencySymbol] = CASE 
    -- North America
    WHEN [CountryCode] IN ('US', 'USA') THEN '$'
    WHEN [CountryCode] IN ('CA', 'CAN') THEN '$'
    WHEN [CountryCode] IN ('MX', 'MEX') THEN '$'
    
    -- Europe (Eurozone)
    WHEN [CountryCode] IN ('AT','AUT', 'BE','BEL', 'CY','CYP', 'EE','EST', 'FI','FIN', 'FR','FRA', 'DE','DEU', 'GR','GRC', 'IE','IRL', 'IT','ITA', 'LV','LVA', 'LT','LTU', 'LU','LUX', 'MT','MLT', 'NL','NLD', 'PT','PRT', 'SK','SVK', 'SI','SVN', 'ES','ESP') THEN '€'
    
    -- Europe (Non-Euro / Others)
    WHEN [CountryCode] IN ('GB', 'GBR', 'UK') THEN '£'
    WHEN [CountryCode] IN ('CH', 'CHE') THEN 'Fr'
    WHEN [CountryCode] IN ('SE', 'SWE') THEN 'kr'
    WHEN [CountryCode] IN ('NO', 'NOR') THEN 'kr'
    WHEN [CountryCode] IN ('DK', 'DNK') THEN 'kr'
    WHEN [CountryCode] IN ('RU', 'RUS') THEN '₽'
    WHEN [CountryCode] IN ('TR', 'TUR') THEN '₺'
    
    -- Asia
    WHEN [CountryCode] IN ('IN', 'IND') THEN '₹'
    WHEN [CountryCode] IN ('JP', 'JPN') THEN '¥'
    WHEN [CountryCode] IN ('CN', 'CHN') THEN '¥'
    WHEN [CountryCode] IN ('KR', 'KOR') THEN '₩'
    WHEN [CountryCode] IN ('MY', 'MYS') THEN 'RM'
    WHEN [CountryCode] IN ('TH', 'THA') THEN '฿'
    WHEN [CountryCode] IN ('SG', 'SGP') THEN '$'
    WHEN [CountryCode] IN ('HK', 'HKG') THEN '$'
    WHEN [CountryCode] IN ('ID', 'IDN') THEN 'Rp'
    WHEN [CountryCode] IN ('PH', 'PHL') THEN '₱'
    WHEN [CountryCode] IN ('VN', 'VNM') THEN '₫'
    WHEN [CountryCode] IN ('LK', 'LKA') THEN 'Rs'
    WHEN [CountryCode] IN ('PK', 'PAK') THEN 'Rs'
    WHEN [CountryCode] IN ('BD', 'BGD') THEN '৳'
    
    -- Middle East
    WHEN [CountryCode] IN ('AE', 'ARE') THEN 'د.إ'
    WHEN [CountryCode] IN ('SA', 'SAU') THEN '﷼'
    WHEN [CountryCode] IN ('QA', 'QAT') THEN '﷼'
    WHEN [CountryCode] IN ('OM', 'OMN') THEN '﷼'
    WHEN [CountryCode] IN ('KW', 'KWT') THEN 'د.ك'
    WHEN [CountryCode] IN ('BH', 'BHR') THEN '.د.ب'
    WHEN [CountryCode] IN ('JO', 'JOR') THEN 'د.ا'
    WHEN [CountryCode] IN ('LB', 'LBN') THEN 'ل.ل'
    WHEN [CountryCode] IN ('IL', 'ISR') THEN '₪'
    
    -- Oceania
    WHEN [CountryCode] IN ('AU', 'AUS') THEN '$'
    WHEN [CountryCode] IN ('NZ', 'NZL') THEN '$'
    
    -- South America
    WHEN [CountryCode] IN ('BR', 'BRA') THEN 'R$'
    WHEN [CountryCode] IN ('AR', 'ARG') THEN '$'
    WHEN [CountryCode] IN ('CO', 'COL') THEN '$'
    WHEN [CountryCode] IN ('CL', 'CHL') THEN '$'
    WHEN [CountryCode] IN ('PE', 'PER') THEN 'S/'
    
    -- Africa
    WHEN [CountryCode] IN ('ZA', 'ZAF') THEN 'R'
    WHEN [CountryCode] IN ('EG', 'EGY') THEN 'E£'
    WHEN [CountryCode] IN ('NG', 'NGA') THEN '₦'
    WHEN [CountryCode] IN ('GH', 'GHA') THEN '₵'
    WHEN [CountryCode] IN ('KE', 'KEN') THEN 'KSh'
    
    ELSE [CurrencySymbol] -- Preserve existing if not matched
END
WHERE [CurrencySymbol] IS NULL OR [CurrencySymbol] = '';
GO
