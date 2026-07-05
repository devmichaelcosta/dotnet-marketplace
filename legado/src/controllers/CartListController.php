<?php
    namespace controllers;
    use models\CartViewModel;
    use services\CartService;

    class CartListController implements IBaseController
    {
        private $cartService;
        
        public function __construct($factory)
        {
            $this->cartService = new CartService($factory);
        }
        
        public function proccessRequest() : void
        {
            $cartViewModel = $this->cartService->getCurrentCart();
            require "views/home/carrinho-itens.php";
        }
    }
?>