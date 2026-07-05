<?php
    namespace controllers;
    use services\CartService;

    class CartCheckoutController implements IBaseController
    {
        private $cartService;
        
        public function __construct($factory)
        {
            $this->cartService = new CartService($factory);
        }
        
        public function proccessRequest() : void
        {
            if (!$this->cartService->checkoutVerifyAuth())
                header("Location: /login?returnto=checkout");

            if (is_null($_SESSION["cart"]))
                header("Location: /carrinho");
                
            $model = $this->cartService->getCheckoutViewModel();
            
            require "views/home/carrinho-checkout.php";
        }
    }
?>