<?php
    namespace controllers;
    use services\CartService;
    use models\JsonSuccess;
    use models\JsonError;

    class CartCheckoutPostController implements IBaseController
    {
        private $cartService;
        
        public function __construct($factory)
        {
            $this->cartService = new CartService($factory);
        }
        
        public function proccessRequest() : void
        {
            $checkoutResult = $this->cartService->checkout();
            if ($checkoutResult->success)
            {
                $retorno = new JsonSuccess("Número do pedido: " . $checkoutResult->orderId);
                header('Content-type:application/json;charset=utf-8');
            } 
            else 
                $retorno = new JsonError("Não foi possivel realizar o pedido");                
                
            echo json_encode($retorno);
        }
    }
?>