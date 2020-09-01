CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
BEGIN
set nocount on; 
	select id, ProductName, [Description],RetailPrice,QuantityInStock
	FROM dbo.Product
	order by ProductName;
END
