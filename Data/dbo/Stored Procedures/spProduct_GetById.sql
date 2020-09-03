CREATE PROCEDURE [dbo].[spProduct_GetById]
	@Id int
AS
BEGIN
set nocount on;
	select id, ProductName, [Description],RetailPrice,QuantityInStock, IsTaxable
	FROM dbo.Product
	where Id=@Id;
end 
