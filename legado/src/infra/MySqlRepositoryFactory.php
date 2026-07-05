<?php

namespace infra;

use infra\repositories\ProductOnOfferRepository;
use infra\repositories\ProductRepository;
use infra\repositories\CarouselRepository;
use infra\repositories\UserRepository;
use infra\repositories\SeedRepository;
use infra\repositories\CartRepository;
use infra\repositories\CategoryRepository;
use infra\repositories\SubCategoryRepository;
use infra\repositories\SellerRepository;
use infra\repositories\StateRepository;
use infra\repositories\OrderRepository;
use infra\repositories\AddressRepository;
use infra\repositories\AttributeRepository;

use infra\RepositoryFactory;

use PDO;
use PDOException;

class MySqlRepositoryFactory extends RepositoryFactory
{
    private $host = "localhost";
    private $dbname = "marketplace";
    private $port = "3306";
    private $username = "michael";
    private $password = "giacom";

    public $conn;

    public function getConnection()
    {
        $this->conn = null;

        try {
            $this->conn = new PDO(
                "mysql:host=" . $this->host .
                ";port=" . $this->port .
                ";dbname=" . $this->dbname,
                $this->username,
                $this->password
            );
        } catch (PDOException $exception) {
            //echo "erro ao tentar conectar com pdo: " .
            $exception->getMessage();
        }
        return $this->conn;
    }

    public function getAttributeRepository()
    {
        return new AttributeRepository($this->getConnection());
    }

    public function getOrderRepository()
    {
        return new OrderRepository($this->getConnection());
    }

    public function getStateRepository()
    {
        return new StateRepository($this->getConnection());
    }

    public function getUserRepository()
    {
        return new UserRepository($this->getConnection());
    }

    public function getProductOnOfferRepository()
    {
        return new ProductOnOfferRepository($this->getConnection());
    }

    public function getProductRepository()
    {
        return new ProductRepository($this->getConnection());
    }

    public function getCarouselRepository()
    {
        return new CarouselRepository($this->getConnection());
    }

    public function getCategoryRepository()
    {
        return new CategoryRepository($this->getConnection());
    }

    public function getSubCategoryRepository()
    {
        return new SubCategoryRepository($this->getConnection());
    }

    public function getSeedRepository()
    {
        return new SeedRepository($this->getConnection());
    }

    public function getCartRepository()
    {
        return new CartRepository($this->getConnection());
    }

    public function getSellerRepository()
    {
        return new SellerRepository($this->getConnection());
    }

    public function getAddressRepository()
    {
        return new AddressRepository($this->getConnection());
    }
}
