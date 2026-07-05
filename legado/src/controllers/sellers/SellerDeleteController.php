<?php

namespace controllers\sellers;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use Exception;

class SellerDeleteController implements IBaseController
{
    private $_repoSeller;

    public function __construct($factory)
    {
        $this->_repoSeller = $factory->getSellerRepository();
    }

    public function proccessRequest(): void
    {
        try {
            $id = $_POST["id"];
            if (is_null($id) || $id === false) {
                $retorno = new JsonError("Não foi possivel encontrar o vendedor.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            } else {
                $this->_repoSeller->remove($_POST["id"]);
                $retorno = new JsonSuccess("Vendedor removido com sucesso.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            }
        } catch (Exception $e) {
            $retorno = new JsonError("Não foi possivel remover o vendedor.");
            header('Content-type:application/json;charset=utf-8');
            echo json_encode($retorno);
        }
    }
}
