<?php
    namespace controllers\products;
    
    use controllers\IBaseController;

    class ProductsLikedsController implements IBaseController
    {
        private $productRepository;

        public function __construct($factory)
        {
            $this->productRepository = $factory->getProductRepository();
        }

        public function proccessRequest() : void
        {
            $products = $this->productRepository->getAllLikeds($_SESSION["userId"]);
            require $_SERVER['DOCUMENT_ROOT'] . '\\views\\produto\\likeds.php';
        }
    }