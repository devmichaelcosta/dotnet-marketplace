<?php
    namespace controllers;
    use controllers\IBaseController;
    use services\ProductService;

    class ProductRateController implements IBaseController
    {
        public function __construct($factory)
        {
            $this->_productService = new ProductService($factory);
            $this->_productRepository = $factory->getProductRepository();
        }

        public function proccessRequest(): void
        {
            $product = $this->_productRepository->getById($_GET["id"]);
            
            require "views/produto/product-rating.php";
        }
    }
?>