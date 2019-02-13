SELECT p.Name,
       c.Name
FROM Products p
LEFT JOIN ProductCategories pc ON p.Id = pc.ProductId
LEFT JOIN Categories c ON c.Id = pc.CategoryId

/*
CREATE TABLE Products(
   Id   INTEGER PRIMARY KEY
  ,Name NVARCHAR(30) NOT NULL
);
INSERT INTO Products(Id,Name) VALUES (1,N'ProductA');
INSERT INTO Products(Id,Name) VALUES (2,N'ProductB');
INSERT INTO Products(Id,Name) VALUES (3,N'ProductC');
INSERT INTO Products(Id,Name) VALUES (4,N'ProductD');
INSERT INTO Products(Id,Name) VALUES (5,N'ProductE');
INSERT INTO Products(Id,Name) VALUES (6,N'ProductF');

CREATE TABLE Categories(
   Id   INTEGER PRIMARY KEY
  ,Name NVARCHAR(30) NOT NULL
);
INSERT INTO Categories(Id,Name) VALUES (1,N'Category1');
INSERT INTO Categories(Id,Name) VALUES (2,N'Category2');
INSERT INTO Categories(Id,Name) VALUES (3,N'Category3');
INSERT INTO Categories(Id,Name) VALUES (4,N'Category4');
INSERT INTO Categories(Id,Name) VALUES (5,N'Category5');

CREATE TABLE ProductCategories(
   ProductId  INTEGER REFERENCES Products (Id)
  ,CategoryId INTEGER REFERENCES Categories (Id)
  ,PRIMARY KEY(ProductId,CategoryId)
);
INSERT INTO ProductCategories(ProductId,CategoryId) VALUES (1,1);
INSERT INTO ProductCategories(ProductId,CategoryId) VALUES (1,2);
INSERT INTO ProductCategories(ProductId,CategoryId) VALUES (1,3);
INSERT INTO ProductCategories(ProductId,CategoryId) VALUES (2,1);
INSERT INTO ProductCategories(ProductId,CategoryId) VALUES (3,2);
INSERT INTO ProductCategories(ProductId,CategoryId) VALUES (3,3);
INSERT INTO ProductCategories(ProductId,CategoryId) VALUES (4,3);
INSERT INTO ProductCategories(ProductId,CategoryId) VALUES (4,4);
*/
