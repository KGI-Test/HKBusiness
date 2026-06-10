/*
SBLRateData
SG340014_GetImportSBLRateData
SG340014_GetImportSBLRateData.sql


<Process>
    <Execution>
    <Commander Timeout = "120" />
    </Execution>
</Process> 
DECLARE @HKDSR_DATE INT = 20260318
*/

SELECT 
    [HKDSR_DATE]
    ,[HKDSR_STOCK_CODE]
    ,[HKDSR_STOCK_SHORT_NAME]
    ,[HKDSR_CURRENCY]
    ,[HKDSR_TICKER]
    ,[HKDSR_SBL_RATE_1]
    ,[HKDSR_SBL_RATE_2]
FROM [EXTSRC].[dbo].[HK_SBL_DAILY_SBL_RATE]
WHERE [HKDSR_DATE] = @HKDSR_DATE