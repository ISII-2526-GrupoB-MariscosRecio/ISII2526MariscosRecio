USE [aspnet-AppForSEII2526.Web-660902f3-55b0-42b4-a4c6-7893d47fb56a];
GO
SET IDENTITY_INSERT [dbo].[Model] ON
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (1, N'Samsung')
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (2, N'Apple')
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (3, N'Xiaomi')
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (4, N'Huawei')
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (5, N'Sony')
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (6, N'Fake Apple')
SET IDENTITY_INSERT [dbo].[Model] OFF

SET IDENTITY_INSERT [dbo].[Device] ON
INSERT INTO [dbo].[Device] ([id], [Color], [Brand], [Name], [PriceForRent], [PriceForPurchase], [Quality], [Year], [QuantityForPurchase], [QuantityForRent], [ModelId], [Description]) VALUES (1, N'Negro', N'Samsung', N'Galaxy S10', 15, 199, 2, 2019, 5, 3, 1, N'Smartphone clásico, buen rendimiento para su año.')
INSERT INTO [dbo].[Device] ([id], [Color], [Brand], [Name], [PriceForRent], [PriceForPurchase], [Quality], [Year], [QuantityForPurchase], [QuantityForRent], [ModelId], [Description]) VALUES (2, N'Blanco', N'Apple', N'iPhone 11', 20, 229, 1, 2019, 4, 2, 2, N'iPhone en muy buen estado, batería OK.')
INSERT INTO [dbo].[Device] ([id], [Color], [Brand], [Name], [PriceForRent], [PriceForPurchase], [Quality], [Year], [QuantityForPurchase], [QuantityForRent], [ModelId], [Description]) VALUES (7, N'Azul', N'Xiaomi', N'Redmi Note 9', 7, 89, 3, 2020, 5, 3, 3, N'Económico, buena relación calidad/precio.')
INSERT INTO [dbo].[Device] ([id], [Color], [Brand], [Name], [PriceForRent], [PriceForPurchase], [Quality], [Year], [QuantityForPurchase], [QuantityForRent], [ModelId], [Description]) VALUES (11, N'Gris', N'Huawei', N'P30 Lite', 9, 120, 2, 2018, 3, 5, 4, N'Modelo utilizado para pruebas, con funda incluida.')
INSERT INTO [dbo].[Device] ([id], [Color], [Brand], [Name], [PriceForRent], [PriceForPurchase], [Quality], [Year], [QuantityForPurchase], [QuantityForRent], [ModelId], [Description]) VALUES (12, N'Rojo', N'Sony', N'Xperia Z5 Compact', 6, 60, 5, 2016, 1, 1, 5, N'Pequeño y robusto, batería con desgaste moderado.')
SET IDENTITY_INSERT [dbo].[Device] OFF

-- Nota: El PasswordHash de abajo corresponde a "Password123!"
-- Así podrás entrar con cualquiera de ellos usando esa contraseña.

INSERT INTO [dbo].[AspNetUsers] ([Id], [Name], [Surname], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) 
VALUES 
(N'1', N'juan', N'lopez', N'juanKILLA', N'JUANKILLA', N'juanXkilla@gmail.com', N'JUANXKILLA@GMAIL.COM', 1, 
N'AQAAAAIAAYagAAAAELlWr7tDq+8j4Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q==', -- Hash de ejemplo (Genera uno real y pégalo aquí)
NEWID(), NEWID(), N'1234', 1, 0, NULL, 1, 0),

(N'2', N'Paco', N'Nuñez', N'PacoSinger', N'PACOSINGER', N'pacoSinger@gmail.com', N'PACOSINGER@GMAIL.COM', 1, 
N'AQAAAAIAAYagAAAAELlWr7tDq+8j4Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q==', 
NEWID(), NEWID(), N'2345', 1, 0, NULL, 1, 0),

(N'3', N'Laura', N'Jimenez', N'LauraWita', N'LAURAWITA', N'Laura@gmail.com', N'LAURA@GMAIL.COM', 1, 
N'AQAAAAIAAYagAAAAELlWr7tDq+8j4Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q==', 
NEWID(), NEWID(), N'4567', 1, 0, NULL, 1, 0),

(N'4', N'Marcos', N'Gonzalez', N'MarcosGamerXXX', N'MARCOSGAMERXXX', N'MarcosGAMER@gmail.com', N'MARCOSGAMER@GMAIL.COM', 1, 
N'AQAAAAIAAYagAAAAELlWr7tDq+8j4Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q==', 
NEWID(), NEWID(), N'5678', 1, 0, NULL, 0, 0),

(N'5', N'Aitor', N'Herrero', N'AitorPedos', N'AITORPEDOS', N'AitorAitor@gmail.com', N'AITORAITOR@GMAIL.COM', 1, 
N'AQAAAAIAAYagAAAAELlWr7tDq+8j4Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q==', 
NEWID(), NEWID(), N'1456', 1, 0, NULL, 1, 0);