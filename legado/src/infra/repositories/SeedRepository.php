<?php

namespace infra\repositories;

use Exception;

use infra;
use infra\MySqlRepository;
use infra\interfaces\ISeedRepository;
use models\User;
use infra\repositories\UserRepository;
use infra\repositories\SellerRepository;

class SeedRepository
extends MySqlRepository
implements ISeedRepository
{
    public function seedProducts()
    {
        //inserindo produtos...
        $this->conn->exec(
            "insert into products(
                    title,
                    description,
                    price,
                    createdat,
                    createdby,
                    offer,
                    stock,
                    sku,
                    userId
                ) values (
                    'GUITARRA FENDER STANDARD TELECASTER MEXICANA BLACK - 014-5102-506',
                    'A Fender traz a Standard Telecaster para guitarristas que apreciam estilo e versatilidade por um super valor!',
                    5690.99,
                    now(),
                    'michael',
                    1,
                    10,
                    '001',
                    (select UserId from users where role = 'vendedor' limit 1)
                );"
        );
        $this->conn->exec(
            "insert into products(
                    title,
                    description,
                    price,
                    createdat,
                    createdby,
                    offer,
                    stock,
                    sku,
                    userId
                ) values(
                    'GUITARRA FENDER AMERICAN SPECIAL STRATOCASTER MAPLE 2-COLOR SUNBURST (2012) - ACOMPANHA HARD CASE',
                    'A lendária guitarra Stratocaster em sua versão mais tradicional!',
                    7990,
                    now(),
                    'michael',
                    1,
                    10,
                    '002',
                    (select UserId from users where role = 'vendedor' limit 1)
                );"
        );
        $this->conn->exec(
            "insert into products(
                    title,
                    description,
                    price,
                    createdat,
                    createdby,
                    offer,
                    stock,
                    sku,
                    userId
                )values(
                    'GUITARRA JACKSON DINKY JS11 GLOSS BLACK - 291 0110 503',
                    'Uma guitarra de alta qualidade e excelente preço da lendária marca Jackson!',
                    1790,
                    now(),
                    'michael',
                    1,
                    10,
                    '003',
                    (select UserId from users where role = 'vendedor' limit 1)
                );"
        );

        //inserindo imagens de produtos...
        $this->conn->exec(
            "insert into productsimages(
                    productid,
                    filename
                )
                values 
                (
                    (
                        select ProductId from products where 
                        title like '%GUITARRA FENDER STANDARD TELECASTER MEXICANA BLACK - 014-5102-506%' 
                        limit 1
                    ) 
                , 'fender-american-especial-stratocaster-maple2-color-sunburst2012.jpg');"
        );
        $this->conn->exec(
            "insert into productsimages(productid,filename)
                values 
                (
                    (
                        select ProductId from products where 
                        title like '%GUITARRA FENDER AMERICAN SPECIAL STRATOCASTER MAPLE 2-COLOR SUNBURST (2012) - ACOMPANHA HARD CASE%' 
                        limit 1
                    ) 
                , 'fender-mex-black-014-5102-506.jpg');"
        );
        $this->conn->exec(
            "insert into productsimages(productid,filename)
                values 
                (
                    (
                        select ProductId from products where 
                        title like '%GUITARRA JACKSON DINKY JS11 GLOSS BLACK - 291 0110 503%' 
                        limit 1
                    ) 
                , 'jackson-dincky-JS11GLOSSBLACK2910110503.jpg');"
        );
    }

    public function seedUsersAndSellers()
    {
        $_repoUser = new UserRepository($this->conn);
        $_repoUser->add(new User(null, "michael", "123456", "michael", "admin", "", ""));
        $userId = $_repoUser->add(new User(null, "multisom", "123456", "Multisom", "vendedor", "", ""));
        echo "vendedorId: " . $userId . "<br>";

        $_repoSeller = new SellerRepository($this->conn);
        $_repoSeller->addSimplifiedSeller($userId);
    }

    public function seedCarousel()
    {
        // carrossel
        $this->conn->exec("insert into carouselimages(filename,`order`) values ('guitar-1920x384-1.jpg',1)");
        $this->conn->exec("insert into carouselimages(filename,`order`) values ('guitar-1920x384-2.jpg',2)");
        $this->conn->exec("insert into carouselimages(filename,`order`) values ('guitar-1920x384-3.jpg',3)");
    }

    public function seedStates()
    {
        $this->conn->exec("insert into states(name, stateabreviattion) values ('Rio Grande do Sul','RS')");
        $this->conn->exec("insert into states(name, stateabreviattion) values ('Santa Catarina','SC')");
    }

    public function seed()
    {
        $this->destroyDatabase();
        echo "Destruiu o banco<br>";
        $this->createDb();
        echo "Criou o banco<br>";
        $this->seedCarousel();
        echo "Adicionou imagens de carrosel<br>";
        $this->seedUsersAndSellers();
        echo "Adicionou usuarios e vendedores padroes de carrosel<br>";
        $this->seedStates();
        echo "Adicionou estados<br>";
        $this->seedProducts();
        echo "Adicionou produtos<br>";

        //header('Location: /');
    }

    public function createDb()
    {
        $this->createTableCategories();
        $this->createTableSubCategories();
        $this->createTableStates();
        $this->createTableUsers();
        $this->createTableAddress();

        $this->createTableSellers();
        $this->createTableProducts();
        $this->createTableProductImages();
        $this->createTableCarouselImages();
        $this->createTableOrder();
        $this->createTableOrderItens();
        $this->createTableSimilarProducts();

        $this->createTableAttributes();
        $this->createTableAttributeValues();
        $this->createTableComments();
    }

    public function destroyDatabase()
    {
        try {
            $this->conn->exec("drop table if exists CarouselImages");
            $this->conn->exec("drop table if exists Sellers");

            $this->conn->exec("drop table if exists OrderItens");
            $this->conn->exec("drop table if exists Orders");
            $this->conn->exec("drop table if exists ProductsImages");
            $this->conn->exec("drop table if exists Products");
            $this->conn->exec("drop table if exists SubCategories");
            $this->conn->exec("drop table if exists Categories");
            $this->conn->exec("drop table if exists Addresses");
            $this->conn->exec("drop table if exists Users");

            $this->conn->exec("drop table if exists states");
            $this->conn->exec("drop table if exists SimilarProducts");
        } catch (Exception $ex) {
            echo "Ocorreu um erro ao tentar destruir o banco de dados.";
        }
    }

    public function createTableStates()
    {
        $this->conn->exec(
            "CREATE TABLE States (
                    StateId INT NOT NULL  PRIMARY KEY AUTO_INCREMENT,
                    Name varchar(255) NOT NULL,
                    StateAbreviattion varchar(255)
                );"
        );
    }

    public function createTableUsers()
    {
        $query =  "create table Users(
                UserId  int NOT NULL PRIMARY KEY AUTO_INCREMENT,
                Login varchar(100) not null unique,
                Password varchar(255) not null,
                Name varchar(255) not null,
                Role varchar(45) not null,
                Cpf  varchar(14),
                LastName  varchar(100)
            );";
        $this->conn->exec($query);
    }

    public function createTableCategories()
    {
        $query = "CREATE TABLE Categories (
                CategoryId int NOT NULL AUTO_INCREMENT,
                Title varchar(255) NOT NULL,    
                Image varchar(100),    
                PRIMARY KEY (CategoryId)
            );";
        $this->conn->exec($query);
    }

    public function createTableSubCategories()
    {
        $query = "CREATE TABLE SubCategories (
                SubCategoryId int NOT NULL PRIMARY KEY AUTO_INCREMENT,
                CategoryId int,
                Title varchar(255) NOT NULL,    
                Image varchar(100), 
                FOREIGN KEY(CategoryId) REFERENCES Categories(CategoryId)
            );";
        $this->conn->exec($query);
    }

    public function createTableCarouselImages()
    {
        $query = "CREATE TABLE CarouselImages (
                CarouselImageId int NOT NULL AUTO_INCREMENT,
                FileName varchar(255) NOT NULL,    
                `Order` int not null,
                PRIMARY KEY (CarouselImageId)
            );";
        $this->conn->exec($query);
    }

    public function createTableProducts()
    {
        $query = "CREATE TABLE Products (
                ProductId int NOT NULL PRIMARY KEY AUTO_INCREMENT,
                Title varchar(255) NOT NULL,
                Description varchar(10000),
                Price decimal(10,2),
                CreatedAt datetime,
                CreatedBy varchar(255),
                Offer bit(1) not null,
                Stock int(11) not null,
                Sku varchar(45) not null,
                UserId int not null,
                FOREIGN KEY(UserId) REFERENCES Users(UserId),
                SubCategoryId int, 
                FOREIGN KEY(SubCategoryId) REFERENCES SubCategories(SubCategoryId)
            );";
        $this->conn->exec($query);
    }

    public function createTableProductImages()
    {
        $query = "CREATE TABLE ProductsImages (
                ProductImageId int PRIMARY KEY AUTO_INCREMENT, 
                ProductId int NOT NULL,
                FileName nvarchar(255) not null, 
                FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
            );";
        $this->conn->exec($query);
    }

    public function createTableAddress()
    {
        $this->conn->exec(
            "CREATE TABLE Addresses (
                    AddressId int NOT NULL PRIMARY KEY AUTO_INCREMENT,
                    Street varchar(255) NOT NULL,
                    CEP varchar(9) NOT NULL,
                    Neighborhood nvarchar(100) NOT NULL,
                    City varchar(100) NOT NULL,
                    StateId int NOT NULL,
                    Complement varchar(255) NOT NULL,
                    UserId int not null,
                    FOREIGN KEY(StateId) REFERENCES States(StateId),
                    FOREIGN KEY(UserId) REFERENCES Users(UserId)
                );"
        );
    }

    public function createTableSellers()
    {
        $this->conn->exec(
            "CREATE TABLE Sellers (
                    SellerId int NOT NULL PRIMARY KEY AUTO_INCREMENT,
                    Age int,    
                    Email varchar(100),
                    DateOfBirth datetime,
                    WebSite varchar(255),
                    Company varchar(150),
                    CNPJ varchar(150),
                    BranchOfActivity varchar(150),
                    FantasyName varchar(150),
                    UserId int,
                    FOREIGN KEY(UserId) REFERENCES Users(UserId)
                );"
        );
    }

    public function createTableOrder()
    {
        $this->conn->exec(
            "CREATE TABLE Orders (
                    OrderId int NOT NULL PRIMARY KEY AUTO_INCREMENT ,
                    Total int NOT NULL , 
                    CreatedAt datetime NOT NULL , 
                    UserId int NOT NULL ,
                    FOREIGN KEY(UserId) REFERENCES Users(UserId)  ,
                    StateId int NOT NULL ,
                    FOREIGN KEY(StateId) REFERENCES States(StateId) ,
                    CardOwnerName varchar(150) NOT NULL ,
                    ExpirationDate varchar(150) NOT NULL ,
                    Name varchar(150) NOT NULL ,
                    Address varchar(150) NOT NULL ,
                    Neighborhood varchar(150) NOT NULL ,
                    CPF varchar(14) NOT NULL ,
                    CEP varchar(9) NOT NULL ,                
                    City varchar(150) NOT NULL, 
                    Complement varchar(150) NOT NULL
                );"
        );
    }

    public function createTableOrderItens()
    {
        $this->conn->exec(
            "CREATE TABLE OrderItens (
                    OrderItemId int NOT NULL PRIMARY KEY AUTO_INCREMENT, 
                    OrderId int not null,
                    FOREIGN KEY(OrderId) REFERENCES Orders(OrderId),
                    ProductId int,
                    FOREIGN KEY(ProductId) REFERENCES Products(ProductId),
                    Qtd int
                );"
        );
    }

    public function createTableSimilarProducts()
    {
        $query = "CREATE TABLE SimilarProducts (
                SimilarProductId int PRIMARY KEY AUTO_INCREMENT, 
                ParentProductId int NOT NULL,
                ChildProductId int NOT NULL, 
                FOREIGN KEY(ParentProductId) REFERENCES Products(ProductId),
                FOREIGN KEY(ChildProductId) REFERENCES Products(ProductId)
            );";
        $this->conn->exec($query);
    }

    public function createTableAttributes()
    {
        $query = "create table Attributes (
                AttributeId int NOT NULL PRIMARY KEY AUTO_INCREMENT,
                Name varchar(255) NOT NULL
        );";
        $this->conn->exec($query);
    }

    public function createTableAttributeValues()
    {
        $query = "create table AttributeValues (
            AttributeValueId int NOT NULL PRIMARY KEY AUTO_INCREMENT,
            AttributeId int NOT NULL,
            ProductId int NOT NULL,
            Value varchar(255) NOT NULL,
            FOREIGN KEY(AttributeId) REFERENCES Attributes(AttributeId),
            FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
        );";
        $this->conn->exec($query);
    }

    public function createTableComments()
    {
        $query = "create table ProductsComments (
            ProductCommentId int NOT NULL PRIMARY KEY AUTO_INCREMENT,    
            ProductId int NOT NULL,
            Title varchar(30) NOT NULL,
            Description varchar(100) NOT NULL,
            Recommended bit not null,
            Ratting varchar(30) not null,    
            FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
        );";
        $this->conn->exec($query);
    }
}
