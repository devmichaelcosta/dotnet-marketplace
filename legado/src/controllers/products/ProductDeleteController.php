<?php

namespace controllers\products;

use controllers;
use models\Product;
use controllers\IBaseController;
use services\ProductService;
use models\JsonError;
use models\JsonSuccess;
use Exception;

class ProductDeleteController implements IBaseController
{
    private $_productService;

    public function __construct($factory)
    {
        $this->_productService = new ProductService($factory);
    }

    public function proccessRequest(): void
    {
        try {
            $this->_productService->remove($_POST["id"]);
            $retorno = new JsonSuccess("Produto removido com sucesso.");
            header('Content-type:application/json;charset=utf-8');
            echo json_encode($retorno);
        } catch (Exception $e) {
            $retorno = new JsonError("Não foi possivel remover produto");
            header('Content-type:application/json;charset=utf-8');
            echo json_encode($retorno);
        }
    }
}
