<?php

namespace controllers\products;

use controllers\IBaseController;
use Exception;
use models\JsonError;
use models\JsonSuccess;

class DislikeProductController implements IBaseController
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
            $this->_productRepository->dislike(intval($_POST["productId"]), intval($_SESSION["userId"]));
            $retornoJson = new JsonSuccess("Produto descurtido com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } catch (Exception $e) {
            $retornoJson = new JsonError("Não foi possivel descurtido o produto");
        }
        echo json_encode($retornoJson);
    }
}
