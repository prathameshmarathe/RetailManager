CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
BEGIN
set nocount on; 
	select id, ProductName, [Description],RetailPrice,QuantityInStock, IsTaxable
	FROM dbo.Product
	order by ProductName;
END
