USE [QuickXeCRUD];
GO

-- 1. Populate CountriesMaster with specific Currency Symbols for ALL countries
-- This list covers the countries shown in your image (AF, AL, DZ, etc.) and many others.

UPDATE [dbo].[CountriesMaster]
SET [CurrencySymbol] = CASE 
    -- A
    WHEN [CountryCode] = 'AF' THEN '؋' -- Afghanistan
    WHEN [CountryCode] = 'AL' THEN 'L' -- Albania
    WHEN [CountryCode] = 'DZ' THEN 'د.ج' -- Algeria
    WHEN [CountryCode] = 'AD' THEN '€' -- Andorra
    WHEN [CountryCode] = 'AO' THEN 'Kz' -- Angola
    WHEN [CountryCode] = 'AG' THEN '$' -- Antigua and Barbuda (East Caribbean Dollar)
    WHEN [CountryCode] = 'AR' THEN '$' -- Argentina
    WHEN [CountryCode] = 'AM' THEN '֏' -- Armenia
    WHEN [CountryCode] = 'AU' THEN '$' -- Australia
    WHEN [CountryCode] = 'AT' THEN '€' -- Austria
    WHEN [CountryCode] = 'AZ' THEN '₼' -- Azerbaijan

    -- B
    WHEN [CountryCode] = 'BS' THEN '$' -- Bahamas
    WHEN [CountryCode] = 'BH' THEN '.د.ب' -- Bahrain
    WHEN [CountryCode] = 'BD' THEN '৳' -- Bangladesh
    WHEN [CountryCode] = 'BB' THEN '$' -- Barbados
    WHEN [CountryCode] = 'BY' THEN 'Br' -- Belarus
    WHEN [CountryCode] = 'BE' THEN '€' -- Belgium
    WHEN [CountryCode] = 'BZ' THEN '$' -- Belize
    WHEN [CountryCode] = 'BJ' THEN 'CFA' -- Benin
    WHEN [CountryCode] = 'BT' THEN 'Nu.' -- Bhutan
    WHEN [CountryCode] = 'BO' THEN 'Bs.' -- Bolivia
    WHEN [CountryCode] = 'BA' THEN 'KM' -- Bosnia and Herzegovina
    WHEN [CountryCode] = 'BW' THEN 'P' -- Botswana
    WHEN [CountryCode] = 'BR' THEN 'R$' -- Brazil
    WHEN [CountryCode] = 'BN' THEN '$' -- Brunei
    WHEN [CountryCode] = 'BG' THEN 'лв' -- Bulgaria
    WHEN [CountryCode] = 'BF' THEN 'CFA' -- Burkina Faso
    WHEN [CountryCode] = 'BI' THEN 'Fr' -- Burundi

    -- C
    WHEN [CountryCode] = 'KH' THEN '៛' -- Cambodia
    WHEN [CountryCode] = 'CM' THEN 'CFA' -- Cameroon
    WHEN [CountryCode] = 'CA' THEN '$' -- Canada
    WHEN [CountryCode] = 'CV' THEN '$' -- Cape Verde
    WHEN [CountryCode] = 'CF' THEN 'CFA' -- Central African Republic
    WHEN [CountryCode] = 'TD' THEN 'CFA' -- Chad
    WHEN [CountryCode] = 'CL' THEN '$' -- Chile
    WHEN [CountryCode] = 'CN' THEN '¥' -- China
    WHEN [CountryCode] = 'CO' THEN '$' -- Colombia
    WHEN [CountryCode] = 'KM' THEN 'Fr' -- Comoros
    WHEN [CountryCode] = 'CG' THEN 'CFA' -- Congo
    WHEN [CountryCode] = 'CD' THEN 'Fr' -- Congo, Democratic Republic
    WHEN [CountryCode] = 'CR' THEN '₡' -- Costa Rica
    WHEN [CountryCode] = 'HR' THEN '€' -- Croatia (Updated to Euro)
    WHEN [CountryCode] = 'CU' THEN '$' -- Cuba
    WHEN [CountryCode] = 'CY' THEN '€' -- Cyprus
    WHEN [CountryCode] = 'CZ' THEN 'Kč' -- Czech Republic

    -- D
    WHEN [CountryCode] = 'DK' THEN 'kr' -- Denmark
    WHEN [CountryCode] = 'DJ' THEN 'Fr' -- Djibouti
    WHEN [CountryCode] = 'DM' THEN '$' -- Dominica
    WHEN [CountryCode] = 'DO' THEN '$' -- Dominican Republic

    -- E
    WHEN [CountryCode] = 'EC' THEN '$' -- Ecuador
    WHEN [CountryCode] = 'EG' THEN 'E£' -- Egypt
    WHEN [CountryCode] = 'SV' THEN '$' -- El Salvador
    WHEN [CountryCode] = 'GQ' THEN 'CFA' -- Equatorial Guinea
    WHEN [CountryCode] = 'ER' THEN 'Nfk' -- Eritrea
    WHEN [CountryCode] = 'EE' THEN '€' -- Estonia
    WHEN [CountryCode] = 'SZ' THEN 'L' -- Eswatini
    WHEN [CountryCode] = 'ET' THEN 'Br' -- Ethiopia

    -- F
    WHEN [CountryCode] = 'FJ' THEN '$' -- Fiji
    WHEN [CountryCode] = 'FI' THEN '€' -- Finland
    WHEN [CountryCode] = 'FR' THEN '€' -- France

    -- G
    WHEN [CountryCode] = 'GA' THEN 'CFA' -- Gabon
    WHEN [CountryCode] = 'GM' THEN 'D' -- Gambia
    WHEN [CountryCode] = 'GE' THEN '₾' -- Georgia
    WHEN [CountryCode] = 'DE' THEN '€' -- Germany
    WHEN [CountryCode] = 'GH' THEN '₵' -- Ghana
    WHEN [CountryCode] = 'GR' THEN '€' -- Greece
    WHEN [CountryCode] = 'GD' THEN '$' -- Grenada
    WHEN [CountryCode] = 'GT' THEN 'Q' -- Guatemala
    WHEN [CountryCode] = 'GN' THEN 'Fr' -- Guinea
    WHEN [CountryCode] = 'GW' THEN 'CFA' -- Guinea-Bissau
    WHEN [CountryCode] = 'GY' THEN '$' -- Guyana

    -- H
    WHEN [CountryCode] = 'HT' THEN 'G' -- Haiti
    WHEN [CountryCode] = 'HN' THEN 'L' -- Honduras
    WHEN [CountryCode] = 'HK' THEN '$' -- Hong Kong
    WHEN [CountryCode] = 'HU' THEN 'Ft' -- Hungary

    -- I
    WHEN [CountryCode] = 'IS' THEN 'kr' -- Iceland
    WHEN [CountryCode] = 'IN' THEN '₹' -- India
    WHEN [CountryCode] = 'ID' THEN 'Rp' -- Indonesia
    WHEN [CountryCode] = 'IR' THEN '﷼' -- Iran
    WHEN [CountryCode] = 'IQ' THEN 'ع.د' -- Iraq
    WHEN [CountryCode] = 'IE' THEN '€' -- Ireland
    WHEN [CountryCode] = 'IL' THEN '₪' -- Israel
    WHEN [CountryCode] = 'IT' THEN '€' -- Italy
    WHEN [CountryCode] = 'CI' THEN 'CFA' -- Ivory Coast

    -- J
    WHEN [CountryCode] = 'JM' THEN '$' -- Jamaica
    WHEN [CountryCode] = 'JP' THEN '¥' -- Japan
    WHEN [CountryCode] = 'JO' THEN 'د.ا' -- Jordan

    -- K
    WHEN [CountryCode] = 'KZ' THEN '₸' -- Kazakhstan
    WHEN [CountryCode] = 'KE' THEN 'KSh' -- Kenya
    WHEN [CountryCode] = 'KI' THEN '$' -- Kiribati
    WHEN [CountryCode] = 'KP' THEN '₩' -- North Korea
    WHEN [CountryCode] = 'KR' THEN '₩' -- South Korea
    WHEN [CountryCode] = 'KW' THEN 'د.ك' -- Kuwait
    WHEN [CountryCode] = 'KG' THEN 'с' -- Kyrgyzstan

    -- L
    WHEN [CountryCode] = 'LA' THEN '₭' -- Laos
    WHEN [CountryCode] = 'LV' THEN '€' -- Latvia
    WHEN [CountryCode] = 'LB' THEN 'ل.ل' -- Lebanon
    WHEN [CountryCode] = 'LS' THEN 'L' -- Lesotho
    WHEN [CountryCode] = 'LR' THEN '$' -- Liberia
    WHEN [CountryCode] = 'LY' THEN 'ل.د' -- Libya
    WHEN [CountryCode] = 'LI' THEN 'Fr' -- Liechtenstein
    WHEN [CountryCode] = 'LT' THEN '€' -- Lithuania
    WHEN [CountryCode] = 'LU' THEN '€' -- Luxembourg

    -- M
    WHEN [CountryCode] = 'MG' THEN 'Ar' -- Madagascar
    WHEN [CountryCode] = 'MW' THEN 'MK' -- Malawi
    WHEN [CountryCode] = 'MY' THEN 'RM' -- Malaysia
    WHEN [CountryCode] = 'MV' THEN 'Rv' -- Maldives
    WHEN [CountryCode] = 'ML' THEN 'CFA' -- Mali
    WHEN [CountryCode] = 'MT' THEN '€' -- Malta
    WHEN [CountryCode] = 'MH' THEN '$' -- Marshall Islands
    WHEN [CountryCode] = 'MR' THEN 'UM' -- Mauritania
    WHEN [CountryCode] = 'MU' THEN '₨' -- Mauritius
    WHEN [CountryCode] = 'MX' THEN '$' -- Mexico
    WHEN [CountryCode] = 'FM' THEN '$' -- Micronesia
    WHEN [CountryCode] = 'MD' THEN 'L' -- Moldova
    WHEN [CountryCode] = 'MC' THEN '€' -- Monaco
    WHEN [CountryCode] = 'MN' THEN '₮' -- Mongolia
    WHEN [CountryCode] = 'ME' THEN '€' -- Montenegro
    WHEN [CountryCode] = 'MA' THEN 'د.م.' -- Morocco
    WHEN [CountryCode] = 'MZ' THEN 'MT' -- Mozambique
    WHEN [CountryCode] = 'MM' THEN 'K' -- Myanmar

    -- N
    WHEN [CountryCode] = 'NA' THEN '$' -- Namibia
    WHEN [CountryCode] = 'NR' THEN '$' -- Nauru
    WHEN [CountryCode] = 'NP' THEN '₨' -- Nepal
    WHEN [CountryCode] = 'NL' THEN '€' -- Netherlands
    WHEN [CountryCode] = 'NZ' THEN '$' -- New Zealand
    WHEN [CountryCode] = 'NI' THEN 'C$' -- Nicaragua
    WHEN [CountryCode] = 'NE' THEN 'CFA' -- Niger
    WHEN [CountryCode] = 'NG' THEN '₦' -- Nigeria
    WHEN [CountryCode] = 'MK' THEN 'ден' -- North Macedonia
    WHEN [CountryCode] = 'NO' THEN 'kr' -- Norway

    -- O
    WHEN [CountryCode] = 'OM' THEN '﷼' -- Oman

    -- P
    WHEN [CountryCode] = 'PK' THEN '₨' -- Pakistan
    WHEN [CountryCode] = 'PW' THEN '$' -- Palau
    WHEN [CountryCode] = 'PA' THEN 'B/.' -- Panama
    WHEN [CountryCode] = 'PG' THEN 'K' -- Papua New Guinea
    WHEN [CountryCode] = 'PY' THEN 'Gs' -- Paraguay
    WHEN [CountryCode] = 'PE' THEN 'S/' -- Peru
    WHEN [CountryCode] = 'PH' THEN '₱' -- Philippines
    WHEN [CountryCode] = 'PL' THEN 'zł' -- Poland
    WHEN [CountryCode] = 'PT' THEN '€' -- Portugal

    -- Q
    WHEN [CountryCode] = 'QA' THEN '﷼' -- Qatar

    -- R
    WHEN [CountryCode] = 'RO' THEN 'lei' -- Romania
    WHEN [CountryCode] = 'RU' THEN '₽' -- Russia
    WHEN [CountryCode] = 'RW' THEN 'Fr' -- Rwanda

    -- S
    WHEN [CountryCode] = 'KN' THEN '$' -- Saint Kitts and Nevis
    WHEN [CountryCode] = 'LC' THEN '$' -- Saint Lucia
    WHEN [CountryCode] = 'VC' THEN '$' -- Saint Vincent and the Grenadines
    WHEN [CountryCode] = 'WS' THEN 'T' -- Samoa
    WHEN [CountryCode] = 'SM' THEN '€' -- San Marino
    WHEN [CountryCode] = 'ST' THEN 'Db' -- Sao Tome and Principe
    WHEN [CountryCode] = 'SA' THEN '﷼' -- Saudi Arabia
    WHEN [CountryCode] = 'SN' THEN 'CFA' -- Senegal
    WHEN [CountryCode] = 'RS' THEN 'дин.' -- Serbia
    WHEN [CountryCode] = 'SC' THEN '₨' -- Seychelles
    WHEN [CountryCode] = 'SL' THEN 'Le' -- Sierra Leone
    WHEN [CountryCode] = 'SG' THEN '$' -- Singapore
    WHEN [CountryCode] = 'SK' THEN '€' -- Slovakia
    WHEN [CountryCode] = 'SI' THEN '€' -- Slovenia
    WHEN [CountryCode] = 'SB' THEN '$' -- Solomon Islands
    WHEN [CountryCode] = 'SO' THEN 'S' -- Somalia
    WHEN [CountryCode] = 'ZA' THEN 'R' -- South Africa
    WHEN [CountryCode] = 'SS' THEN '£' -- South Sudan
    WHEN [CountryCode] = 'ES' THEN '€' -- Spain
    WHEN [CountryCode] = 'LK' THEN 'Rs' -- Sri Lanka
    WHEN [CountryCode] = 'SD' THEN 'ج.س.' -- Sudan
    WHEN [CountryCode] = 'SR' THEN '$' -- Suriname
    WHEN [CountryCode] = 'SE' THEN 'kr' -- Sweden
    WHEN [CountryCode] = 'CH' THEN 'Fr' -- Switzerland
    WHEN [CountryCode] = 'SY' THEN '£' -- Syria

    -- T
    WHEN [CountryCode] = 'TW' THEN 'NT$' -- Taiwan
    WHEN [CountryCode] = 'TJ' THEN 'SM' -- Tajikistan
    WHEN [CountryCode] = 'TZ' THEN 'TSh' -- Tanzania
    WHEN [CountryCode] = 'TH' THEN '฿' -- Thailand
    WHEN [CountryCode] = 'TL' THEN '$' -- Timor-Leste
    WHEN [CountryCode] = 'TG' THEN 'CFA' -- Togo
    WHEN [CountryCode] = 'TO' THEN 'T$' -- Tonga
    WHEN [CountryCode] = 'TT' THEN '$' -- Trinidad and Tobago
    WHEN [CountryCode] = 'TN' THEN 'د.ت' -- Tunisia
    WHEN [CountryCode] = 'TR' THEN '₺' -- Turkey
    WHEN [CountryCode] = 'TM' THEN 'm' -- Turkmenistan
    WHEN [CountryCode] = 'TV' THEN '$' -- Tuvalu

    -- U
    WHEN [CountryCode] = 'UG' THEN 'USh' -- Uganda
    WHEN [CountryCode] = 'UA' THEN '₴' -- Ukraine
    WHEN [CountryCode] = 'AE' THEN 'د.إ' -- United Arab Emirates
    WHEN [CountryCode] = 'GB' THEN '£' -- United Kingdom
    WHEN [CountryCode] = 'US' THEN '$' -- United States
    WHEN [CountryCode] = 'UY' THEN '$U' -- Uruguay
    WHEN [CountryCode] = 'UZ' THEN 'so''m' -- Uzbekistan

    -- V
    WHEN [CountryCode] = 'VU' THEN 'Vt' -- Vanuatu
    WHEN [CountryCode] = 'VA' THEN '€' -- Vatican City
    WHEN [CountryCode] = 'VE' THEN 'Bs.' -- Venezuela
    WHEN [CountryCode] = 'VN' THEN '₫' -- Vietnam

    -- Y
    WHEN [CountryCode] = 'YE' THEN '﷼' -- Yemen

    -- Z
    WHEN [CountryCode] = 'ZM' THEN 'ZK' -- Zambia
    WHEN [CountryCode] = 'ZW' THEN '$' -- Zimbabwe

    ELSE [CurrencySymbol]
END
WHERE [CurrencySymbol] IS NULL OR [CurrencySymbol] = 'â,-' OR [CurrencySymbol] = '';
GO

-- 2. Update existing Country table from CountriesMaster
-- This ensures that the Country table gets its symbols from the now-populated Master table
UPDATE c
SET c.[CurrencySymbol] = cm.[CurrencySymbol]
FROM [dbo].[Country] c
INNER JOIN [dbo].[CountriesMaster] cm ON c.[CountryCode] = cm.[CountryCode]
WHERE cm.[CurrencySymbol] IS NOT NULL;
GO
