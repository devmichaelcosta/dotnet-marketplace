<?php

namespace controllers\products;

use controllers\IBaseController;
use services\SimilarProductService;
use models\JsonError;
use models\JsonSuccess;
use Exception;

class DeleteSimilarProductControler implements IBaseController
{
    private $_service;

    public function __construct($factory)
    {
        $this->_service = new SimilarProductService($factory);
    }

    public function proccessRequest(): void
    {
        $retornoJson = null;
        try {
            $this->_service->delete($_POST["parentProductId"], $_POST["childProductId"]);
            $retornoJson = new JsonSuccess("Produto deletado com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } catch (Exception $e) {
            $retornoJson = new JsonError("Não foi possivel deletar o produto");
        }
        echo json_encode($retornoJson);
    }
}
