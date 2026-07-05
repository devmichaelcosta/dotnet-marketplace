<?php

namespace controllers\sellers;

use controllers\IBaseController;
use services\SellerService;
use models\requests\SellerRegisterRequest;
use models\JsonSuccess;
use models\JsonError;

class SellerRegisterPostController implements IBaseController
{
    private $_sellerService;

    public function __construct($factory)
    {
        $this->_sellerService = new SellerService($factory);
    }

    public function proccessRequest(): void
    {
        $retornoJson = null;

        $request = new SellerRegisterRequest(
            $_POST["name"],
            $_POST["lastName"],
            $_POST["login"],
            $_POST["password"]
        );

        //validando modelo se valido retornamos um JSON...
        $retornoJson = new JsonError("Não foi possivel cadastrar o Vendedor");
        if ($request->isValid()) {
            $response = $this->_sellerService->register($request);

            if ($response->getSuccess())
                $retornoJson = new JsonSuccess("Vendedor registrado com sucesso");
            else
                $retornoJson = new JsonError($response->getMsg());
        }
        header('Content-type:application/json;charset=utf-8');
        echo json_encode($retornoJson);
    }
}
