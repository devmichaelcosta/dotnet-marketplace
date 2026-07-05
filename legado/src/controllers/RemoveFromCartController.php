<?php
    namespace controllers;
    use services\CartService;
    use models\JsonSuccess;
    use models\JsonError;
    class RemoveFromCartController implements IBaseController
    {
        private $_cartService;
        
        public function __construct($factory)
        {
            $this->_cartService = new CartService($factory);
        }

        public function proccessRequest() : void
        {
            try
            {
                $this->_cartService->removeProduct($_POST["productId"]);
                $retorno = new JsonSuccess("Item removido com sucesso.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            }  
            catch (Exception $e) 
            {
                $retorno = new JsonError("Não foi possivel remover o produto.");   
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            }
        }
    }
?>