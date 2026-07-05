<?php
    namespace controllers;
    use models\JsonSuccess;
    use models\JsonError;
    use services\CartService;
    use Exception;

    class CartAtualizarQtdProdutoController implements IBaseController
    {
        private $_cartService;
        
        public function __construct($factory)
        {
            $this->_cartService = new CartService($factory);
        }
        
        public function proccessRequest() : void
        {
            $productId = $_POST["productId"];
            $qtd = $_POST["qtd"];
            $retorno = null;
            try
            {
                $response = $this->_cartService->updateQtdProduct($productId, $qtd);
                if ($response->getSuccess()){
                    $retorno = new JsonSuccess("Quantidade atualizada com sucesso.");
                } else {
                    $retorno = new JsonError($response->getMsg());
                }
                $retorno->currentQtd = $response->getQtd();
                $retorno->stock = $response->getStock();
                $retorno->finalValue = $response->getFinalValue();
                $retorno->subTotal = $response->getSubTotal();
                $retorno->total = $_SESSION["cart"]->getTotalFormatted(); // gambiarra alert
            }  
            catch (Exception $e) 
            {
                $retorno = new JsonError("Não foi possivel atualizar a quantidade.");   
            }
            header('Content-type:application/json;charset=utf-8');

            echo json_encode($retorno);
        }
    }
?>