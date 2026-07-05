<?php

namespace controllers\products;

use controllers\IBaseController;
use Exception;
use infra\repositories\ProductRepository;
use models\JsonError;
use models\JsonSuccess;

class LikeProductController implements IBaseController
{
    private $_productRepository;

    public function __construct($factory)
    {
        $this->_productRepository = $factory->getProductRepository();
    }

    public function proccessRequest(): void
    {
        $retornoJson = null;
        try {
            $this->_productRepository->like(intval($_POST["productId"]), intval($_SESSION["userId"]));
            $retornoJson = new JsonSuccess("Produto curtido com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } catch (Exception $e) {
            $retornoJson = new JsonError("Não foi possivel curtir o produto");
        }
        echo json_encode($retornoJson);
    }
}
