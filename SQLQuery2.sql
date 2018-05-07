
SELECT Copy.CopyID, [Transaction].TransactionID, [Transaction].CustID
FROM Copy INNER JOIN [Transaction] on Copy.CopyID = [Transaction].CopyID
